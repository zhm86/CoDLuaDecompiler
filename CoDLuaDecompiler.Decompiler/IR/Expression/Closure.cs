using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class Closure : IExpression
    {
        public Function Function { get; set; }
        public Closure(Function fun)
        {
            Function = fun;
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            return Function.UpvalueBindings.ToHashSet();
        }
        
        public override void RenameUses(Identifier orig, Identifier newId)
        {
            for (int i = 0; i < Function.UpvalueBindings.Count; i++)
            {
                if (Function.UpvalueBindings[i].Name == orig.Name)
                {
                    Function.UpvalueBindings[i] = newId;
                    newId.IsClosureBound = true;
                }
            }
        }

        public override string ToString()
        {
            return Function.ToString();
        }
    }
}