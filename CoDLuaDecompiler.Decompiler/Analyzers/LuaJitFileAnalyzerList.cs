using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.Analyzers.Shared;

namespace CoDLuaDecompiler.Decompiler.Analyzers
{
    public class LuaJitFileAnalyzerList : IAnalyzerList
    {
        public List<IAnalyzer> GetAnalyzers()
        {
            return new List<IAnalyzer>()
            {
                new PrePostLoadFuncAnalyzer(),
                new PostRequireStatementsNewLine(),
                new UIModelFunctionValueVarNamesAnalyzer(),
            };
        }
    }
}