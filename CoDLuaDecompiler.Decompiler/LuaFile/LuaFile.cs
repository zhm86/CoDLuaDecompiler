using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.Decompiler.Analyzers;
using CoDLuaDecompiler.Decompiler.InstructionConverters;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;

namespace CoDLuaDecompiler.Decompiler.LuaFile
{
    public abstract class LuaFile : ILuaFile
    {
        public IFileHeader Header { get; private set; }
        public IList<IHavokLuaConstant> Constants { get; private set; }
        public ILuaFunction MainFunction { get; private set; }
        public virtual IInstructionConverter InstructionConverter { get; }
        public virtual IAnalyzerList AnalyzerList { get; }
        public virtual IAnalyzerList FileAnalyzerList { get; }

        // private variables
        protected readonly BinaryReader Reader;

        protected LuaFile(BinaryReader reader)
        {
            Reader = reader;

            Parse();
        }

        private void Parse()
        {
            Header = ReadHeader();
            if (Header == null)
                return;
            Constants = ReadConstants();
            MainFunction = ReadFunctions();
        }

        protected abstract IFileHeader ReadHeader();
        protected abstract IList<IHavokLuaConstant> ReadConstants();
        protected abstract ILuaFunction ReadFunctions();
    }
}