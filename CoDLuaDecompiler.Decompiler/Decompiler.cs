using CoDLuaDecompiler.Decompiler.Analyzers;
using CoDLuaDecompiler.Decompiler.IR;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction;

namespace CoDLuaDecompiler.Decompiler
{
    public class Decompiler : IDecompiler
    {
        public static SymbolTable SymbolTable = new SymbolTable();

        public Function GetDecompiledFile(ILuaFile luaFile)
        {
            Function.IdCounter = 0;
            Function.IndentLevel = 0;
            var function = new Function(SymbolTable);
            
            DecompileFunction(function, luaFile.MainFunction);

            return function;
        }
        
        public string Decompile(ILuaFile luaFile)
        {
            var function = GetDecompiledFile(luaFile);
            return function.ToString();
        }

        public void DecompileFunction(Function function, ILuaFunction luaFunction)
        {
            // First register closures for all the children
            for (uint i = 0; i < luaFunction.ChildFunctions.Count; i++)
            {
                var func = new Function(function, function.SymbolTable);
                luaFunction.ChildFunctions[(int) i].IRFunction = func;
                function.Closures.Add(func);
            }
            // Start a new scope for the function's variables
            function.SymbolTable.BeginScope();
            // Register the parameters of the function
            for (uint i = 0; i < luaFunction.Header.ParameterCount; i++)
                function.Parameters.Add(function.SymbolTable.GetRegister(i));
            // Convert lua instructions to IR instructions
            var converter = luaFunction.LuaFile.InstructionConverter;
            converter.Convert(function, luaFunction);
            // Pass analysis
            var analyzers = luaFunction.LuaFile.AnalyzerList.GetAnalyzers();
            for (int i = 0; i < analyzers.Count; i++)
                analyzers[i].Analyze(function);
            // Decompile all child functions
            for (int i = 0; i < luaFunction.ChildFunctions.Count; i++)
                DecompileFunction(function.Closures[i], luaFunction.ChildFunctions[i]);
            // Close the scope of this function
            function.SymbolTable.EndScope();
        }
    }
}