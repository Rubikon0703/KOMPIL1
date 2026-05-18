using System;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFile = 
                "C:\\Users\\sterh\\source\\repos\\" +
                "Компилято\\Компилято\\test_program.pas";

            try
            {
                InputOutput.Init(testFile);

                bool line4Error = false;
                bool line5Error = false;

                while (true)
                {
                    uint line =
                        InputOutput.positionNow.lineNumber;

                  
                    if (line == 4 && !line4Error)
                    {
                        InputOutput.Error(84,
                            new TextPosition(4, 3));
                        line4Error = true;
                    }

                   
                    if (line == 5 && !line5Error)
                    {
                        InputOutput.Error(203,
                            new TextPosition(5, 8));
                        line5Error = true;
                    }

                    InputOutput.NextCh();
                }
            }
            catch
            {
            }
            finally
            {
                InputOutput.Close();
            }
        }
    }
}