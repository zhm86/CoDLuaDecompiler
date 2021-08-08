using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.Havok;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok
{
    public class HavokLuaFunctionT6 : HavokLuaFunction
    {
        public HavokLuaFunctionT6(ILuaFile luaFile, BinaryReader reader) : base(luaFile, reader)
        {
        }
        
        protected override FunctionHeader ReadFunctionHeader()
        {
            var header = new HavokFunctionHeader()
            {
                UpvaluesCount = Reader.ReadInt32(),
                ParameterCount = Reader.ReadInt32(),
                UsesVarArg = Reader.ReadByte() == 1,
                RegisterCount = Reader.ReadInt32(),
                InstructionCount = Reader.ReadInt32()
            };
            // Add some padding
            var extra = 4 - (int)Reader.BaseStream.Position % 4;
            if (extra > 0 && extra < 4)
            {
                Reader.ReadBytes(extra);
            }

            return header;
        }
        
        protected override void ReadConstants()
        {
            var constants = new List<IHavokLuaConstant>();

            var constantCount = Reader.ReadInt32();
            for (var i = 0; i < constantCount; i++)
            {
                constants.Add(new HavokLuaConstantT6(Reader));
            }

            Constants = constants;
        }
        
        protected override List<ILuaFunction> ReadChildFunctions()
        {
            var childFunctions = new List<ILuaFunction>();

            for (var i = 0; i < Footer.SubFunctionCount; i++)
            {
                childFunctions.Add(new HavokLuaFunctionT6(LuaFile, Reader));
            }

            return childFunctions;
        }
    }
}