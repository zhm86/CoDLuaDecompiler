using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class WidgetEmptyLinesAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 2; i++)
                {
                    var inst = b.Instructions[i];
                    if (inst is Assignment {Right: FunctionCall fc} a && fc.Function is IdentifierReference ir && ir.TableIndices.Count == 1 && ir.TableIndices[0] is Constant c1 && 
                        c1.Type == ValueType.String && c1.String == "addElement" &&
                        b.Instructions[i + 1] is Assignment a22 && a22.Left.Count == 1 && 
                        a22.Left[0].TableIndices.Count == 1 && a22.Left[0].TableIndices[0] is Constant
                        {
                            Type: ValueType.String
                        } c &&
                        a22.Right is IdentifierReference ir2 && fc.Arguments[0] is IdentifierReference ir1 && ir2.Identifier == ir1.Identifier)
                    {
                        if (!(b.Instructions[b.Instructions.IndexOf(ir2.Identifier.DefiningInstruction) - 1] is NewLine))
                        {
                            b.Instructions.Insert(b.Instructions.IndexOf(ir2.Identifier.DefiningInstruction), new NewLine());
                            i++;
                        }
                        b.Instructions.Insert(i + 2, new NewLine());
                    }
                }
            }
        }
    }
}