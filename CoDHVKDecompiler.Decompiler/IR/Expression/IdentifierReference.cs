using System;
using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using ValueType = CoDHVKDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDHVKDecompiler.Decompiler.IR.Expression
{
    public class IdentifierReference : IExpression
    {
        public Identifier Identifier { get; set; }
        // Each entry represents a new level of indirection for multidimensional arrays
        public List<IExpression> TableIndices = new List<IExpression>();
        public bool DotNotation { get; set; } = false;
        public bool HasIndex
        {
            get => TableIndices.Any();
        }
        
        public IdentifierReference(Identifier id)
        {
            Identifier = id;
        }
        
        public IdentifierReference(Identifier id, IExpression index)
        {
            Identifier = id;
            TableIndices = new List<IExpression>() { index };
        }
        
        public override void Parenthesize()
        {
            foreach (var idx in TableIndices)
            {
                idx.Parenthesize();
            }
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            if (!regOnly || Identifier.IdentifierType == IdentifierType.Register)
            {
                ret.Add(Identifier);
            }
            foreach (var idx in TableIndices)
            {
                ret.UnionWith(idx.GetUses(regOnly));
            }
            return ret;
        }
        
        public override void RenameUses(Identifier orig, Identifier newid)
        {
            if (Identifier == orig)
            {
                Identifier = newid;
                Identifier.UseCount++;
            }
            foreach (var idx in TableIndices)
            {
                idx.RenameUses(orig, newid);
            }
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool changed = false;
            for (int i = 0; i < TableIndices.Count; i++)
            {
                if (ShouldReplace(orig, TableIndices[i]))
                {
                    TableIndices[i] = sub;
                    changed = true;
                }
                else
                {
                    changed = TableIndices[i].ReplaceUses(orig, sub);
                }
            }
            if (orig == Identifier && sub is IdentifierReference ir && ir.TableIndices.Count == 0)
            {
                Identifier = ir.Identifier;
                changed = true;
            }
            else if (orig == Identifier && sub is IdentifierReference ir2 && ir2.TableIndices.Count > 0)
            {
                Identifier = ir2.Identifier;
                var newl = new List<IExpression>();
                newl.AddRange(ir2.TableIndices);
                newl.AddRange(TableIndices);
                TableIndices = newl;
                changed = true;
            }
            return changed;
        }

        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            foreach (var idx in TableIndices)
            {
                ret.AddRange(idx.GetExpressions());
            }
            return ret;
        }
        
        public override int GetLowestConstantId()
        {
            var id = Identifier.ConstantId;
            foreach (var idx in TableIndices)
            {
                var nid = idx.GetLowestConstantId();
                if (id == -1)
                {
                    id = nid;
                }
                else if (nid != -1)
                {
                    id = Math.Min(id, idx.GetLowestConstantId());
                }
            }
            return id;
        }
        
        public override string ToString()
        {
            string ret = Identifier.ToString();
            foreach (var idx in TableIndices)
            {
                if (idx is Constant {Type: ValueType.String} c)
                {
                    ret += "." + c.String;
                }
                else
                {
                    ret += $@"[{idx}]";
                }
            }
            return ret;
        }
    }
}