using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class RemovingDataAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            for (int i = f.Instructions.Count - 1; i > 0; i--)
            {
                if (f.Instructions[i] is Data)
                {
                    f.Instructions.RemoveAt(i);
                    i++;
                }
                else if (f.Instructions[i] is Close)
                {
                    f.Instructions.RemoveAt(i);
                }
            }
        }
    }
}