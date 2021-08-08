using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class IExpression
    {
        public virtual HashSet<Identifier> GetUses(bool regOnly)
        {
            return new HashSet<Identifier>();
        }

        public virtual void RenameUses(Identifier orig, Identifier newId) { }

        public static bool ShouldReplace(Identifier orig, IExpression cand)
        {
            return (cand is IdentifierReference ident && ident.TableIndices.Count == 0 && ident.Identifier == orig);
        }

        public virtual bool ReplaceUses(Identifier orig, IExpression sub) { return false; }

        public virtual void Parenthesize() { return; }

        public virtual List<IExpression> GetExpressions()
        {
            return new List<IExpression>() { this };
        }
        
        public virtual int GetLowestConstantId()
        {
            return -1;
        }
    }
}