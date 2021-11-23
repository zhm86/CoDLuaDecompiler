using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using System.Text;

namespace CoDLuaDecompiler.Decompiler.IR.Expression
{
    public class Constant : IExpression
    {
        private static readonly StringBuilder constStringBuilder = new StringBuilder();

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
            for (int i = 0; i != str.Length; i++)
            {
                constStringBuilder.Append(str[i] switch
                {
                    '\0' => "\\0",
                    '\a' => "\\a",
                    '\t' => "\\t",
                    '\n' => "\\n",
                    '\v' => "\\v",
                    '\r' => "\\r",
                    '\\' => "\\\\",
                    '"' => "\\\"",
                    _ => str[i],
                });
            }
            String = constStringBuilder.ToString();
            constStringBuilder.Length = 0;
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