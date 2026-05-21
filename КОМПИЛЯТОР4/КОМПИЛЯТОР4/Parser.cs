using System;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _currentIndex;
        private readonly List<string> _errors;

        private SymbolTable _symbolTable;
        private string _currentFunctionName;

        private Token CurrentToken
        {
            get
            {
                if (_currentIndex < _tokens.Count)
                {
                    return _tokens[_currentIndex];
                }

                return null;
            }
        }

        private class DeclaredIdentifier
        {
            private string _name;
            private int _line;

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

            public DeclaredIdentifier(string name, int line)
            {
                _name = name;
                _line = line;
            }
        }

        public Parser(List<Token> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            _tokens = tokens;
            _currentIndex = 0;
            _errors = new List<string>();
            _symbolTable = new SymbolTable();
            _currentFunctionName = null;
        }

        public void Parse()
        {
            try
            {
                Program();

                if (_errors.Count == 0)
                {
                   
                    Console.WriteLine
                        ("\nParsing completed successfully!");
                    
                }
                else
                {
                  
                    Console.WriteLine
                        ($"\nParsing completed with " +
                        $"{_errors.Count} errors:");
                   

                    foreach (string error in _errors)
                    {
                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine
                    ($"\nFatal error: {ex.Message}");
                
            }
        }

        private void Program()
        {
            Match(LexicalAnalyzer.programsy);
            Match(LexicalAnalyzer.identifier);
            Match(LexicalAnalyzer.semicolon);

            if (CurrentToken !=
                null && CurrentToken.Code ==
                LexicalAnalyzer.varsy)
            {
                VarDeclarations();
            }

            while (CurrentToken !=
                null && CurrentToken.Code ==
                LexicalAnalyzer.functionsy)
            {
                FunctionDeclaration();
            }

            Match(LexicalAnalyzer.beginsy);
            Statements();
            Match(LexicalAnalyzer.endsy);
            Match(LexicalAnalyzer.point);
        }

        private void FunctionDeclaration()
        {
            Match(LexicalAnalyzer.functionsy);

            string functionName = CurrentToken !=
                null ? CurrentToken.Value : null;
            int functionLine = CurrentToken !=
                null ? CurrentToken.Line : 0;

            Match(LexicalAnalyzer.identifier);

            List<ParameterSymbol> parameters =
                FormalParameters();

            Match(LexicalAnalyzer.colon);

            string returnType = CurrentToken !=
                null ? CurrentToken.Value : null;
            StandardType();

            Match(LexicalAnalyzer.semicolon);

            if (!_symbolTable.AddFunction
                (functionName, returnType, parameters))
            {
                AddError($"Duplicate identifier " +
                    $"'{functionName}' at line {functionLine}");
            }

            SymbolTable previousTable = _symbolTable;
            string previousFunctionName = _currentFunctionName;

            _symbolTable = new SymbolTable(previousTable);
            _currentFunctionName = functionName;

            _symbolTable.AddVariable(functionName, returnType);

            foreach (ParameterSymbol parameter in parameters)
            {
                if (!_symbolTable.AddVariable(parameter.Name, parameter.Type))
                {
                    AddError(
                        $"Duplicate parameter " +
                        $"'{parameter.Name}'" +
                        $" in function '{functionName}'" +
                        $" at line {parameter.Line}"
                    );
                }
            }

            if (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.varsy)
            {
                VarDeclarations();
            }

            Match(LexicalAnalyzer.beginsy);
            Statements();
            Match(LexicalAnalyzer.endsy);
            Match(LexicalAnalyzer.semicolon);

            _symbolTable = previousTable;
            _currentFunctionName = previousFunctionName;
        }

        private List<ParameterSymbol> FormalParameters()
        {
            List<ParameterSymbol> parameters =
                new List<ParameterSymbol>();

            if (CurrentToken == null ||
                CurrentToken.Code != LexicalAnalyzer.leftpar)
            {
                return parameters;
            }

            Match(LexicalAnalyzer.leftpar);

            if (CurrentToken != null &&
                CurrentToken.Code != LexicalAnalyzer.rightpar)
            {
                do
                {
                    List<DeclaredIdentifier> names = IdentifierList();

                    Match(LexicalAnalyzer.colon);

                    string type = CurrentToken !=
                        null ? CurrentToken.Value : null;
                    StandardType();

                    foreach (DeclaredIdentifier name in names)
                    {
                        parameters.Add(new ParameterSymbol
                            (name.Name, type, name.Line));
                    }

                    if (CurrentToken != null &&
                        CurrentToken.Code == LexicalAnalyzer.semicolon)
                    {
                        Match(LexicalAnalyzer.semicolon);
                    }
                    else
                    {
                        break;
                    }
                }
                while (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.identifier);
            }

            Match(LexicalAnalyzer.rightpar);

            return parameters;
        }

        private void VarDeclarations()
        {
            Match(LexicalAnalyzer.varsy);

            while (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.identifier)
            {
                List<DeclaredIdentifier> variables =
                    IdentifierList();

                Match(LexicalAnalyzer.colon);

                string type = CurrentToken != 
                    null ? CurrentToken.Value : null;
                StandardType();

                Match(LexicalAnalyzer.semicolon);

                foreach (DeclaredIdentifier variable in variables)
                {
                    if (!_symbolTable.AddVariable(variable.Name, type))
                    {
                        AddError($"Duplicate identifier " +
                            $"'{variable.Name}'" +
                            $" at line {variable.Line}");
                    }
                }
            }
        }

        private List<DeclaredIdentifier> IdentifierList()
        {
            List<DeclaredIdentifier> identifiers =
                new List<DeclaredIdentifier>();

            string firstName = CurrentToken !=
                null ? CurrentToken.Value : null;
            int firstLine = CurrentToken !=
                null ? CurrentToken.Line : 0;

            identifiers.Add
                (new DeclaredIdentifier(firstName, firstLine));
            Match(LexicalAnalyzer.identifier);

            while (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.comma)
            {
                Match(LexicalAnalyzer.comma);

                string name = CurrentToken !=
                    null ? CurrentToken.Value : null;
                int line = CurrentToken !=
                    null ? CurrentToken.Line : 0;

                identifiers.Add
                    (new DeclaredIdentifier(name, line));
                Match(LexicalAnalyzer.identifier);
            }

            return identifiers;
        }

        private void StandardType()
        {
            if (CurrentToken == null)
            {
                AddError("Unexpected end of file," +
                    " expected standard type");
                return;
            }

            string value = CurrentToken.Value.ToLower();

            if (value == "integer" ||
                value == "real" ||
                CurrentToken.Code == LexicalAnalyzer.stringsy)
            {
                _currentIndex++;
            }
            else
            {
                AddError(
                    $"Expected standard type, but found " +
                    $"'{CurrentToken.Value}'" +
                    $" at line {CurrentToken.Line}"
                );

                _currentIndex++;
            }
        }

        private void Statements()
        {
            if (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.endsy)
            {
                return;
            }

            Statement();

            while (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.semicolon)
            {
                Match(LexicalAnalyzer.semicolon);

                if (CurrentToken != null &&
                    CurrentToken.Code == LexicalAnalyzer.endsy)
                {
                    break;
                }

                Statement();
            }
        }

        private void Statement()
        {
            if (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.identifier)
            {
                AssignmentStatement();
            }
            else if (CurrentToken != null && 
                CurrentToken.Code == LexicalAnalyzer.writelnsy)
            {
                WritelnStatement();
            }
            else if (CurrentToken != null &&
                     CurrentToken.Code != LexicalAnalyzer.endsy &&
                     CurrentToken.Code != LexicalAnalyzer.endoffile)
            {
                AddError($"Unexpected token '" +
                    $"{CurrentToken.Value}'" +
                    $" at line {CurrentToken.Line}");

                SynchronizeTo
                    (LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy);
                
            }
        }

        private void AssignmentStatement()
        {
            string varName = CurrentToken !=
                null ? CurrentToken.Value : null;
            int line = CurrentToken !=
                null ? CurrentToken.Line : 0;

            Match(LexicalAnalyzer.identifier);

            if (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.point)
            {
                AddError(
                    $"Record fields are not allowed in this variant. Use only simple variables at line {CurrentToken.Line}"
                );

                SynchronizeTo(
                    LexicalAnalyzer.assign,
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy
                );
            }

            bool isFunctionResult =
                _currentFunctionName != null &&
                string.Equals(
                    varName,
                    _currentFunctionName,
                    StringComparison.OrdinalIgnoreCase
                );

            if (!_symbolTable.ContainsVariable(varName)
                && !isFunctionResult)
            {
                AddError($"Undeclared variable '" +
                    $"{varName}' at line {line}");
            }

            Match(LexicalAnalyzer.assign);
            Expression();
        }

        private void WritelnStatement()
        {
            Match(LexicalAnalyzer.writelnsy);
            Match(LexicalAnalyzer.leftpar);

            if (CurrentToken != null &&
                CurrentToken.Code != LexicalAnalyzer.rightpar)
            {
                WritelnParameter();

                while (CurrentToken != null &&
                    CurrentToken.Code == LexicalAnalyzer.comma)
                {
                    Match(LexicalAnalyzer.comma);
                    WritelnParameter();
                }
            }

            Match(LexicalAnalyzer.rightpar);
        }

        private void WritelnParameter()
        {
            if (CurrentToken != null &&
               CurrentToken.Code == LexicalAnalyzer.stringconst)
            {
                Match(LexicalAnalyzer.stringconst);
            }
            else
            {
                Expression();
            }
        }

        private void Expression()
        {
            SimpleExpression();

            if (CurrentToken != null &&
                CurrentToken.Code == LexicalAnalyzer.equal)
            {
                Match(LexicalAnalyzer.equal);
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            if (CurrentToken != null &&
                (CurrentToken.Code == LexicalAnalyzer.plus ||
                 CurrentToken.Code == LexicalAnalyzer.minus))
            {
                _currentIndex++;
            }

            Term();

            while (CurrentToken != null &&
                   (CurrentToken.Code == LexicalAnalyzer.plus ||
                    CurrentToken.Code == LexicalAnalyzer.minus))
            {
                _currentIndex++;
                Term();
            }
        }

        private void Term()
        {
            Factor();

            while (CurrentToken != null &&
                   (CurrentToken.Code == LexicalAnalyzer.star ||
                    CurrentToken.Code == LexicalAnalyzer.slash))
            {
                _currentIndex++;
                Factor();
            }
        }

        private void Factor()
        {
            if (CurrentToken == null)
            {
               AddError("Unexpected end of file in expression");
                return;
            }

            if (CurrentToken.Code == LexicalAnalyzer.identifier)
            {
                string name = CurrentToken.Value;
                int line = CurrentToken.Line;

                Match(LexicalAnalyzer.identifier);

                if (CurrentToken != null && 
                    CurrentToken.Code == LexicalAnalyzer.point)
                {
                    AddError(
                        $"Record fields are " +
                        $"not allowed in expressions." +
                        $" Use only simple " +
                        $"variables at line {CurrentToken.Line}"
                    );

                    SynchronizeTo(
                        LexicalAnalyzer.semicolon,
                        LexicalAnalyzer.rightpar,
                        LexicalAnalyzer.endsy
                    );

                    return;
                }

                if (CurrentToken != null &&
                   CurrentToken.Code == LexicalAnalyzer.leftpar)
                {
                    FunctionCallArguments(name, line);
                }
                else if (!_symbolTable.ContainsVariable(name)
                    && !IsVisibleFunction(name))
                {
                    AddError($"Undeclared identifier '{name}" +
                        $"' at line {line}");
                }
            }
            else if (CurrentToken.Code == LexicalAnalyzer.integer ||
                     CurrentToken.Code == LexicalAnalyzer.real ||
                     CurrentToken.Code == LexicalAnalyzer.stringconst)
            {
                _currentIndex++;
            }
            else if 
                (CurrentToken.Code == LexicalAnalyzer.leftpar)
            {
                Match(LexicalAnalyzer.leftpar);
                Expression();
                Match(LexicalAnalyzer.rightpar);
            }
            else if (CurrentToken.Code == LexicalAnalyzer.semicolon ||
                     CurrentToken.Code == LexicalAnalyzer.rightpar ||
                     CurrentToken.Code == LexicalAnalyzer.endsy)
            {
                AddError
                   ($"Unexpected factor '{CurrentToken.Value}'"+
                    $" at line {CurrentToken.Line}");
            }
            else
            {
                AddError
                    ($"Unexpected factor '{CurrentToken.Value}"+
                    $"' at line {CurrentToken.Line}");
                _currentIndex++;
            }
        }

        private void FunctionCallArguments(string functionName, int line)
        {
            Symbol symbol = _symbolTable.GetSymbol(functionName);
            FunctionSymbol function = symbol as FunctionSymbol;

            if (function == null)
            {
                AddError
                    ($"Undeclared function '" +
                    $"{functionName}' at line {line}");
            }

            Match(LexicalAnalyzer.leftpar);

            int argumentCount = 0;

            if (CurrentToken != null &&
                CurrentToken.Code != LexicalAnalyzer.rightpar)
            {
                Expression();
                argumentCount++;

                while (CurrentToken != null &&
                    CurrentToken.Code == LexicalAnalyzer.comma)
                {
                    Match(LexicalAnalyzer.comma);
                    Expression();
                    argumentCount++;
                }
            }

            Match(LexicalAnalyzer.rightpar);

            if (function != null && argumentCount !=
                function.Parameters.Count)
            {
                AddError
                    ($"Function '{functionName}'" +
                    $" expects {function.Parameters.Count}" +
                    $" argument(s), but got {argumentCount}" +
                    $" at line {line}");
            }
        }

        private bool IsVisibleFunction(string name)
        {
            Symbol symbol = _symbolTable.GetSymbol(name);

            if (symbol is FunctionSymbol)
            {
                return true;
            }

            return false;
        }

        private void Match(byte expectedCode)
        {
            if (CurrentToken == null)
            {
                AddError($"Unexpected end of file, expected" +
                    $" '{GetTokenName(expectedCode)}'");
                return;
            }

            if (CurrentToken.Code == expectedCode)
            {
                _currentIndex++;
            }
            else
            {
                AddError
                    ($"Expected '{GetTokenName(expectedCode)}' " +
                    $"but found '{CurrentToken.Value}' " +
                    $"at line {CurrentToken.Line}");
                _currentIndex++;
            }
        }

        private void SynchronizeTo(params byte[] stopCodes)
        {
            bool stopFound;

            while (CurrentToken != null &&
                   CurrentToken.Code != LexicalAnalyzer.endoffile)
            {
                stopFound = false;

                foreach (byte code in stopCodes)
                {
                    if (CurrentToken.Code == code)
                    {
                        stopFound = true;
                    }
                }

                if (stopFound)
                {
                    return;
                }

                _currentIndex++;
            }
        }

        private string GetTokenName(byte code)
        {
            switch (code)
            {
                case LexicalAnalyzer.programsy:
                    {
                        return "program";
                    }

                case LexicalAnalyzer.varsy:
                    {
                        return "var";
                    }

                case LexicalAnalyzer.functionsy:
                    {
                        return "function";
                    }

                case LexicalAnalyzer.beginsy:
                    {
                        return "begin";
                    }

                case LexicalAnalyzer.endsy:
                    {
                        return "end";
                    }

                case LexicalAnalyzer.semicolon:
                    {
                        return ";";
                    }

                case LexicalAnalyzer.colon:
                    {
                        return ":";
                    }

                case LexicalAnalyzer.assign:
                    {
                        return ":=";
                    }

                case LexicalAnalyzer.leftpar:
                    {
                        return "(";
                    }

                case LexicalAnalyzer.rightpar:
                    {
                        return ")";
                    }

                case LexicalAnalyzer.writelnsy:
                    {
                        return "writeln";
                    }

                case LexicalAnalyzer.point:
                    {
                        return ".";
                    }

                case LexicalAnalyzer.comma:
                    {
                        return ",";
                    }

                case LexicalAnalyzer.identifier:
                    {
                        return "identifier";
                    }

                default:
                    {
                        return $"token with code {code}";
                    }
            }
        }

        private void AddError(string message)
        {
            _errors.Add(message);
        }
    }
}