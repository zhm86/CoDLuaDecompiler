using System;
using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class Concat : IExpression, IOperator
    {
        public List<IExpression> Expressions { get; set; }
        public bool HasParentheses { get; set; }
        
        public Concat(List<IExpression> expr)
        {
            Expressions = expr;
        }
        
        public int GetPrecedence()
        {
            return 4;
        }
        
        public override void Parenthesize()
        {
            foreach (var expr in Expressions)
            {
                if (expr is IOperator op && op.GetPrecedence() > GetPrecedence())
                {
                    op.SetHasParentheses(true);
                }
            }
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            foreach (var arg in Expressions)
            {
                ret.UnionWith(arg.GetUses(regOnly));
            }
            return ret;
        }

        public override void RenameUses(Identifier orig, Identifier newId)
        {
            foreach (var arg in Expressions)
            {
                arg.RenameUses(orig, newId);
            }
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool replaced = false;
            for (int i = 0; i < Expressions.Count; i++)
            {
                if (ShouldReplace(orig, Expressions[i]))
                {
                    Expressions[i] = sub;
                    replaced = true;
                }
                else
                {
                    replaced = replaced || Expressions[i].ReplaceUses(orig, sub);
                }
            }
            return replaced;
        }

        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            foreach(var exp in Expressions)
            {
                ret.AddRange(exp.GetExpressions());
            }
            return ret;
        }

        public void SetHasParentheses(bool paren)
        {
            HasParentheses = paren;
        }
        
        public override int GetLowestConstantId()
        {
            var id = int.MaxValue;
            foreach (var e in Expressions)
            {
                var nid = e.GetLowestConstantId();
                if (nid != -1)
                {
                    id = Math.Min(id, e.GetLowestConstantId());
                }
            }
            return id != int.MaxValue ? id : -1;
        }

        public override string ToString()
        {
            string ret = "";

            // Pattern match special lua this call
            if (HasParentheses)
            {
                ret += "(";
            }
            for (int i = 0; i < Expressions.Count; i++)
            {
                ret += Expressions[i].ToString();
                if (i != Expressions.Count - 1)
                {
                    ret += " .. ";
                }
            }
            if (HasParentheses)
            {
                ret += ")";
            }
            return ret;
        }
    }
}