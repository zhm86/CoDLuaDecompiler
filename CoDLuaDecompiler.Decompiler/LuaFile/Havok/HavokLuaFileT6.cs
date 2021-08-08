using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Havok
{
    public class HavokLuaFileT6 : HavokLuaFile
    {
        public HavokLuaFileT6(BinaryReader reader) : base(reader)
        {
        }
        
        protected override ILuaFunction ReadFunctions()
        {
            return new HavokLuaFunctionT6(this, Reader);
        }
    }
}