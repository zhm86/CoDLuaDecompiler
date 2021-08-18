using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.Analyzers.Havok;
using CoDLuaDecompiler.Decompiler.Analyzers.Shared;

namespace CoDLuaDecompiler.Decompiler.Analyzers
{
    public class HavokFileAnalyzerList : IAnalyzerList
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