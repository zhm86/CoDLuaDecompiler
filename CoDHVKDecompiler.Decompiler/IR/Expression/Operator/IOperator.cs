namespace CoDHVKDecompiler.Decompiler.IR.Expression.Operator
{
    public interface IOperator
    {
        int GetPrecedence();
        void SetHasParentheses(bool paren);
    }
}