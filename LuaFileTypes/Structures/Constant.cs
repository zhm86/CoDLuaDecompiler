using System;
using System.Globalization;

namespace DSLuaDecompiler.LuaFileTypes.Structures
{
    public class Constant
    {
        public ConstantType Type { get; set; }
        public string StringValue { get; set; }
        public double NumberValue { get; set; }
        public bool BoolValue { get; set; }
        public ulong HashValue { get; set; }
        
        public override string ToString()
        {
            if (Type == ConstantType.TString)
                return StringValue; 
            if (Type == ConstantType.TNumber)
                return NumberValue.ToString(CultureInfo.InvariantCulture);
            if (Type == ConstantType.TNil)
                return "nill"; 
            if (Type == ConstantType.TBoolean)
                return BoolValue ? "true" : "false";
            if (Type == ConstantType.THash)
                return $"0x{HashValue & 0xFFFFFFFFFFFFFFF:X}";
            return "NULL";
        }
    }
}