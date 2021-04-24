using CoDHVKDecompiler.Decompiler.IR.Identifiers;

namespace CoDHVKDecompiler.Decompiler.IR.Expression
{
    public class Constant : IExpression
    {
        public ValueType Type { get; set; }
        public double Number { get; set; }
        public string String { get; set; }
        public bool Boolean { get; set; }
        public ulong Hash { get; set; }

        public int Id { get; set; }
        
        public Constant(double num, int id)
        {
            Type = ValueType.Number;
            Number = num;
            Id = id;
        }

        public Constant(string str, int id)
        {
            Type = ValueType.String;
            String = str.Replace("\n", "\\n");
            Id = id;
        }

        public Constant(bool b, int id)
        {
            Type = ValueType.Boolean;
            Boolean = b;
            Id = id;
        }

        public Constant(ulong h, int id)
        {
            Type = ValueType.Hash;
            Hash = h;
            Id = id;
        }

        public Constant(ValueType typ, int id)
        {
            Type = typ;
            Id = id;
        }
        
        public override string ToString()
        {
            switch (Type)
            {
                case ValueType.Number:
                    return Number.ToString();
                case ValueType.String:
                    return "\"" + String + "\"";
                case ValueType.Boolean:
                    return Boolean ? "true" : "false";
                case ValueType.Hash:
                    return $"0x{Hash & 0xFFFFFFFFFFFFFFF:X}";
                case ValueType.Table:
                    return "{}";
                case ValueType.VarArgs:
                    return "...";
                case ValueType.Nil:
                    return "nil";
            }
            return "";
        }
        
        public override int GetLowestConstantId()
        {
            return Id;
        }
    }
}