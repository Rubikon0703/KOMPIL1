class InputOutput
{
    const byte ERRMAX = 9;
    private static char _ch;
    private static TextPosition _positionNow;
    private static bool _isEof;
    private static string _line;
    private static byte _lastInLine;
    private static List<Err> _err;
    private static StreamReader _file;
    private static uint _errCount;

    public static char Ch
    {
        get
        {
            return _ch;
        }
        set
        {
            _ch = value;
        }
    }

    public static TextPosition PositionNow
    {
        get
        {
            return _positionNow;
        }
        set
        {
            _positionNow = value;
        }
    }

    public static List<Err> Err
    {
        get
        {
            return _err;
        }
        set
        {
            _err = value;
        }
    }
    public static bool IsEof
    {
        get
        {
            return _isEof;
        }
    }

    static public void Initialization(string inputPath)
    {
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Ошибка: файл '{inputPath}' не найден.");
            return;
        }

        _isEof = false;
        _errCount = 0;
        _positionNow = new TextPosition();
        _err = new List<Err>();
        _file = new StreamReader(inputPath);

        if (!_file.EndOfStream)
        {
            _line = _file.ReadLine() + " ";
            _lastInLine = (byte)(_line.Length - 1);
            _positionNow.LineNumber = 1;
            _positionNow.CharNumber = 0;
            _ch = _line[0];
        }
        else
        {
            _line = " ";
            _lastInLine = 0;
            _ch = (char)0;
            _isEof = true;
        }
    }

    static public void NextCh()
    {
        if (_isEof)
        {
            return;
        }
        if (_positionNow.CharNumber == _lastInLine)
        {
            ListThisLine();
            if (_err.Count > 0)
            {
                ListErrors();
            }
            ReadNextLine();
            if (_isEof)
            {
                return;
            }
            _positionNow.LineNumber++;
            _positionNow.CharNumber = 0;
        }
        else
        {
            ++_positionNow.CharNumber;
        }
        _ch = _line[_positionNow.CharNumber];
    }

    private static void ListThisLine()
    {
        string displayLine = "      " + _line;
        Console.WriteLine(displayLine);
    }

    private static void ReadNextLine()
    {
        if (!_file.EndOfStream)
        {
            _line = _file.ReadLine() + " ";
            _lastInLine = (byte)(_line.Length - 1);
            _err = new List<Err>();
        }
        else
        {
            End();
        }
    }

    static void End()
    {
        _ch = (char)0;
        _isEof = true;
        _file?.Close();
        Console.WriteLine($"Скомпилировано: ошибок — {_errCount}!");
    }

    static void ListErrors()
    {
        int pos = 6 - $"{_positionNow.LineNumber}".Length;
        string s;
        foreach (Err item in _err)
        {
            ++_errCount;
            s = "**";
            if (_errCount < 10)
                s += "0";
            s += $"{_errCount}**";
            while (s.Length - 1 < pos + item.ErrorPosition.CharNumber)
                s += " ";
            s += $"^ ошибка код {item.ErrorCode}";
            Console.WriteLine(s);
        }
    }

    static public void Error(TextPosition position, byte errorCode)
    {
        if (_err == null)
        {
            return;
        }
        if (_err.Count <= ERRMAX)
        {
            _err.Add(new Err(position, errorCode));
        }
    }
}