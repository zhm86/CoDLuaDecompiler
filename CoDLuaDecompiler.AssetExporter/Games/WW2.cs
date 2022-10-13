using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CoDLuaDecompiler.AssetExporter.Games;

public class WW2 : IGame
{
    public struct DBAssetPool
    {
        public int FreeHead { get; set; }
        public int Entries { get; set; }
    }
    
    public struct LuaFile
    {
        /// <summary>
        /// Asset Name Hash
        /// </summary>
        public long NamePtr { get; set; }

        /// <summary>
        /// Data Size
        /// </summary>
        public Int32 AssetSize { get; set; }
        public Int32 DataSize2 { get; set; }

        /// <summary>
        /// Asset Hash
        /// </summary>
        public long RawDataPtr { get; set; }
    }
    
    public override string ExportFolder => "WW2/";
    public override unsafe List<LuaFileData> LoadLuaFiles(bool isMP = true)
    {
        // Get Base Address for ASLR and Scans
        long BaseAddress = AssetExport.Reader.GetBaseAddress();
        
        var scanDBAssetPools = AssetExport.Reader.FindBytes(
            new byte?[] { 0x4A, 0x8B, 0xAC, null, null, null, null, null, 0x48, 0x85, 0xED },
            BaseAddress,
            BaseAddress + AssetExport.Reader.GetModuleMemorySize(),
            true);
        var scanDBAssetPoolSizes = AssetExport.Reader.FindBytes(
            new byte?[] { 0x83, 0xBC, null, null, null, null, null, 0x01, 0x7F, 0x48 },
            BaseAddress,
            BaseAddress + AssetExport.Reader.GetModuleMemorySize(),
            true);

        if (scanDBAssetPools.Length > 0 && scanDBAssetPoolSizes.Length > 0)
        {
            var assetPoolAddress = AssetExport.Reader.ReadInt32(scanDBAssetPools[0] + 0x4) + BaseAddress;
            var assetPoolSizesAddress = AssetExport.Reader.ReadInt32(scanDBAssetPoolSizes[0] + 0x3) + BaseAddress;
                
            var address = assetPoolAddress + sizeof(DBAssetPool) * 69;
            long pool = AssetExport.Reader.ReadInt64(address) + Marshal.SizeOf<DBAssetPool>();
            var poolSize = AssetExport.Reader.ReadStruct<UInt32>(assetPoolSizesAddress + 4 * 69);
            return FetchFiles(pool, poolSize);
        }

        return null;
    }
    
    private List<LuaFileData> FetchFiles(long poolAddress, uint poolSize)
    {
        var filesList = new List<LuaFileData>();
        for (int i = 0; i < poolSize; i++)
        {
            var luaFile = AssetExport.Reader.ReadStruct<LuaFile>(poolAddress + (i * Marshal.SizeOf<LuaFile>()));

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
