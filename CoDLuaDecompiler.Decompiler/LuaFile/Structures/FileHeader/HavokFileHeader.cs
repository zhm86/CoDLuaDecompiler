namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader
{
    public class HavokFileHeader : IFileHeader
    {
        public byte CompilerVersion { get; set; }
        public byte Endianness { get; set; }
        public byte SizeOfInt { get; set; }
        public byte SizeOfSizeT { get; set; }
        public byte SizeOfInstruction { get; set; }
        public byte SizeOfLuaNumber { get; set; }
        public byte IntegralFlag { get; set; }
        public byte GameByte { get; set; }
        public int ConstantTypeCount { get; set; }
    }
}