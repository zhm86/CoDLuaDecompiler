using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class ListAssignment : IExpression
    {
        public IExpression Left { get; set; }
        public IExpression Right { get; set; }

        public ListAssignment(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public override void Parenthesize()
        {
            Left.Parenthesize();
            Right.Parenthesize();
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            ret.UnionWith(Left.GetUses(regOnly));
            ret.UnionWith(Right.GetUses(regOnly));
            return ret;
        }
        
        public override void RenameUses(Identifier orig, Identifier newId)
        {
            Left.RenameUses(orig, newId);
            Right.RenameUses(orig, newId);
        }
        
        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool replaced = false;
            if (ShouldReplace(orig, Left))
            {
                Left = sub;
                replaced = true;
            }
            else
            {
                replaced = replaced ||Left.ReplaceUses(orig, sub);
            }
            if (ShouldReplace(orig, Right))
            {
                Right = sub;
                replaced = true;
            }
            else
            {
                replaced = replaced || Right.ReplaceUses(orig, sub);
            }
            return replaced;
        }
        
        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            ret.AddRange(Left.GetExpressions());
            ret.AddRange(Right.GetExpressions());
            return ret;
        }

        public override string ToString()
        {
            // If it's a string constant, we need it without the "'s and with other values we need a casket
            string left;
            if (Left is Constant c && c.String != null)
                left = Left.ToString()?.Replace("\"", "");
            else
                left = $"[{Left}]";
            return $"{left} = {Right}";
        }
    }
}