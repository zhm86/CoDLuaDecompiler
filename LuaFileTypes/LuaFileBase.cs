using System;
using System.Collections.Generic;
using System.IO;
using DSLuaDecompiler.LuaFileTypes.Structures;
using PhilLibX.IO;

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
            reader.Seek(0, SeekOrigin.Begin);
            FilePath = filePath;
            Reader = reader;
            
            ReadHeader();
            ReadFunctions();
            
            
        }
        
        public static LuaFile LoadLuaFile(string filePath, Stream stream)
        {
            var reader = new BinaryReader(stream);
            var bytes = reader.ReadBytes(13);
            
            // Check the .LJ magic in IW8 lua
            if(bytes[0] == 0x1B && bytes[1] == 0x4C && bytes[2] == 0x4A)
                throw new NotImplementedException("Modern Warfare lua isn't implemented yet");
            
            // Check if lua version is 5.0
            if(bytes[4] == 0x50)
                throw new NotImplementedException("5.0 lua isn't implemented yet");
            
            // Check compiler version
            if(bytes[5] == 0x0D)
                throw new NotImplementedException("Black Ops 2 lua isn't implemented yet");
            
            // Check if they use big endian
            if(bytes[6] == 0x00)
                return new LuaFileDS(filePath, reader);
            
            if(bytes[12] == 0x03)
                throw new NotImplementedException("Infinity Ward lua isn't implemented yet");
            
            return new LuaFileT7(filePath, reader);
        }
        
        public byte GetBit(long input, int bit)
        {
            return (byte)((input >> bit) & 1);
        }
    }
}