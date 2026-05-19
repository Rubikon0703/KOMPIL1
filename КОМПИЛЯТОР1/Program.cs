internal class Program
{
    private static void Main()
    {
        Console.WriteLine("TEST 1");
        RunTest("C:\\Users\\sterh\\source\\repos\\КОМПИЛЯТОР1\\КОМПИЛЯТОР1\\test1.txt");
        Console.WriteLine();
        Console.WriteLine("TEST 2");
        RunTest("C:\\Users\\sterh\\source\\repos\\КОМПИЛЯТОР1\\КОМПИЛЯТОР1\\test2.txt");
        Console.WriteLine();
        Console.WriteLine("TEST 3");
        RunTest("C:\\Users\\sterh\\source\\repos\\КОМПИЛЯТОР1\\КОМПИЛЯТОР1\\test3.txt");
        Console.WriteLine();
        Console.WriteLine("TEST 4");
        RunTest("test4.txt");
    }

    private static void RunTest(string path)
    {
        InputOutput.Initialization(path);
        while (!InputOutput.IsEof)
        {
            if (InputOutput.Ch == '$' ||
                InputOutput.Ch == '#' ||
                InputOutput.Ch == '@' ||
                InputOutput.Ch == '%' ||
                InputOutput.Ch == '!' ||
                InputOutput.Ch == '?' ||
                InputOutput.Ch == '&' ||
                InputOutput.Ch == '^' ||
                InputOutput.Ch == '~' ||
                InputOutput.Ch == '\\')
            {
                InputOutput.Error(InputOutput.PositionNow, 1);
            }
            InputOutput.NextCh();
        }
    }
}