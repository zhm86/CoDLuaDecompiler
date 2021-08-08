using System;
using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class TwoWayConditionalsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            var debugVisited = new HashSet<BasicBlock>();
            HashSet<BasicBlock> Visit(BasicBlock b)
            {
                var unresolved = new HashSet<BasicBlock>();
                // This makes it loop in descending order
                // for (all nodes m in descending order)
                foreach (var succ in b.DominanceTreeSuccessors)
                {
                    if (debugVisited.Contains(succ))
                    {
                        throw new Exception("Revisited dom tree node " + succ);
                    }
                    debugVisited.Add(succ);
                    unresolved.UnionWith(Visit(succ));
                }

                // if ((nodeType(m) == 2-way) ^ (inHeadLatch(m) == false)
                if (b.Successors.Count() == 2 && b.Instructions.Last() is Jump jmp && (!b.IsLoopHead || b.LoopType != LoopType.LoopPretested))
                {
                    int maxEdges = 0;
                    BasicBlock maxNode = null;
                    foreach (var d in b.DominanceTreeSuccessors)
                    {
                        int successorsReq = 2;
                        // If there is a break or while, the follow node is only going to have one backedge
                        if (b.LoopBreakFollow != null || b.LoopContinueFollow != null)
                        {
                            successorsReq = 1;
                        }
                        if (d.Predecessors.Count() >= successorsReq && d.Predecessors.Count() > maxEdges && !d.IsContinueNode && !d.IsBreakNode && d != f.EndBlock)
                        {
                            maxEdges = d.Predecessors.Count();
                            maxNode = d;
                        }
                    }
                    // Heuristic: if the true branch leads to a return or is if-orphaned and the follow isn't defined already, then the follow is always the false branch
                    // If the true branch also has a follow chain defined that leads to a return or if-orphaned node, then it is also disjoint from the rest of the CFG
                    // and the false branch is the follow
                    bool isDisjoint = false;
                    var testfollow = b.Successors[0].Follow;
                    while (testfollow != null)
                    {
                        if (testfollow.Instructions.Any() && testfollow.Instructions.Last() is Return || testfollow.IfOrphaned)
                        {
                            isDisjoint = true;
                            break;
                        }
                        testfollow = testfollow.Follow;
                    }
                    if (maxNode == null && (b.Successors[0].Instructions.Any() && b.Successors[0].Instructions.Last() is Return || b.Successors[0].IfOrphaned || isDisjoint))
                    {
                        // If the false branch leads to an isolated return node or an if-orphaned node, then we are if-orphaned, which essentially means we don't
                        // have a follow defined in the CFG. This means that to structure this, the if-orphaned node must be adopted by the next node with a CFG
                        // determined follow and this node will inherit that follow
                        if ((b.Successors[1].Instructions.Any() && b.Successors[1].Instructions.Last() is Return && b.Successors[1].Predecessors.Count() == 1) || b.Successors[1].IfOrphaned)
                        {
                            b.IfOrphaned = true;
                        }
                        else
                        {
                            maxNode = b.Successors[1];
                        }
                    }
                    // If you don't match anything, but you dominate the end node, then it's probably the follow
                    if (maxNode == null && b.DominanceTreeSuccessors.Contains(f.EndBlock))
                    {
                        maxNode = f.EndBlock;
                    }

                    // If we are a latch and the false node leads to a loop head, then the follow is the loop head
                    if (maxNode == null && b.IsLoopLatch && b.Successors[1].IsLoopHead)
                    {
                        maxNode = b.Successors[1];
                    }

                    if (maxNode == null && b.Successors[0].Successors.Count == 1 && b.Successors[1].Successors.Count == 1 &&
                        b.Successors[0].Successors[0] == b.Successors[1].Successors[0] &&
                        b.Instructions.Any() && b.Instructions.Last() is Jump j && j.BlockDest == b.Successors[1] &&
                        b.Successors[0].Instructions.Any() && b.Successors[0].Instructions.Last() is Jump j2 && j2.BlockDest == b.Successors[0].Successors[0] &&
                        b.Successors[0].Successors[0].IsLoopHead)
                    {
                        maxNode = b.Successors[1].Successors[0];
                    }
                    
                    if (maxNode != null)
                    {
                        b.Follow = maxNode;
                        bool keepMN = false;
                        var unresolvedClone = new HashSet<BasicBlock>(unresolved);
                        foreach (var x in unresolvedClone)
                        {
                            if (x != maxNode && !x.Dominance.Contains(maxNode))
                            {
                                bool inc = (x.DominanceTreeSuccessors.Count() == 0);
                                // Do a BFS down the dominance heirarchy to search for a follow
                                var bfsQueue = new Queue<BasicBlock>(x.DominanceTreeSuccessors);
                                //foreach (var domsucc in x.DominanceTreeSuccessors)
                                //{
                                while (bfsQueue.Count() > 0)
                                {
                                    var domsucc = bfsQueue.Dequeue();
                                    if (domsucc.Successors.Contains(maxNode) || domsucc.Follow == maxNode)
                                    {
                                        inc = true;
                                        break;
                                    }
                                    domsucc.DominanceTreeSuccessors.ForEach(s => bfsQueue.Enqueue(s));
                                }
                                //}
                                if (x.IfOrphaned)
                                {
                                    inc = true;
                                }
                                if (inc)
                                {
                                    x.Follow = maxNode;
                                    unresolved.Remove(x);
                                }
                            }
                        }

                    }
                    else
                    {
                        unresolved.Add(b);
                    }
                }

                // The loop head or latch is the implicit follow of any unmatched conditionals
                if (b.IsLoopHead)
                {
                    foreach (var ur in unresolved)
                    {
                        // If there's a single loop latch and it has multiple predecessors, it's probably the follow
                        if (b.LoopLatches.Count == 1 && b.LoopLatches[0].Predecessors.Count > 1)
                        {
                            ur.Follow = b.LoopLatches[0];
                        }
                        // Otherwise the detected latch (of multiple) is probably within an if statement and the head is the true follow
                        else if (ur.Successors.Any())
                        {
                            ur.Follow = ur.Successors.Last();
                        }
                        else
                        {
                            ur.Follow = b;
                        }
                    }
                    unresolved.Clear();
                }

                return unresolved;
            }

            // Unsure about this logic, but the idea is that an if chain at the end that only returns will be left unmatched and unadopted,
            // and thus the follows need to be the end blocks
            var unmatched = Visit(f.StartBlock);
            foreach (var u in unmatched)
            {
                u.Follow = f.EndBlock;
                if (u.Successors.Any())
                {
                    u.Follow = u.Successors.Last();
                }
            }
        }
    }
}