using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class InlineClosuresAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            void SearchClosureFunctionCall(FunctionCall fc)
            {
                foreach (var arg in fc.Arguments)
                {
                    if (arg is Closure c)
                    {
                        c.Function.IsInline = true;
                    }

                    if (arg is InitializerList il)
                    {
                        SearchClosureList(il);
                    }

                    if (arg is FunctionCall fc2)
                    {
                        SearchClosureFunctionCall(fc2);
                    }
                }
            }
            
            void SearchClosureList(InitializerList list)
            {
                foreach (var expression in list.Expressions)
                {
                    if (expression is Closure c)
                    {
                        c.Function.IsInline = true;
                    }

                    if (expression is ListAssignment la && la.Right is Closure c2)
                    {
                        c2.Function.IsInline = true;
                    }
                    
                    if (expression is ListAssignment la2 && la2.Right is InitializerList il)
                    {
                        SearchClosureList(il);
                    }

                    if (expression is InitializerList il2)
                    {
                        SearchClosureList(il2);
                    }
                }
            }
            
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a && a.Right is FunctionCall fc)
                    {
                        SearchClosureFunctionCall(fc);
                    }

                    if (i is Assignment a2 && a2.Right is InitializerList il)
                    {
                        SearchClosureList(il);
                    }
                }
            }
        }
    }
}