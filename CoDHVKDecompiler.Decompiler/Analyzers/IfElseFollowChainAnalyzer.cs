using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.CFG;
using CoDHVKDecompiler.Decompiler.IR.Functions;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Sometimes, due to if edges always leading to returns, the follows selected aren't always the most optimal for clean lua generation,
    /// even though they technically generate correct code. For example, you might get:
    /// if a then
    ///     blah
    /// elseif b then
    ///     blah
    /// else
    ///     if c then
    ///         return
    ///     elseif d then
    ///         blah
    ///     end
    /// end
    /// 
    /// This can be simplified into a single if else chain. The problem is since "if c then" leads to a return, there's never an explicit jump
    /// to the last block, or the true logical follow. It becomes "orphaned" and is adopted by "elseif d then" as the follow. This pass detects such
    /// cases and simplifies them.
    /// </summary>
    public class IfElseFollowChainAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool IsIsolated(BasicBlock b, BasicBlock target)
            {
                var visited = new HashSet<BasicBlock>();
                var queue = new Queue<BasicBlock>();

                queue.Enqueue(b);
                visited.Add(b);
                while (queue.Any())
                {
                    var c = queue.Dequeue();
                    if (c == target)
                    {
                        return false;
                    }
                    foreach (var succ in c.Successors)
                    {
                        if (!visited.Contains(succ))
                        {
                            queue.Enqueue(succ);
                            visited.Add(succ);
                        }
                    }
                }

                // No follow found
                return true;
            }

            // This relies on reverse postorder
            f.NumberReversePostorder();

            var processed = new HashSet<BasicBlock>();
            foreach (var b in f.PostorderTraversal(false))
            {
                var chain = new List<BasicBlock>();
                if (b.Follow != null)
                {
                    var iter = b;
                    BasicBlock highestFollow = b.Follow;
                    int highestFollowNumber = b.Follow.ReversePostorderNumber;
                    chain.Add(b);
                    while (!processed.Contains(iter) && iter.Successors.Count() == 2 && 
                        iter.Follow == iter.Successors[1] && iter.Successors[1].Instructions.Count() == 1 && IsIsolated(iter.Successors[0], b.Follow)
                        && b.Successors[1] != iter && iter.Follow.Predecessors.Count() == 1)
                    {
                        processed.Add(iter);
                        iter = iter.Follow;
                        chain.Add(iter);
                        if (iter.Follow != null && iter.Follow.ReversePostorderNumber > highestFollowNumber)
                        {
                            highestFollowNumber = iter.Follow.ReversePostorderNumber;
                            highestFollow = iter.Follow;
                        }
                    }
                    if (highestFollow != null && chain.Any() && chain.Last().Successors.Count() == 2)
                    {
                        foreach (var c in chain)
                        {
                            var oldf = c.Follow;
                            var newf = chain.Last().Follow;

                            // Update any matching follows inside the dominance tree of the true branch
                            var toVisit = new Stack<BasicBlock>();
                            toVisit.Push(c.Successors[0]);
                            while (toVisit.Count() > 0)
                            {
                                var v = toVisit.Pop();
                                if (v.Follow == oldf)
                                {
                                    v.Follow = newf;
                                }
                                foreach (var d in v.DominanceTreeSuccessors)
                                {
                                    toVisit.Push(d);
                                }
                            }
                            c.Follow = newf;
                        }
                    }
                }
                processed.Add(b);
            }
        }
    }
}