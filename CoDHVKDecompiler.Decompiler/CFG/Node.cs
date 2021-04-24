using System;
using System.Collections.Generic;

namespace CoDHVKDecompiler.Decompiler.CFG
{
    public class Node : IComparable<Node>
    {
        public List<Node> Successors { get; set; } = new List<Node>();
        public List<Node> Predecessors { get; set; } = new List<Node>();
        /// <summary>
        /// Child in the successor graph. Only set if this is an interval graph head
        /// </summary>
        public Node IntervalGraphChild { get; set; }
        /// <summary>
        /// The interval head in the parent graph that collapses into this node
        /// </summary>
        public Node IntervalGraphParent { get; set; }
        /// <summary>
        /// Pointer to the interval set this node belongs to
        /// </summary>
        public HashSet<Node> Interval { get; set; }
        /// <summary>
        /// The basic block that this node ultimately maps to
        /// </summary>
        public BasicBlock OriginalBlock { get; set; }
        /// <summary>
        /// Node is marked as being part of a loop
        /// </summary>
        public bool InLoop { get; set; } = false;

        public bool IsHead { get; set; } = false;
        /// <summary>
        /// Node is the terminal node (where returns jump to)
        /// </summary>
        public bool IsTerminal { get; set; } = false;
        /// <summary>
        /// Number in this graph based on the reverse postorder traversal number
        /// </summary>
        public int ReversePostorderNumber { get; set; }
        
        public int CompareTo(Node other)
        {
            if (other.ReversePostorderNumber > ReversePostorderNumber)
                return -1;
            if (other.ReversePostorderNumber == ReversePostorderNumber)
                return 0;
            return 1;
        }
    }
}