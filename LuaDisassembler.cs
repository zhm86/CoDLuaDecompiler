using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLuaDecompiler.LuaFileTypes;
using DSLuaDecompiler.LuaFileTypes.Structures;
using SoulsFormats;

namespace luadec
{
    class LuaDisassembler
    {
        public enum Lua502Ops
        {
            OpMove = 0,
            OpLoadK = 1,
            OpLoadBool = 2,
            OpLoadNil = 3,
            OpGetUpVal = 4,
            OpGetGlobal = 5,
            OpGetTable = 6,
            OpSetGlobal = 7,
            OpSetUpVal = 8,
            OpSetTable = 9,
            OpNewTable = 10,
            OpSelf = 11,
            OpAdd = 12,
            OpSub = 13,
            OpMul = 14,
            OpDiv = 15,
            OpPow = 16,
            OpUnm = 17,
            OpNot = 18,
            OpConcat = 19,
            OpJmp = 20,
            OpEq = 21,
            OpLt = 22,
            OpLe = 23,
            OpTest = 24,
            OpCall = 25,
            OpTailCall = 26,
            OpReturn = 27,
            OpForLoop = 28,
            OpTForLoop = 29,
            OpTForPRep = 30,
            OpSetList = 31,
            OpSetListTo = 32,
            OpClose = 33,
            OpClosure = 34
        }

        public enum LuaHKSOps
        {
            OpGetField = 0,
            OpTest = 1,
            OpCallI = 2,
            OpCallC = 3,
            OpEq = 4,
            OpEqBk = 5,
            OpGetGlobal = 6,
            OpMove = 7,
            OpSelf = 8,
            OpReturn = 9,
            OpGetTableS = 10,
            OpGetTableN = 11,
            OpGetTable = 12,
            OpLoadBool = 13,
            OpTForLoop = 14,
            OpSetField = 15,
            OpSetTableS = 16,
            OpSetTableSBK = 17,
            OpSetTableN = 18,
            OpSetTableNBK = 19,
            OpSetTable = 20,
            OpSetTableBK = 21,
            OpTailCallI = 22,
            OpTailCallC = 23,
            OpTailCallM = 24,
            OpLoadK = 25,
            OpLoadNil = 26,
            OpSetGlobal = 27,
            OpJmp = 28,
            OpCallM = 29,
            OpCall = 30,
            OpIntrinsicIndex = 31,
            OpIntrinsicNewIndex = 32,
            OpIntrinsicSelf = 33,
            OpIntrinsicIndexLiteral = 34,
            OpIntrinsicNewIndexLiteral = 35,
            OpIntrinsicSelfLiteral = 36,
            OpTailCall = 37,
            OpGetUpVal = 38,
            OpSetUpVal = 39,
            OpAdd = 40,
            OpAddBk = 41,
            OpSub = 42,
            OpSubBk = 43,
            OpMul = 44,
            OpMulBk = 45,
            OpDiv = 46,
            OpDivBk = 47,
            OpMod = 48,
            OpModBk = 49,
            OpPow = 50,
            OpPowBk = 51,
            OpNewTable = 52,
            OpUnm = 53,
            OpNot = 54,
            OpLen = 55,
            OpLt = 56,
            OpLtBk = 57,
            OpLe = 58,
            OpLeBk = 59,
            OpConcat = 60,
            OpTestSet = 61,
            OpForPrep = 62,
            OpForLoop = 63,
            OpSetList = 64,
            OpClose = 65,
            OpClosure = 66,
            OpVarArg = 67,
            OpTailCallIR1 = 68,
            OpCallIR1 = 69,
            OpSetUpValR1 = 70,
            OpTestR1 = 71,
            OpNotR1 = 72,
            OpGetFieldR1 = 73,
            OpSetFieldR1 = 74,
            OpNewStruct = 75,
            OpData = 76,
            OpSetSlotN = 77,
            OpSetSlotI = 78,
            OpSetSlot = 79,
            OpSetSlotS = 80,
            OpSetSlotMT = 81,
            OpCheckType = 82,
            OpCheckTypeS = 83,
            OpGetSlot = 84,
            OpGetSlotMT = 85,
            OpSelfSlot = 86,
            OpSelfSlotMT = 87,
            OpGetFieldMM = 88,
            OpCheckTypeD = 89,
            OpGetSlotD = 90,
            OpGetGlobalMem = 91,
        }

        public enum OpMode
        {
            IABC,
            IABx,
            IAsBx,
        }

        public class OpProperties
        {
            public string OpName;
            public OpMode OpMode;

            public OpProperties (string name)
            {
                OpName = name;
            }

            public OpProperties(string name, OpMode mode)
            {
                OpName = name;
                OpMode = mode;
            }
        }

        public static OpProperties[] OpProperties502 =
        {
            new OpProperties("MOVE", OpMode.IABC),
            new OpProperties("LOADK", OpMode.IABx),
            new OpProperties("LOADBOOL", OpMode.IABC),
            new OpProperties("LOADNIL", OpMode.IABC),
            new OpProperties("GETUPVAL", OpMode.IABC),
            new OpProperties("GETGLOBAL", OpMode.IABx),
            new OpProperties("GETTABLE", OpMode.IABC),
            new OpProperties("SETGLOBAL", OpMode.IABx),
            new OpProperties("SETUPVAL", OpMode.IABC),
            new OpProperties("SETTABLE", OpMode.IABC),
            new OpProperties("NEWTABLE", OpMode.IABC),
            new OpProperties("SELF", OpMode.IABC),
            new OpProperties("ADD", OpMode.IABC),
            new OpProperties("SUB", OpMode.IABC),
            new OpProperties("MUL", OpMode.IABC),
            new OpProperties("DIV", OpMode.IABC),
            new OpProperties("POW", OpMode.IABC),
            new OpProperties("UNM", OpMode.IABC),
            new OpProperties("NOT", OpMode.IABC),
            new OpProperties("CONCAT", OpMode.IABC),
            new OpProperties("JMP", OpMode.IAsBx),
            new OpProperties("EQ", OpMode.IABC),
            new OpProperties("LT", OpMode.IABC),
            new OpProperties("LE", OpMode.IABC),
            new OpProperties("TEST", OpMode.IABC),
            new OpProperties("CALL", OpMode.IABC),
            new OpProperties("TAILCALL", OpMode.IABC),
            new OpProperties("RETURN", OpMode.IABC),
            new OpProperties("FORLOOP", OpMode.IAsBx),
            new OpProperties("TFORLOOP", OpMode.IABC),
            new OpProperties("TFORPREP", OpMode.IAsBx),
            new OpProperties("SETLIST", OpMode.IABx),
            new OpProperties("SETLISTTO", OpMode.IABx),
            new OpProperties("CLOSE", OpMode.IABC),
            new OpProperties("CLOSURE", OpMode.IABx),
        };

        public static OpProperties[] OpPropertiesHKS =
        {
            new OpProperties("GETFIELD", OpMode.IABC),
            new OpProperties("TEST", OpMode.IABC),
            new OpProperties("CALL_I", OpMode.IABC),
            new OpProperties("CALL_C"),
            new OpProperties("EQ", OpMode.IABC),
            new OpProperties("EQ_BK"),
            new OpProperties("GETGLOBAL"),
            new OpProperties("MOVE", OpMode.IABC),
            new OpProperties("SELF", OpMode.IABC),
            new OpProperties("RETURN", OpMode.IABC),
            new OpProperties("GETTABLE_S", OpMode.IABC),
            new OpProperties("GETTABLE_N"),
            new OpProperties("GETTABLE"),
            new OpProperties("LOADBOOL", OpMode.IABC),
            new OpProperties("TFORLOOP", OpMode.IABC),
            new OpProperties("SETFIELD", OpMode.IABC),
            new OpProperties("SETTABLE_S", OpMode.IABC),
            new OpProperties("SETTABLE_S_BK", OpMode.IABC),
            new OpProperties("SETTABLE_N"),
            new OpProperties("SETTABLE_N_BK"),
            new OpProperties("SETTABLE", OpMode.IABC),
            new OpProperties("SETTABLE_BK"),
            new OpProperties("TAILCALL_I", OpMode.IABC),
            new OpProperties("TAILCALL_C"),
            new OpProperties("TAILCALL_M"),
            new OpProperties("LOADK", OpMode.IABx),
            new OpProperties("LOADNIL", OpMode.IABC),
            new OpProperties("SETGLOBAL", OpMode.IABx),
            new OpProperties("JMP", OpMode.IAsBx),
            new OpProperties("CALL_M"),
            new OpProperties("CALL"),
            new OpProperties("INTRINSIC_INDEX"),
            new OpProperties("INTRINSIC_NEWINDEX"),
            new OpProperties("INTRINSIC_SELF"),
            new OpProperties("INTRINSIC_INDEX_LITERAL"),
            new OpProperties("INTRINSIC_NEWINDEX_LITERAL"),
            new OpProperties("INTRINSIC_SELF_LITERAL"),
            new OpProperties("TAILCALL"),
            new OpProperties("GETUPVAL", OpMode.IABC),
            new OpProperties("SETUPVAL", OpMode.IABC),
            new OpProperties("ADD", OpMode.IABC),
            new OpProperties("ADD_BK", OpMode.IABC),
            new OpProperties("SUB", OpMode.IABC),
            new OpProperties("SUB_BK", OpMode.IABC),
            new OpProperties("MUL", OpMode.IABC),
            new OpProperties("MUL_BK", OpMode.IABC),
            new OpProperties("DIV", OpMode.IABC),
            new OpProperties("DIV_BK", OpMode.IABC),
            new OpProperties("MOD", OpMode.IABC),
            new OpProperties("MOD_BK", OpMode.IABC),
            new OpProperties("POW", OpMode.IABC),
            new OpProperties("POW_BK", OpMode.IABC),
            new OpProperties("NEWTABLE", OpMode.IABC),
            new OpProperties("UNM", OpMode.IABC),
            new OpProperties("NOT", OpMode.IABC),
            new OpProperties("LEN", OpMode.IABC),
            new OpProperties("LT", OpMode.IABC),
            new OpProperties("LT_BK", OpMode.IABC),
            new OpProperties("LE", OpMode.IABC),
            new OpProperties("LE_BK", OpMode.IABC),
            new OpProperties("CONCAT", OpMode.IABC),
            new OpProperties("TESTSET"),
            new OpProperties("FORPREP", OpMode.IAsBx),
            new OpProperties("FORLOOP", OpMode.IAsBx),
            new OpProperties("SETLIST", OpMode.IABC),
            new OpProperties("CLOSE"),
            new OpProperties("CLOSURE", OpMode.IABx),
            new OpProperties("VARARG", OpMode.IABC),
            new OpProperties("TAILCALL_I_R1"),
            new OpProperties("CALL_I_R1", OpMode.IABC),
            new OpProperties("SETUPVAL_R1", OpMode.IABC),
            new OpProperties("TEST_R1", OpMode.IABC),
            new OpProperties("NOT_R1"),
            new OpProperties("GETFIELD_R1", OpMode.IABC),
            new OpProperties("SETFIELD_R1", OpMode.IABC),
            new OpProperties("NEWSTRUCT"),
            new OpProperties("DATA", OpMode.IABx),
            new OpProperties("SETSLOTN"),
            new OpProperties("SETSLOTI"),
            new OpProperties("SETSLOT"),
            new OpProperties("SETSLOTS"),
            new OpProperties("SETSLOTMT"),
            new OpProperties("CHECKTYPE"),
            new OpProperties("CHECKTYPES"),
            new OpProperties("GETSLOT"),
            new OpProperties("GETSLOTMT"),
            new OpProperties("SELFSLOT"),
            new OpProperties("SELFSLOTMT"),
            new OpProperties("GETFIELD_MM"),
            new OpProperties("CHECKTYPE_D"),
            new OpProperties("GETSLOT_D"),
            new OpProperties("GETGLOBAL_MEM", OpMode.IABx),
        };

        public static IR.SymbolTable SymbolTable = new IR.SymbolTable();

        private static string RK(Function fun, uint val)
        {
            if (val < 250)
            {
                return $@"R({val})";
            }
            else
            {
                return fun.Constants[(int) (val - 250)].ToString();
            }
        }

        private static IR.Expression RKIR(Function fun, uint val)
        {
            if (val < 250)
            {
                return new IR.IdentifierReference(SymbolTable.GetRegister(val));
            }
            else
            {
                return ToConstantIR(fun.Constants[(int) (val - 250)]);
            }
        }

        private static IR.Expression RKIRHKS(Function fun, uint val, bool szero)
        {
            if (val >= 0 && !szero)
            {
                return new IR.IdentifierReference(SymbolTable.GetRegister((uint)val));
            }
            else if (szero)
            {
                return ToConstantIR(fun.Constants[(int) val]);
            }
            else
            {
                return ToConstantIR(fun.Constants[(int) -val]);
            }
        }

        private static IR.IdentifierReference Register(uint reg)
        {
            return new IR.IdentifierReference(SymbolTable.GetRegister((uint)reg));
        }

        /*public static void DisassembleFunction(Function fun)
        {
            Console.WriteLine($@"Constants:");
            for (int i = 0; i < fun.Constants.Count; i++)
            {
                Console.WriteLine($@"{i}: {fun.Constants[i].ToString()}");
            }
            Console.WriteLine();
            BinaryReaderEx br = new BinaryReaderEx(false, fun.Bytecode);
            for (int i = 0; i < fun.Bytecode.Length; i += 4)
            {
                uint instruction = br.ReadUInt32();
                uint opcode = instruction & 0x3F;
                uint a = instruction >> 24;
                uint b = (instruction >> 15) & 0x1FF;
                uint c = (instruction >> 6) & 0x1FF;
                uint bx = (instruction >> 6) & 0x3FFFF;
                int sbx = ((int)bx - (((1 << 18) - 1) >> 1));
                string args = "";
                switch ((Lua502Ops)opcode)
                {
                    case Lua502Ops.OpMove:
                        Console.WriteLine($@"R({a}) := R({b})");
                        break;
                    case Lua502Ops.OpLoadK:
                        Console.WriteLine($@"R({a}) := {fun.Constants[bx].ToString()}");
                        break;
                    case Lua502Ops.OpLoadBool:
                        Console.WriteLine($@"R({a}) := (Bool){b}");
                        Console.WriteLine($@"if ({c}) PC++");
                        break;
                    case Lua502Ops.OpGetGlobal:
                        Console.WriteLine($@"R({a}) := Gbl[{fun.Constants[bx].ToString()}]");
                        break;
                    case Lua502Ops.OpGetTable:
                        Console.WriteLine($@"R({a}) := R({b})[{RK(fun, c)}]");
                        break;
                    case Lua502Ops.OpSetGlobal:
                        Console.WriteLine($@"Gbl[{fun.Constants[bx].ToString()}] := R({a})");
                        break;
                    case Lua502Ops.OpNewTable:
                        Console.WriteLine($@"R({a}) := {{}} size = {b}, {c}");
                        break;
                    case Lua502Ops.OpSelf:
                        Console.WriteLine($@"R({a+1}) := R({b})");
                        Console.WriteLine($@"R({a}) := R({b})[{RK(fun, c)}]");
                        break;
                    case Lua502Ops.OpAdd:
                        Console.WriteLine($@"R({a}) := {RK(fun, b)} + {RK(fun, c)}");
                        break;
                    case Lua502Ops.OpSub:
                        Console.WriteLine($@"R({a}) := {RK(fun, b)} - {RK(fun, c)}");
                        break;
                    case Lua502Ops.OpMul:
                        Console.WriteLine($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}");
                        break;
                    case Lua502Ops.OpDiv:
                        Console.WriteLine($@"R({a}) := {RK(fun, b)} / {RK(fun, c)}");
                        break;
                    case Lua502Ops.OpPow:
                        Console.WriteLine($@"R({a}) := {RK(fun, b)} ^ {RK(fun, c)}");
                        break;
                    case Lua502Ops.OpUnm:
                        Console.WriteLine($@"R({a}) := -R({b})");
                        break;
                    case Lua502Ops.OpNot:
                        Console.WriteLine($@"R({a}) := not R({b})");
                        break;
                    case Lua502Ops.OpJmp:
                        Console.WriteLine($@"PC += {sbx}");
                        break;
                    case Lua502Ops.OpEq:
                        Console.WriteLine($@"if (({RK(fun, b)} == {RK(fun, c)}) ~= {a}) PC++");
                        break;
                    case Lua502Ops.OpLt:
                        Console.WriteLine($@"if (({RK(fun, b)} <  {RK(fun, c)}) ~= {a}) PC++");
                        break;
                    case Lua502Ops.OpLe:
                        Console.WriteLine($@"if (({RK(fun, b)} <= {RK(fun, c)}) ~= {a}) PC++");
                        break;
                    //case Lua502Ops.OpTest:
                    //    Console.WriteLine($@"if (R({b}) <=> {c}) then R({a}) := R({b}) else PC++");
                    //    break;
                    case Lua502Ops.OpSetTable:
                        Console.WriteLine($@"R({a})[{RK(fun, b)}] := R({c})");
                        break;
                    case Lua502Ops.OpCall:
                        args = "";
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            if (arg != a + 1)
                                args += ", ";
                            args += $@"R({arg})";
                        }
                        Console.WriteLine($@"R({a}) := R({a})({args})");
                        break;
                    case Lua502Ops.OpReturn:
                        args = "";
                        for (int arg = (int)a; arg < a + b - 1; arg++)
                        {
                            if (arg != a)
                                args += ", ";
                            args += $@"R({arg})";
                        }
                        Console.WriteLine($@"return {args}");
                        break;
                    case Lua502Ops.OpClosure:
                        args = "";
                        for (int arg = (int)a; arg < a + fun.ChildFunctions[bx].ParameterCount; arg++)
                        {
                            args += ", ";
                            args += $@"R({arg})";
                        }
                        Console.WriteLine($@"R({a}) := closure(KPROTO[{bx}]{args})");
                        break;
                    default:
                        switch (OpProperties502[opcode].OpMode)
                        {
                            case OpMode.IABC:
                                Console.WriteLine($@"{OpProperties502[opcode].OpName} {instruction >> 24} {(instruction >> 15) & 0x1FF} {(instruction >> 6) & 0x1FF}");
                                break;
                            case OpMode.IABx:
                                Console.WriteLine($@"{OpProperties502[opcode].OpName} {instruction >> 24} {(instruction >> 6) & 0x3FFFF}");
                                break;
                            case OpMode.IAsBx:
                                Console.WriteLine($@"{OpProperties502[opcode].OpName} {instruction >> 24} {(instruction >> 6) & 0x3FFFF}");
                                break;
                        }
                        break;
                }
            }

            Console.WriteLine("\nClosures {");
            for (int i = 0; i < fun.ChildFunctions.Count; i++)
            {
                Console.WriteLine($@"Closure {i}:");
                DisassembleFunction(fun.ChildFunctions[i]);
            }
            Console.WriteLine("}");
        }*/

        /*private static IR.Constant ToConstantIR(Constant con)
        {
            if (con.Type == ConstantType.TNumber)
            {
                return new IR.Constant(con.NumberValue);
            }
            else if (con.Type == ConstantType.TString)
            {
                return new IR.Constant(con.StringValue);
            }
            return new IR.Constant(IR.Constant.ConstantType.ConstNil);
        }*/

        private static IR.Constant ToConstantIR(Constant con)
        {
            if (con.Type == ConstantType.TNumber)
            {
                return new IR.Constant(con.NumberValue);
            }
            else if (con.Type == ConstantType.TString)
            {
                return new IR.Constant(con.StringValue);
            }
            else if (con.Type == ConstantType.TBoolean)
            {
                return new IR.Constant(con.BoolValue);
            }
            return new IR.Constant(IR.Constant.ConstantType.ConstNil);
        }

        public static void GenerateIR50(IR.Function irfun, Function fun)
        {
            // First register closures for all the children
            for (int i = 0; i < fun.ChildFunctions.Count; i++)
            {
                irfun.AddClosure(new IR.Function());
            }

            SymbolTable.BeginScope();
            var parameters = new List<IR.Identifier>();
            for (uint i = 0; i < fun.ParameterCount; i++)
            {
                parameters.Add(SymbolTable.GetRegister(i));
            }
            irfun.SetParameters(parameters);

            for (int i = 0; i < fun.Instructions.Count * 4; i += 4)
            {
                var instruction = fun.Instructions[i / 4];
                //uint opcode = instruction & 0x3F;
                // Uhhh thanks again hork
                var opcode = instruction.OpCode;
                var a = instruction.A;
                var c = instruction.C;
                var b = instruction.B;
                var szero = instruction.ExtraCBit;

                var bx = instruction.Bx;
                var sbx = instruction.SBx;
                uint addr;
                var pc = i / 4;
                
                List<IR.Expression> args = null;
                List<IR.IInstruction> instructions = new List<IR.IInstruction>();
                IR.Assignment assn;
                switch ((Lua502Ops)opcode)
                {
                    case Lua502Ops.OpMove:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister(b)));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpLoadK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {fun.Constants[b].ToString()}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), ToConstantIR(fun.Constants[(int) bx]));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpLoadBool:
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.Constant(b == 1));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        if (c > 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2))));
                        }
                        break;
                    case Lua502Ops.OpLoadNil:
                        var nassn = new List<IR.IdentifierReference>();
                        for (int arg = (int)a; arg <= b; arg++)
                        {
                            nassn.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        assn = new IR.Assignment(nassn, new IR.Constant(IR.Constant.ConstantType.ConstNil));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpGetUpVal:
                        var up = SymbolTable.GetUpvalue(b);
                        if (fun.Upvalues.Count() > 0 && !up.UpvalueResolved)
                        {
                            up.Name = fun.Upvalues[(int) b].Name;
                            up.UpvalueResolved = true;
                        }
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(up));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpGetGlobal:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := Gbl[{fun.Constants[bx].ToString()}]")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetGlobal(fun.Constants[(int) bx].ToString())));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpGetTable:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})[{RK(fun, c)}]")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister(b), RKIR(fun, c)));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpSetGlobal:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"Gbl[{fun.Constants[bx].ToString()}] := R({a})")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetGlobal(fun.Constants[(int) bx].ToString()), new IR.IdentifierReference(SymbolTable.GetRegister(a))));
                        break;
                    case Lua502Ops.OpSetUpVal:
                        var up2 = SymbolTable.GetUpvalue(b);
                        if (fun.Upvalues.Count() > 0 && !up2.UpvalueResolved)
                        {
                            up2.Name = fun.Upvalues[(int) b].Name;
                            up2.UpvalueResolved = true;
                        }
                        instructions.Add(new IR.Assignment(up2, new IR.IdentifierReference(SymbolTable.GetRegister(a))));
                        break;
                    case Lua502Ops.OpNewTable:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {{}} size = {b}, {c}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.InitializerList(new List<IR.Expression>()));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpSelf:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a + 1}) := R({b})")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a + 1), new IR.IdentifierReference(SymbolTable.GetRegister(b))));
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})[{RK(fun, c)}]")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister(b), RKIR(fun, c))));
                        break;
                    case Lua502Ops.OpAdd:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} + {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpAdd));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpSub:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} - {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpSub));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpMul:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpMul));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpDiv:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} / {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpDiv));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpPow:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} ^ {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpPow));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpUnm:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := -R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a),
                            new IR.UnaryOp(new IR.IdentifierReference(SymbolTable.GetRegister(b)), IR.UnaryOp.OperationType.OpNegate));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpNot:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := not R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a),
                            new IR.UnaryOp(new IR.IdentifierReference(SymbolTable.GetRegister(b)), IR.UnaryOp.OperationType.OpNot));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpConcat:
                        args = new List<IR.Expression>();
                        for (int arg = (int)b; arg <= c; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.Concat(args));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpJmp:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"PC += {sbx}")));
                        instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + sbx + 1))));
                        break;
                    case Lua502Ops.OpEq:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} == {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpNotEqual)));
                        }
                        break;
                    case Lua502Ops.OpLt:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} < {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpLessThan)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpGreaterEqual)));
                        }
                        break;
                    case Lua502Ops.OpLe:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} <= {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpLessEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), RKIR(fun, c), IR.BinOp.OperationType.OpGreaterThan)));
                        }
                        break;
                    case Lua502Ops.OpTest:
                        // This op is weird
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (R({b}) <=> {c}) then R({a}) := R({b}) else PC++")));
                        if (c == 0)
                        {
                            //instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), new IR.Constant(0.0), IR.BinOp.OperationType.OpNotEqual)));
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), RKIR(fun, b)));
                        }
                        else
                        {
                            //instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), new IR.Constant(0.0), IR.BinOp.OperationType.OpEqual)));
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.UnaryOp(RKIR(fun, b), IR.UnaryOp.OperationType.OpNot)));
                        }
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister(b))));
                        break;
                    case Lua502Ops.OpSetTable:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a})[{RK(fun, b)}] := R({c})")));
                        instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), RKIR(fun, b)), RKIR(fun, c)));
                        break;
                    case Lua502Ops.OpCall:
                        args = new List<IR.Expression>();
                        var rets = new List<IR.IdentifierReference>();
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        for (int r = (int)a; r <= (int)(a) + c - 2; r++)
                        {
                            rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)r)));
                        }
                        if (c == 0)
                        {
                            rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)a)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        var funcall = new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args);
                        funcall.IsIndeterminantArgumentCount = (b == 0);
                        funcall.IsIndeterminantReturnCount = (c == 0);
                        funcall.BeginArg = a + 1;
                        assn = new IR.Assignment(rets, funcall);
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case Lua502Ops.OpTailCall:
                        args = new List<IR.Expression>();
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        instructions.Add(new IR.Return(new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args)));
                        break;
                    case Lua502Ops.OpReturn:
                        args = new List<IR.Expression>();
                        if (b != 0)
                        {
                            for (int arg = (int)a; arg < a + b - 1; arg++)
                            {
                                args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                            }
                        }
                        var ret = new IR.Return(args);
                        if (b == 0)
                        {
                            ret.BeginRet = a;
                            ret.IsIndeterminantReturnCount = true;
                        }
                        instructions.Add(ret);
                        //instructions.Add(new IR.PlaceholderInstruction(($@"return {args}")));
                        break;
                    case Lua502Ops.OpForLoop:
                        instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                            new IR.IdentifierReference(SymbolTable.GetRegister(a + 2)), IR.BinOp.OperationType.OpAdd)));
                        instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 1 + sbx)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                            new IR.IdentifierReference(SymbolTable.GetRegister(a + 1)), IR.BinOp.OperationType.OpLoopCompare)));
                        break;
                    case Lua502Ops.OpSetList:
                    case Lua502Ops.OpSetListTo:
                        for (int j = 1; j <= (bx%32) + 1; j++)
                        {
                            instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), new IR.Constant((double)(bx - (bx % 32) + j))),
                                new IR.IdentifierReference(SymbolTable.GetRegister(a + (uint)j))));
                        }
                        break;
                    case Lua502Ops.OpClosure:
                        //args = "";
                        //for (int arg = (int)a; arg < a + fun.ChildFunctions[bx].NumParams; arg++)
                        //{
                        //    args += ", ";
                        //    args += $@"R({arg})";
                        //}
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := closure(KPROTO[{bx}]{args})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.Closure(irfun.LookupClosure(bx)));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    default:
                        switch (OpProperties502[(int) opcode].OpMode)
                        {
                            case OpMode.IABC:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{OpProperties502[(int) opcode].OpName} {a} {b} {c}")));
                                break;
                            case OpMode.IABx:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{OpProperties502[(int) opcode].OpName} {a} {bx}")));
                                break;
                            case OpMode.IAsBx:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{OpProperties502[(int) opcode].OpName} {a} {sbx}")));
                                break;
                        }
                        throw new Exception($@"Unimplemented opcode {OpProperties502[(int) opcode].OpName}");
                        break;
                }
                foreach (var inst in instructions)
                {
                    inst.OpLocation = i / 4;
                    irfun.AddInstruction(inst);
                }
            }
            irfun.ApplyLabels();

            // Simple post-ir and idiom recognition analysis passes
            /*irfun.EliminateRedundantAssignments();
            irfun.MergeConditionalJumps();
            irfun.MergeConditionalAssignments();
            //irfun.PeepholeOptimize();
            irfun.CheckControlFlowIntegrity();

            // Control flow graph construction and SSA conversion
            irfun.ConstructControlFlowGraph();
            irfun.ResolveIndeterminantArguments(SymbolTable);
            irfun.ConvertToSSA(SymbolTable.GetAllRegistersInScope());

            // Data flow passes
            irfun.PerformExpressionPropogation();

            // CFG passes
            irfun.StructureCompoundConditionals();
            irfun.DetectLoops();
            irfun.DetectLoopConditionals();
            irfun.DetectTwoWayConditionals();
            //irfun.StructureCompoundConditionals();
            irfun.EliminateDeadAssignments(true);

            // Convert out of SSA and rename variables
            irfun.DropSSANaive();
            irfun.RenameVariables();

            // Convert to AST
            irfun.ConvertToAST();*/

            // Simple post-ir and idiom recognition analysis passes
            irfun.ResolveVarargListAssignment();
            irfun.MergeMultiBoolAssignment();
            irfun.EliminateRedundantAssignments();
            irfun.MergeConditionalJumps();
            irfun.MergeConditionalAssignments();
            //irfun.PeepholeOptimize();
            irfun.CheckControlFlowIntegrity();

            // Control flow graph construction and SSA conversion
            irfun.ConstructControlFlowGraph();
            irfun.ResolveIndeterminantArguments(SymbolTable);
            irfun.ConvertToSSA(SymbolTable.GetAllRegistersInScope());

            // Data flow passes
            irfun.EliminateDeadAssignments(true);
            irfun.PerformExpressionPropogation();
            irfun.DetectListInitializers();

            // CFG passes
            irfun.StructureCompoundConditionals();
            irfun.DetectLoops();
            irfun.DetectLoopConditionals();
            irfun.DetectTwoWayConditionals();
            irfun.SimplifyIfElseFollowChain();
            irfun.EliminateDeadAssignments(true);
            irfun.PerformExpressionPropogation();
            irfun.VerifyLivenessNoInterference();

            // Convert out of SSA and rename variables
            irfun.DropSSADropSubscripts();
            irfun.AnnotateLocalDeclarations();
            irfun.ArgumentNames = fun.LocalsAt(0);
            irfun.RenameVariables();
            irfun.Parenthesize();

            // Convert to AST
            irfun.ConvertToAST(true);

            // Now generate IR for all the child closures
            for (int i = 0; i < fun.ChildFunctions.Count; i++)
            {
                //if (i == 49)
                {
                    GenerateIR50(irfun.LookupClosure((uint)i), fun.ChildFunctions[i]);
                }
            }
            SymbolTable.EndScope();
        }

        private static void CheckLocal(IR.Assignment a, Function fun, int index)
        {
            a.LocalAssignments = fun.LocalsAt(index + 1);
        }

        private static void CheckLocal(IR.Data d, Function fun, int index)
        {
            d.Locals = fun.LocalsAt(index + 1);
        }

        public static void GenerateIRHKS(IR.Function irfun, Function fun)
        {
            // First register closures for all the children
            for (int i = 0; i < fun.ChildFunctions.Count; i++)
            {
                irfun.AddClosure(new IR.Function());
            }

            SymbolTable.BeginScope();
            var parameters = new List<IR.Identifier>();
            for (uint i = 0; i < fun.ParameterCount; i++)
            {
                parameters.Add(SymbolTable.GetRegister(i));
            }
            irfun.SetParameters(parameters);

            for (int i = 0; i < fun.Instructions.Count * 4; i += 4)
            {
                var instruction = fun.Instructions[i / 4];
                //uint opcode = instruction & 0x3F;
                // Uhhh thanks again hork
                var opcode = instruction.OpCode;
                var a = instruction.A;
                var c = instruction.C;
                var b = instruction.B;
                var szero = instruction.ExtraCBit;

                var bx = instruction.Bx;
                var sbx = instruction.SBx;
                uint addr;
                var pc = i / 4;
                
                List<IR.Expression> args = null;
                List<IR.IdentifierReference> rets = null;
                List<IR.IInstruction> instructions = new List<IR.IInstruction>();
                IR.Assignment assn;
                switch (opcode)
                {
                    case LuaOpCode.HKS_OPCODE_MOVE:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), Register((uint)b));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {fun.Constants[b].ToString()}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), ToConstantIR(fun.Constants[(int) bx]));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADBOOL:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := (Bool){b}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.Constant(b == 1));
                        assn.NilAssignmentReg = a;
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        //instructions.Add(new IR.PlaceholderInstruction($@"if ({c}) PC++"));
                        if (c > 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2))));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_LOADNIL:
                        var nlist = new List<IR.IdentifierReference>();
                        for (int arg = (int)a; arg <= b; arg++)
                        {
                            nlist.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        assn = new IR.Assignment(nlist, new IR.Constant(IR.Constant.ConstantType.ConstNil));
                        assn.NilAssignmentReg = a;
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_GETUPVAL:
                        var up = SymbolTable.GetUpvalue((uint)b);
                        if (fun.Upvalues.Count() > 0 && !up.UpvalueResolved)
                        {
                            up.Name = fun.Upvalues[(int) b].Name;
                            up.UpvalueResolved = true;
                        }
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(up)));
                        break;
                    case LuaOpCode.HKS_OPCODE_GETGLOBAL_MEM:
                    case LuaOpCode.HKS_OPCODE_GETGLOBAL:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := Gbl[{fun.Constants[bx].ToString()}]")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetGlobal(fun.Constants[(int) bx].ToString())));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_GETTABLE_S:
                    case LuaOpCode.HKS_OPCODE_GETTABLE:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})[{RK(fun, c)}]")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister((uint)b), RKIRHKS(fun, c, szero)));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_SETGLOBAL:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"Gbl[{fun.Constants[bx].ToString()}] := R({a})")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetGlobal(fun.Constants[(int) bx].ToString()), new IR.IdentifierReference(SymbolTable.GetRegister(a))));
                        break;
                    case LuaOpCode.HKS_OPCODE_NEWTABLE:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {{}} size = {b}, {c}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.InitializerList(new List<IR.Expression>()));
                        assn.VarargAssignmentReg = a;
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_SELF:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a + 1}) := R({b})")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a + 1), Register((uint)b)));
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({b})[{RK(fun, c)}]")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister((uint)b), RKIRHKS(fun, c, szero))));
                        break;
                    case LuaOpCode.HKS_OPCODE_ADD:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} + {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpAdd));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_ADD_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} + {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpAdd));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_SUB:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} - {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpSub));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_SUB_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} - {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpSub));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_MUL:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpMul));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_MUL_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpMul));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_DIV:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} / {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpDiv));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_DIV_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpDiv));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_MOD:
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpMod));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_MOD_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpMod));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_POW:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} ^ {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpPow));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_POW_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := {RK(fun, b)} * {RK(fun, c)}")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpPow));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_UNM:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := -R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a),
                            new IR.UnaryOp(new IR.IdentifierReference(SymbolTable.GetRegister((uint)b)), IR.UnaryOp.OperationType.OpNegate));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_NOT:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := not R({b})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a),
                            new IR.UnaryOp(new IR.IdentifierReference(SymbolTable.GetRegister((uint)b)), IR.UnaryOp.OperationType.OpNot));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_LEN:
                        assn = new IR.Assignment(SymbolTable.GetRegister(a),
                            new IR.UnaryOp(new IR.IdentifierReference(SymbolTable.GetRegister((uint)b)), IR.UnaryOp.OperationType.OpLength));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_CONCAT:
                        args = new List<IR.Expression>();
                        for (int arg = (int)b; arg <= c; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.Concat(args));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_JMP:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"PC += {sbx}")));
                        addr = (uint)((i / 4) + 2 + ((sbx << 16) >> 16));
                        if ((sbx & 0x10000) != 0)
                        {
                            // Unsigned address?
                            addr = (uint)((sbx & 0xFFFF) + 2 + (uint)(i / 4));
                        }
                        instructions.Add(new IR.Jump(irfun.GetLabel((uint) (pc + 1 + sbx))));
                        break;
                    case LuaOpCode.HKS_OPCODE_EQ:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} == {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpNotEqual)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_LT:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} < {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpLessThan)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpGreaterEqual)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_LT_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} < {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpLessThan)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpGreaterEqual)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_LE:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} <= {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpLessEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)b), RKIRHKS(fun, c, szero), IR.BinOp.OperationType.OpGreaterThan)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_LE_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (({RK(fun, b)} <= {RK(fun, c)}) ~= {a}) PC++")));
                        if (a == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpLessEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(ToConstantIR(fun.Constants[(int) b]), Register((uint)c), IR.BinOp.OperationType.OpGreaterThan)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_TEST:
                    case LuaOpCode.HKS_OPCODE_TEST_R1:
                        // This op is weird
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (R({b}) <=> {c}) then R({a}) := R({b}) else PC++")));
                        if (c == 0)
                        {
                            //instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)a), new IR.Constant(IR.Constant.ConstantType.ConstNil), IR.BinOp.OperationType.OpNotEqual)));
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), Register((uint)a)));
                        }
                        else
                        {
                            //instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)a), new IR.Constant(IR.Constant.ConstantType.ConstNil), IR.BinOp.OperationType.OpEqual)));
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.UnaryOp(Register((uint)a), IR.UnaryOp.OperationType.OpNot)));
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_TESTSET:
                        // This op is weird
                        //instructions.Add(new IR.PlaceholderInstruction(($@"if (R({b}) <=> {c}) then R({a}) := R({b}) else PC++")));
                        if (c == 0)
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), new IR.Constant(0.0), IR.BinOp.OperationType.OpNotEqual)));
                        }
                        else
                        {
                            instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(RKIR(fun, b), new IR.Constant(0.0), IR.BinOp.OperationType.OpEqual)));
                        }
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetRegister(b))));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETTABLE:
                    case LuaOpCode.HKS_OPCODE_SETTABLE_S:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a})[{RK(fun, b)}] := R({c})")));
                        instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), Register(b)), RKIRHKS(fun, c, szero)));
                        break;
                    case LuaOpCode.HKS_OPCODE_TAILCALL:
                    case LuaOpCode.HKS_OPCODE_TAILCALL_I:
                        args = new List<IR.Expression>();
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        instructions.Add(new IR.Return(new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args)));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETTABLE_S_BK:
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a})[{RK(fun, b)}] := R({c})")));
                        instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), ToConstantIR(fun.Constants[(int) b])), RKIRHKS(fun, c, szero)));
                        break;
                    case LuaOpCode.HKS_OPCODE_CALL_I:
                    case LuaOpCode.HKS_OPCODE_CALL_I_R1:
                    case LuaOpCode.HKS_OPCODE_CALL:
                        args = new List<IR.Expression>();
                        rets = new List<IR.IdentifierReference>();
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        for (int r = (int)a + 1; r < a + c; r++)
                        {
                            rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)r - 1)));
                        }
                        if (c == 0)
                        {
                            rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)a)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        var funcall = new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args);
                        funcall.IsIndeterminantArgumentCount = (b == 0);
                        funcall.IsIndeterminantReturnCount = (c == 0);
                        funcall.BeginArg = a + 1;
                        assn = new IR.Assignment(rets, funcall);
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;/*
                    case LuaHKSOps.OpTailCall:
                        args = new List<IR.Expression>();
                        for (int arg = (int)a + 1; arg < a + b; arg++)
                        {
                            args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        instructions.Add(new IR.Return(new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args)));
                        break;*/
                    case LuaOpCode.HKS_OPCODE_RETURN:
                        args = new List<IR.Expression>();
                        if (b != 0)
                        {
                            for (int arg = (int)a; arg < a + b - 1; arg++)
                            {
                                args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                            }
                        }
                        var ret = new IR.Return(args);
                        if (b == 0)
                        {
                            ret.BeginRet = a;
                            ret.IsIndeterminantReturnCount = true;
                        }
                        instructions.Add(ret);
                        //instructions.Add(new IR.PlaceholderInstruction(($@"return {args}")));
                        break;
                    case LuaOpCode.HKS_OPCODE_FORLOOP:
                        addr = (uint)((i / 4) + 2 + ((sbx << 16) >> 16));
                        if ((sbx & 0x10000) != 0)
                        {
                            // Unsigned address?
                            addr = (uint)((sbx & 0xFFFF) + 2 + (uint)(i / 4));
                        }
                        instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                            new IR.IdentifierReference(SymbolTable.GetRegister(a + 2)), IR.BinOp.OperationType.OpAdd)));
                        var jmp = new IR.Jump(irfun.GetLabel((uint) (pc + 1 + sbx)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                            new IR.IdentifierReference(SymbolTable.GetRegister(a + 1)), IR.BinOp.OperationType.OpLoopCompare));
                        var pta = new IR.Assignment(SymbolTable.GetRegister(a + 3), Register((uint)a));
                        pta.PropogateAlways = true;
                        jmp.PostTakenAssignment = pta;
                        instructions.Add(jmp);
                        break;
                    case LuaOpCode.HKS_OPCODE_TFORLOOP:
                        args = new List<IR.Expression>();
                        rets = new List<IR.IdentifierReference>();
                        args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)a + 1)));
                        args.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)a + 2)));
                        if (c == 0)
                        {
                            rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)a + 3)));
                        }
                        else
                        {
                            for (int r = (int)a + 3; r <= a + c + 2; r++)
                            {
                                rets.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)r)));
                            }
                        }
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := R({a})({args})")));
                        var fcall = new IR.FunctionCall(new IR.IdentifierReference(SymbolTable.GetRegister(a)), args);
                        fcall.IsIndeterminantReturnCount = (c == 0);
                        assn = new IR.Assignment(rets, fcall);
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        instructions.Add(new IR.Jump(irfun.GetLabel((uint)((i / 4) + 2)), new IR.BinOp(Register((uint)a + 3), new IR.Constant(IR.Constant.ConstantType.ConstNil), IR.BinOp.OperationType.OpEqual)));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a + 2), new IR.IdentifierReference(SymbolTable.GetRegister(a + 3))));
                        break;
                    case LuaOpCode.HKS_OPCODE_FORPREP:
                        addr = (uint)((i / 4) + 2 + ((sbx << 16) >> 16));
                        if ((sbx & 0x10000) != 0)
                        {
                            // Unsigned address?
                            addr = (uint)((sbx & 0xFFFF) + 2 + (uint)(i / 4));
                        }
                        // The VM technically does a subtract, but we don't actually emit it since it simplifies things to map better to the high level Lua
                        //instructions.Add(new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a)), new IR.BinOp(new IR.IdentifierReference(SymbolTable.GetRegister(a)),
                        //    new IR.IdentifierReference(SymbolTable.GetRegister(a + 2)), IR.BinOp.OperationType.OpSub)));
                        instructions.Add(new IR.Jump(irfun.GetLabel((uint) (pc + 1 + sbx))));
                        break;
                    case LuaOpCode.HKS_OPCODE_SETLIST:
                        if (b == 0)
                        {
                            // Indeterminant assignment
                            if (c == 1)
                            {
                                assn = new IR.Assignment(SymbolTable.GetRegister(a), Register(a + 1));
                                assn.VarargAssignmentReg = a;
                                assn.IsIndeterminantVararg = true;
                                CheckLocal(assn, fun, pc);
                                instructions.Add(assn);
                            }
                        }
                        else
                        {
                            for (int j = 1; j <= b; j++)
                            {
                                assn = new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), new IR.Constant((double)(c - 1) * 32 + j)),
                                    new IR.IdentifierReference(SymbolTable.GetRegister(a + (uint)j)));
                                CheckLocal(assn, fun, pc);
                                instructions.Add(assn);
                            }
                        }
                        break;
                    case LuaOpCode.HKS_OPCODE_CLOSURE:
                        //args = "";
                        //for (int arg = (int)a; arg < a + fun.ChildFunctions[bx].NumParams; arg++)
                        //{
                        //    args += ", ";
                        //    args += $@"R({arg})";
                        //}
                        //instructions.Add(new IR.PlaceholderInstruction(($@"R({a}) := closure(KPROTO[{bx}]{args})")));
                        instructions.Add(new IR.Assignment(SymbolTable.GetRegister(a), new IR.Closure(irfun.LookupClosure(bx))));
                        break;
                    case LuaOpCode.HKS_OPCODE_GETFIELD:
                    case LuaOpCode.HKS_OPCODE_GETFIELD_R1:
                        assn = new IR.Assignment(Register((uint)a), new IR.IdentifierReference(SymbolTable.GetRegister((uint)b), new IR.Constant(fun.Constants[(int) c].ToString())));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_DATA:
                        var dat = new IR.Data();
                        CheckLocal(dat, fun, pc);
                        instructions.Add(dat);
                        break;
                    case LuaOpCode.HKS_OPCODE_SETFIELD:
                    case LuaOpCode.HKS_OPCODE_SETFIELD_R1:
                        assn = new IR.Assignment(new IR.IdentifierReference(SymbolTable.GetRegister(a), new IR.Constant(fun.Constants[(int) b].ToString())), RKIRHKS(fun, c, szero));
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        break;
                    case LuaOpCode.HKS_OPCODE_VARARG:
                        //instructions.Add(new IR.PlaceholderInstruction("VARARG"));
                        var vargs = new List<IR.IdentifierReference>();
                        for (int arg = (int)a; arg <= a + b - 1; arg++)
                        {
                            vargs.Add(new IR.IdentifierReference(SymbolTable.GetRegister((uint)arg)));
                        }
                        if (b != 0)
                        {
                            assn = new IR.Assignment(vargs, new IR.IdentifierReference(SymbolTable.GetVarargs()));
                        }
                        else
                        {
                            assn = new IR.Assignment(SymbolTable.GetRegister(a), new IR.IdentifierReference(SymbolTable.GetVarargs()));
                            assn.IsIndeterminantVararg = true;
                            assn.VarargAssignmentReg = a;
                        }
                        CheckLocal(assn, fun, pc);
                        instructions.Add(assn);
                        irfun.IsVarargs = true;
                        break;
                    default:
                        switch ((OpPropertiesHKS.Length > (int)opcode) ? OpPropertiesHKS[(int) opcode].OpMode : OpMode.IABC)
                        {
                            case OpMode.IABC:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{opcode} {a} {b} {c}")));
                                break;
                            case OpMode.IABx:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{opcode} {a} {bx}")));
                                break;
                            case OpMode.IAsBx:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{opcode} {a} {sbx}")));
                                break;
                            default:
                                instructions.Add(new IR.PlaceholderInstruction(($@"{opcode} {a} {b} {c}")));
                                break;
                        }
                        //throw new Exception($@"Unimplemented opcode {OpPropertiesHKS[(int) opcode].OpName}");
                        
                        break;
                }
                foreach (var inst in instructions)
                {
                    inst.OpLocation = i / 4;
                    irfun.AddInstruction(inst);
                }
            }
            irfun.ApplyLabels();

            // Simple post-ir and idiom recognition analysis passes
            irfun.ClearDataInstructions();
            irfun.ResolveVarargListAssignment();
            irfun.MergeMultiBoolAssignment();
            irfun.EliminateRedundantAssignments();
            irfun.MergeConditionalJumps();
            irfun.MergeConditionalAssignments();
            //irfun.PeepholeOptimize();
            irfun.CheckControlFlowIntegrity();

            // Control flow graph construction and SSA conversion
            irfun.ConstructControlFlowGraph();
            irfun.ResolveIndeterminantArguments(SymbolTable);
            irfun.CompleteLua51Loops();
            irfun.ConvertToSSA(SymbolTable.GetAllRegistersInScope());

            // Data flow passes
            irfun.EliminateDeadAssignments(true);
            irfun.PerformExpressionPropogation();
            irfun.DetectListInitializers();
            irfun.PerformExpressionPropogation();

            // CFG passes
            irfun.StructureCompoundConditionals();
            irfun.DetectLoops();
            irfun.DetectLoopConditionals();
            irfun.DetectTwoWayConditionals();
            irfun.SimplifyIfElseFollowChain();
            irfun.EliminateDeadAssignments(true);
            irfun.PerformExpressionPropogation();
            irfun.VerifyLivenessNoInterference();

            // Convert out of SSA and rename variables
            irfun.DropSSADropSubscripts();
            irfun.AnnotateLocalDeclarations();
            irfun.ArgumentNames = fun.LocalsAt(0);
            irfun.RenameVariables();
            irfun.Parenthesize();
            irfun.AnnotateEnvActFunctions();

            // Convert to AST
            irfun.ConvertToAST(true);

            // Now generate IR for all the child closures
            for (int i = 0; i < fun.ChildFunctions.Count; i++)
            {
                //if (i != 2 && i != 8 && i != 30)
                //if (i == 16)
                //if (i == 26)
                //if (i == 79)
                //if (i == 11)
                GenerateIRHKS(irfun.LookupClosure((uint)i), fun.ChildFunctions[i]);
            }
            SymbolTable.EndScope();
        }
    }
}
