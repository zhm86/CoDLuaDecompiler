using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Finishes implementing the last part of the Lua 5.1 FORLOOP op. I.e. in the block that follows the loop head that doesn't
    /// break the loop insert the following IR: R(A+3) := R(A)
    /// </summary>
    public class Lua51LoopsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                if (b.Instructions.Any() && b.Instructions.Last() is Jump {PostTakenAssignment: { }} jmp)
                {
                    b.Successors[1].Instructions.Insert(0, jmp.PostTakenAssignment);
                    jmp.PostTakenAssignment.PropagateAlways = true;
                    jmp.PostTakenAssignment.Block = b.Successors[1];
                    jmp.PostTakenAssignment = null;
                }
            }
        }
    }
}