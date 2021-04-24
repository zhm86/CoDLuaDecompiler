using System;
using System.Globalization;
using System.IO;

namespace CoDHVKDecompiler.Common.LuaConstant
{
    public abstract class LuaConstant : ILuaConstant
    {
        // private variables
        protected readonly BinaryReader Reader;

        protected LuaConstant(BinaryReader reader)
        {
            Reader = reader;
            Parse();
        }

        private void Parse()
        {
            Type = (ConstantType) Reader.ReadByte();

            switch (Type)
            {
                case ConstantType.TString:
                    StringValue = ReadString();
                    break;
                case ConstantType.TNumber:
                    NumberValue = ReadNumber();
                    break;
                case ConstantType.TBoolean:
                    BoolValue = ReadBool();
                    break;
                case ConstantType.THash:
                    HashValue = ReadHash();
                    break;
                case ConstantType.TNil:
                    break;
                case ConstantType.TLightUserData:
                case ConstantType.TTable:
                case ConstantType.TFunction:
                case ConstantType.TUserData:
                case ConstantType.TThread:
                case ConstantType.TIFunction:
                case ConstantType.TCFunction:
                case ConstantType.TUI64:
                case ConstantType.TStruct:
                case ConstantType.TUnk:
                    throw new NotImplementedException();
                    break;
                default:
                    break; // throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract string ReadString();
        protected abstract double ReadNumber();
        protected abstract bool ReadBool();
        protected abstract ulong ReadHash();

        public override string ToString()
        {
            return Type switch
            {
                ConstantType.TString => StringValue,
                ConstantType.TNumber => NumberValue.ToString(CultureInfo.InvariantCulture),
                ConstantType.TNil => "nil",
                ConstantType.TBoolean => BoolValue ? "true" : "false",
                ConstantType.THash => $"0x{HashValue & 0xFFFFFFFFFFFFFFF:X}",
                _ => "NULL"
            };
        }
    }
}