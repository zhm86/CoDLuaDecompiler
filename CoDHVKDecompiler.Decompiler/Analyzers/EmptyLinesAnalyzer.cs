using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class EmptyLinesAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 2; i++)
                {
                    var inst = b.Instructions[i];
                    if (inst is Assignment {Right: FunctionCall fc} && fc.Function.ToString().Contains("addElement") && fc.Arguments.Count == 2 && 
                        fc.Arguments[1] is IdentifierReference ir && b.Instructions[i + 1] is Assignment a22 && a22.Left.Count == 1 && 
                        a22.Left[0].TableIndices.Count == 1 && a22.Left[0].TableIndices[0] is Constant
                        {
                            Type: ValueType.String
                        } c)
                    {
                        var inst2 = b.Instructions[i + 1];
                        if (inst2 is Assignment a2 && fc.Arguments[1].ToString() == a2.Right.ToString() && a2.Left[0].Identifier.ToString() == fc.Arguments[0].ToString())
                        {
                            b.Instructions.Insert(i + 2, new NewLine());
                        }
                    }
                }
            }
        }
    }
}