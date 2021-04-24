using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class MultiBoolAssignmentAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            for (int i = 0; i < f.Instructions.Count - 2; i++)
            {
                if (f.Instructions[i] is Assignment a1 && a1.Left.Any() && !a1.Left[0].HasIndex &&
                    a1.Right is Constant {Type: ValueType.Boolean, Boolean: false} &&
                    f.Instructions[i + 1] is Assignment a2 && a2.Left.Any() && !a2.Left[0].HasIndex &&
                    a2.Right is Constant {Type: ValueType.Nil})
                {
                    a1.Left.AddRange(a2.Left);
                    f.Instructions.RemoveAt(i + 1);
                }
            }
        }
    }
}