using System.Collections.Generic;
using System.IO;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction;
using CoDHVKDecompiler.Common.Structures;

namespace CoDHVKDecompiler.Common
{
    public class LuaFileIW : LuaFile
    {
        public LuaFileIW(BinaryReader reader) : base(reader)
        {
        }

        protected override FileHeader ReadHeader()
        {
            var header = new FileHeader()
            {
                Magic = Reader.ReadChars(4).ToString(),
                LuaVersion = Reader.ReadByte(),
                CompilerVersion = Reader.ReadByte(),
                Endianness = Reader.ReadByte(),
                SizeOfInt = Reader.ReadByte(),
                SizeOfSizeT = Reader.ReadByte(),
                SizeOfInstruction = Reader.ReadByte(),
                SizeOfLuaNumber = Reader.ReadByte(),
                IntegralFlag = Reader.ReadByte(),
                GameByte = Reader.ReadByte(),
            };
            
            Reader.ReadByte();
            header.ConstantTypeCount = Reader.ReadInt32();

            return header;
        }

        protected override IList<ILuaConstant> ReadConstants()
        {
            var constants = new List<ILuaConstant>();

            for (var i = 0; i < Header.ConstantTypeCount; i++)
            {
                var type = (ConstantType)Reader.ReadInt32();
                var length = Reader.ReadInt32();
                var constant = Reader.ReadBytes(length);

                //constants.Add(new LuaConstantT6(Reader));
            }

            return constants;
        }

        protected override ILuaFunction ReadFunctions()
        {
            return new LuaFunctionIW(this, Reader);
        }
    }
}