using System;

namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte star = 21, slash = 60,
            equal = 16, comma = 20, semicolon = 14,
            colon = 5, point = 61, arrow = 62, 
            leftpar = 9, rightpar = 4,
            lbracket = 11, rbracket = 12, flpar = 63,
            frpar = 64,
            later = 65, greater = 66, laterequal = 67,
            greaterequal = 68,
            latergreater = 69, plus = 70, minus = 71,
            lcomment = 72,
            rcomment = 73, assign = 51, twopoints = 74,
            ident = 2, floatc = 82, intc = 15,
            casesy = 31, elsesy = 32, filesy = 57, gotosy = 33,
            thensy = 52, typesy = 34, untilsy = 53, dosy = 54,
            withsy = 37, ifsy = 56, insy = 100, ofsy = 101,
            orsy = 102, tosy = 103, endsy = 104, varsy = 105,
            divsy = 106, andsy = 107, notsy = 108, forsy = 109,
            modsy = 110, nilsy = 111,
            setsy = 112, beginsy = 113,
            whilesy = 114, arraysy = 115,
            constsy = 116, labelsy = 117,
            downtosy = 118, packedsy = 119,
            recordsy = 120, repeatsy = 121,
            programsy = 122, functionsy = 123,
            procedurensy = 124;

        byte symbol;
        TextPosition token;
        string addrName;
        int nmb_int;
        float nmb_float;
        char one_symbol;
        Keywords keywords = new Keywords();

        public byte NextSym()
        {
            while (InputOutput.Ch == ' ')
            {
                InputOutput.NextCh();
            }
            token.lineNumber = InputOutput.positionNow.lineNumber;
            token.charNumber = InputOutput.positionNow.charNumber;

            char ch = InputOutput.Ch;

            if (char.IsDigit(ch))
            {
                ScanNumber();
                return symbol;
            }

            if (char.IsLetter(ch))
            {
                ScanIdentifierOrKeyword();
                return symbol;
            }

            if (ch == '\'')
            {
                ScanCharConstant();
                return symbol;
            }

            switch (ch)
            {
                case '<':
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = laterequal;
                            InputOutput.NextCh();
                        }
                        else if (InputOutput.Ch == '>')
                        {
                            symbol = latergreater;
                            InputOutput.NextCh();
                        }
                        else symbol = later;
                        break;
                    }
                case '>':
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = greaterequal;
                            InputOutput.NextCh();
                        }
                        else symbol = greater;
                        break;
                    }
                case ':':
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = assign;
                            InputOutput.NextCh();
                        }
                        else symbol = colon;
                        break;
                    }
                case ';':
                    {
                        symbol = semicolon;
                        InputOutput.NextCh();
                        break;
                    }
                case '.':
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '.')
                        {
                            symbol = twopoints; InputOutput.NextCh();
                        }
                        else
                        {
                            symbol = point;
                        }
                        break;
                    }
                case '+':
                    {
                        symbol = plus;
                        InputOutput.NextCh();
                        break;
                    }
                case '-':
                    {
                        symbol = minus;
                        InputOutput.NextCh();
                        break;
                    }
                case '*':
                    {
                        symbol = star;
                        InputOutput.NextCh();
                        break;

                    }
                case '/':
                    {
                        symbol = slash;
                        InputOutput.NextCh();
                        break;
                    }
                case '=':
                    { 
                        symbol = equal;
                        InputOutput.NextCh();
                        break;
                    }
                case ',':
                    {
                        symbol = comma;
                        InputOutput.NextCh();
                        break;
                    }
                case '(':
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '*')
                        {
                            symbol = lcomment;
                            InputOutput.NextCh();
                        }
                        else
                        {
                            symbol = leftpar;
                        }
                        break;
                    }
                case ')':
                    {
                        symbol = rightpar;
                        InputOutput.NextCh();
                        break;
                    }
                case '[':
                    {
                        symbol = lbracket;
                        InputOutput.NextCh();
                        break;
                    }
                case ']':
                    {
                        symbol = rbracket;
                        InputOutput.NextCh();
                        break;
                    }
                case '{':
                    {
                        symbol = flpar;
                        InputOutput.NextCh();
                        break;
                    }
                case '}':
                    {
                        symbol = frpar;
                        InputOutput.NextCh(); 
                        break;
                    }
                case '^':
                    {
                        symbol = arrow;
                        InputOutput.NextCh();
                        break;
                    }
                default:
                    {
                        InputOutput.Error(1, InputOutput.positionNow);
                        InputOutput.NextCh();
                        symbol = 0;
                        break;
                    }
            }
            return symbol;
        }

        private void ScanNumber()
        {
            nmb_int = 0;
            bool isFloat = false;
            short maxint = short.MaxValue;

            while (char.IsDigit(InputOutput.Ch))
            {
                byte digit = (byte)(InputOutput.Ch - '0');
                if (nmb_int < maxint / 10 || (nmb_int == maxint
                    / 10 && digit <= maxint % 10))
                {
                    nmb_int = 10 * nmb_int + digit;
                }
                else
                {
                    InputOutput.Error(203, InputOutput.positionNow);
                    while (char.IsDigit(InputOutput.Ch))
                    {
                        InputOutput.NextCh();
                    }
                }
                InputOutput.NextCh();
            }

            if (InputOutput.Ch == '.')
            {
                InputOutput.NextCh();
                if (char.IsDigit(InputOutput.Ch))
                {
                    isFloat = true;
                    nmb_float = nmb_int;
                    double frac = 0, factor = 0.1;
                    while (char.IsDigit(InputOutput.Ch))
                    {
                        frac += (InputOutput.Ch - '0') * factor;
                        factor *= 0.1;
                        InputOutput.NextCh();
                    }
                    nmb_float += (float)frac;
                }
                else
                {
                    InputOutput.DecrementCharNumber();
                }
            }

            if (InputOutput.Ch == 'E' || InputOutput.Ch == 'e')
            {
                isFloat = true;
                InputOutput.NextCh();
                bool negExp = false;
                if (InputOutput.Ch == '-')
                {
                    negExp = true;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '+')
                {
                    InputOutput.NextCh();
                }
                int exp = 0;
                while (char.IsDigit(InputOutput.Ch))
                {
                    exp = exp * 10 + (InputOutput.Ch - '0');
                    InputOutput.NextCh();
                }
                if (negExp)
                {
                    exp = -exp;
                }
                nmb_float *= (float)Math.Pow(10, exp);
            }

            symbol = isFloat ? floatc : intc;
        }

        private void ScanIdentifierOrKeyword()
        {
            string name = "";
            while (char.IsLetterOrDigit(InputOutput.Ch))
            {
                name += InputOutput.Ch;
                InputOutput.NextCh();
            }
            byte kwCode = IsKeyword(name);
            if (kwCode != 0)
            {
                symbol = kwCode;
            }
            else 
            { 
                symbol = ident;
                addrName = name;
            }
        }

        private void ScanCharConstant()
        {
            InputOutput.NextCh();
            if (InputOutput.Ch == '\'')
            {
                InputOutput.Error(204, InputOutput.positionNow);
                one_symbol = ' ';
            }
            else
            {
                one_symbol = InputOutput.Ch;
                InputOutput.NextCh();
                if (InputOutput.Ch != '\'')
                {
                    InputOutput.Error(205, InputOutput.positionNow);
                }
                else
                    InputOutput.NextCh();
            }
        }

        private byte IsKeyword(string name)
        {
            foreach (var group in keywords.Kw.Values)
            {
                if (group.ContainsKey(name))
                {
                    return group[name];
                }
            }
            return 0;
        }
    }
}