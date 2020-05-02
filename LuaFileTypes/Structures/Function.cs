using System.Collections.Generic;

namespace DSLuaDecompiler.LuaFileTypes.Structures
{
    public class Function
    {
        public LuaFile LuaFile { get; set; }
        public int Index { get; set; }
        public long FunctionPos { get; set; }
        public int FunctionLength { get; set; }

        public int UpvalCount { get; set; }
        public int ParameterCount { get; set; }
        public bool UsesVarArg { get; set; }
        public int RegisterCount { get; set; }
        public int InstructionCount { get; set; }
        public int ConstantCount { get; set; }
        public int SubFunctionCount { get; set; }
        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();
        public IList<Constant> Constants { get; set; } = new List<Constant>();
        public IList<Function> ChildFunctions { get; set; } = new List<Function>();

        public int LocalVarsCount { get; set; }
        public IList<Upvalue> Upvalues { get; set; } = new List<Upvalue>();
        public IList<Local> Locals { get; set; } = new List<Local>();
        public string Path { get; set; }
        public string Name { get; set; }
        
        public Dictionary<int, List<Local>> LocalMap { get; set; } = new Dictionary<int, List<Local>>();
        
        public Function(LuaFile luaFile)
        {
            LuaFile = luaFile;

            FunctionPos = luaFile.Reader.BaseStream.Position;
            luaFile.ReadFunctionHeader(this);

            for (int i = 0; i < InstructionCount; i++)
            {
                Instructions.Add(luaFile.ReadInstruction(this));
            }
            luaFile.ReadConstants(this);
            luaFile.ReadFunctionFooter(this);
            luaFile.ReadSubFunctions(this);
        }
        
        public List<Local> LocalsAt(int i)
        {
            if (LocalMap.ContainsKey(i))
                return LocalMap[i];
            
            return null;
        }
    }
}