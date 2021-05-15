using System.Collections.Generic;
using System.Text;

namespace CoDHVKDecompiler.Decompiler.IR.Expression
{
    public class IdentifierRefList : IExpression
    {
        public List<IExpression> References { get; set; }

        public IdentifierRefList(List<IExpression> references)
        {
            References = references;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            for(int i = 0; i < References.Count; i++)
            {
                if (i > 0)
                    str.Append(", ");
                str.Append(References[i]);
            }

            return str.ToString();
        }
    }
}