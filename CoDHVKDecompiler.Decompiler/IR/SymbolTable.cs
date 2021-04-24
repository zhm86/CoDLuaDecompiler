using System;
using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using ValueType = CoDHVKDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDHVKDecompiler.Decompiler.IR
{
    public class SymbolTable
    {
        private readonly Stack<Dictionary<String, Identifier>> _scopeStack;
        // Use this stack for debugging
        private readonly List<Dictionary<String, Identifier>> _finalizedScopes;

        public SymbolTable()
        {
            _scopeStack = new Stack<Dictionary<string, Identifier>>();
            _finalizedScopes = new List<Dictionary<string, Identifier>>();
            // Add a stack for the globals
            _scopeStack.Push(new Dictionary<string, Identifier>());
        }
        
        public void BeginScope()
        {
            _scopeStack.Push(new Dictionary<string, Identifier>());
        }
        
        public void EndScope()
        {
            _finalizedScopes.Add(_scopeStack.Pop());
        }
        
        public Identifier GetRegister(uint reg)
        {
            if (!_scopeStack.Peek().ContainsKey($@"REG{reg}"))
            {
                Identifier id = new Identifier
                {
                    IdentifierType = IdentifierType.Register,
                    ValueType = ValueType.Unknown,
                    Name = $@"REG{reg}",
                    RegNum = reg
                };
                _scopeStack.Peek().Add(id.Name, id);
            }
            return _scopeStack.Peek()[$@"REG{reg}"];
        }
        
        public Identifier GetUpValue(uint upValue)
        {
            if (!_scopeStack.Peek().ContainsKey($@"UPVAL{upValue}"))
            {
                Identifier id = new Identifier
                {
                    IdentifierType = IdentifierType.Upvalue,
                    ValueType = ValueType.Unknown,
                    Name = $@"UPVAL{upValue}"
                };
                _scopeStack.Peek().Add(id.Name, id);
            }
            return _scopeStack.Peek()[$@"UPVAL{upValue}"];
        }

        public Identifier GetGlobal(string global, int constantId)
        {
            if (!_scopeStack.First().ContainsKey(global))
            {
                Identifier id = new Identifier
                {
                    IdentifierType = IdentifierType.Global,
                    ValueType = ValueType.Unknown,
                    Name = global,
                    ConstantId = constantId
                };
                _scopeStack.First().Add(id.Name, id);
            }
            return _scopeStack.First()[global];
        }

        public Identifier GetVarargs()
        {
            if (!_scopeStack.First().ContainsKey("$VARARGS"))
            {
                Identifier id = new Identifier
                {
                    IdentifierType = IdentifierType.Varargs, ValueType = ValueType.Unknown, Name = "$VARARGS"
                };
                _scopeStack.First().Add(id.Name, id);
            }
            return _scopeStack.First()["$VARARGS"];
        }
        
        public HashSet<Identifier> GetAllRegistersInScope()
        {
            var ret = new HashSet<Identifier>();
            foreach (var reg in _scopeStack.Peek())
            {
                if (reg.Value.IdentifierType == IdentifierType.Register)
                {
                    ret.Add(reg.Value);
                }
            }
            return ret;
        }
    }
}