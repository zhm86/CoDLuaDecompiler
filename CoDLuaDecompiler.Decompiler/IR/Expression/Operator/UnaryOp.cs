using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Expression.Operator
{
    public class UnaryOp : IExpression, IOperator
    {
        public IExpression Expression { get; set; }
        public UnOperationType OperationType { get; set; }
        public bool HasParentheses { get; set; }
        public bool IsImplicit { get; set; } = false;
        
        public UnaryOp(IExpression exp, UnOperationType op)
        {
            Expression = exp;
            OperationType = op;
        }

        public int GetPrecedence()
        {
            return 1;
        }

        public void SetHasParentheses(bool paren)
        {
            HasParentheses = paren;
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            ret.UnionWith(Expression.GetUses(regOnly));
            return ret;
        }

        public override void RenameUses(Identifier orig, Identifier newId)
        {
            Expression.RenameUses(orig, newId);
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            if (ShouldReplace(orig, Expression))
            {
                Expression = sub;
                return true;
            }
            else
            {
                return Expression.ReplaceUses(orig, sub);
            }
        }

        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            ret.AddRange(Expression.GetExpressions());
            return ret;
        }
        
        public override int GetLowestConstantId()
        {
            return Expression.GetLowestConstantId();
        }

        public override string ToString()
        {
            string op = "";
            switch (OperationType)
            {
                case UnOperationType.OpNegate:
                    op = "-";
                    break;
                case UnOperationType.OpNot:
                    op = "not ";
                    break;
                case UnOperationType.OpLength:
                    op = "#";
                    break;
            }

            if (IsImplicit)
                op = "";
            string ret = "";
            if (HasParentheses)
            {
                ret += "(";
            }
            ret += $@"{op}{Expression}";
            if (HasParentheses)
            {
                ret += ")";
            }
            return ret;
        }
        
        public override void Parenthesize()
        {
            // If left has a lower precedence than this op, then add parantheses to evaluate it first
            if (Expression is IOperator op1 && op1.GetPrecedence() > GetPrecedence())
            {
                op1.SetHasParentheses(true);
            }
            Expression.Parenthesize();
        }
    }
}