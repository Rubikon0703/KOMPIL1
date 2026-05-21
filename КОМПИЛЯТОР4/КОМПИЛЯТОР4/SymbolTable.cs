using System.Collections.Generic;

namespace PascalCompiler
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _symbols;
        private readonly SymbolTable _parent;

        public SymbolTable(SymbolTable parent = null)
        {
            _symbols = new Dictionary<string, Symbol>();
            _parent = parent;
        }

        public bool AddVariable(string name, string type)
        {
            string key = name.ToLower();

            if (_symbols.ContainsKey(key))
            {
                return false;
            }

            _symbols[key] = new VariableSymbol(name, type);
            return true;
        }

        public bool AddFunction(string name, string returnType,
            List<ParameterSymbol> parameters)
        {
            string key = name.ToLower();

            if (_symbols.ContainsKey(key))
            {
                return false;
            }

            _symbols[key] = new FunctionSymbol(name, returnType,
                parameters);
            return true;
        }

        public bool ContainsVariable(string name)
        {
            return GetSymbol(name) is VariableSymbol;
        }

        public bool ContainsFunction(string name)
        {
            return GetSymbol(name) is FunctionSymbol;
        }

        public Symbol GetLocalSymbol(string name)
        {
            Symbol symbol;

            if (name == null)
            {
                return null;
            }

            if (_symbols.TryGetValue(name.ToLower(), out symbol))
            {
                return symbol;
            }

            return null;
        }

        public Symbol GetSymbol(string name)
        {
            Symbol symbol;

            if (name == null)
            {
                return null;
            }

            if (_symbols.TryGetValue(name.ToLower(), out symbol))
            {
                return symbol;
            }

            if (_parent != null)
            {
                return _parent.GetSymbol(name);
            }

            return null;
        }
    }

    public abstract class Symbol
    {
        private string _name;
        private string _type;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        protected Symbol(string name, string type)
        {
            _name = name;
            _type = type;
        }
    }

    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, string type)
            : base(name, type)
        {
        }
    }

    public class ParameterSymbol : VariableSymbol
    {
        private int _line;

        public int Line
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
            }
        }

        public ParameterSymbol(string name, string type, int line)
            : base(name, type)
        {
            _line = line;
        }
    }

    public class FunctionSymbol : Symbol
    {
        private List<ParameterSymbol> _parameters;

        public List<ParameterSymbol> Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        public FunctionSymbol(string name, string returnType,
            List<ParameterSymbol> parameters)
            : base(name, returnType)
        {
            if (parameters == null)
            {
                _parameters = new List<ParameterSymbol>();
            }
            else
            {
                _parameters = parameters;
            }
        }
    }
}