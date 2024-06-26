using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Removes multiple 2 nots after eachother
    /// </summary>
    public class MultipleNotsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                foreach (var instr in b.Instructions)
                {
                    instr.GetExpressions().ForEach(SearchUnaryOp);
                }
            }
        }

        private void SearchUnaryOp(IExpression expression)
        {
            if (expression == null)
                return;
            
            if (expression is UnaryOp {OperationType: UnOperationType.OpNot, Expression: UnaryOp
                {
                    OperationType: UnOperationType.OpNot
                } uo2
            } uo)
            {
                uo.IsImplicit = true;
                uo2.IsImplicit = true;
            }
            
            expression.GetExpressions().ForEach(e =>
            {
                if (e != expression && e != null)
                    SearchUnaryOp(e);
            });
        }
    }
}