namespace PascalCompiler
{
    public static class LexicalAnalyzer
    {
        // Коды операторов и разделителей
        public const byte star = 1;
        public const byte slash = 2;
        public const byte equal = 3;
        public const byte comma = 4;
        public const byte semicolon = 5;
        public const byte colon = 6;
        public const byte point = 7;
        public const byte leftpar = 9;
        public const byte rightpar = 10;
        public const byte plus = 18;
        public const byte minus = 19;
        public const byte assign = 21;

        // Коды ключевых слов
        public const byte endsy = 35;
        public const byte varsy = 36;
        public const byte functionsy = 41;
        public const byte beginsy = 49;
        public const byte programsy = 58;
        public const byte stringsy = 60;
        public const byte writelnsy = 61;

        // Другие коды
        public const byte identifier = 100;
        public const byte integer = 101;
        public const byte real = 102;
        public const byte stringconst = 103;
        public const byte error = 254;
        public const byte endoffile = 255;
    }
}