
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.Analyzers
{
    public interface IAnalyzer
    {
        public void Analyze(Function f);
    }
}