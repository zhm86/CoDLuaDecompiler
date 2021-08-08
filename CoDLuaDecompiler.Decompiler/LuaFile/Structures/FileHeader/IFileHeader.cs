namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.FileHeader
{
    public abstract class IFileHeader
    {
        public string Magic { get; set; }
        public int Version { get; set; }
    }
}