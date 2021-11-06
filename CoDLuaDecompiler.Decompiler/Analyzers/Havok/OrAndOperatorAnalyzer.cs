using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Removed the if statement for the or + and operator and changes it into an binop expression, helps a lot with propagation later on
    /// Changed:
    /// local var2 = var1
    /// if var2 then -- if not var2 then
    ///     var2 = ""
    /// end
    /// local var3 = var2
    /// Into:
    /// local var3 = var1 and "y" -- local var3 = var1 or "y"
    /// </summary>
    public class OrAndOperatorAnalyzer : IAnalyzer
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
                        f.Instructions[i + 1] is Jump {Conditional: true} j && 
                        f.Instructions[i + 2] is Assignment a2 && a2.Left.Count == 1 &&
                        f.Instructions[i + 3] is Label l && l == j.Dest)
                    {
                        IdentifierReference ir = null;
                        BinOperationType type = BinOperationType.None;
                        if (j.Condition is UnaryOp uo && uo.OperationType == UnOperationType.OpNot &&
                            uo.Expression is IdentifierReference ir2)
                        {
                            ir = ir2;
                            type = BinOperationType.OpOr;
                        }
                        else if (j.Condition is IdentifierReference ir3)
                        {
                            ir = ir3;
                            type = BinOperationType.OpAnd;
                        }
                        
                        if (ir == null || type == BinOperationType.None || !(a.Left[0].Identifier == ir.Identifier && a2.Left[0].Identifier == ir.Identifier))
                            return;
                        
                        f.Instructions.Insert(f.Instructions.IndexOf(j.Dest), new Assignment(ir.Identifier, new BinOp(new IdentifierReference(a.Left[0].Identifier), a2.Right, type))
                        {
                            LocalAssignments = a2.LocalAssignments,
                            LineLocation = a2.LineLocation
                        });

                        if (l.UsageCount > 1)
                            l.UsageCount--;
                        else
                            f.Instructions.Remove(l);
                        f.Instructions.Remove(j);
                        f.Instructions.Remove(a2);
                        
                        changes = true;
                    }
                }
            }
        }
    }
}