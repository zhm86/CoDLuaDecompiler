using System;
using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;
using CoDLuaDecompiler.Decompiler.LuaFile.Havok.Debug;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;
using ValueType = CoDLuaDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Rename variables from their temporary register based names to something more generic
    /// </summary>
    public class RenameVariablesAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            HashSet<Identifier> renamed = new HashSet<Identifier>();

            // Rename function arguments
            for (int i = 0; i < f.Parameters.Count; i++)
            {
                renamed.Add(f.Parameters[i]);
                if (f.ArgumentNames != null && f.ArgumentNames.Count > i)
                {
                    f.Parameters[i].Name = f.ArgumentNames[i].Name;
                }
                else
                {
                    f.Parameters[i].Name = $@"f{f.Id}_arg{i}";
                }
            }

            // Rename all the locals
            int localCounter = 0;
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a)
                    {
                        int ll = 0;
                        foreach (var l in a.Left)
                        {
                            if (!l.HasIndex && l.Identifier.IdentifierType == IdentifierType.Register && !renamed.Contains(l.Identifier))
                            {
                                renamed.Add(l.Identifier);
                                if (a.LocalAssignments != null && ll < a.LocalAssignments.Count)
                                {
                                    l.Identifier.Name = a.LocalAssignments[ll].Name;
                                }
                                else
                                {
                                    l.Identifier.Name = $@"f{f.Id}_local{localCounter}";
                                    localCounter++;
                                }
                            }
                            ll++;

                            if (!l.HasIndex &&
                                l.Identifier.IdentifierType == IdentifierType.Upvalue &&
                                !renamed.Contains(l.Identifier) &&
                                !String.IsNullOrEmpty(l.Identifier.UpvalueVarName))
                            {
                                renamed.Add(l.Identifier);
                                l.Identifier.Name = l.Identifier.UpvalueVarName;
                            }
                        }
                    }
                }
            }
            
            if (f.FunctionDebugInfo != null)
                return;

            // Adding special variable names for LUI widgets
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a && a.Right is FunctionCall fc && a.Left.Count == 1)
                    {
                        string name;
                        if (fc.Function.ToString().StartsWith("LUI.UI") && fc.Function.ToString().EndsWith(".new") || fc.Function.ToString().Contains("CoD.Menu.NewForUIEditor"))
                        {
                            name = "self";
                        }
                        else 
                            continue;
                        if (a.Left[0] is IdentifierReference ir && !ir.HasIndex && ir.Identifier.IdentifierType == IdentifierType.Register)
                        {
                            ir.Identifier.Name = name;
                        }
                    }
                }
            }

            // Change parameter names for menus and widgets
            if (f.Blocks[0] != null && f.Blocks[0].Instructions.Count > 0 && f.Blocks[0].Instructions[0] is Assignment a2 && a2.Right is FunctionCall fc2)
            {
                if (fc2.Function.ToString().StartsWith("LUI.UI") && fc2.Function.ToString().EndsWith(".new") && f.Parameters.Count == 2)
                {
                    f.Parameters[0].Name = "menu";
                    f.Parameters[1].Name = "controller";
                }
                else if (fc2.Function.ToString().Contains("CoD.Menu.NewForUIEditor") && f.Parameters.Count == 1)
                {
                    f.Parameters[0].Name = "controller";
                }
            }

            // Add ModelRef as parameter and 
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment {Right: FunctionCall c3} && (c3.Function.ToString().Contains("subscribeToGlobalModel") || c3.Function.ToString().Contains("linkToElementModel")|| c3.Function.ToString().Contains("subscribeToModel"))) 
                    {
                        foreach (var arg in c3.Arguments)
                        {
                            if (arg is Closure c)
                            {
                                c.Function.ArgumentNames = new List<Local>(){new Local(){Name = "modelRef"}};
                            }
                        }
                    }

                    if (i is Assignment {Right: FunctionCall fc5} && fc5.Function.ToString().EndsWith("registerEventHandler"))
                    {
                        if (fc5.Arguments.Count >= 2 && fc5.Arguments[^1] is Closure c2)
                        {
                            c2.Function.ArgumentNames = new List<Local>() {new Local(){Name = "element"}, new Local(){Name = "event"}};
                        }
                    }
                    
                    if (i is Assignment {Right: FunctionCall fc6} && fc6.Function.ToString().Contains("LUI.OverrideFunction_CallOriginalSecond"))
                    {
                        if (fc6.Arguments.Count == 3 && fc6.Arguments[2] is Closure c2)
                        {
                            c2.Function.ArgumentNames = new List<Local>() {new Local(){Name = "element"}};
                        }
                    }
                    
                    if (i is Assignment {Right: FunctionCall fc7} && fc7.Function.ToString().Contains("LUI.OverrideFunction_CallOriginalFirst"))
                    {
                        if (fc7.Arguments.Count == 3 && fc7.Arguments[2] is Closure c2)
                        {
                            c2.Function.ArgumentNames = new List<Local>() {new Local(){Name = "element"}, new Local(){Name = "controller"}};
                        }
                    }
                }
            }
            if (f.Blocks[0] != null && f.Blocks[0].Instructions.Count > 0 && f.Blocks[0].Instructions[0] is Assignment
            {
                Right: FunctionCall fc3
            } a4)
            {
                if (fc3.Function.ToString().Contains("Engine.GetModelValue") && f.Parameters.Count == 1)
                {
                    a4.Left[0].Identifier.Name = "modelValue";
                }
            }
            
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 2; i++)
                {
                    var inst = b.Instructions[i];
                    if (inst is Assignment {Right: FunctionCall fc} a && fc.Function is IdentifierReference ir && ir.TableIndices.Count == 1 && ir.TableIndices[0] is Constant c1 && 
                        c1.Type == ValueType.String && c1.String == "addElement" &&
                        b.Instructions[i + 1] is Assignment a22 && a22.Left.Count == 1 && 
                        a22.Left[0].TableIndices.Count == 1 && a22.Left[0].TableIndices[0] is Constant
                        {
                            Type: ValueType.String
                        } c &&
                        a22.Right is IdentifierReference ir2 && fc.Arguments[0] is IdentifierReference ir1 && ir2.Identifier == ir1.Identifier)
                    {
                        ir2.Identifier.Name = c.String;
                    }
                }
            }

            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment {Right: FunctionCall {Function: IdentifierReference ir} fc} && ir.TableIndices.Count == 1 && ir.TableIndices[0] is Constant
                    {
                        Type: ValueType.String, String: "mergeStateConditions"
                    } && fc.Arguments.Count == 1 && fc.Arguments[0] is InitializerList il)
                    {
                        foreach (var e in il.Expressions)
                        {
                            if (e is InitializerList il2 && il2.Expressions.Count == 2 && il2.Expressions.All(ex => ex is ListAssignment) &&
                                il2.Expressions[0] is ListAssignment {Left: Constant {Type: ValueType.String, String: "stateName"}, Right: Constant
                                    {
                                        Type: ValueType.String
                                    }
                                } && il2.Expressions[1] is ListAssignment {Left: Constant {Type: ValueType.String, String: "condition"}, Right: Closure cl})
                            {
                                cl.Function.ArgumentNames = new List<Local>() {new Local(){Name = "menu"}, new Local(){Name = "element"}, new Local(){Name = "event"}};
                            }
                        }
                    }
                }
            }
        }
    }
}