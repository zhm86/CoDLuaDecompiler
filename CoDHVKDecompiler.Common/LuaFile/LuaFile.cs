using System.Collections.Generic;
using System.IO;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction;
using CoDHVKDecompiler.Common.Structures;

namespace CoDHVKDecompiler.Common
{
    public abstract class LuaFile : ILuaFile
    {
        public FileHeader Header { get; private set; }
        public IList<ILuaConstant> Constants { get; private set; }
        public ILuaFunction MainFunction { get; private set; }
        
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
            Constants = ReadConstants();
            MainFunction = ReadFunctions();
        }

        protected abstract FileHeader ReadHeader();
        protected abstract IList<ILuaConstant> ReadConstants();
        protected abstract ILuaFunction ReadFunctions();
    }
}