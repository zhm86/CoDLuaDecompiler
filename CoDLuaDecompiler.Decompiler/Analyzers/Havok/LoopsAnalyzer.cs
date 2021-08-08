using System;
using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class LoopsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Build an abstract graph to analyze with
            var blockIDMap = new Dictionary<BasicBlock, int>();
            var abstractNodes = new List<Node>();
            for (int i = 0; i < f.Blocks.Count; i++)
            {
                blockIDMap.Add(f.Blocks[i], i);
                var node = new Node {OriginalBlock = f.Blocks[i]};
                if (i == f.Blocks.Count - 1)
                {
                    node.IsTerminal = true;
                }
                abstractNodes.Add(node);
            }
            foreach (var b in blockIDMap)
            {
                foreach (var pred in b.Key.Predecessors)
                {
                    abstractNodes[b.Value].Predecessors.Add(abstractNodes[blockIDMap[pred]]);
                }
                foreach (var succ in b.Key.Successors)
                {
                    abstractNodes[b.Value].Successors.Add(abstractNodes[blockIDMap[succ]]);
                }
            }

            // Calculate intervals and the graph sequence in preperation for loop detection
            var headGraph = new AbstractGraph();
            headGraph.StartNode = abstractNodes[blockIDMap[f.StartBlock]];
            headGraph.Nodes = abstractNodes;
            headGraph.CalculateIntervals();
            headGraph.LabelReversePostorderNumbers();

            DetectLoopsForIntervalLevel(headGraph);

            foreach (var latch in headGraph.LoopLatches)
            {
                foreach (var head in latch.Value)
                {
                    var b = head.OriginalBlock;
                    b.IsLoopHead = true;
                    b.LoopLatches.Add(latch.Key.OriginalBlock);
                    b.LoopType = headGraph.LoopTypes[head];
                    if(headGraph.LoopFollows[head] == null)
                        continue;
                    b.LoopFollow = headGraph.LoopFollows[head].OriginalBlock;
                    latch.Key.OriginalBlock.IsLoopLatch = true;
                }
            }
        }
        
        private void DetectLoopsForIntervalLevel(CFG.AbstractGraph graph)
        {
            foreach (var interval in graph.Intervals)
            {
                var head = interval.Key;
                var intervalNodes = interval.Value;
                var latches = head.Predecessors.Where(p => intervalNodes.Contains(p)).OrderBy(p => p.ReversePostorderNumber).ToList();
                foreach (var latch in latches)
                {
                    AddLoopLatch(graph, head, latch, intervalNodes);
                }
            }
            var subgraph = graph.GetIntervalSubgraph();
            if (subgraph != null)
            {
                DetectLoopsForIntervalLevel(subgraph);
                foreach (var entry in subgraph.LoopLatches)
                {
                    var parentInterval = entry.Key.IntervalGraphParent.Interval;
                    foreach (var head in entry.Value)
                    {
                        var latches = head.IntervalGraphParent.Predecessors.Where(p => parentInterval.Contains(p)).ToList();
                        var headersInLoop = subgraph.LoopHeads.Where(e => e.Value == head).Select(e => e.Key.IntervalGraphParent).ToList();
                        var intervalsInLoop = new HashSet<Node>(graph.Intervals.Where(e => (headersInLoop.Contains(e.Key)/* || e.Value == parentInterval*/)).SelectMany(e => e.Value));
                        foreach (var latch in latches)
                        {
                            AddLoopLatch(graph, head.IntervalGraphParent, latch, intervalsInLoop);
                        }
                    }
                }
            }
        }
        
        private void AddLoopLatch(CFG.AbstractGraph graph, Node head, Node latch, HashSet<Node> interval)
        {
            if (!graph.LoopLatches.ContainsKey(latch))
            {
                graph.LoopLatches.Add(latch, new List<Node>());
            }
            graph.LoopLatches[latch].Add(head);

            var loopNodes = new HashSet<Node>();

            // Analyze the loop to determine the beginning of the range of postordered nodes that represent the loop
            int beginNum = head.ReversePostorderNumber;
            /*foreach (var succ in head.Successors)
            {
                if (succ.ReversePostorderNumber > beginNum && succ.ReversePostorderNumber <= latch.ReversePostorderNumber)
                {
                    beginNum = succ.ReversePostorderNumber;
                }
            }
            if (beginNum == -1)
            {
                throw new Exception("Bad loop analysis");
            }*/

            //loopNodes.Add(head);
            //head.InLoop = true;
            head.IsHead = true;
            graph.LoopHeads[head] = head;
            foreach (var l in interval.Where(x => x.ReversePostorderNumber >= beginNum && x.ReversePostorderNumber <= latch.ReversePostorderNumber))
            {
                loopNodes.Add(l);
                l.InLoop = true;
            }

            if (!graph.LoopFollows.ContainsKey(head))
            {
                CFG.LoopType type;
                if (head.Successors.Any(next => !loopNodes.Contains(next)))
                {
                    type = CFG.LoopType.LoopPretested;
                }
                else
                {
                    type = latch.Successors.Any(next => !loopNodes.Contains(next)) ? CFG.LoopType.LoopPostTested : CFG.LoopType.LoopEndless;
                }
                graph.LoopTypes[head] = type;
                List<Node> follows;
                if (type == LoopType.LoopPretested)
                {
                    follows = head.Successors.Where(next => !loopNodes.Contains(next)).ToList();
                }
                else if (type == LoopType.LoopPostTested)
                {
                    follows = latch.Successors.Where(next => !loopNodes.Contains(next)).ToList();
                }
                else
                {
                    //follows = loopNodes.SelectMany(loopNode => loopNode.Successors.Where(next => !loopNodes.Contains(next))).ToList();
                    // Heuristic: make the follow any loop successor node with a post-order number larger than the latch
                    follows = loopNodes.SelectMany(loopNode => loopNode.Successors.Where(next => next.ReversePostorderNumber > latch.ReversePostorderNumber)).ToList();
                }
                Node follow;
                if (follows.Count == 0)
                {
                    if (type != CFG.LoopType.LoopEndless)
                    {
                        throw new Exception("No follow for loop found");
                    }
                    follow = null;
                }
                else
                {
                    follow = follows.OrderBy(cand => cand.ReversePostorderNumber).First(); 
                }
                graph.LoopFollows[head] = follow;
            }
        }
    }
}