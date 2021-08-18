using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Shared
{
    /**
     * Add a new line after the require statements at the top of the file
     */
    public class PostRequireStatementsNewLine : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool foundRequire = false;
            for (int i = 0; i < f.Blocks[0].Instructions.Count; i++)
            {
                // Check if the instruction is a require statement
                if (f.Blocks[0].Instructions[i] is Assignment a && a.Left.Count == 0 && a.Right is FunctionCall fc &&
                    fc.Arguments.Count == 1 &&
                    fc.Function.ToString() == "require")
                {
                    foundRequire = true;
                }
                else if (foundRequire)
                {
                    // Add the new line at the end of the require statements
                    f.Blocks[0].Instructions.Insert(i, new NewLine());
                    return;
                }
                else
                {
                    return;
                }
            }
        }
    }
}