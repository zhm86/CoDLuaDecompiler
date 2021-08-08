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
        }
    }
}