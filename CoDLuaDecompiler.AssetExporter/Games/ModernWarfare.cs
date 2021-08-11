using System.Collections.Generic;
using System.IO;

namespace CoDLuaDecompiler.AssetExporter.Games
{
    public class ModernWarfare : IGame
    {
        public override string ExportFolder => "MW/";
        public static long[] GameOffsets =
        {
            0xCFA24C0,
        };
        // -- Modern Warfare 4 Pool Data Structure
        public struct AssetPool
        {
            // The beginning of the pool
            public long PoolPointer { get; set; }

            // A pointer to the closest free header
            public long PoolFreeHeadPtr { get; set; }

            // The maximum pool size
            public int PoolSize { get; set; }

            // The size of the asset header
            public int AssetSize { get; set; }
        }
        
        private struct LuaFile
        {
            public long NamePointer;
            public int AssetSize;
            public int Unk;
            public long RawDataPtr;
        }
        
        public override unsafe List<LuaFileData> LoadLuaFiles(bool isMP = true)
        {
            // Get Base Address for ASLR and Scans
            long baseAddress = AssetExport.Reader.GetBaseAddress();

            foreach (var gameOffset in GameOffsets)
            {
                var xmodelPoolData = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + gameOffset + sizeof(AssetPool) * 9);
                
                // Check XModel Hash
                if (AssetExport.Reader.ReadNullTerminatedString(AssetExport.Reader.ReadInt64(xmodelPoolData.PoolPointer)) ==
                    "axis_guide_createfx")
                {
                    var luaPoolData = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + gameOffset + sizeof(AssetPool) * 62);

                    return FetchFiles(luaPoolData);
                }
            }
            
            var dbAssetsScan = AssetExport.Reader.FindBytes(new byte?[] { 0x48, 0x8D, 0x04, 0x40, 0x4C, 0x8D, 0x8E, null, null, null, null, 0x4D, 0x8D, 0x0C, 0xC1, 0x8D, 0x42, 0xFF }, baseAddress, baseAddress + AssetExport.Reader.GetModuleMemorySize(), true);

            // Check that we had hits
            if (dbAssetsScan.Length > 0)
            {
                var assetPoolAddress = AssetExport.Reader.ReadUInt32(dbAssetsScan[0] + 0x7);
                var xmodelPoolData      = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + assetPoolAddress + sizeof(AssetPool) * 9);
                
                // Check XModel Hash
                if (AssetExport.Reader.ReadNullTerminatedString(AssetExport.Reader.ReadInt64(xmodelPoolData.PoolPointer)) ==
                    "axis_guide_createfx")
                {
                    var luaPoolData = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + assetPoolAddress + sizeof(AssetPool) * 62);

                    return FetchFiles(luaPoolData);
                }
            }

            return null;
        }

        private List<LuaFileData> FetchFiles(AssetPool luaPoolData)
        {
            var filesList = new List<LuaFileData>();
            for (int i = 0; i < luaPoolData.PoolSize; i++)
            {
                var luaFile = AssetExport.Reader.ReadStruct<LuaFile>(luaPoolData.PoolPointer + (i * luaPoolData.AssetSize));

                if (luaFile.AssetSize == 0 || luaFile.RawDataPtr == 0)
                    continue;
                
                var luaFileData = AssetExport.Reader.ReadBytes(luaFile.RawDataPtr, luaFile.AssetSize);
                        
                filesList.Add(new LuaFileData()
                {
                    Reader = new BinaryReader(new MemoryStream(luaFileData)),
                    Name = AssetExport.Reader.ReadNullTerminatedString(luaFile.NamePointer),
                });
            }

            return filesList;
        }
    }
}