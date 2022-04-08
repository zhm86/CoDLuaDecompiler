using System.Linq;
using System.Text.RegularExpressions;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Shared
{
    public class UIModelFunctionValueVarNamesAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            if (f.FunctionDebugInfo != null)
                return;
            // Get the widget.new function
            Function createWidgetFunction = null;
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count; i++)
                {
                    // Find the inheritance call
                    if (b.Instructions[i] is Assignment a && a.Left.Count == 1 &&
                        a.Right is FunctionCall fc &&
                        fc.Function.ToString() == "InheritFrom")
                    {
                        // Find the .new assignment with the function
                        for (int j = i + 1; j < f.Blocks[0].Instructions.Count; j++)
                        {
                            if (f.Blocks[0].Instructions[j] is Assignment a2 && a2.Left.Count == 1 &&
                                a2.Left[0].ToString() == a.Left[0].ToString() + ".new" && a2.Right is Closure c)
                            {
                                createWidgetFunction = c.Function;
                                break;
                            }
                        }

                        if (createWidgetFunction != null)
                            break;
                    }
                }
                if (createWidgetFunction != null)
                    break;
            }

            // If we can't find it, it can also be a menu
            if (createWidgetFunction == null)
            {
                foreach (var b in f.Blocks)
                {
                    for (int i = 0; i < b.Instructions.Count; i++)
                    {
                        if (b.Instructions[i] is Assignment a && a.Left.Count == 1 && a.Right is Closure c && a.Left[0].ToString().StartsWith("LUI.createMenu."))
                        {
                            createWidgetFunction = c.Function;
                            break;
                        }
                    }
                    if (createWidgetFunction != null)
                        break;
                }
            }
            
            // If we didn't find the function or it doesn't have enough blocks for the if statement we won't continue
            if (createWidgetFunction == null || createWidgetFunction.Blocks.Count < 4)
                return;

            var cwf = createWidgetFunction;

            foreach (var b in cwf.Blocks)
            {
                foreach (var instr in b.Instructions)
                {
                    if (instr is Assignment a && a.Left.Count == 0 && a.Right is FunctionCall fc && fc.Arguments.Any() && fc.Arguments[^1] is Closure c && 
                        c.Function.Blocks.Any() && c.Function.Blocks[0].Instructions.Any() && c.Function.Blocks[0].Instructions[0] is Assignment a2 && 
                        a2.Right is FunctionCall fc2 && fc2.Function.ToString() == "Engine.GetModelValue" && a2.Left.Count == 1)
                    {
                        if (fc.Function.ToString().EndsWith("subscribeToGlobalModel") && fc.Arguments.Count >= 4 && fc.Arguments[^2] is Constant co && co.Type == ValueType.String)
                        {
                            var variableName = ToCamelCase(co.String.Replace(".", "_"));
                            if (c.Function.UpvalueBindings.Exists(ub => ub.Name == variableName))
                            {
                                variableName = "_" + variableName;
                            }
                            a2.Left[0].Identifier.Name = variableName;
                        }
                        
                        if (fc.Function.ToString().EndsWith("linkToElementModel") && fc.Arguments.Count >= 4 && fc.Arguments[^3] is Constant co2 && co2.Type == ValueType.String)
                        {
                            var variableName = ToCamelCase(co2.String.Replace(".", "_"));
                            if (c.Function.UpvalueBindings.Exists(ub => ub.Name == variableName))
                            {
                                variableName = "_" + variableName;
                            }
                            a2.Left[0].Identifier.Name = variableName;
                        }
                    }
                }
            }
        }
        
        private string ToCamelCase(string str)
        {
            return Regex.Replace(str, "_[a-z]", m => m.ToString().TrimStart('_').ToUpper());
        }
    }
}