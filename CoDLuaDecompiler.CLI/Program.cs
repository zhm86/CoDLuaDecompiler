using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoDLuaDecompiler.AssetExporter;
using CoDLuaDecompiler.Decompiler;
using CoDLuaDecompiler.Decompiler.LuaFile;

namespace CoDLuaDecompiler.CLI
{
    class Program
    {
        private readonly IAssetExport _assetExport;
        private readonly IDecompiler _decompiler;

        public Program(IDecompiler decompiler, IAssetExport assetExport)
        {
            _decompiler = decompiler;
            _assetExport = assetExport;
        }

        private void HandleFile(string filePath)
        {
            try
            {
                // parse lua file
                var file = LuaFileFactory.Create(filePath);

                // decompile file
                var output = _decompiler.Decompile(file);

                // replace extension
                var outFileName = Path.ChangeExtension(filePath, ".dec.lua");

                // save output
                File.WriteAllText(outFileName, output);
                
                Console.WriteLine($"Decompiled file: {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static List<string> ParseFilesFromArgs(IEnumerable<string> args)
        {
            var files = new List<string>();

            foreach (var arg in args)
            {
                if (!File.Exists(arg) && !Directory.Exists(arg))
                    continue;
                
                var attr = File.GetAttributes(arg);
                // determine if we're a directory first
                // if so only includes file that are of ".lua" or ".luac" extension
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    files.AddRange(Directory.GetFiles(arg, "*.lua*", SearchOption.AllDirectories).ToList());
                }
                else if (Path.GetExtension(arg).Contains(".lua"))
                {
                    files.Add(arg);
                }
                else
                {
                    Console.WriteLine($"Invalid argument passed {arg} | {File.GetAttributes(arg)}!");
                }
            }
            
            // make sure to remove duplicates
            files = files.Distinct().ToList();

            // also remove any already dumped files
            files.RemoveAll(elem => elem.EndsWith(".dec.lua"));
            files.RemoveAll(elem => elem.EndsWith(".luadec"));

            return files;
        }
        
        public void Main(string[] args)
        {
            if (args.Contains("--export"))
            {
                Console.WriteLine("Starting asset export from memory.");
                _assetExport.ExportAssets(args.Contains("--dump"));
            }
            
            // parse files from arguments
            var files = ParseFilesFromArgs(args);
            
            Console.WriteLine($"Total of {files.Count} to process.");

            Parallel.ForEach(files, HandleFile);
        }
    }
}