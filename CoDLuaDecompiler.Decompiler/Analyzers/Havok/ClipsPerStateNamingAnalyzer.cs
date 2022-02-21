using System;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;
using ValueType = CoDLuaDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class ClipsPerStateNamingAnalyzer : IAnalyzer
    
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
                    if (instr is Assignment a && a.Left.Count == 1 && a.Left[0].HasIndex &&
                        a.Left[0].TableIndices.Count == 1 && a.Left[0].TableIndices[0] is Constant c &&
                        c.Type == ValueType.String && c.String == "clipsPerState" && a.Right is InitializerList il)
                    {
                        foreach (var exp1 in il.Expressions)
                        {
                            var state = (ListAssignment) exp1;
                            var clips = ((InitializerList)state.Right).Expressions;
                            foreach (var exp2 in clips)
                            {
                                var clip = (ListAssignment) exp2;
                                var clipFunction = (clip.Right as Closure)?.Function;
                                
                                RenameClipFunctionVariables(clipFunction);
                            }
                        }
                    }
                }
            }
        }

        private void RenameClipFunctionVariables(Function f)
        {
            foreach (var b in f.Blocks)
            {
                foreach (var instr in b.Instructions)
                {
                    if (instr is Assignment a && a.Left.Count == 0 && a.Right is FunctionCall fc && fc.Function is IdentifierReference ir && !ir.HasIndex &&
                        fc.Arguments.Count == 2 && fc.Arguments[0] is IdentifierReference {HasIndex: false} ir2 && fc.Arguments[1] is InitializerList il && il.Expressions.Count == 0 &&
                        ir.Identifier.DefiningInstruction is Assignment a2 && a2.Left.Count == 1 && a2.Right is Closure c && c.Function.Parameters.Count == 2)
                    {
                        ir.Identifier.Name = ir2.Identifier.Name + "Frame2";
                        RenameVariables(c.Function, ir2.Identifier.Name, 3);
                    }
                }
            }
        }

        private void RenameVariables(Function f, string elementName, int frameNumber)
        {
            f.Parameters[0].Name = elementName;
            f.Parameters[1].Name = "event";

            foreach (var b in f.Blocks)
            {
                foreach (var instr in b.Instructions)
                {
                    if (instr is Assignment a && a.Left.Count == 1 && a.Right is Closure)
                    {
                        a.Left[0].Identifier.Name = elementName + "Frame" + frameNumber;
                    }
                }
            }
            
            foreach (var func in f.Closures)
            {
                RenameVariables(func, elementName, frameNumber + 1);
            }
        }
    }
}