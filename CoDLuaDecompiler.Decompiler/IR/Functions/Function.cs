using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.Extensions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;

namespace CoDLuaDecompiler.Decompiler.IR.Functions
{
    public class Function
    {
        public static int IdCounter = 0;
        public int Id { get; set; }
        public SymbolTable SymbolTable { get; set; }
        public List<Identifier> Parameters { get; set; }
        public Function Parent { get; set; }
        public List<Function> Closures { get; set; }
        public List<IInstruction> Instructions { get; set; }
        public Dictionary<uint, Label> Labels { get; set; }
        public List<Local> ArgumentNames { get; set; } = new List<Local>();
        public bool IsVarArgs { get; set; } = false;

        /// <summary>
        /// The first basic block in which control flow enters upon the function being called
        /// </summary>
        public BasicBlock StartBlock { get; set; }
        /// <summary>
        /// The final (empty) basic block that is the successor to the end of the function and any blocks that end with a return instruction
        /// </summary>
        public BasicBlock EndBlock { get; set; }
        /// <summary>
        /// List of all the blocks for some analyses
        /// </summary>
        public List<BasicBlock> Blocks { get; set; } = new List<BasicBlock>();

        /// <summary>
        /// Identifiers that are used in more than one basic block
        /// </summary>
        public HashSet<Identifier> GlobalIdentifiers { get; set; } = new HashSet<Identifier>();

        public HashSet<Identifier> SSAVariables { get; set; } = new HashSet<Identifier>();

        public bool IsControlFlowGraph { get; set; } = false;
        public bool IsAST { get; set; } = false;
        public int IndentLevel { get; set; } = 0;
        public bool IsInline { get; set; } = false;
        /// <summary>
        /// Upvalue binding symbold from parent closure
        /// </summary>
        public List<Identifier> UpvalueBindings = new List<Identifier>();

        public Function(SymbolTable symbolTable)
        {
            Id = IdCounter++;
            SymbolTable = symbolTable;
            Parameters = new List<Identifier>();
            Closures = new List<Function>();
            Instructions = new List<IInstruction>();
            Labels = new Dictionary<uint, Label>();
        }

        public Function(Function parent, SymbolTable symbolTable) : this(symbolTable)
        {
            Parent = parent;
        }
        
        public Label GetLabel(uint pos)
        {
            if (Labels.ContainsKey(pos))
            {
                Labels[pos].UsageCount++;
                return Labels[pos];
            }

            var label = new Label {OpLocation = (int) pos, UsageCount = 1};
            Labels.Add(pos, label);
            return label;
        }
        
        public List<BasicBlock> PostorderTraversal(bool reverse)
        {
            var ret = new List<BasicBlock>();
            var visited = new HashSet<BasicBlock>();
            visited.Add(EndBlock);

            void Visit(BasicBlock b)
            {
                visited.Add(b);
                foreach (var succ in b.Successors)
                {
                    if (!visited.Contains(succ))
                    {
                        Visit(succ);
                    }
                }
                ret.Add(b);
            }

            Visit(StartBlock);

            if (reverse)
            {
                ret.Reverse();
            }
            return ret;
        }
        
        /// <summary>
        /// Computes the dominance sets for all the nodes as well as the dominance tree
        /// </summary>
        public void ComputeDominance()
        {
            // Start block only has itself in dominance set
            StartBlock.Dominance.Clear();
            StartBlock.DominanceTreeSuccessors.Clear();
            StartBlock.Dominance.Add(StartBlock);

            // All blocks but the start have everything dominate them to begin the algorithm
            for (int i = 1; i < Blocks.Count(); i++)
            {
                Blocks[i].Dominance = new HashSet<BasicBlock>(Blocks);
                Blocks[i].DominanceTreeSuccessors.Clear();
            }

            // Iterative solver of dominance data flow equation
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 1; i < Blocks.Count(); i++)
                {
                    var temp = new HashSet<BasicBlock>(Blocks);
                    foreach (var p in Blocks[i].Predecessors)
                    {
                        temp.IntersectWith(p.Dominance);
                    }
                    temp.UnionWith(new[] { Blocks[i] });
                    if (!temp.SetEquals(Blocks[i].Dominance))
                    {
                        Blocks[i].Dominance = temp;
                        changed = true;
                    }
                }
            }

            // Compute the immediate dominator
            for (int i = 1; i < Blocks.Count(); i++)
            {
                Blocks[i].ComputeImmediateDominator();
            }
        }
        
        public void ComputeGlobalLiveness(HashSet<Identifier> allRegisters)
        {
            foreach (var b in Blocks)
            {
                b.KilledIdentifiers.Clear();
                b.UpwardExposedIdentifiers.Clear();
                b.LiveOut.Clear();
                GlobalIdentifiers.UnionWith(b.ComputeKilledAndUpwardExposed());
            }

            // Compute live out for each block iteratively
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var b in Blocks)
                {
                    var temp = new HashSet<Identifier>();
                    foreach (var succ in b.Successors)
                    {
                        var equation = new HashSet<Identifier>(allRegisters);
                        foreach (var kill in succ.KilledIdentifiers)
                        {
                            equation.Remove(kill);
                        }
                        equation.IntersectWith(succ.LiveOut);
                        equation.UnionWith(succ.UpwardExposedIdentifiers);
                        temp.UnionWith(equation);
                    }
                    if (!b.LiveOut.SetEquals(temp))
                    {
                        b.LiveOut = temp;
                        changed = true;
                    }
                }
            }
        }
        
        public void NumberReversePostorder()
        {
            var ordering = PostorderTraversal(true);
            for (int i = 0; i < ordering.Count(); i++)
            {
                ordering[i].ReversePostorderNumber = i;
            }
        }

        public string PrettyPrint(string funcName = "")
        {
            var str = new StringBuilder();
            // Only add the function header if it isn't the lua file's main function
            if (Parent != null)
            {
                str.Append($"function");
#if DEBUG
                str.Append($" --[[{Id}]]");
#endif
                str.Append($" {funcName}(");
                if (Parameters.Count > 0 || IsVarArgs)
                    str.Append(" ");
                // Add all the parameters
                for (int i = 0; i < Parameters.Count; i++)
                {
                    if (i != 0)
                        str.Append(", ");
                    str.Append(Parameters[i]);
                }
                // Add the vararg if the function has it
                if (IsVarArgs)
                {
                    if (Parameters.Any())
                        str.Append(", ");
                    str.Append("...");
                }
                if (Parameters.Count > 0 || IsVarArgs)
                    str.Append(" ");
                str.Append(")\n");
            }

            if (IsAST)
            {
                var block = StartBlock.PrintBlock(IndentLevel);
                if (Parent != null)
                    block = block.AddIndent();
                str.Append(block + "\n");
            }
            else if (IsControlFlowGraph)
            {
                foreach (var b in Blocks.OrderBy(a => a.Id))
                {
                    if (b == EndBlock)
                    {
                        continue;
                    }
                    str.Append(b.ToStringWithLoop() + "\n");
                    foreach (var inst in b.PhiFunctions.Values)
                    {
                        str.Append(inst + "\n");
                    }
                    foreach (var inst in b.Instructions)
                    {
                        str.Append(inst + "\n");
                    }

                    // Insert an implicit goto for fallthrough blocks if the destination isn't actually the next block
                    var lastinst = (b.Instructions.Count > 0) ? b.Instructions.Last() : null;
                    if (lastinst != null && ((lastinst is Jump j && j.Conditional && b.Successors[0].Id != (b.Id + 1)) ||
                                             (!(lastinst is Jump) && !(lastinst is Return) && b.Successors[0].Id != (b.Id + 1))))
                    {
                        str.Append("(goto " + b.Successors[0] + ")" + "\n");
                    }
                }
            }
            else
            {
                for(int n = 0; n < Instructions.Count; n++)
                {
                    var inst = Instructions[n];
                    str.Append($@"{n}-{inst.OpLocation:D3} ");
                    str.Append(inst + "\n");
                }
            }
            if (Parent != null)
            {
                str.Append("end");
                if (!IsInline)
                    str.Append("\n");
            }
            return str.ToString();
        }

        public override string ToString()
        {
            return PrettyPrint();
        }
    }
}