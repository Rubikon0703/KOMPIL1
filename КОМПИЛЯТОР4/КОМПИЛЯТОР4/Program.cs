using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PascalCompiler
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Pascal Compiler - Variant 1");
            Console.Write("Enter path to Pascal file: ");

            string filePath = Console.ReadLine();

            try
            {
                string sourceCode = File.ReadAllText(filePath);
                string[] lines = sourceCode.Split('\n');

                PrintSourceCode(lines);
                WriteCharCodeFile(lines);
                Analyze(sourceCode);

                Console.WriteLine
                    ("\nCodes saved to char_code.txt");
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
              
            }
        }

        private static void PrintSourceCode(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
              
                Console.Write($"{i + 1,4}: ");
               
                Console.WriteLine(lines[i]);
            }
        }

        private static void WriteCharCodeFile(string[] lines)
        {
            List<string> codeLines = new List<string>();

            foreach (string line in lines)
            {
                Lexer lineLexer = new Lexer(line);
                StringBuilder lineCodes = new StringBuilder();
                Token token;

                while (true)
                {
                    token = lineLexer.NextToken();

                    if (token.Code == LexicalAnalyzer.endoffile)
                    {
                        break;
                    }

                    if (lineCodes.Length > 0)
                    {
                        lineCodes.Append(" ");
                    }

                    lineCodes.Append(token.Code);
                }

                codeLines.Add(lineCodes.ToString());
            }

            File.WriteAllLines("char_code.txt", codeLines);
        }

        private static void Analyze(string sourceCode)
        {
            List<Token> errorTokens = new List<Token>();
            Lexer fullLexer = new Lexer(sourceCode);
            Token currentToken;

            do
            {
                currentToken = fullLexer.NextToken();

                if (currentToken.Code == LexicalAnalyzer.error)
                {
                    errorTokens.Add(currentToken);
                }
            }
            while (currentToken.Code != LexicalAnalyzer.endoffile);

            if (errorTokens.Count > 0)
            {
                PrintLexicalErrors(errorTokens);
            }
            else
            {
                RunParser(sourceCode);
            }
        }

        private static void PrintLexicalErrors
            (List<Token> errorTokens)
        {
           
            Console.WriteLine($"\nFound " +
                $"{errorTokens.Count} lexical errors:");
           

            foreach (Token error in errorTokens)
            {
                Console.WriteLine(
                    $"Line {error.Line}, position" +
                    $" {error.Position}: {error.Value}"
                );
            }
        }

        private static void RunParser(string sourceCode)
        {
            Console.WriteLine("\nStarting syntax analysis...");

            List<Token> tokens = new List<Token>();
            Lexer fullLexer = new Lexer(sourceCode);
            Token currentToken;

            do
            {
                currentToken = fullLexer.NextToken();
                tokens.Add(currentToken);
            }
            while (currentToken.Code != LexicalAnalyzer.endoffile);

            Parser parser = new Parser(tokens);
            parser.Parse();
        }
    }
}