using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// We are making the returns at the end of functions implicit because they don't need to be printed
    /// We're not removing them as it will mess up jumps
    /// </summary>
    public class UnneededReturnAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            if (f.Instructions.Count > 0 && f.Instructions.Last() is Return r && r.Expressions.Count == 0)
            {
                r.IsImplicit = true;
            }
        }
    }
}