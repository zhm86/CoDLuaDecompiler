using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.Havok;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok
{
    public class HavokLuaFunctionT7 : HavokLuaFunction
    {
        public override IHavokLuaOpCodeTable OpCodeTable => new HavokLuaOpCodeTableT7();
        
        public HavokLuaFunctionT7(ILuaFile luaFile, BinaryReader reader) : base(luaFile, reader)
        {
        }
    }
}