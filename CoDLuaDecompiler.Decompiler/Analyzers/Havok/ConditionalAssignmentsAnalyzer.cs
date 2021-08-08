using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class ConditionalAssignmentsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            for (int i = 0; i < f.Instructions.Count - 6; i++)
            {
                if (f.Instructions[i] is Jump {Conditional: true} j1 &&
                    f.Instructions[i + 1] is Assignment a1 && a1.Left.Count == 1 && a1.Left[0] is { } ir1 && a1.Right is Constant {Type: ValueType.Boolean, Boolean: false} &&
                    f.Instructions[i + 2] is Jump {Conditional: false} j2 && 
                    f.Instructions[i + 3] is Label l1 && l1 == j1.Dest && l1.UsageCount == 1 &&
                    f.Instructions[i + 4] is Assignment a2 && a2.Left.Count == 1 && a1.Left[0] is { } ir2 && a2.Right is Constant {Type: ValueType.Boolean, Boolean: true} &&
                    ir1.Identifier == ir2.Identifier &&
                    f.Instructions[i + 5] is Label label2 && label2 == j2.Dest
                )
                {
                    if (j1.Condition is BinOp bop)
                    {
                        bop.NegateCondition();
                    }

                    f.Instructions[i] = new Assignment(ir1, j1.Condition);
                    f.Instructions.RemoveRange(i + 1, 4);
                }
            }
        }
    }
}