using System.Globalization;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.LuaJit
{
    public class LuaJitConstant : ILuaJitConstant
    {
        public LuaJitConstant()
        {
            Type = LuaJitConstantType.Nil;
        }

        public LuaJitConstant(string str)
        {
            Type = LuaJitConstantType.String;
            StringValue = str;
        }

        public LuaJitConstant(double num)
        {
            Type = LuaJitConstantType.Number;
            NumberValue = num;
        }
        
        public LuaJitConstant(ulong num)
        {
            if (Decompiler.HashEntries.ContainsKey(HashValue))
            {
                StringValue = Decompiler.HashEntries[HashValue];
                Type = LuaJitConstantType.String;
            }
            else
            {
                Type = LuaJitConstantType.Hash;
                HashValue = num;
            }
        }

        public LuaJitConstant(LuaJitFunction func)
        {
            Type = LuaJitConstantType.Function;
            Function = func;
        }

        public LuaJitConstant(LuaJitTable luaTable)
        {
            Type = LuaJitConstantType.Table;
            Table = luaTable;
        }

        public LuaJitConstant(bool boolean)
        {
            Type = LuaJitConstantType.Boolean;
            BoolValue = boolean;
        }
        
        public override string ToString()
        {
            return Type switch
            {
                LuaJitConstantType.String => StringValue,
                LuaJitConstantType.Number => NumberValue.ToString(CultureInfo.InvariantCulture),
                LuaJitConstantType.Nil => "nil",
                LuaJitConstantType.Boolean => BoolValue ? "true" : "false",
                LuaJitConstantType.Function => Function.ToString(),
                LuaJitConstantType.Table => Table.ToString(),
                //LuaJitConstantType.Hash => $"0x{HashValue & 0xFFFFFFFFFFFFFFF:X}",
                _ => "NULL"
            };
        }
    }
}