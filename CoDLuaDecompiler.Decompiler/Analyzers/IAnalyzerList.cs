using System.Collections.Generic;

namespace CoDLuaDecompiler.Decompiler.Analyzers
{
    public interface IAnalyzerList
    {
        List<IAnalyzer> GetAnalyzers();
    }
}