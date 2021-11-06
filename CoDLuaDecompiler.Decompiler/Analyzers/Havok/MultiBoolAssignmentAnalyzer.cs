using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
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
                    if (a1.LocalAssignments == null)
                        a1.LocalAssignments = a2.LocalAssignments;
                    else
                        a1.LocalAssignments.AddRange(a2.LocalAssignments);
                    f.Instructions.RemoveAt(i + 1);
                }
            }
        }
    }
}