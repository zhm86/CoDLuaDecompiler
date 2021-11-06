using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.Analyzers;
using CoDLuaDecompiler.Decompiler.InstructionConverters;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;

namespace CoDLuaDecompiler.Decompiler.LuaFile
{
    public interface ILuaFile
    {
        public int FunctionIdCounter { get; set; }
        IFileHeader Header { get; }
        IList<IHavokLuaConstant> Constants { get; }
        ILuaFunction MainFunction { get; }
        IInstructionConverter InstructionConverter { get; }
        IAnalyzerList AnalyzerList { get; }
        IAnalyzerList FileAnalyzerList { get; }
    }
}