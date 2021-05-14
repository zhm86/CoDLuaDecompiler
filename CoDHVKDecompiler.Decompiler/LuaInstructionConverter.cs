using System;
using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Common;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction;
using CoDHVKDecompiler.Decompiler.IR;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Expression.Operator;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;
using ValueType = CoDHVKDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDHVKDecompiler.Decompiler
{
    public class LuaInstructionConverter
    {
        private readonly SymbolTable _symbolTable;
        
        public LuaInstructionConverter(SymbolTable symbolTable)
        {
            this._symbolTable = symbolTable;
        }
        
        private IdentifierReference RegisterReference(uint regIndex)
        {
            return new IdentifierReference(_symbolTable.GetRegister(regIndex));
        }

        private Constant ConvertLuaConstant(ILuaConstant con, int id)
        {
            if (con.Type == ConstantType.TNumber)
                return new Constant(con.NumberValue, id);
            if (con.Type == ConstantType.TString)
                return new Constant(con.StringValue, id);
            if (con.Type == ConstantType.TBoolean)
                return new Constant(con.BoolValue, id);
            if (con.Type == ConstantType.THash)
                return new Constant(con.HashValue, id);
            return new Constant(ValueType.Nil, id);
        }
        
        // CHeck this
        private IExpression RKIR(ILuaFunction fun, uint val)
        {
            if (val < 250)
            {
                return new IdentifierReference(_symbolTable.GetRegister(val));
            }
            else
            {
                return ConvertLuaConstant(fun.Constants[(int) (val - 250)], (int)val);
            }
        }

        private IExpression GetConstantBitFix(ILuaFunction luaFunction, uint val, bool extraBit)
        {
            if (extraBit)
                return ConvertLuaConstant(luaFunction.Constants[(int) val], (int) val);
            return new IdentifierReference(_symbolTable.GetRegister(val));
        }
        
        public void Convert(Function function, ILuaFunction luaFunction)
        {
            // Loop over all instructions
            for (int pos = 0; pos < luaFunction.Instructions.Count; pos++)
            {
                var i = luaFunction.Instructions[pos];

                List<IInstruction> instrs = new List<IInstruction>();
                switch (i.OpCode)
                {
                    case LuaOpCode.HKS_OPCODE_MOVE:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A), 
                            RegisterReference(i.B)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A), 
                            ConvertLuaConstant(luaFunction.Constants[(int) i.Bx], (int) i.Bx)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADBOOL:
                        var assn = new Assignment(_symbolTable.GetRegister(i.A), new Constant(i.B == 1, -1));
                        instrs.Add(assn);
                        if (i.C > 0)
                            instrs.Add(new Jump(function.GetLabel((uint)(pos + 2))));
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADNIL:
                        var nlist = new List<IdentifierReference>();
                        for (int arg = (int) i.A; arg <= i.B; arg++)
                            nlist.Add(new IdentifierReference(_symbolTable.GetRegister((uint) arg)));
                        assn = new Assignment(nlist, new Constant(ValueType.Nil, -1));
                        instrs.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_GETUPVAL:
                        Identifier up = function.UpvalueBindings[(int) i.B];
                        up.IsClosureBound = true;
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new IdentifierReference(up)));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETUPVAL:
                    case LuaOpCode.HKS_OPCODE_SETUPVAL_R1:
                        up = _symbolTable.GetUpValue(i.B);
                        if (luaFunction.Upvalues.Any() && !up.UpValueResolved)
                        {
                            up.Name = luaFunction.Upvalues[(int) i.B].Name;
                            up.UpValueResolved = true;
                        }
                        instrs.Add(new Assignment(up, new IdentifierReference(_symbolTable.GetRegister(i.A))));
                        break;
                    case LuaOpCode.HKS_OPCODE_GETGLOBAL_MEM:
                    case LuaOpCode.HKS_OPCODE_GETGLOBAL:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new IdentifierReference(_symbolTable.GetGlobal(luaFunction.Constants[(int) i.Bx].ToString(), (int) i.Bx))
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETGLOBAL:
                        instrs.Add(new Assignment(
                            _symbolTable.GetGlobal(luaFunction.Constants[(int) i.Bx].ToString(), (int)i.Bx),
                            new IdentifierReference(_symbolTable.GetRegister(i.A))));
                        break;
                    case LuaOpCode.HKS_OPCODE_GETTABLE_S:
                    case LuaOpCode.HKS_OPCODE_GETTABLE:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A), 
                            new IdentifierReference(_symbolTable.GetRegister(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit))
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_NEWTABLE:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A), 
                            new InitializerList()
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SELF:
                        var op = new Assignment(_symbolTable.GetRegister(i.A + 1), new IdentifierReference(_symbolTable.GetRegister(i.B)));
                        op.IsSelfAssignment = true;
                        instrs.Add(op);
                        op = new Assignment(_symbolTable.GetRegister(i.A), new IdentifierReference(_symbolTable.GetRegister(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit)));
                        op.IsSelfAssignment = true;
                        instrs.Add(op);
                        break;
                    case LuaOpCode.HKS_OPCODE_ADD:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpAdd)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_ADD_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpAdd)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SUB:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpSub)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SUB_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpSub)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_MUL:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpMul)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_MUL_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpMul)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_DIV:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpDiv)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_DIV_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpDiv)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_MOD:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpMod)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_MOD_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpMod)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_POW:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpPow)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_POW_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpPow)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_UNM:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new UnaryOp(new IdentifierReference(_symbolTable.GetRegister(i.B)), UnOperationType.OpNegate)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_NOT:
                    case LuaOpCode.HKS_OPCODE_NOT_R1:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new UnaryOp(new IdentifierReference(_symbolTable.GetRegister(i.B)), UnOperationType.OpNot)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_LEN:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new UnaryOp(new IdentifierReference(_symbolTable.GetRegister(i.B)), UnOperationType.OpLength)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SHIFT_LEFT:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpShiftLeft)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SHIFT_LEFT_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpShiftLeft)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SHIFT_RIGHT:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpShiftRight)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_SHIFT_RIGHT_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpShiftRight)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_BITWISE_AND:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpBAnd)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_BITWISE_AND_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpBAnd)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_BITWISE_OR:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), BinOperationType.OpBOr)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_BITWISE_OR_BK:
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A),
                            new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), BinOperationType.OpBOr)
                        ));
                        break;
                    case LuaOpCode.HKS_OPCODE_CONCAT:
                        var args = new List<IExpression>();
                        for (int arg = (int) i.B; arg <= i.C; arg++)
                            args.Add(new IdentifierReference(_symbolTable.GetRegister((uint)arg)));
                        instrs.Add(new Assignment(
                            _symbolTable.GetRegister(i.A), 
                            new Concat(args)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_JMP:
                        instrs.Add(new Jump(function.GetLabel((uint) (pos + 1 + i.SBx))));
                        break;
                    case LuaOpCode.HKS_OPCODE_EQ:
                        var operation = BinOperationType.OpEqual;
                        if (i.A == 1)
                            operation = BinOperationType.OpNotEqual;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_EQ_BK:
                        operation = BinOperationType.OpEqual;
                        if (i.A == 1)
                            operation = BinOperationType.OpNotEqual;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_LT:
                        operation = BinOperationType.OpLessThan;
                        if (i.A == 1)
                            operation = BinOperationType.OpGreaterEqual;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_LT_BK:
                        operation = BinOperationType.OpLessThan;
                        if (i.A == 1)
                            operation = BinOperationType.OpGreaterEqual;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_LE:
                        operation = BinOperationType.OpLessEqual;
                        if (i.A == 1)
                            operation = BinOperationType.OpGreaterThan;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(RegisterReference(i.B), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_LE_BK:
                        operation = BinOperationType.OpLessEqual;
                        if (i.A == 1)
                            operation = BinOperationType.OpGreaterThan;
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int) i.B), RegisterReference(i.C), operation)));
                        break;
                    case LuaOpCode.HKS_OPCODE_TEST:
                    case LuaOpCode.HKS_OPCODE_TEST_R1:
                        // This op is weird
                        instrs.Add(i.C == 0
                            ? new Jump(function.GetLabel((uint) (pos + 2)), RegisterReference(i.A))
                            : new Jump(function.GetLabel((uint) (pos + 2)),
                                new UnaryOp(RegisterReference(i.A), UnOperationType.OpNot)));
                        break;
                    case LuaOpCode.HKS_OPCODE_TESTSET:
                        // This op is weird
                        instrs.Add(i.C == 0
                            ? new Jump(function.GetLabel((uint) (pos + 2)),
                                new BinOp(RKIR(luaFunction, i.B), new Constant(0.0, -1), BinOperationType.OpNotEqual))
                            : new Jump(function.GetLabel((uint) (pos + 2)),
                                new BinOp(RKIR(luaFunction, i.B), new Constant(0.0, -1), BinOperationType.OpEqual)));
                        instrs.Add(new Assignment(_symbolTable.GetRegister(i.A), new IdentifierReference(_symbolTable.GetRegister(i.B))));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETTABLE:
                    case LuaOpCode.HKS_OPCODE_SETTABLE_S:
                        instrs.Add(new Assignment(
                            new IdentifierReference(_symbolTable.GetRegister(i.A), RegisterReference(i.B)), 
                            GetConstantBitFix(luaFunction, i.C, i.ExtraCBit)
                            ));
                        break;
                    case LuaOpCode.HKS_OPCODE_TAILCALL:
                    case LuaOpCode.HKS_OPCODE_TAILCALL_I:
                    case LuaOpCode.HKS_OPCODE_TAILCALL_I_R1:
                        args = new List<IExpression>();
                        for (int arg = (int)i.A + 1; arg < i.A + i.B; arg++)
                            args.Add(new IdentifierReference(_symbolTable.GetRegister((uint)arg)));
                        var funCall = new FunctionCall(new IdentifierReference(_symbolTable.GetRegister(i.A)), args);
                        funCall.IsIndeterminateArgumentCount = (i.B == 0);
                        funCall.BeginArg = i.A + 1;
                        instrs.Add(new Return(funCall) { IsTailReturn = true });
                        break;
                    case LuaOpCode.HKS_OPCODE_SETTABLE_S_BK:
                        instrs.Add(new Assignment(
                            new IdentifierReference(_symbolTable.GetRegister(i.A), ConvertLuaConstant(luaFunction.Constants[(int) i.B], (int)i.B)), 
                            GetConstantBitFix(luaFunction, i.C, i.ExtraCBit)));
                        break;
                    
                    case LuaOpCode.HKS_OPCODE_CALL_I:
                    case LuaOpCode.HKS_OPCODE_CALL_I_R1:
                    case LuaOpCode.HKS_OPCODE_CALL:
                        args = new List<IExpression>();
                        var rets = new List<IdentifierReference>();
                        for (int arg = (int) i.A + 1; arg < i.A + i.B; arg++)
                        {
                            args.Add(new IdentifierReference(_symbolTable.GetRegister((uint)arg)));
                        }
                        for (int r = (int)i.A + 1; r < i.A + i.C; r++)
                        {
                            rets.Add(new IdentifierReference(_symbolTable.GetRegister((uint)r - 1)));
                        }
                        if (i.C == 0)
                        {
                            rets.Add(new IdentifierReference(_symbolTable.GetRegister(i.A)));
                        }
                        funCall = new FunctionCall(new IdentifierReference(_symbolTable.GetRegister(i.A)), args);
                        funCall.IsIndeterminateArgumentCount = (i.B == 0);
                        funCall.IsIndeterminateReturnCount = (i.C == 0);
                        funCall.BeginArg = i.A + 1;
                        assn = new Assignment(rets, funCall);
                        instrs.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_RETURN:
                        args = new List<IExpression>();
                        if (i.B != 0)
                        {
                            for (int arg = (int)i.A; arg < i.A + i.B - 1; arg++)
                            {
                                args.Add(new IdentifierReference(_symbolTable.GetRegister((uint)arg)));
                            }
                        }
                        var ret = new Return(args);
                        if (i.B == 0)
                        {
                            ret.BeginRet = i.A;
                            ret.IsIndeterminateReturnCount = true;
                        }
                        instrs.Add(ret);
                        break;
                    case LuaOpCode.HKS_OPCODE_FORLOOP:
                        instrs.Add(new Assignment(new IdentifierReference(_symbolTable.GetRegister(i.A)), new BinOp(new IdentifierReference(_symbolTable.GetRegister(i.A)),
                            new IdentifierReference(_symbolTable.GetRegister(i.A + 2)), BinOperationType.OpAdd)));
                        var jmp = new Jump(function.GetLabel((uint) (pos + 1 + i.SBx)), new BinOp(new IdentifierReference(_symbolTable.GetRegister(i.A)),
                            new IdentifierReference(_symbolTable.GetRegister(i.A + 1)), BinOperationType.OpLoopCompare));
                        var pta = new Assignment(_symbolTable.GetRegister(i.A + 3), RegisterReference(i.A));
                        pta.PropagateAlways = true;
                        jmp.PostTakenAssignment = pta;
                        instrs.Add(jmp);
                        break;
                    case LuaOpCode.HKS_OPCODE_TFORLOOP:
                        args = new List<IExpression>();
                        rets = new List<IdentifierReference>();
                        args.Add(new IdentifierReference(_symbolTable.GetRegister(i.A + 1)));
                        args.Add(new IdentifierReference(_symbolTable.GetRegister(i.A + 2)));
                        if (i.C == 0)
                        {
                            rets.Add(new IdentifierReference(_symbolTable.GetRegister(i.A + 3)));
                        }
                        else
                        {
                            for (int r = (int)i.A + 3; r <= i.A + i.C + 2; r++)
                            {
                                rets.Add(new IdentifierReference(_symbolTable.GetRegister((uint)r)));
                            }
                        }
                        var fCall = new FunctionCall(new IdentifierReference(_symbolTable.GetRegister(i.A)), args);
                        fCall.IsIndeterminateReturnCount = (i.C == 0);
                        assn = new Assignment(rets, fCall);
                        instrs.Add(assn);
                        instrs.Add(new Jump(function.GetLabel((uint)(pos + 2)), new BinOp(RegisterReference(i.A + 3), new Constant(ValueType.Nil, -1), BinOperationType.OpEqual)));
                        instrs.Add(new Assignment(_symbolTable.GetRegister(i.A + 2), new IdentifierReference(_symbolTable.GetRegister(i.A + 3))));
                        break;
                    case LuaOpCode.HKS_OPCODE_FORPREP:
                        // The VM technically does a subtract, but we don't actually emit it since it simplifies things to map better to the high level Lua
                        //instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                        //    new IR.IdentifierReference(SymbolTable.GetRegister(a + 2)), IR.BinOp.OperationType.OpSub)));
                        instrs.Add(new Jump(function.GetLabel((uint) (pos + 1 + i.SBx))));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETLIST:
                        if (i.B == 0)
                        {
                            if (i.C == 1)
                            {
                                assn = new Assignment(_symbolTable.GetRegister(i.A), RegisterReference(i.A + 1))
                                {
                                    VarargAssignmentReg = i.A, IsIndeterminateVararg = true
                                };
                                instrs.Add(assn);
                            }
                        }
                        else
                        {
                            for (int j = 1; j <= i.B; j++)
                            {
                                assn = new Assignment(new IdentifierReference(_symbolTable.GetRegister(i.A), new Constant((double)(i.C - 1) * 32 + j, -1)),
                                    new IdentifierReference(_symbolTable.GetRegister(i.A + (uint)j)));
                                instrs.Add(assn);
                            }
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_CLOSURE:
                        instrs.Add(new Assignment(_symbolTable.GetRegister(i.A), new Closure(function.Closures[(int) i.Bx])));
                        break;
                    case LuaOpCode.HKS_OPCODE_GETFIELD:
                    case LuaOpCode.HKS_OPCODE_GETFIELD_R1:
                        assn = new Assignment(RegisterReference(i.A), new IdentifierReference(_symbolTable.GetRegister(i.B), new Constant(luaFunction.Constants[(int) i.C].ToString(), -1)));
                        instrs.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_DATA:
                        if (i.A != 0)
                        {
                            Function closureFunc = null;
                            int index = pos;

                            while (index >= 0)
                            {
                                if (luaFunction.Instructions[index].OpCode == LuaOpCode.HKS_OPCODE_CLOSURE)
                                {
                                    closureFunc = function.Closures[(int) luaFunction.Instructions[index].Bx];
                                    break;
                                }
                                index--;
                            }

                            if (closureFunc == null)
                            {
                                continue;
                            }

                            if (i.A == 1)
                            {
                                closureFunc.UpvalueBindings.Add(_symbolTable.GetRegister(i.C));
                            }
                            else if (i.A == 2)
                            {
                                closureFunc.UpvalueBindings.Add(function.UpvalueBindings[(int) i.C]);
                            }
                        }
                        else
                        {
                            instrs.Add(new Data(i));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_SETFIELD:
                    case LuaOpCode.HKS_OPCODE_SETFIELD_R1:
                        assn = new Assignment(new IdentifierReference(_symbolTable.GetRegister(i.A), new Constant(luaFunction.Constants[(int) i.B].ToString(), (int)i.B)), GetConstantBitFix(luaFunction, i.C, i.ExtraCBit));
                        instrs.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_VARARG:
                        var varArgs = new List<IdentifierReference>();
                        for (int arg = (int)i.A; arg <= i.A + i.B - 1; arg++)
                        {
                            varArgs.Add(new IdentifierReference(_symbolTable.GetRegister((uint)arg)));
                        }
                        if (i.B != 0)
                        {
                            assn = new Assignment(varArgs, new IdentifierReference(_symbolTable.GetVarargs()));
                        }
                        else
                        {
                            assn = new Assignment(_symbolTable.GetRegister(i.A), new IdentifierReference(_symbolTable.GetVarargs()));
                            assn.IsIndeterminateVararg = true;
                            assn.VarargAssignmentReg = i.A;
                        }
                        instrs.Add(assn);
                        function.IsVarArgs = true;
                        break;
                    case LuaOpCode.HKS_OPCODE_CLOSE:
                        // LUA source : close all variables in the stack up to (>=) R(A)
                        // Let's ignore this for now, doesn't print anything and don't know if it affects SSA
                        instrs.Add(new Close());
                        break;
                    default:
                        Console.WriteLine($@"Missing op: {i.OpCode} {i.A} {i.B} {i.C}");
                        instrs.Add(new PlaceholderInstruction($@"{i.OpCode} {i.A} {i.B} {i.C}"));
                        break;
                }
                instrs.ForEach(instr =>
                {
                    instr.OpLocation = pos;
                    function.Instructions.Add(instr);
                });
            }
        }
    }
}