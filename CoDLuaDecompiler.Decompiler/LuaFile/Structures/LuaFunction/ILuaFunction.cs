using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction
{
    public interface ILuaFunction
    {
        ILuaFile LuaFile { get; }
        FunctionHeader Header { get; }
        long FunctionPos { get; }
        long FunctionLength { get; }
        List<Instruction> Instructions { get; }
        List<ILuaFunction> ChildFunctions { get; }
        List<Upvalue> Upvalues { get; }

        public Function IRFunction { get; set; }
    }
}