using System.Collections.Generic;
using CoDHVKDecompiler.Common.LuaFunction.Structures;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
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
                            if (l is IdentifierReference ir && !ir.HasIndex && ir.Identifier.IdentifierType == IdentifierType.Register && !renamed.Contains(ir.Identifier))
                            {
                                renamed.Add(l.Identifier);
                                ir.Identifier.Name = $@"f{f.Id}_local{localCounter}";
                                localCounter++;
                            }
                            ll++;
                        }
                    }
                }
            }

            // Adding special variable names for LUI widgets
            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a && a.Right is FunctionCall fc && a.Left.Count == 1)
                    {
                        string name;
                        if (fc.Function.ToString().Contains("LUI.UIElement.new"))
                        {
                            name = "Widget";
                        }
                        else if (fc.Function.ToString().Contains("CoD.Menu.NewForUIEditor"))
                        {
                            name = "HudRef";
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
                if (fc2.Function.ToString().Contains("LUI.UIElement.new") && f.Parameters.Count == 2)
                {
                    f.Parameters[0].Name = "HudRef";
                    f.Parameters[1].Name = "InstanceRef";
                }
                else if (fc2.Function.ToString().Contains("CoD.Menu.NewForUIEditor") && f.Parameters.Count == 1)
                {
                    f.Parameters[0].Name = "InstanceRef";
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
                                c.Function.ArgumentNames = new List<Local>(){new Local(){Name = "ModelRef"}};
                            }
                        }
                    }

                    if (i is Assignment {Right: FunctionCall fc5} && fc5.Function.ToString().Contains("registerEventHandler"))
                    {
                        if (fc5.Arguments.Count == 3 && fc5.Arguments[2] is Closure c2)
                        {
                            c2.Function.ArgumentNames = new List<Local>() {new Local(){Name = "Sender"}, new Local(){Name = "Event"}};
                        }
                    }
                    
                    if (i is Assignment {Right: FunctionCall fc6} && fc6.Function.ToString().Contains("LUI.OverrideFunction_CallOriginalSecond"))
                    {
                        if (fc6.Arguments.Count == 3 && fc6.Arguments[2] is Closure c2)
                        {
                            c2.Function.ArgumentNames = new List<Local>() {new Local(){Name = "Sender"}};
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
                    a4.Left[0].Identifier.Name = "ModelValue";
                }
            }
            
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 2; i++)
                {
                    var inst = b.Instructions[i];
                    if (inst is Assignment {Right: FunctionCall fc} && fc.Function.ToString().Contains("addElement") && fc.Arguments.Count == 2 && 
                        fc.Arguments[1] is IdentifierReference ir && b.Instructions[i + 1] is Assignment a22 && a22.Left.Count == 1 && 
                        a22.Left[0].TableIndices.Count == 1 && a22.Left[0].TableIndices[0] is Constant
                    {
                        Type: ValueType.String
                    } c)
                    {
                        ir.Identifier.Name = c.String;
                    }
                }
            }

            foreach (var b in f.Blocks)
            {
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a3 && a3.Right is FunctionCall c3 && c3.Function.ToString().Contains("mergeStateConditions") && c3.Arguments.Count == 2 && c3.Arguments[1] is InitializerList il)
                    {
                        foreach (var expression in il.Expressions)
                        {
                            if (expression is InitializerList il2 && il2.Expressions.Count == 2 && il2.Expressions[1] is ListAssignment la && la.Left is Constant c && c.String == "condition" && la.Right is Closure cl)
                            {
                                cl.Function.ArgumentNames = new List<Local>() {new Local(){Name = "HudRef"}, new Local(){Name = "ItemRef"}, new Local(){Name = "UpdateTable"}};
                            }
                        }
                    }
                }
            }
        }
    }
}