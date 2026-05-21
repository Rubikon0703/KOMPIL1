using System;
using System.Globalization;

namespace PascalCompiler
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private int _line;
        private int _column;

        private const long MinInteger = 0;
        private const long MaxInteger = 32767;

        public Lexer(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;
        }

        public Token NextToken()
        {
            while (_position < _input.Length &&
                char.IsWhiteSpace(_input[_position]))
            {
                if (_input[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }

                _position++;
            }

            if (_position >= _input.Length)
            {
                return new Token(LexicalAnalyzer.endoffile,
                    string.Empty, _line, _column);
            }

            char currentChar = _input[_position];

            if (char.IsDigit(currentChar))
            {
                return ReadNumber();
            }

            if (char.IsLetter(currentChar) ||
                currentChar == '_')
            {
                return ReadIdentifierOrKeyword();
            }

            if (currentChar == '\'')
            {
                return ReadString();
            }

            return ReadOperatorOrDelimiter();
        }

        private Token ReadNumber()
        {
            int start = _position;
            int startColumn = _column;

            while (_position < _input.Length &&
                char.IsDigit(_input[_position]))
            {
                _position++;
                _column++;
            }

            if (_position < _input.Length &&
                _input[_position] == '.')
            {
                _position++;
                _column++;

                while (_position < _input.Length &&
                    char.IsDigit(_input[_position]))
                {
                    _position++;
                    _column++;
                }

                string realValue = _input.Substring(start,
                    _position - start);
                return new Token(LexicalAnalyzer.real,
                    realValue, _line, startColumn);
            }

            string value = _input.Substring(start, _position - start);
            long number;

            if (!long.TryParse(value, NumberStyles.None,
                CultureInfo.InvariantCulture, out number) ||
                number < MinInteger ||
                number > MaxInteger)
            {
                return new Token(
                    LexicalAnalyzer.error,
                    $"Integer constant '{value}'" +
                    $" is out of range " +
                    $"[{MinInteger}; {MaxInteger}]",
                    _line,
                    startColumn
                );
            }

            return new Token(LexicalAnalyzer.integer,
                value, _line, startColumn);
        }

        private Token ReadIdentifierOrKeyword()
        {
            int start = _position;
            int startColumn = _column;

            while (_position < _input.Length &&
                   (char.IsLetterOrDigit(_input[_position])
                   || _input[_position] == '_'))
            {
                _position++;
                _column++;
            }

            string value = _input.Substring(start,
                _position - start);
            string normalized = value.ToLower();
            byte keywordCode;

            if (Keywords.Kw.TryGetValue(normalized,
                out keywordCode))
            {
                return new Token(keywordCode, value,
                    _line, startColumn);
            }

            return new Token(LexicalAnalyzer.identifier,
                value, _line, startColumn);
        }

        private Token ReadString()
        {
            int start = _position;
            int startColumn = _column;

            _position++;
            _column++;

            while (_position < _input.Length &&
                _input[_position] != '\'')
            {
                if (_input[_position] == '\n')
                {
                    return new Token(
                        LexicalAnalyzer.error,
                        "Unterminated string",
                        _line,
                        startColumn
                    );
                }

                _position++;
                _column++;
            }

            if (_position >= _input.Length)
            {
                return new Token(
                    LexicalAnalyzer.error,
                    "Unterminated string",
                    _line,
                    startColumn
                );
            }

            _position++;
            _column++;

            string value = _input.Substring
                (start, _position - start);
            return new Token
                (LexicalAnalyzer.stringconst,
                value, _line, startColumn);
        }

        private Token ReadOperatorOrDelimiter()
        {
            int startColumn = _column;

            if (_position + 1 < _input.Length)
            {
                string twoChars = _input.Substring(_position, 2);
                byte twoCharCode;

                if (Keywords.Kw.TryGetValue
                    (twoChars, out twoCharCode))
                {
                    _position += 2;
                    _column += 2;

                    return new Token(twoCharCode,
                        twoChars, _line, startColumn);
                }
            }

            string singleChar = _input[_position].ToString();
            byte singleCharCode;

            if (Keywords.Kw.TryGetValue
                (singleChar, out singleCharCode))
            {
                _position++;
                _column++;

                return new Token(singleCharCode, singleChar, _line, startColumn);
            }

            _position++;
            _column++;

            return new Token(
                LexicalAnalyzer.error,
                $"Unknown symbol '{singleChar}'",
                _line,
                startColumn
            );
        }
    }
}