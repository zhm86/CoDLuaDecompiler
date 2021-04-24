using System.Collections.Generic;
using System.Linq;

namespace CoDHVKDecompiler.Decompiler.CFG
{
    public class AbstractGraph
    {
        public Node StartNode { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
        public Dictionary<Node, HashSet<Node>> Intervals { get; set; }

        public Dictionary<Node, Node> LoopHeads { get; set; } = new Dictionary<Node, Node>();
        public Dictionary<Node, List<Node>> LoopLatches { get; set; } = new Dictionary<Node, List<Node>>();
        public Dictionary<Node, Node> LoopFollows { get; set; } = new Dictionary<Node, Node>();
        public Dictionary<Node, LoopType> LoopTypes { get; set; } = new Dictionary<Node, LoopType>();
        
        public List<Node> PostorderTraversal(bool reverse)
        {
            var ret = new List<Node>();
            var visited = new HashSet<Node>();

            void Visit(Node b)
            {
                visited.Add(b);
                foreach (var succ in b.Successors.OrderByDescending(n => n.OriginalBlock.Id))
                {
                    if (!visited.Contains(succ))
                    {
                        Visit(succ);
                    }
                }
                ret.Add(b);
            }

            Visit(StartNode);

            if (reverse)
            {
                ret.Reverse();
            }
            return ret;
        }

        public void LabelReversePostorderNumbers()
        {
            var postorder = PostorderTraversal(true);
            for (int i = 0; i < postorder.Count(); i++)
            {
                postorder[i].ReversePostorderNumber = i;
            }
        }

        // Thanks @thefifthmatt :forestcat:
        public void CalculateIntervals()
        {
            Intervals = new Dictionary<Node, HashSet<Node>>();
            var headers = new HashSet<Node> { StartNode };
            while (headers.Count > 0)
            {
                var h = headers.First();
                headers.Remove(h);
                var interval = new HashSet<Node> { h };
                Intervals.Add(h, interval);
                h.Interval = interval;
                int lastCount = 0;
                while (lastCount != interval.Count)
                {
                    lastCount = interval.Count;
                    foreach (var start in interval.ToList())
                    {
                        foreach (var cand in start.Successors)
                        {
                            if (cand.Predecessors.All(n => interval.Contains(n)) && !Intervals.ContainsKey(cand))
                            {
                                interval.Add(cand);
                                cand.Interval = interval;
                            }
                        }
                    }
                }
                foreach (var cand in interval)
                {
                    headers.UnionWith(cand.Successors.Except(interval).Except(Intervals.Keys));
                }
            }
        }

        /// <summary>
        /// Get a collapsed version of the graph where interval heads become nodes
        /// </summary>
        /// <returns>A new collapsed graph</returns>
        public AbstractGraph GetIntervalSubgraph()
        {
            if (Intervals == null)
            {
                CalculateIntervals();
            }
            if (Intervals.Count() == Nodes.Count() || Intervals.Values.All(i => i.Count == 1))
            {
                return null;
            }

            AbstractGraph cfg = new AbstractGraph();
            foreach (var n in Intervals.Keys)
            {
                var node = new Node();
                n.IntervalGraphChild = node;
                node.IntervalGraphParent = n;
                node.OriginalBlock = n.OriginalBlock;
                cfg.Nodes.Add(node);
            }

            var header = Intervals.SelectMany(e => e.Value.Select(i => (i, e.Key))).ToDictionary(i => i.i, i => i.Key);
            foreach (var entry in Nodes)
            {
                if (!header.ContainsKey(entry))
                {
                    continue;
                }
                var h1 = header[entry];
                var hnode = h1.IntervalGraphChild;
                //hnode.Successors.UnionWith(entry.Successors.Select(n => header[n]).Where(h => h != h1).Select(n => n.IntervalGraphChild));
                var nodestoadd = entry.Successors.Select(n => header[n]).Where(h => h != h1).Select(n => n.IntervalGraphChild);
                foreach (var n in nodestoadd)
                {
                    if (!hnode.Successors.Contains(n))
                    {
                        hnode.Successors.Add(n);
                    }
                }
            }
            foreach (var entry in cfg.Nodes)
            {
                foreach (var succ in entry.Successors)
                {
                    succ.Predecessors.Add(entry);
                }
            }
            cfg.StartNode = cfg.Nodes.First(x => !x.Predecessors.Any());
            cfg.LabelReversePostorderNumbers();
            cfg.CalculateIntervals();
            return cfg;
        }
    }
}