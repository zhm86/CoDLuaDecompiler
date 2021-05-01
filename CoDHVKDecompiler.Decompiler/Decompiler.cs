using CoDHVKDecompiler.Common;
using CoDHVKDecompiler.Common.LuaFunction;
using CoDHVKDecompiler.Decompiler.Analyzers;
using CoDHVKDecompiler.Decompiler.IR;
using CoDHVKDecompiler.Decompiler.IR.Functions;

namespace CoDHVKDecompiler.Decompiler
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
            if (function.Id == 3)
            {
                
            }
            // First register closures for all the children
            luaFunction.ChildFunctions.ForEach(cf =>
            {
                function.Closures.Add(new Function(function, function.SymbolTable));
            });
            // Start a new scope for the function's variables
            SymbolTable.BeginScope();
            // Register the parameters of the function
            for (uint i = 0; i < luaFunction.Header.ParameterCount; i++)
                function.Parameters.Add(SymbolTable.GetRegister(i));
            // Convert lua instructions to IR instructions
            var converter = new LuaInstructionConverter(SymbolTable);
            converter.Convert(function, luaFunction);
            // Pass analysis
            var analyzers = new AnalyzerList().Analyzers;
            for (int i = 0; i < analyzers.Count; i++)
            {
                analyzers[i].Analyze(function);
            }
            // Decompile all child functions
            for (int i = 0; i < luaFunction.ChildFunctions.Count; i++)
                DecompileFunction(function.Closures[i], luaFunction.ChildFunctions[i]);
            // Close the scope of this function
            SymbolTable.EndScope();
        }
    }
}