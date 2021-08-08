using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CoDLuaDecompiler.Decompiler.Analyzers;
using CoDLuaDecompiler.Decompiler.InstructionConverters;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.LuaJit
{
    public abstract class LuaJitFile : LuaFile
    {
        public override IInstructionConverter InstructionConverter => new LuaJitInstructionConverter();
        public override IAnalyzerList AnalyzerList => new LuaJitAnalyzerList();
        public LuaJitFile(BinaryReader reader) : base(reader)
        {
        }

        public Stack<LuaFunction> Functions { get; set; } = new Stack<LuaFunction>();

        protected override IFileHeader ReadHeader()
        {
            var header = new LuaJitFileHeader
            {
                Magic = Encoding.UTF8.GetString(Reader.ReadBytes(3)),
                Version = Reader.ReadByte()
            };

            var flags = Reader.ReadByte();
            header.IsBigEndian = Convert.ToBoolean(flags & 0b_0000_0001);
            header.IsStripped = Convert.ToBoolean(flags & 0b_0000_0010);
            header.HasFfi = Convert.ToBoolean(flags & 0b_0000_0100);
            header.IsFr2 = Convert.ToBoolean(flags & 0b_0000_1000);

            return header;
        }

        protected override IList<IHavokLuaConstant> ReadConstants()
        {
            // Not present in LuaJit
            return null;
        }

        protected override ILuaFunction ReadFunctions()
        {
            // Continue reading functions untill we are at the end
            ILuaFunction mainFunc = null;
            var i = 0;
            while (true)
            {
                var function = ReadFunction();

                if (function.FunctionPos == -1)
                    break;
                i++;

                Functions.Push(function);
                mainFunc = function;
            }

            return mainFunc;
        }

        protected abstract LuaJitFunction ReadFunction();
    }
}