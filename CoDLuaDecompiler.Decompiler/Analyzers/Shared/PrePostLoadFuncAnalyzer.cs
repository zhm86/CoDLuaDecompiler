using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Shared
{
    /**
     * Rename the Pre and Post load functions to the right name and apply parameter names
     */
    class PrePostLoadFuncAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
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
            
            // Find the Pre Load Func
            if (cwf.Blocks[0].Instructions[^1] is IfStatement preIf && preIf.Condition is IdentifierReference preIfIr &&
                cwf.Blocks[1].Instructions.Count == 1 && cwf.Blocks[1].Instructions[0] is Assignment preA && preA.Left.Count == 0 &&
                preA.Right is FunctionCall preFc && preFc.Function is IdentifierReference {HasIndex: false} preIr &&
                preIr.Identifier == preIfIr.Identifier)
            {
                preIr.Identifier.Name = "PreLoadFunc";
                if (preIr.Identifier.DefiningInstruction is Assignment preFuncA && preFuncA.Right is Closure preC && preC.Function.Parameters.Count == 2)
                {
                    preC.Function.Parameters[0].Name = "self";
                    preC.Function.Parameters[1].Name = "controller";
                }
            }
            
            // Find the Post Load Func
            if (cwf.Blocks[^3].Instructions.Count == 1 && cwf.Blocks[^3].Instructions[0] is Assignment postA && postA.Left.Count == 0 && 
                postA.Right is FunctionCall postFc && postFc.Function is IdentifierReference {HasIndex: false} postIr &&
                cwf.Blocks[^4].Instructions[^1] is IfStatement postIf && postIf.Condition is IdentifierReference postIfIr && postIfIr.Identifier == postIr.Identifier)
            {
                postIr.Identifier.Name = "PostLoadFunc";
                if (postIr.Identifier.DefiningInstruction is Assignment postFuncA && postFuncA.Right is Closure postC && postC.Function.Parameters.Count == 3)
                {
                    postC.Function.Parameters[0].Name = "self";
                    postC.Function.Parameters[1].Name = "controller";
                    postC.Function.Parameters[2].Name = "menu";
                }
                if (cwf.Blocks[^4].Instructions.Count > 3 && cwf.Blocks[^4].Instructions[^2] is not NewLine) 
                    cwf.Blocks[^4].Instructions.Insert(cwf.Blocks[^4].Instructions.Count - 1, new NewLine());
                cwf.Blocks[^2].Instructions.Insert(0, new NewLine());
            }
        }
    }
}