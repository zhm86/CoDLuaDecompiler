using System;
using System.IO;
using System.Text;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok
{
    public class HavokLuaConstantT6 : HavokLuaConstant
    {
        public HavokLuaConstantT6(BinaryReader reader) : base(reader)
        {
        }

        protected override string ReadString()
        {
            var length = Reader.ReadInt32();
            var str = Encoding.ASCII.GetString(Reader.ReadBytes(length - 1));
            Reader.ReadByte();
            return str;
        }

        protected override double ReadNumber()
        {
            return Math.Round(Reader.ReadSingle(), 2, MidpointRounding.ToEven);
        }

        protected override bool ReadBool()
        {
            return Reader.ReadByte() == 1;
        }

        protected override ulong ReadHash()
        {
            return Reader.ReadUInt64();
        }
    }
}