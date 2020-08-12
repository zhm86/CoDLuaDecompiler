

using luadec.IR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Function = luadec.IR.Function;

namespace luadec
{
    class Program
    {
        /*static void Main(string[] args)
        {
            Encoding outEncoding = Encoding.UTF8;
            // Super bad arg parser until I decide to use a better libary
            bool writeFile = true;
            string outfilename = null;
            string infilename = null;
            int arg = 0;
            try
            {
                if (args[arg] == "-d")
                {
                    writeFile = false;
                    arg++;
                }
                else if (args[arg] == "-o")
                {
                    outfilename = args[arg + 1];
                    arg += 2;
                }
                infilename = args[arg];
                if (outfilename == null)
                {
                    outfilename = Path.GetFileNameWithoutExtension(infilename) + ".dec.lua";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Usage: DSLuaDecompiler.exe [options] inputfile.lua\n-o outputfile.lua\n-d Print output in console");
            }

            Console.OutputEncoding = outEncoding;
            using (FileStream stream = File.OpenRead(infilename))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                var lua = new LuaFile(br);
                IR.Function main = new IR.Function();
                //LuaDisassembler.DisassembleFunction(lua.MainFunction);
                if (lua.Version == LuaFile.LuaVersion.Lua50)
                {
                    LuaDisassembler.GenerateIR50(main, lua.MainFunction);
                    outEncoding = Encoding.GetEncoding("shift_jis");
                }
                else if (lua.Version == LuaFile.LuaVersion.Lua51HKS)
                {
                    LuaDisassembler.GenerateIRHKS(main, lua.MainFunction);
                    outEncoding = Encoding.UTF8;
                }

                if (writeFile)
                {
                    File.WriteAllText(outfilename, main.ToString(), outEncoding);
                }
                else
                {
                    Console.WriteLine(main.ToString());
                }
            }
        }*/

        static void Main(string[] args)
        {
            Console.WriteLine("CoD Havok Decompiler made from katalash's DSLuaDecompiler");

            var files = new List<string>();
            
            foreach (var arg in args)
            {
                var attr = File.GetAttributes(arg);
                // determine if we're a directory first
                // if so only includes file that are of ".lua" extension
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    files.AddRange(Directory.GetFiles(arg, "*.lua", SearchOption.AllDirectories).ToList());
                }
                // next determine if we're a file (lua is considered an archive apparently)
                else if (attr.HasFlag(FileAttributes.Archive) && Path.GetExtension(arg) == ".lua")
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

            // if we ever want to pursue directory structure
            //if (!Directory.Exists("output"))
            //{
            //    Directory.CreateDirectory("output");
            //}

            Console.WriteLine($"Total of {files.Count} to process.");
            var count = 0;
            var errors = 0; // TODO: ??
            foreach (var filePath in files)
            {
                Console.WriteLine($"Decompiling file {filePath}");
                try
                {
                    var output = DSLuaDecompiler.LuaFileTypes.LuaFile.LoadLuaFile(filePath, new MemoryStream(File.ReadAllBytes(filePath)));

                    // TODO: ??
                    Function.DebugIDCounter = 0;
                    Function.IndentLevel = 0;
                    LuaDisassembler.SymbolTable = new SymbolTable();

                    var main = new IR.Function();

                    output.GenerateIR(main, output.MainFunction);

                    var outputPath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".dec.lua";
                    
                    File.WriteAllText(outputPath, main.ToString());
                    count++;
                }
                catch (Exception e)
                {
                    errors++;
                    
                    var tempColor = Console.ForegroundColor;
                    
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: Decompilation Failure -- {e.Message}, no file generated.");
                    Console.ForegroundColor = tempColor;
                }
            }

            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Decompilation complete! Processed {count} files with {errors} errors.");
            Console.ForegroundColor = prevColor;
        }
    }
}
