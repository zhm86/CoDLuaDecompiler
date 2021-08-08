using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.LuaJit
{
    public class LuaJitIndeterminateArgumentsAnalyzer : IAnalyzer
    {
        /**
         * Fixes the function calls that have a function call as last argument
         */
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                bool changes = true;
                while (changes)
                {
                    changes = false;
                    for(int i = 0; i < b.Instructions.Count; i++)
                    {
                        if (b.Instructions[i] is Assignment {Right: FunctionCall {IsIndeterminateArgumentCount: true} fc} && 
                            b.Instructions[i + 1] is Assignment {Right: FunctionCall fc2})
                        {
                            changes = true;
                            fc2.Arguments.Add(fc);
                            b.Instructions.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}