

using System;
using System.IO;
using System.Text;

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
            Console.WriteLine("CoD Havok Decompiler by JariK");

            string fileName = @"/home/jari/ui/lui/lui.luac";
            fileName = @"/home/jari/ui_mp/t6/hud/loading.luac";
            fileName = @"/home/jari/ui/uieditor/widgets/zm_expiresin.luac";
            //fileName = @"E:\Users\Jari_new\Documents\Github\CoDLUIDecompiler\CoDLUIDecompiler\bin\Release\t8_luafiles\LuaFile_1a3d1f301d13ce9.lua";
            //fileName = @"/home/jari\ui\uieditor\widgets\aar\xpbarframe.luac";
            //fileName = @"E:\modding_tools\hydra\hydrax_old\exported_files\ui\test.luac";
            //fileName = @"C:\Users\Jerri\Downloads\c0000.hks";
            //fileName = @"E:\Users\Jari_new\Documents\Github\CoDLUIDecompiler\CoDLUIDecompiler\bin\Debug\t6_luafiles\ui_mp\t6\zombie\basezombie.lua";
            //fileName = @"/home/jari/ui/testfile.luac";
            Console.WriteLine("Decompiling file: " + Path.GetFileName(fileName));
            var file = DSLuaDecompiler.LuaFileTypes.LuaFile.LoadLuaFile(fileName, new MemoryStream(File.ReadAllBytes(fileName)));
            
            IR.Function main = new IR.Function();
            file.GenerateIR(main, file.MainFunction);
            
            File.WriteAllText(@"/home/jari/ui/testfile.luadc", main.ToString());
            
            //BinaryReaderEx br = new BinaryReaderEx(false, File.OpenRead(fileName));
           // var lua = new LuaFile_old(br);
            
            Console.WriteLine("Decompiling complete");
        }
    }
}
