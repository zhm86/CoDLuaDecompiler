using System;
using System.Collections.Generic;
using System.Text;
using CoDLuaDecompiler.Decompiler.Extensions;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.LuaJit;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class InitializerList : IExpression
    {
        public List<IExpression> Expressions { get; set; } = new List<IExpression>();
        
        public InitializerList()
        {
        }
        
        public override void Parenthesize()
        {
            Expressions.ForEach(x => x.Parenthesize());
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
            foreach (var exp in Expressions)
            {
                ret.AddRange(exp.GetExpressions());
            }
            return ret;
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
            StringBuilder str = new StringBuilder("{");

            bool usePrettyPrint = Expressions.Count > 0;
            if (usePrettyPrint)
            {
                str.Append("\n");
            }

            // Pattern match special lua this call
            for (int i = 0; i < Expressions.Count; i++)
            {
                str.Append("\t" + Expressions[i].ToString().AddIndent(true));
                if (i != Expressions.Count - 1)
                    str.Append(",");
                str.Append("\n");
            }
            
            str.Append("}");
                
            return str.ToString();
        }
    }
}