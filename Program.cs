

using System;
using System.IO;
using System.Linq;
using System.Text;
using DSLuaDecompiler.LuaFileTypes.Structures;
using luadec.IR;
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

            string[] files = new string[1];
            if (args.Length == 0)
            {
                Console.WriteLine("Give the folder that you want to decompile: ");
                string folder = Console.ReadLine();
                if (Directory.Exists(folder))
                {
                    files = Directory.GetFiles(folder, "*.lua*", SearchOption.AllDirectories);
                }
            }
            else
            {
                files = args.Where(x => (Path.GetExtension(x) == ".lua" || Path.GetExtension(x) == ".luac") && File.Exists(x)).ToArray();
            }

            int errors = 0;
            
            foreach (string fileName in files)
            {
                if (Path.GetExtension(fileName) != ".lua" && Path.GetExtension(fileName) != ".luac")
                {
                    continue;
                }
                Console.WriteLine("Decompiling file: " + Path.GetFileName(fileName));
                try
                {
                    var file = DSLuaDecompiler.LuaFileTypes.LuaFile.LoadLuaFile(fileName,
                        new MemoryStream(File.ReadAllBytes(fileName)));

                    // TODO: shite static stuff that I need to change
                    Function.DebugIDCounter = 0;
                    Function.IndentLevel = 0;
                    LuaDisassembler.SymbolTable = new SymbolTable();
                    
                    IR.Function main = new IR.Function();
                    file.GenerateIR(main, file.MainFunction);

                    File.WriteAllText(fileName + "d", main.ToString());
                }
                catch(Exception e)
                {
                    errors++;
                    Console.WriteLine(e);
                }
            }
            
            //Console.WriteLine($"Errors: {errors}/{files.Length}");

            Console.WriteLine("Decompiling complete");
        }
    }
}
