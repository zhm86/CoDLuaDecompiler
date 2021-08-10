using System.IO;
using CoDLuaDecompiler.AssetExporter.Games;

namespace CoDLuaDecompiler.AssetExporter
{
    public class LuaFileData
    {
        public string Name { get; set; }
        public long Hash { get; set; }
        public BinaryReader Reader { get; set; }
    }
}