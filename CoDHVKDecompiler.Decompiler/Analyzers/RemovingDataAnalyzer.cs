using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
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