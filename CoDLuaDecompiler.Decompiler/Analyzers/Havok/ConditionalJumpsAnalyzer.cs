using System;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Simple analysis pass to recognize lua conditional jumping patterns and merge them into a single instruction
    /// </summary>
    public class ConditionalJumpsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Lua conditional jumps often follow this pattern when naively translated into the IR:
            //   if RegA == b then goto Label_1:
            //   goto Label_2:
            //   Label_1:
            //   ...
            //
            // This pass recognizes and simplifies this into:
            //   if RegA ~= b then goto Label_2:
            //   ...
            //
            // This will greatly simplify the generated control flow graph, so this is done first
            for (int i = 0; i < f.Instructions.Count - 2; i++)
            {
                // Pattern match the prerequisites
                if (f.Instructions[i] is Jump {Conditional: true} jmp1 && f.Instructions[i + 1] is Jump {Conditional: false} jmp2 && f.Instructions[i + 2] is Label shortLabel && jmp1.Dest == shortLabel)
                {
                    switch (jmp1.Condition)
                    {
                        // flip the condition and change the destination to the far jump. Then remove the following goto and label
                        case BinOp op:
                            op.NegateCondition();
                            jmp1.Dest.UsageCount--;
                            f.Instructions.RemoveRange(i + 1, jmp1.Dest.UsageCount <= 0 ? 2 : 1);
                            jmp1.Dest = jmp2.Dest;
                            break;
                        case UnaryOp {OperationType: UnOperationType.OpNot}:
                        case IdentifierReference _:
                            jmp1.Dest.UsageCount--;
                            f.Instructions.RemoveRange(i + 1, jmp1.Dest.UsageCount <= 0 ? 2 : 1);
                            jmp1.Dest = jmp2.Dest;
                            break;
                        default:
                            throw new Exception("Recognized jump pattern does not use a binary op conditional");
                    }
                }
            }
        }
    }
}