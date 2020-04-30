using System.Collections.Generic;
using System.IO;
using DSLuaDecompiler.LuaFileTypes.Structures;

namespace DSLuaDecompiler.LuaFileTypes
{
    public abstract class LuaFile
    {
        public string FilePath { get; set; }
        public BinaryReader Reader { get; set; }
        public FileHeader Header { get; set; }
        public Function MainFunction { get; set; }
        public abstract Dictionary<int, LuaOpCode> OpCodeTable { get; }
        public abstract void ReadHeader();
        public abstract void ReadFunctions();
        public abstract void ReadFunctionHeader(Function function);
        public abstract Instruction ReadInstruction(Function function);
        public abstract void ReadConstants(Function function);
        public abstract void ReadFunctionFooter(Function function);
        public abstract void ReadSubFunctions(Function function);
        
        public LuaFile(string filePath, BinaryReader reader)
        {
            FilePath = filePath;
            Reader = reader;
            
            ReadHeader();
            ReadFunctions();
            
            
        }
        
        public static LuaFile LoadLuaFile(string filePath, Stream stream)
        {
            var reader = new BinaryReader(stream);
            // TODO: Check from what CoD this lua file is
            return new LuaFileT7(filePath, reader);
        }
    }
}