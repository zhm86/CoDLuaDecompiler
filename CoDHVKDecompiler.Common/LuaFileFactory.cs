using System;
using System.IO;

namespace CoDHVKDecompiler.Common
{
    public static class LuaFileFactory
    {
        public static ILuaFile Create(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(13);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            if (bytes[0] != 0x1B || bytes[1] != 0x4C || bytes[2] != 0x75 || bytes[3] != 0x61)
            {
                throw new Exception("Invalid file magic");
            }

            return new LuaFileT7(reader);
        }
        public static ILuaFile Create(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {filePath} not found!", filePath);
            }

            using var stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream);

            return Create(reader);
        }
    }
}