using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Expression.Operator;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Removed the if statement for the or operator and changes it into an binop expression, helps a lot with propagation later on
    /// Changed:
    /// local var2 = var1
    /// if not var2 then
    ///     var2 = ""
    /// end
    /// local var3 = var2
    /// Into:
    /// local var3 = var1 or "y"
    /// </summary>
    public class OrOperatorAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool changes = true;
            while (changes)
            {
                changes = false;
                for (int i = 0; i < f.Instructions.Count - 5; i++)
                {
                    if (f.Instructions[i] is Assignment a && a.Left.Count == 1 &&
                        f.Instructions[i + 1] is Jump {Conditional: true, Condition: UnaryOp {OperationType: UnOperationType.OpNot, Expression: IdentifierReference ir}} j && 
                        a.Left[0].Identifier == ir.Identifier && f.Instructions[i + 2] is Assignment a2 && a2.Left.Count == 1 &&
                        a2.Left[0].Identifier == ir.Identifier && f.Instructions[i + 3] is Label l && l == j.Dest)
                    {
                        if (l.UsageCount > 1)
                            l.UsageCount--;
                        else
                            f.Instructions.RemoveAt(i + 3);
                        f.Instructions.RemoveRange(i + 1, 2);
                        
                        f.Instructions.Insert(i + 1, new Assignment(ir.Identifier, new BinOp(new IdentifierReference(a.Left[0].Identifier), a2.Right, BinOperationType.OpOr)));

                        changes = true;
                    }
                }
            }
        }
    }
}