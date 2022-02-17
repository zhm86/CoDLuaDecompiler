using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class UnnecessaryReturnsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            if (f.Blocks[^1].Instructions.Count == 0)
            {
                var b = f.Blocks[^2];
                if (b.Instructions[^1] is Return r && r.Expressions.Count == 0)
                {
                    b.Instructions.Remove(r);
                }
            }

            // Remove the weird variable names without anything else
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count; i++)
                {
                    if (b.Instructions[i] is Assignment a && a.Left.Count == 1 && a.Left[0] is {HasIndex: false} && a.Right == null && !a.IsLocalDeclaration)
                    {
                        b.Instructions.Remove(a);
                        i--;
                    }
                }
            }
        }
    }
}