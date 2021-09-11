using System;
using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class ConvertToASTAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Traverse all the nodes in post-order and try to convert jumps to if statements
            var usedFollows = new HashSet<BasicBlock>();

            // Heads of for statements
            var forHeads = new HashSet<BasicBlock>();
            
            var relocalize = new HashSet<Identifier>();
            
            // Order the blocks sequentially
            for (int i = 0; i < f.Blocks.Count; i++)
            {
                f.Blocks[i].OrderNumber = i;
            }

            // Step 1: build the AST for ifs/loops based on follow information
            foreach (var node in f.PostorderTraversal(true))
            {
                // Search instructions for identifiers we need to relocalize
                foreach (var inst in node.Instructions)
                {
                    if (inst is Assignment asn)
                    {
                        foreach (var def in inst.GetDefines(true))
                        {
                            if (relocalize.Contains(def))
                            {
                                asn.IsLocalDeclaration = true;
                                relocalize.Remove(def);
                            }
                        }
                    }
                }
                
                // A for loop is a pretested loop where the follow does not match the head
                if (node.LoopFollow != null && node.LoopFollow != node && node.Predecessors.Count() >= 2 && node.LoopType == LoopType.LoopPretested)
                {
                    var loopInitializer = node.Predecessors.First(x => !node.LoopLatches.Contains(x));

                    // Match a numeric for
                    if (node.Instructions.Last() is Jump loopJump && loopJump.Condition is BinOp loopCondition && loopCondition.OperationType == BinOperationType.OpLoopCompare && node.Instructions[^2] is Assignment aaa3 && aaa3.Right != null && aaa3.Right is BinOp)
                    {
                        var nfor = new NumericFor();
                        nfor.Limit = loopCondition.Right;
                        Identifier loopvar = (loopCondition.Left as IdentifierReference).Identifier;
                        var incinst = node.Instructions[node.Instructions.Count() - 2];
                        nfor.Increment = ((incinst as Assignment).Right as BinOp).Right;

                        // Search the predecessor block for the initial assignments (i.e. the definition)
                        /*for (int i = loopInitializer.Instructions.Count() - 1; i >= 0; i--)
                        {
                            if (loopInitializer.Instructions[i] is Assignment a && a.GetDefines(true).Contains(loopvar))
                            {
                                nfor.Initial = a;
                                //if (!lua51)
                                loopInitializer.Instructions.RemoveAt(i);
                                break;
                            }
                        }*/

                        // Extract the step variable definition
                        if (loopInitializer.Instructions.Count > 2 && loopInitializer.Instructions[^2] is Assignment incassn)
                        {
                            nfor.Increment = incassn.Right;
                            if (incassn.IsLocalDeclaration)
                            {
                                relocalize.Add(incassn.Left[0].Identifier);
                            }
                            loopInitializer.Instructions.RemoveAt(loopInitializer.Instructions.Count - 2);
                        }

                        // Extract the limit variable definition
                        if (loopInitializer.Instructions.Count > 2 && loopInitializer.Instructions[^2] is Assignment limitassn)
                        {
                            nfor.Limit = limitassn.Right;
                            if (limitassn.IsLocalDeclaration)
                            {
                                relocalize.Add(limitassn.Left[0].Identifier);
                            }
                            loopInitializer.Instructions.RemoveAt(loopInitializer.Instructions.Count - 2);
                        }

                        // Extract the initializer variable definition
                        if (loopInitializer.Instructions.Count > 1 && loopInitializer.Instructions[loopInitializer.Instructions.Count - 2] is Assignment initassn)
                        {
                            nfor.Initial = initassn;
                            if (initassn.IsLocalDeclaration)
                            {
                                relocalize.Add(initassn.Left[0].Identifier);
                            }
                            loopInitializer.Instructions.RemoveAt(loopInitializer.Instructions.Count - 2);
                        }
                        
                        nfor.Body = node.Successors[1];
                        nfor.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            nfor.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                        {
                            loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = nfor;
                        }
                        else
                        {
                            loopInitializer.Instructions.Add(nfor);
                        }
                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                        
                        // Remove any jump instructions from the latches if they exist
                        foreach (var latch in node.LoopLatches)
                        {
                            if (latch.Instructions.Count > 0 && latch.Instructions.Last() is Jump jmp2 && !jmp2.Conditional && jmp2.BlockDest == node)
                            {
                                latch.Instructions.RemoveAt(latch.Instructions.Count - 1);
                            }
                        }
                    }

                    // Match a generic for with a predecessor initializer
                    else if (node.Instructions.Count > 0 && node.Instructions.Last() is Jump loopJump2 && loopJump2.Condition is BinOp loopCondition2 &&
                             loopInitializer.Instructions.Count >= 2 && loopInitializer.Instructions[loopInitializer.Instructions.Count - 2] is Assignment la &&
                             la.Left.Any() && la.Left[0] is IdentifierReference f1 && node.Instructions[0] is Assignment ba && ba.Right is FunctionCall fc &&
                             fc.Function is IdentifierReference fci && fci.Identifier == f1.Identifier)
                    {
                        var gfor = new GenericFor();
                        // Search the predecessor block for the initial assignment which contains the right expression
                        IExpression right = new IExpression();
                        for (int i = loopInitializer.Instructions.Count() - 1; i >= 0; i--)
                        {
                            if (loopInitializer.Instructions[i] is Assignment a)
                            {
                                right = a.Right;
                                loopInitializer.Instructions.RemoveAt(i);
                                break;
                            }
                        }

                        // Loop head has the loop variables
                        if (node.Instructions.First() is Assignment a2)
                        {
                            gfor.Iterator = new Assignment(a2.Left, right);
                            node.Instructions.RemoveAt(0);
                        }
                        else
                        {
                            throw new Exception("Unkown for pattern");
                        }

                        // Body contains more loop bytecode that can be removed
                        //var body = (node.Successors[0].ReversePostorderNumber > node.Successors[1].ReversePostorderNumber) ? node.Successors[0] : node.Successors[1];
                        var body = node.Successors[0];
                        if (body.Instructions[0] is Assignment a3)
                        {
                            body.Instructions.RemoveAt(0);
                        }

                        gfor.Body = body;
                        gfor.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            gfor.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                        {
                            loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = gfor;
                        }
                        else
                        {
                            loopInitializer.Instructions.Add(gfor);
                        }
                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                    }
                    // Generic for but without the func call
                    else if (node.Instructions.Count == 2 && node.Instructions.Last() is Jump loopJump3 && loopJump3.Condition is BinOp loopCondition3 &&
                             node.Instructions[0] is Assignment a3 && a3.Left.Count > 0 && a3.Right is FunctionCall fc3 && fc3.Function is IdentifierReference fc3ir &&
                             loopInitializer.Instructions.Count >= 2 && loopInitializer.Instructions.First() is Assignment a4 && a4.Right is IdentifierReference ir2 &&
                             a4.Left.Count == 1 && a4.Left[0].Identifier == fc3ir.Identifier
                    )
                    {
                        var gFor = new GenericFor();
                        var ids = new List<IExpression>();
                        for (int i = 0; i < loopInitializer.Instructions.Count - 1; i++)
                        {
                            ids.Add(((Assignment)loopInitializer.Instructions[i]).Right);
                            loopInitializer.Instructions.RemoveAt(i);
                            i--;
                        }
                        gFor.Iterator = new Assignment(a3.Left, new IdentifierRefList(ids));
                        
                        var body = node.Successors[0];
                        if (body.Instructions[0] is Assignment)
                        {
                            body.Instructions.RemoveAt(0);
                        }

                        gFor.Body = body;
                        gFor.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            gFor.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                        {
                            loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = gFor;
                        }
                        else
                        {
                            loopInitializer.Instructions.Add(gFor);
                        }
                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                    }

                    // Match a while
                    else if (node.Instructions.First() is Jump loopJump4 && loopJump4.Condition is { } loopCondition4)
                    {
                        var whiles = new While();

                        // Loop head has condition
                        whiles.Condition = loopCondition4;
                        node.Instructions.RemoveAt(node.Instructions.Count - 1);

                        //whiles.Body = (node.Successors[0].ReversePostorderNumber > node.Successors[1].ReversePostorderNumber) ? node.Successors[0] : node.Successors[1];
                        whiles.Body = node.Successors[0];
                        whiles.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            whiles.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        // If there's a goto to this loop head, replace it with the while. Otherwise replace the last instruction of this node
                        if (loopInitializer.Successors.Count == 1)
                        {
                            if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                            {
                                loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = whiles;
                            }
                            else
                            {
                                loopInitializer.Instructions.Add(whiles);
                            }
                        }
                        else
                        {
                            node.Instructions.Add(whiles);
                        }

                        // Remove gotos in latch
                        foreach (var pred in node.Predecessors)
                        {
                            if (pred.IsLoopLatch && pred.Instructions.Any() && pred.Instructions.Last() is Jump lj && !lj.Conditional)
                            {
                                pred.Instructions.RemoveAt(pred.Instructions.Count - 1);
                            }
                        }

                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                    }
                    // Match a while with iterator
                    else if (node.Instructions.Count == 2 && node.Instructions[0] is Assignment a5 && a5.Right is FunctionCall fc5 && node.Instructions[1] is Jump j5 && j5.Conditional)
                    {
                        var whileBlock = new While() {Condition = j5.Condition};
                        loopInitializer.Instructions.Add(a5);
                        
                        whileBlock.Body = node.Successors[0];
                        whileBlock.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            whileBlock.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        // If there's a goto to this loop head, replace it with the while. Otherwise replace the last instruction of this node
                        if (loopInitializer.Successors.Count == 1)
                        {
                            if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                            {
                                loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = whileBlock;
                            }
                            else
                            {
                                loopInitializer.Instructions.Add(whileBlock);
                            }
                        }
                        else
                        {
                            node.Instructions.Add(whileBlock);
                        }

                        // Remove gotos in latch
                        foreach (var pred in node.Predecessors)
                        {
                            if (pred.IsLoopLatch && pred.Instructions.Last() is Jump lj && !lj.Conditional)
                            {
                                pred.Instructions.RemoveAt(pred.Instructions.Count - 1);
                            }
                        }
                        
                        // Add the iterator instruction at the end
                        if (node.LoopLatches.Count == 1)
                        {
                            node.LoopLatches[0].Instructions.Add(new Assignment(new IdentifierReference(a5.Left[0].Identifier), a5.Right));
                        }

                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                    }

                    // Match a repeat while (single block)
                    else if (node.Instructions.Last() is Jump loopJump5 && loopJump5.Condition is IExpression loopCondition5 && node.LoopLatches.Count == 1 && node.LoopLatches[0] == node)
                    {
                        var whiles = new While();
                        whiles.IsPostTested = true;

                        // Loop head has condition
                        whiles.Condition = loopCondition5;
                        node.Instructions.RemoveAt(node.Instructions.Count - 1);

                        //whiles.Body = (node.Successors[0].ReversePostorderNumber > node.Successors[1].ReversePostorderNumber) ? node.Successors[0] : node.Successors[1];
                        whiles.Body = node.Successors[1];
                        whiles.Body.MarkCodegenerated(f.Id);
                        if (!usedFollows.Contains(node.LoopFollow))
                        {
                            whiles.Follow = node.LoopFollow;
                            usedFollows.Add(node.LoopFollow);
                            node.LoopFollow.MarkCodegenerated(f.Id);
                        }
                        // If there's a goto to this loop head, replace it with the while. Otherwise replace the last instruction of this node
                        if (loopInitializer.Successors.Count == 1)
                        {
                            if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                            {
                                loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = whiles;
                            }
                            else
                            {
                                loopInitializer.Instructions.Add(whiles);
                            }
                        }
                        else
                        {
                            node.Instructions.Add(whiles);
                        }

                        // Remove gotos in latch
                        foreach (var pred in node.Predecessors)
                        {
                            if (pred.IsLoopLatch && pred.Instructions.Last() is Jump lj && !lj.Conditional)
                            {
                                pred.Instructions.RemoveAt(pred.Instructions.Count - 1);
                            }
                        }

                        node.MarkCodegenerated(f.Id);
                        // The head might be the follow of an if statement, so do this to not codegen it
                        usedFollows.Add(node);
                    }
                }

                // repeat...until loop
                if (node.LoopType == LoopType.LoopPostTested)
                {
                    var whiles = new While();
                    whiles.IsPostTested = true;

                    // Loop head has condition
                    if (node.LoopLatches.Count != 1 || node.LoopLatches[0].Instructions.Count == 0 || !(node.LoopLatches[0].Instructions.Last() is Jump))
                    {
                        throw new Exception("Unrecognized post-tested loop");
                    }
                    whiles.Condition = ((Jump)node.LoopLatches[0].Instructions.Last()).Condition;

                    whiles.Body = node;
                    if (node.LoopFollow != null && !usedFollows.Contains(node.LoopFollow))
                    {
                        whiles.Follow = node.LoopFollow;
                        usedFollows.Add(node.LoopFollow);
                        node.LoopFollow.MarkCodegenerated(f.Id);
                    }

                    if (node.Predecessors.Count == 2)
                    {
                        var loopInitializer = node.Predecessors.First(x => x != node.LoopLatches[0]);
                        if (loopInitializer.Successors.Count == 1)
                        {
                            if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                            {
                                loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = whiles;
                            }
                            else
                            {
                                loopInitializer.Instructions.Add(whiles);
                            }
                        }
                        else
                        {
                            whiles.IsBlockInlined = true;
                            node.IsInfiniteLoop = true;
                            node.Instructions.Insert(0, whiles);
                        }
                    }
                    else
                    {
                        whiles.IsBlockInlined = true;
                        node.IsInfiniteLoop = true;
                        node.Instructions.Insert(0, whiles);
                    }

                    // Remove jumps in latch
                    foreach (var pred in node.Predecessors)
                    {
                        if (pred.IsLoopLatch && pred.Instructions.Last() is Jump lj)
                        {
                            pred.Instructions.RemoveAt(pred.Instructions.Count - 1);
                        }
                    }

                    node.MarkCodegenerated(f.Id);
                    // The head might be the follow of an if statement, so do this to not codegen it
                    usedFollows.Add(node);
                }

                // Infinite while loop
                if (node.LoopType == LoopType.LoopEndless)
                {
                    var whiles = new While {Condition = new Constant(true, -1), Body = node};

                    // Loop head has condition

                    if (node.LoopFollow != null && !usedFollows.Contains(node.LoopFollow))
                    {
                        whiles.Follow = node.LoopFollow;
                        usedFollows.Add(node.LoopFollow);
                        node.LoopFollow.MarkCodegenerated(f.Id);
                    }

                    if (node.Predecessors.Count == 2)
                    {
                        var loopInitializer = node.Predecessors.First(x => !node.LoopLatches.Contains(x));
                        if (loopInitializer.Successors.Count == 1)
                        {
                            if (loopInitializer.Instructions.Any() && loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] is Jump)
                            {
                                loopInitializer.Instructions[loopInitializer.Instructions.Count() - 1] = whiles;
                            }
                            else
                            {
                                loopInitializer.Instructions.Add(whiles);
                            }
                        }
                        else
                        {
                            whiles.IsBlockInlined = true;
                            node.IsInfiniteLoop = true;
                            node.Instructions.Insert(0, whiles);
                        }
                    }
                    else
                    {
                        whiles.IsBlockInlined = true;
                        node.IsInfiniteLoop = true;
                        node.Instructions.Insert(0, whiles);
                    }

                    // Remove gotos in latch
                    foreach (var pred in node.Predecessors)
                    {
                        if (pred.IsLoopLatch && pred.Instructions.Last() is Jump {Conditional: false})
                        {
                            pred.Instructions.RemoveAt(pred.Instructions.Count - 1);
                        }
                    }

                    node.MarkCodegenerated(f.Id);
                    // The head might be the follow of an if statement, so do this to not codegen it
                    usedFollows.Add(node);
                }

                // Pattern match for an if statement
                if (node.Follow != null && node.Instructions.Count > 0 && node.Instructions.Last() is Jump jmp)
                {
                    var ifStatement = new IfStatement {Condition = jmp.Condition};
                    // Check for empty if block
                    if (node.Successors[0] != node.Follow)
                    {
                        ifStatement.TrueBody = node.Successors[0];
                        ifStatement.TrueBody.MarkCodegenerated(f.Id);
                        if (ifStatement.TrueBody.Instructions.Any() && ifStatement.TrueBody.Instructions.Last() is Jump lj && !lj.Conditional)
                        {
                            if (ifStatement.TrueBody.IsBreakNode)
                            {
                                ifStatement.TrueBody.Instructions[ifStatement.TrueBody.Instructions.Count() - 1] = new Break();
                            }
                            else if (ifStatement.TrueBody.IsContinueNode)
                            {
                                ifStatement.TrueBody.Instructions[ifStatement.TrueBody.Instructions.Count() - 1] = new Continue();
                            }
                            else if (ifStatement.TrueBody.IsLoopLatch || !ifStatement.TrueBody.Successors[0].IsLoopHead)
                            {
                                if (!ifStatement.TrueBody.IsLoopLatch && lj.BlockDest == node.Follow && node.Successors[1] == node.Follow && ifStatement.TrueBody.OrderNumber + 1 == node.Follow.OrderNumber)
                                {
                                    // Generate an empty else statement if there's a jump to the follow, the follow is the next block sequentially, and it isn't fallthrough
                                    ifStatement.FalseBody = new BasicBlock();
                                }
                                ifStatement.TrueBody.Instructions.Remove(lj);
                            }
                        }
                        //if (ifStatement.True.Instructions.Last() is Jump && ifStatement.True.IsContinueNode)
                        if (node.IsContinueNode)// && node.Successors[0].IsLoopHead)
                        {
                            var bb = new BasicBlock {Instructions = new List<IInstruction>() {new Continue()}};
                            ifStatement.TrueBody = bb;
                        }
                    }
                    if (node.Successors[1] != node.Follow)
                    {
                        ifStatement.FalseBody = node.Successors[1];
                        ifStatement.FalseBody.MarkCodegenerated(f.Id);
                        if (ifStatement.FalseBody.Instructions.Any() && ifStatement.FalseBody.Instructions.Last() is Jump
                        {
                            Conditional: false
                        } fj)
                        {
                            if (ifStatement.FalseBody.IsBreakNode)
                            {
                                ifStatement.FalseBody.Instructions[ifStatement.FalseBody.Instructions.Count() - 1] = new Break();
                            }
                            else if (!ifStatement.FalseBody.Successors[0].IsLoopHead)
                            {
                                ifStatement.FalseBody.Instructions.Remove(fj);
                            }
                        }
                        if (node.IsContinueNode && node.Successors[1].IsLoopHead)
                        {
                            var bb = new BasicBlock {Instructions = new List<IInstruction>() {new Continue()}};
                            ifStatement.FalseBody = bb;
                        }
                    }
                    if (!usedFollows.Contains(node.Follow))
                    {
                        ifStatement.Follow = node.Follow;
                        ifStatement.Follow.MarkCodegenerated(f.Id);
                        usedFollows.Add(node.Follow);
                    }
                    node.Instructions[node.Instructions.Count() - 1] = ifStatement;
                }
            }

            // Step 2: Remove Jmp instructions from follows if they exist
            foreach (var follow in usedFollows)
            {
                if (follow.Instructions.Any() && follow.Instructions.Last() is Jump jmp)
                {
                    follow.Instructions.Remove(jmp);
                }
            }

            // Step 3: For debug walk the CFG and print blocks that haven't been codegened
            foreach (var b in f.PostorderTraversal(true))
            {
                if (b != f.StartBlock && !b.IsCodeGenerated)
                {
#if DEBUG
                    Console.WriteLine($@"Warning: block_{b.Id} in function {f.Id} was not used in code generation. THIS IS LIKELY A DECOMPILER BUG!");
#endif
                }
            }

            f.IsAST = true;
        }
    }
}