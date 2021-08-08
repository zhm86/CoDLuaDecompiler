using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class PhiFunction : IInstruction
    {
        public Identifier Left { get; set; }
        public List<Identifier> Right { get; set; }

        public PhiFunction(Identifier left, List<Identifier> right)
        {
            Left = left;
            Right = right;
        }
        
        public override HashSet<Identifier> GetDefines(bool regOnly)
        {
            return new HashSet<Identifier>(new List<Identifier>() { Left });
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var uses = new HashSet<Identifier>();
            foreach (var id in Right)
            {
                uses.UnionWith(Right);
            }
            return uses;
        }
        
        public override void RenameDefines(Identifier orig, Identifier newId)
        {
            if (Left == orig)
            {
                Left = newId;
            }
        }
        
        public override void RenameUses(Identifier orig, Identifier newId)
        {
            for (int i = 0; i < Right.Count; i++)
            {
                if (Right[i] == orig)
                {
                    if (orig != null)
                    {
                        orig.UseCount--;
                    }
                    Right[i] = newId;
                    if (newId != null)
                    {
                        newId.UseCount++;
                    }
                }
            }
        }
        
        public override string ToString()
        {
            string ret = $@"{Left} = phi(";
            for (int i = 0; i < Right.Count; i++)
            {
                if (Right[i] != null)
                {
                    ret += Right[i].ToString();
                }
                else
                {
                    ret += "undefined";
                }
                if (i != Right.Count - 1)
                {
                    ret += ", ";
                }
            }
            ret += ")";
            return ret;
        }
    }
}