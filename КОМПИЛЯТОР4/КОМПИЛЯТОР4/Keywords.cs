using System.Collections.Generic;

namespace PascalCompiler
{
    public class Keywords
    {
        private static readonly Dictionary<string, byte> _kw =
            new Dictionary<string, byte>
        {
            ["*"] = LexicalAnalyzer.star,
            ["/"] = LexicalAnalyzer.slash,
            ["="] = LexicalAnalyzer.equal,
            [","] = LexicalAnalyzer.comma,
            [";"] = LexicalAnalyzer.semicolon,
            [":"] = LexicalAnalyzer.colon,
            ["."] = LexicalAnalyzer.point,
            ["("] = LexicalAnalyzer.leftpar,
            [")"] = LexicalAnalyzer.rightpar,
            ["+"] = LexicalAnalyzer.plus,
            ["-"] = LexicalAnalyzer.minus,
            [":="] = LexicalAnalyzer.assign,

            ["program"] = LexicalAnalyzer.programsy,
            ["var"] = LexicalAnalyzer.varsy,
            ["function"] = LexicalAnalyzer.functionsy,
            ["begin"] = LexicalAnalyzer.beginsy,
            ["end"] = LexicalAnalyzer.endsy,
            ["string"] = LexicalAnalyzer.stringsy,
            ["writeln"] = LexicalAnalyzer.writelnsy
        };

        public static Dictionary<string, byte> Kw
        {
            get
            {
                return _kw;
            }
        }

        public static bool IsKeyword(string value)
        {
            if (value == null)
            {
                return false;
            }

            return _kw.ContainsKey(value.ToLower());
        }
    }
}