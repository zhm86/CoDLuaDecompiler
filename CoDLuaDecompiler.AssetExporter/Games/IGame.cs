using System.Collections.Generic;

namespace CoDLuaDecompiler.AssetExporter.Games
{
    public abstract class IGame
    {
        public abstract string ExportFolder { get; }
        public abstract List<LuaFileData> LoadLuaFiles(bool isMP = true);
    }
}