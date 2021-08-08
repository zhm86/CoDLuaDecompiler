using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.LuaJit
{
    public class LuaJitFileBocw : LuaJitFile
    {
        public LuaJitFileBocw(BinaryReader reader) : base(reader)
        {
        }

        protected override LuaJitFunction ReadFunction()
        {
            return new LuaJitFunctionBOCW(this, Reader);
        }
    }
}