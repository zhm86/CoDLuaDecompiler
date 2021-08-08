using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class ParenthesizeAnalyzer : IAnalyzer
    {
        /// <summary>
        /// Inserts parentheses in all the expressions if they are needed (i.e. the result of an operation is used by an operation
        /// with lower precedence: a + b * c + d would become (a + b) * (c + d) for certain expression trees for example
        /// </summary>
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    i.Parenthesize();
                }
            }
        }
    }
}