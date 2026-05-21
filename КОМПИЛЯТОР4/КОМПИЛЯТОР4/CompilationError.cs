namespace PascalCompiler
{
    public class CompilationError
    {
        public int Line 
        { 
            get; 
        }
        public int Position 
        { 
            get;
        }
        public string Phase 
        { 
            get;
        }
        public string Message 
        { 
            get; 
        }
        public CompilationError(int line, int position,
            string phase, string message)
        {
            Line = line; 
            Position = position;
            Phase = phase;
            Message = message;
        }
        public override string ToString()
        {
            return $"{Phase,-14} |" +
                $" Стр {Line,3} | Поз {Position,3} | {Message}";
        }
    }
}