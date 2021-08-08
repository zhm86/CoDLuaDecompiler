using System;
using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.CFG
{
    public class BasicBlock
    {
        public static int IdCounter { get; set; } = 0;
        public int Id { get; set; }

        public List<IInstruction> Instructions { get; set; } = new List<IInstruction>();
        public List<BasicBlock> Predecessors { get; set; } = new List<BasicBlock>();
        public List<BasicBlock> Successors { get; set; } = new List<BasicBlock>();

        public Dictionary<Identifier, PhiFunction> PhiFunctions { get; set; } =
            new Dictionary<Identifier, PhiFunction>();

        public HashSet<Identifier> PhiMerged { get; set; } = new HashSet<Identifier>();
        /// <summary>
        /// Set of basic blocks that dominate this function
        /// </summary>
        public HashSet<BasicBlock> Dominance { get; set; } = new HashSet<BasicBlock>();
        /// <summary>
        /// The closest node that dominates this block
        /// </summary>
        public BasicBlock ImmediateDominator { get; set; }
        /// <summary>
        /// Blocks that have this block as their immediate dominator
        /// </summary>
        public List<BasicBlock> DominanceTreeSuccessors { get; set; } = new List<BasicBlock>();

        /// <summary>
        /// Used for SSA construction
        /// </summary>
        public HashSet<BasicBlock> DominanceFrontier { get; set; } = new HashSet<BasicBlock>();
        
        // Live analysis stuff
        public HashSet<Identifier> UpwardExposedIdentifiers { get; set; } = new HashSet<Identifier>();
        public HashSet<Identifier> KilledIdentifiers { get; set; } = new HashSet<Identifier>();
        public HashSet<Identifier> LiveOut { get; set; } = new HashSet<Identifier>();
        
        /// <summary>
        /// Register IDs of registers killed (i.e. redefined) under the scope of this block (excluding this block)
        /// </summary>
        public HashSet<uint> ScopeKilled { get; set; }= new HashSet<uint>();
        
        // Control flow analysis
        public int ReversePostorderNumber { get; set; } = 0;
        public int OrderNumber { get; set; } = 0;
        public bool IsLoopLatch { get; set; } = false;
        public bool IsLoopHead { get; set; } = false;
        public LoopType LoopType { get; set; }
        public BasicBlock LoopFollow { get; set; }
        public List<BasicBlock> LoopLatches { get; set; } = new List<BasicBlock>();
        public BasicBlock Follow { get; set; }
        public BasicBlock LoopBreakFollow { get; set; }
        public BasicBlock LoopContinueFollow { get; set; }
        public bool IsBreakNode { get; set; } = false;
        public bool IsContinueNode { get; set; } = false;
        // Set to true if both the true and false branch lead to a return
        public bool IfOrphaned { get; set; } = false;

        public bool IsInfiniteLoop { get; set; } = false;
        public bool IsCodeGenerated { get; set; } = false;

        public BasicBlock()
        {
            Id = IdCounter++;
        }
        
        public void MarkCodegenerated(int id)
        {
            if (IsCodeGenerated)
            {
#if DEBUG
                Console.WriteLine("Warning: Function " + id + " using already code generated block " + ToString());
#endif
            }
            IsCodeGenerated = true;
        }
        
        /// <summary>
        /// Once dominance information is computed, compute the immediate (closest) dominator using BFS
        /// </summary>
        public void ComputeImmediateDominator()
        {
            // Use BFS to encounter the closest dominating node guaranteed
            Queue<BasicBlock> queue = new Queue<BasicBlock>(Predecessors);
            while (queue.Count != 0)
            {
                var b = queue.Dequeue();
                if (Dominance.Contains(b))
                {
                    ImmediateDominator = b;
                    if (b != this)
                    {
                        ImmediateDominator.DominanceTreeSuccessors.Add(this);
                    }
                    break;
                }
                foreach (var p in b.Predecessors)
                {
                    queue.Enqueue(p);
                }
            }
        }
        
        /// <summary>
        /// Prerequisite information for global liveness analysis and SSA generation. Determines the variables used from
        /// predecessor blocks (upwards exposed) and variables that are redefined in this block (killed)
        /// </summary>
        public HashSet<Identifier> ComputeKilledAndUpwardExposed()
        {
            var globals = new HashSet<Identifier>();
            var instructions = new List<IInstruction>(PhiFunctions.Values);
            instructions.AddRange(Instructions);
            foreach (var inst in instructions)
            {
                if (!(inst is PhiFunction))
                {
                    foreach (var use in inst.GetUses(true))
                    {
                        if (!KilledIdentifiers.Contains(use))
                        {
                            UpwardExposedIdentifiers.Add(use);
                            globals.Add(use);
                        }
                    }
                }
                foreach(var def in inst.GetDefines(true))
                {
                    KilledIdentifiers.Add(def);
                }
            }
            return globals;
        }
        
        public string PrintBlock(int indentLevel, bool infLoopPrint = false)
        {
            string ret = "";
            int count = (IsInfiniteLoop && !infLoopPrint) ? 1 : Instructions.Count;
            int begin = (IsInfiniteLoop && infLoopPrint) ? 1 : 0;
            for (int j = begin; j < count; j++)
            {
                var inst = Instructions[j];
                for (int i = 0; i < indentLevel; i++)
                {
                    ret += "\t";
                }
                ret += inst.WriteLua(indentLevel);
                if (!(inst is IfStatement) && j != Instructions.Count - 1)
                {
                    ret += "\n";
                }
            }
            return ret;
        }
        
        public string ToStringWithLoop()
        {
            var ret = $@"basicblock_{Id}:";
            if (IsLoopHead)
            {
                ret += " (Loop head";
                if (LoopType != LoopType.LoopNone)
                {
                    if (LoopType == LoopType.LoopPretested)
                    {
                        ret += ": pretested";
                    }
                    else if (LoopType == LoopType.LoopPostTested)
                    {
                        ret += ": posttested";
                    }
                    else if (LoopType == LoopType.LoopEndless)
                    {
                        ret += ": endless";
                    }
                }
                if (LoopLatches.Count > 0)
                {
                    ret += $@" Latch: ";
                    foreach (var latch in LoopLatches)
                    {
                        ret += $@"{latch}, ";
                    }
                }
                if (LoopFollow != null)
                {
                    ret += $@" LoopFollow: {LoopFollow}";
                }
                if (Follow != null)
                {
                    ret += $@" IfFollow: {Follow}";
                }
                ret += ")";
            }
            else if (Follow != null)
            {
                ret += $@"(IfFollow: {Follow})";
            }
            else if (LoopBreakFollow != null)
            {
                ret += $@"(BreakFollow: {LoopBreakFollow})";
            }
            return ret;
        }
        
        public override string ToString()
        {
            return $@"basicblock_{Id}:";
        }
    }
}