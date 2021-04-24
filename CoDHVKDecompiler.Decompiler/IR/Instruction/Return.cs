using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;

namespace CoDHVKDecompiler.Decompiler.IR.Instruction
{
    public class Return : IInstruction
    {
        public List<IExpression> Expressions { get; set; }
        public bool IsTailReturn { get; set; } = false;
        public bool IsImplicit { get; set; } = false;
        public bool IsIndeterminateReturnCount { get; set; } = false;
        public uint BeginRet { get; set; } = 0;
        
        public Return(List<IExpression> expr)
        {
            Expressions = expr;
        }

        public Return(IExpression expr)
        {
            Expressions = new List<IExpression>() {expr};
        }
        
        public override void Parenthesize()
        {
            Expressions.ForEach(x => x.Parenthesize());
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var uses = new HashSet<Identifier>();
            foreach (var exp in Expressions)
            {
                uses.UnionWith(exp.GetUses(regOnly));
            }
            return uses;
        }
        
        public override void RenameUses(Identifier orig, Identifier newId)
        {
            foreach (var exp in Expressions)
            {
                exp.RenameUses(orig, newId);
            }
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool replace = false;
            for (int i = 0; i < Expressions.Count; i++)
            {
                if (IExpression.ShouldReplace(orig, Expressions[i]))
                {
                    Expressions[i] = sub;
                    replace = true;
                }
                else
                {
                    replace = replace || Expressions[i].ReplaceUses(orig, sub);
                }
            }
            return replace;
        }

        public override List<IExpression> GetExpressions()
        {
            return Expressions.Select(e => e).ToList();
        }
        
        public override string ToString()
        {
            if (IsImplicit)
            {
                return "";
            }
            string ret = "return ";
            for (int i = 0; i < Expressions.Count; i++)
            {
                ret += Expressions[i].ToString();
                if (i != Expressions.Count - 1)
                {
                    ret += ", ";
                }
            }
            return ret;
        }
    }
}