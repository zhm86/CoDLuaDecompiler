using System;
using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.AssetExporter.Util;

namespace CoDLuaDecompiler.AssetExporter.Games
{
    public class BlackOps3 : IGame
    {
        public override string ExportFolder => "BO3/";
        
        public static long[] GameOffsets =
        {
            0x88788D0,
        };
        
        struct AssetPool
        {
            public long PoolPointer { get; set; }
            public int AssetSize { get; set; }
            public int PoolSize { get; set; }
            public bool IsSingleton { get; set; }
            public int ItemAllocCount { get; set; }
            public long FreeHead { get; set; }
        }

        private struct LuaFile
        {
            
            public long NamePtr { get; set; }
            public Int32 AssetSize { get; set; }
            public long RawDataPtr { get; set; }
        }
        public override unsafe List<LuaFileData> LoadLuaFiles(bool isMP = true)
        {
            // Get Base Address for ASLR and Scans
            long baseAddress = AssetExport.Reader.GetBaseAddress();

            foreach (var gameOffset in GameOffsets)
            {
                var xmodelPoolData = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + gameOffset + sizeof(AssetPool) * 0x4);
                
                // Check XModel Hash
                if (AssetExport.Reader.ReadNullTerminatedString(AssetExport.Reader.ReadInt64(xmodelPoolData.PoolPointer)) == "void")
                {
                    var luaPoolData = AssetExport.Reader.ReadStruct<AssetPool>(baseAddress + gameOffset + sizeof(AssetPool) * 47);

                    return FetchFiles(luaPoolData);
                }
            }
            
            var dbAssetsScan = AssetExport.Reader.FindBytes(new byte?[] { 0x63, 0xC1, 0x48, 0x8D, 0x05, null, null, null, null, 0x49, 0xC1, 0xE0, null, 0x4C, 0x03, 0xC0 }, baseAddress, baseAddress + AssetExport.Reader.GetModuleMemorySize(), true);

            // Check that we had hits
            if (dbAssetsScan.Length > 0)
            {
                var assetPoolAddress = AssetExport.Reader.ReadUInt32(dbAssetsScan[0] + 0x5) + dbAssetsScan[0] + 0x9;
                var xmodelPoolData      = AssetExport.Reader.ReadStruct<AssetPool>(assetPoolAddress + sizeof(AssetPool) * 0x4);
                
                // Check XModel Hash
                if (AssetExport.Reader.ReadNullTerminatedString(AssetExport.Reader.ReadInt64(xmodelPoolData.PoolPointer)) == "void")
                {
                    var luaPoolData = AssetExport.Reader.ReadStruct<AssetPool>(assetPoolAddress + sizeof(AssetPool) * 47);

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

                var name = AssetExport.Reader.ReadNullTerminatedString(luaFile.NamePtr);
                if (!name.EndsWith(".lua"))
                    continue;
                
                var luaFileData = AssetExport.Reader.ReadBytes(luaFile.RawDataPtr, luaFile.AssetSize);
                        
                filesList.Add(new LuaFileData()
                {
                    Reader = new BinaryReader(new MemoryStream(luaFileData)),
                    Name = name,
                });
            }

            return filesList;
        }
    }
}