using System;


namespace FileManager
{
    /// <summary>
    /// Основной класс программы.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        static void Main(string[] args)
        {
            const int maxSize = 1000;
            InformationMessages.Greeetings();
            do
            {
                string line = null;
                string[] commandArray;
                bool flag;
                do
                {
                    if (line != null)
                    {
                        InformationMessages.IncorrectCommand();
                    }

                    InformationMessages.PrintCurrenPath();
                    line = Console.ReadLine();
                    flag = false;
                    if (line.Length > maxSize)
                    {
                        commandArray = Array.Empty<string>();
                        flag = true;
                    }
                    else
                    {
                        commandArray = line.Split(' ');
                    }
                } while (!CommandLine.ChooseOperation(commandArray, flag));
            } while (!CommandLine.Finished);

            InformationMessages.Goodbye();
        }
    }
}