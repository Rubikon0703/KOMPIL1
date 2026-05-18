using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public struct TextPosition
    {
        public uint lineNumber;
        public byte charNumber;
        public TextPosition(uint ln = 0, byte c = 0) 
        { 
            lineNumber = ln;
            charNumber = c; 
        }
    }

    public struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;
        public Err(TextPosition pos, byte code)
        { 
            errorPosition = pos; 
            errorCode = code;
        }
    }

    public static class InputOutput
    {
        private const byte ERRMAX = 9;
        private static StreamReader file;
        private static string line;
        private static byte lastInLine;
        private static uint errCount = 0;
        private static TextPosition _positionNow;

        public static char Ch 
        { 
            get; 
            private set;
        }
        public static TextPosition positionNow
        {
            get => _positionNow;
            private set => _positionNow = value;
        }
        public static List<Err> err 
        { 
            get; 
            private set; 
        }

        public static void Init(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException
                    ($"Файл {fileName} не найден.");
            }
            file = new StreamReader(fileName);
            err = new List<Err>();
            positionNow = new TextPosition(1, 0);
            ReadNextLine();
            if (line != null && line.Length > 0)
                Ch = line[0];
        }

        public static void Close()
        {
            file?.Close();
            Console.WriteLine($"\nКомпиляция завершена:" +
                $" ошибок — {errCount}!");
        }

        private static void ReadNextLine()
        {
            if (!file.EndOfStream)
            {
                line = file.ReadLine();
                lastInLine = (byte)(line.Length - 1);
                err = new List<Err>();
            }
            else
            {
                line = null;
                lastInLine = 0;
                End();
            }
        }

        private static void ListThisLine()
        {
            if (line != null)
                Console.WriteLine(line);
        }

        private static void ListErrors()
        {
            int pos = 6 - $"{positionNow.lineNumber} ".Length;
            byte localCount = 0;
            foreach (Err e in err)
            {
                errCount++;
                localCount++;
                string s = "**";
                if (errCount < 10)
                {
                    s += "0";
                }
                s += $"{errCount}**";
                while (s.Length - 1 < pos +
                    e.errorPosition.charNumber)
                {
                    s += " ";
                }
                string errorMsg = ErrorTable.
                    GetMessage(e.errorCode);
                s += $"^ ошибка код {e.errorCode}: {errorMsg}";
                Console.WriteLine(s);
            }
        }

        private static void End()
        {
            if (line != null)
            {
                ListThisLine();
            }
            if (err.Count > 0)
            {
                ListErrors();
            }
            Close();
            Environment.Exit(0);
        }

        public static void NextCh()
        {
            if (positionNow.charNumber == lastInLine)
            {
                ListThisLine();
                if (err.Count > 0)
                {
                    ListErrors();
                }
                ReadNextLine();
                if (line == null)
                {
                    return;
                }
                positionNow = new TextPosition
                    (positionNow.lineNumber + 1, 0);
            }
            else
            {
                positionNow = new TextPosition
                    (positionNow.lineNumber,
                    (byte)(positionNow.charNumber + 1));
            }
            Ch = line[positionNow.charNumber];
        }

        public static void Error(byte errorCode,
            TextPosition position)
        {
            if (err.Count <= ERRMAX)
            {
                err.Add(new Err(position, errorCode));
            }
        }

        public static void SetCharNumber(byte newCharNumber)
        {
            positionNow = new TextPosition
                (positionNow.lineNumber,
                newCharNumber);
        }

        public static void IncrementCharNumber()
        {
            positionNow = new TextPosition
                (positionNow.lineNumber, 
                (byte)(positionNow.charNumber + 1));
        }

        public static void DecrementCharNumber()
        {
            positionNow = new TextPosition
                (positionNow.lineNumber,
                (byte)(positionNow.charNumber - 1));
        }
    }
}