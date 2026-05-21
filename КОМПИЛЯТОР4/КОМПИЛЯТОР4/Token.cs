namespace PascalCompiler
{
    public class Token
    {
        private byte _code;
        private string _value;
        private int _line;
        private int _position;

        public byte Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
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

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Token(byte code, string value,
            int line, int position)
        {
            _code = code;
            _value = value;
            _line = line;
            _position = position;
        }
    }
}