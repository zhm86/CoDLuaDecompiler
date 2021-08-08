using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;

namespace CoDLuaDecompiler.Decompiler.InstructionConverters
{
    public interface IInstructionConverter
    {
        void Convert(Function function, ILuaFunction luaFunction);
    }
}