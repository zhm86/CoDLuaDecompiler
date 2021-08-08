using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.LuaJit
{
    public class LuaJitFileMw : LuaJitFile
    {
        public LuaJitFileMw(BinaryReader reader) : base(reader)
        {
        }

        protected override LuaJitFunction ReadFunction()
        {
            return new LauJitFunctionMW(this, Reader);
        }
    }
}