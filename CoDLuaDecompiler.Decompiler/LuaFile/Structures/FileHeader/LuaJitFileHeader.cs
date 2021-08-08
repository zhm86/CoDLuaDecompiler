namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader
{
    public class LuaJitFileHeader : IFileHeader
    {
        public bool IsStripped { get; set; }
        public bool IsBigEndian { get; set; }
        public bool HasFfi { get; set; }
        public bool IsFr2 { get; set; }
    }
}