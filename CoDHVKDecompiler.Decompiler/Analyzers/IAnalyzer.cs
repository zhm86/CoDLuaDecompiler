
using CoDHVKDecompiler.Decompiler.IR.Functions;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public interface IAnalyzer
    {
        public void Analyze(Function f);
    }
}