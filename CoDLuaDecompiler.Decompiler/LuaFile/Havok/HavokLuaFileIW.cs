using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Havok
{
    public class HavokLuaFileIW : HavokLuaFile
    {
        public HavokLuaFileIW(BinaryReader reader) : base(reader)
        {
        }
        
        protected override ILuaFunction ReadFunctions()
        {
            return new HavokLuaFunctionIW(this, Reader);
        }
    }
}