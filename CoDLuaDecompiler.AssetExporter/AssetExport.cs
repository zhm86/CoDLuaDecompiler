using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CoDLuaDecompiler.AssetExporter.Games;
using CoDLuaDecompiler.AssetExporter.Util;
using CoDLuaDecompiler.Decompiler;
using CoDLuaDecompiler.Decompiler.IR.Instruction;
using CoDLuaDecompiler.Decompiler.LuaFile;

namespace CoDLuaDecompiler.AssetExporter
{
    public class AssetExport : IAssetExport
    {
        private readonly IDecompiler _decompiler;
        public static ProcessReader Reader { get; set; }

        public AssetExport(IDecompiler decompiler)
        {
            _decompiler = decompiler;
        }
        
        public static Dictionary<string, Tuple<IGame, bool>> Games = new Dictionary<string, Tuple<IGame, bool>>()
        {
            { "BlackOps4",      new Tuple<IGame, bool>(new BlackOps4(),    true) },
            { "BlackOpsColdWar",      new Tuple<IGame, bool>(new BlackOpsColdWar(),    true) },
            { "ModernWarfare",      new Tuple<IGame, bool>(new ModernWarfare(),    true) },
        };
        
        public void ExportAssets()
        {
            Process[] Processes = Process.GetProcesses();
            
            foreach(var process in Processes)
            {
                // Check process name against game list
                if(Games.ContainsKey(process.ProcessName))
                {
                    // Result
                    var game = Games[process.ProcessName];
                    // Info
                    Console.WriteLine("Found matching game {0}", process.ProcessName);
                    // Set Reader
                    Reader = new ProcessReader(process);
                    // Load Game
                    var files = game.Item1.LoadLuaFiles(game.Item2);
                    if (files == null)
                        Console.WriteLine("This game is supported, but this update is not.");
                    else
                        HandleLuaFiles(files, game.Item1);
                    // Done
                    return;
                }
            }

            // Failed
            Console.WriteLine("Failed to find a supported game");
        }

        private static MD5 md5 = MD5.Create();
        private string GetHash(LuaFileData luaFile)
        {
            var hash = md5.ComputeHash(luaFile.Reader.BaseStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private void HandleLuaFiles(List<LuaFileData> luaFiles, IGame game)
        {
            luaFiles.ForEach(file =>
            {
                string filePath = file.Name;
                if (String.IsNullOrEmpty(filePath))
                    filePath = String.Format("Luafile_{0:x}.luac", file.Hash & 0xFFFFFFFFFFFFFFF);

                var directory = Path.GetDirectoryName(game.ExportFolder + filePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);
                    
                var outFileName = Path.ChangeExtension(game.ExportFolder + filePath, ".dec.lua");
                
                // Check if we already decompiled the file
                string hash = "";
                if (File.Exists(outFileName))
                {
                    string line = File.ReadLines(outFileName).FirstOrDefault();
                    if (!String.IsNullOrEmpty(line) && line.StartsWith("-- "))
                    {
                        var checksum = line.Remove(0, 3);
                        hash = GetHash(file);
                        if (checksum == hash)
                            return;
                    }
                }
                
                var luaFile = LuaFileFactory.Create(file.Reader);
                try
                {
                    if (String.IsNullOrEmpty(hash))
                        hash = GetHash(file);
                    var prefix = $"-- {hash}\n-- This hash is used for caching, delete to decompile the file again\n\n";
                    var decompiledFile = _decompiler.Decompile(luaFile);
                    
                    // save output
                    File.WriteAllText(outFileName, prefix + decompiledFile);
                    Console.WriteLine($"Decompiled file: {Path.GetFileName(filePath)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while trying to decompile file {Path.GetFileName(filePath)}: {e.Message}");
                }
            });
        }
    }
}