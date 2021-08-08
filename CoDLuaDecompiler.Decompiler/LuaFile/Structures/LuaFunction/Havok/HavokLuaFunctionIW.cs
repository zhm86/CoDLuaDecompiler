using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok
{
    public class HavokLuaFunctionIW : HavokLuaFunction
    {
        public HavokLuaFunctionIW(ILuaFile luaFile, BinaryReader reader) : base(luaFile, reader)
        {
        }
        
        protected override FunctionFooter ReadFunctionFooter()
        {
            return new FunctionFooter
            {
                Unknown1 = Reader.ReadInt32(),
                SubFunctionCount = Reader.ReadInt32(),
            };
        }
        
        protected override List<ILuaFunction> ReadChildFunctions()
        {
            var childFunctions = new List<ILuaFunction>();

            for (var i = 0; i < Footer.SubFunctionCount; i++)
            {
                childFunctions.Add(new HavokLuaFunctionIW(LuaFile, Reader));
            }

            return childFunctions;
        }
    }
}