using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Havok.Debug
{
    public class FunctionDebugInfo
    {
        public int Id { get; set; }
        public List<int> InstructionLocations { get; set; } = new List<int>();
        public List<Local> VariableNames { get; set; } = new List<Local>();
        public List<string> UpvalueNames { get; set; } = new List<string>();
        public string Filename { get; set; }
        public string ChunkName { get; set; }
        public int FunctionStart { get; set; }
        public int FunctionEnd { get; set; }
    }
}