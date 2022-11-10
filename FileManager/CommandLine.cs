using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileManager
{
    /// <summary>
    /// Класс, который отвечает за исполнение всех команд, доступных пользователю
    /// </summary>
    public static class CommandLine
    {
        private const long MaxLength = int.MaxValue;

        /// <summary>
        /// Переменная отвечает за завершение цикла в основном коде программы
        /// </summary>
        internal static bool Finished { get; private set; }


        static CommandLine()
        {
            Finished = false;
        }

        /// <summary>
        /// Копирует файл из одного место в другое.
        /// </summary>
        /// <param name="pathFrom">Путь до файла, который необходимо скопировать.</param>
        /// <param name="pathTo">Путь до целевого файла.</param>
        private static void Copy(string pathFrom, string pathTo)
        {
            if (File.Exists(pathFrom))
            {
                try
                {
                    File.Copy(pathFrom, pathTo, true);
                }
                catch (Exception)
                {
                    InformationMessages.CantGetAccessToTheFile();
                }
            }
            else
            {
                InformationMessages.CopiedOrMovedFileNotExist();
            }
        }

        /// <summary>
        /// Удаляет необходимый файл.
        /// </summary>
        /// <param name="path">Путь до файла, который необходимо удалить.</param>
        private static void Remove(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    InformationMessages.CantGetAccessToTheFile();
                }
            }
            else
            {
                InformationMessages.FileNotExist();
            }
        }

        /// <summary>
        /// Показывает пользователю все доступные ему команды.
        /// </summary>
        private static void Help()
        {
            InformationMessages.AvailableCommands();
        }


        /// <summary>
        /// Возвращает кодировку по строке.
        /// </summary>
        /// <param name="encoding">Кодировка в типе string.</param>
        /// <returns>Возвращается Encoding.</returns>
        /// <exception cref="NotSupportedException">Выбрасывает исключение, если строка не подходит
        /// ни под одну из поддерживаемых кодировок.</exception>
        private static Encoding GetEncodingFromString(string encoding)
        {
            switch (encoding)
            {
                case "utf-8": return Encoding.UTF8;
                case "utf-32": return Encoding.UTF32;
                case "ascii": return Encoding.ASCII;
                case "utf-16": return Encoding.GetEncoding(1200);
                case "latin-1": return Encoding.Latin1;
                default: throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Показывает список доступных дисков, а также некоторую информацию о них.
        /// </summary>
        private static void ShowAvailableDisks()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in allDrives)
            {
                Console.WriteLine($"Name: {driveInfo.Name} Type: {driveInfo.DriveType}");
            }
        }


        /// <summary>
        /// Показывает список доступных файлов в текущей директории.
        /// </summary>
        private static void ShowAvailableFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name);
            }

            foreach (DirectoryInfo directory in directories)
            {
                Console.WriteLine(directory.Name);
            }
        }


        /// <summary>
        /// Делает проверку на то, не является ли текущий файл слишком большим. Это нужно для того,
        /// чтобы программа не получала TL или зависание по оперативной памяти.
        /// </summary>
        /// <param name="file">Содержит информацию о файле.</param>
        /// <returns>Если файл слишком большой, возвращает true, иначе false.</returns>
        private static bool IsFileBig(FileInfo file)
        {
            return file.Length >= MaxLength;
        }

        /// <summary>
        /// Делает проверку на то, доступен ли данный файл на чтение в текущий момент.
        /// </summary>
        /// <param name="file">Содержит информацию о файле.</param>
        /// <returns>Если файл заблокирован, вернёт true, иначе false.</returns>
        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Читает текст из файла и выводит его на экран.
        /// </summary>
        /// <param name="path">Путь до файла.</param>
        /// <param name="encoding">Тип кодировки, в которой будет прочтён файл.</param>
        private static void ReadText(string path, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (File.Exists(path) && !IsFileLocked(new FileInfo(path)) && !IsFileBig(new FileInfo(path)))
            {
                string text = File.ReadAllText(path, encoding);
                Console.WriteLine(text);
            }
            else if (File.Exists(path))
            {
                InformationMessages.CantGetAccessToTheFile();
            }
            else
            {
                InformationMessages.FileNotExist();
            }
        }


        /// <summary>
        /// Создаёт файл и по желанию пользователя записывает в него текст.
        /// </summary>
        /// <param name="path">Путь до файла.</param>
        /// <param name="text">Текст, который ввёл пользователь.</param>
        /// <param name="encoding">Кодировка, в которой пользователь хочет записать файл.</param>
        private static void CreateAndWriteText(string path, string[] text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (File.Exists(path) && IsFileLocked(new FileInfo(path)))
            {
                InformationMessages.CantGetAccessToTheFile();
            }

            File.WriteAllLines(path, text, encoding);
        }

        /// <summary>
        /// Показывает список доступных для пользователя кодировок.
        /// </summary>
        private static void ShowEncodings()
        {
            InformationMessages.ShowEncodings();
        }


        /// <summary>
        /// Объединяет несколько текстовых файлов в один, результат записывает в новый файл, а
        /// также печатает его на экран.
        /// </summary>
        /// <param name="from">Массив файлов, которые объединяют в один.</param>
        /// <param name="to">Целевой файл.</param>
        private static void Concatenate(string[] from, string to)
        {
            List<String> texts = new List<String>();
            if (IsFileLocked(new FileInfo(to)) || IsFileBig(new FileInfo(to)))
            {
                InformationMessages.CantGetAccessToTheFile();
                return;
            }

            foreach (string file in from)
            {
                if (IsFileLocked(new FileInfo(file)) || IsFileBig(new FileInfo(file)))
                {
                    InformationMessages.CantGetAccessToTheFile();
                    return;
                }

                texts.Add(File.ReadAllText(file, Encoding.UTF8));
            }

            File.WriteAllLines(to, texts.ToArray(), Encoding.UTF8);
            Console.WriteLine(File.ReadAllText(to, Encoding.UTF8));
        }


        /// <summary>
        /// Перемещает файл из одного места в другое.
        /// </summary>
        /// <param name="pathFrom">Перемещаемый файл.</param>
        /// <param name="pathTo">Целевой файл.</param>
        private static void Move(string pathFrom, string pathTo)
        {
            if (File.Exists(pathFrom))
            {
                try
                {
                    File.Move(pathFrom, pathTo, true);
                }
                catch (Exception)
                {
                    InformationMessages.CantGetAccessToTheFile();
                }
            }
            else
            {
                InformationMessages.CopiedOrMovedFileNotExist();
            }
        }


        /// <summary>
        /// Записывает в список все файлы в текущей директории и в дочерних, которые удовлетворяют заданной маске.
        /// Работает рекурсивно.
        /// </summary>
        /// <param name="path">путь до директории, которая в текущий момент рассматривается.</param>
        /// <param name="pattern">Маска, под которую подбираются файлы.</param>
        /// <param name="list">Список, в который сохраняются все файлы.</param>
        private static void FindByMaskWithDirectories(string path, string pattern, List<FileInfo> list)
        {
            List<FileInfo> needToAdd = FindByMask(path, pattern);
            foreach (FileInfo element in needToAdd)
            {
                list.Add(element);
            }

            DirectoryInfo current = new DirectoryInfo(path);
            DirectoryInfo[] directories = current.GetDirectories();
            foreach (DirectoryInfo directory in directories)
            {
                FindByMaskWithDirectories(directory.FullName, pattern, list);
            }
        }

        /// <summary>
        /// Ищет все файлы в данной директории, удовлетворяющие маске.
        /// </summary>
        /// <param name="path">Путь до заданной директории.</param>
        /// <param name="pattern">Маска, под которую подбираются файлы.</param>
        /// <returns>возвращает список из файлов, которые удовлетворяют текущей маске.</returns>
        private static List<FileInfo> FindByMask(string path, string pattern)
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(path);
            FileInfo[] files = currentDirectory.GetFiles();
            List<FileInfo> list = new List<FileInfo>();
            Regex regex;
            try
            {
                regex = new Regex(pattern);
            }
            catch (Exception)
            {
                InformationMessages.IncorrectRegularExpression();
                return list;
            }

            foreach (FileInfo file in files)
            {
                if (regex.IsMatch(file.Name))
                {
                    list.Add(file);
                }
            }

            return list;
        }

        /// <summary>
        /// Меняет директорию на заданную.
        /// </summary>
        /// <param name="path">целевая директория.</param>
        private static void ChangeDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                InformationMessages.DirectoryNotExist();
            }
        }

        /// <summary>
        /// Копирует все файлы из конкретной директории и всех её поддиректорий в другую директорию
        /// по заданной маске.
        /// </summary>
        /// <param name="array">Массив параметров, в котором содержатся все данные от пользователя.</param>
        private static void CopyFilesToDirectory(string[] array)
        {
            try
            {
                if (!Directory.Exists(array[1]))
                {
                    InformationMessages.ErrorIncorrectDirectory();
                    return;
                }

                if (!Directory.Exists(array[2]))
                {
                    Directory.CreateDirectory(array[2]);
                }

                List<FileInfo> list = new List<FileInfo>();
                string mask = GetMaskFromArray(array, 4);
                int type = GetTypeOfCopyFromString(array[3]);
                FindByMaskWithDirectories(array[1], mask, list);
                if (type == -1)
                {
                    InformationMessages.ErrorIncorrectTypeOfCopy();
                    return;
                }

                if (type == 1)
                {
                    foreach (FileInfo fileInfo in list)
                    {
                        File.Copy(fileInfo.FullName, Path.Combine(array[2], fileInfo.Name),
                            true);
                    }
                }
                else
                {
                    foreach (FileInfo fileInfo in list)
                    {
                        if (!File.Exists(Path.Combine(array[2], fileInfo.Name)))
                            File.Copy(fileInfo.FullName, Path.Combine(array[2], fileInfo.Name));
                    }
                }
            }
            catch (Exception)
            {
                InformationMessages.CantGetAccessToTheFile();
            }
        }


        /// <summary>
        /// Пытается создать новый файл и записать в него что-то (если пользователь захотел).
        /// </summary>
        /// <param name="array">Массив параметров, в котором содержатся все данные от пользователя.</param>
        private static void TryCreateFile(string[] array)
        {
            int n = array.Length;
            Encoding encoding = null;
            try
            {
                string[] textArray = { };
                if (n >= 3)
                    encoding = GetEncodingFromString(array[2]);
                if (n >= 4)
                {
                    var text = String.Join(' ', array[3..]);
                    textArray = text.Split(@"\n");
                }

                CreateAndWriteText(array[1], textArray, encoding);
            }
            catch
            {
                InformationMessages.IncorrectEncoding();
            }
        }

        /// <summary>
        /// Пытается найти все файлы по заданной маске в текущей директории.
        /// </summary>
        /// <param name="array">Массив параметров, в котором содержатся все данные от пользователя.</param>
        private static void TryFindMask(string[] array)
        {
            string mask = GetMaskFromArray(array);
            List<FileInfo> list = FindByMask(Directory.GetCurrentDirectory(), mask);
            PrintPaths(list);
        }

        /// <summary>
        /// Пытается найти все файлы по заданной маске в текущей директории и её поддиректориях.
        /// </summary>
        /// <param name="array">Массив параметров, в котором содержатся все данные от пользователя.</param>
        private static void TryFindMaskWithSubDirectories(string[] array)
        {
            string mask = GetMaskFromArray(array);
            List<FileInfo> list = new List<FileInfo>();
            try
            {
                Regex regex = new Regex(mask);
            }
            catch
            {
                InformationMessages.IncorrectRegularExpression();
                return;
            }

            FindByMaskWithDirectories(Directory.GetCurrentDirectory(), mask, list);
            PrintPaths(list, true);
        }


        /// <summary>
        /// Пытается прочитать текст из файла.
        /// </summary>
        /// <param name="array">Массив параметров, в котором содержатся данные от пользователя.</param>
        private static void TryRead(string[] array)
        {
            int n = array.Length;
            Encoding encoding = null;
            try
            {
                if (n == 3)
                    encoding = GetEncodingFromString(array[2]);
                ReadText(array[1], encoding);
            }
            catch
            {
                InformationMessages.IncorrectEncoding();
            }
        }


        /// <summary>
        /// Ищет наибольшую общую подпоследовательность за у двух массивов строк.
        /// </summary>
        /// <param name="a">Первый массив строк.</param>
        /// <param name="b">Второй массив строк.</param>
        /// <returns>Возвращает список, элементы, которые есть в обоих массивах.</returns>
        private static string[] FindTheGreatestCommonSequence(string[] a, string[] b)
        {
            int n = a.Length;
            int m = b.Length;
            int[,] dp = new int[n + 1, m + 1];
            int[,] previous = new int[n + 1, m + 1];
            for (int i = 0; i <= n; ++i)
            {
                for (int j = 0; j <= m; ++j)
                {
                    previous[i, j] = -1;
                }
            }

            for (int i = 1; i <= n; ++i)
            {
                for (int j = 1; j <= m; ++j)
                {
                    if (a[i - 1] == b[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                        previous[i, j] = 1;
                    }
                    else
                    {
                        if (dp[i - 1, j] > dp[i, j - 1])
                        {
                            dp[i, j] = dp[i - 1, j];
                            previous[i, j] = 0;
                        }
                        else
                        {
                            dp[i, j] = dp[i, j - 1];
                            previous[i, j] = 2;
                        }
                    }
                }
            }

            List<string> answer = new List<string>();
            int y = n;
            int x = m;
            while (previous[y, x] != -1)
            {
                switch (previous[y, x])
                {
                    case 0:
                        y--;
                        break;
                    case 1:
                        answer.Add(a[y - 1]);
                        y--;
                        x--;
                        break;
                    default:
                        x--;
                        break;
                }
            }

            answer.Reverse();
            return answer.ToArray();
        }

        /// <summary>
        /// Печатает на экран или сохраняет в файл результат сравнения двух файлов.
        /// </summary>
        /// <param name="array">Список параметров, которые задаёт пользователь.</param>
        private static void CompareTwoFilesAndPrintDifference(string[] array)
        {
            string pathOne = array[1];
            string pathTwo = array[2];
            string pathDest = null;
            if (array.Length == 4)
                pathDest = array[^1];
            if (File.Exists(pathOne) && File.Exists(pathTwo) && !IsFileBig(new FileInfo(pathOne)) &&
                !IsFileBig(new FileInfo(pathTwo)) && !IsFileLocked(new FileInfo(pathOne)) &&
                !IsFileLocked(new FileInfo(pathTwo)))
            {
                string[] linesOne = File.ReadAllLines(pathOne, Encoding.UTF8);
                string[] linesTwo = File.ReadAllLines(pathTwo, Encoding.UTF8);
                string[] result = FindTheGreatestCommonSequence(linesOne, linesTwo);
                int n = linesOne.Length;
                int m = linesTwo.Length;
                int p = result.Length;
                List<string> text = new();
                for (int i = 0, j = 0, k = 0; i < n || j < m || k < p;)
                {
                    if (k < p)
                    {
                        while (i < n && linesOne[i] != result[k])
                        {
                            text.Add($"- {linesOne[i++]}");
                        }

                        while (j < m && linesTwo[j] != result[k])
                        {
                            text.Add($"+ {linesTwo[j++]}");
                        }

                        text.Add($"= {result[k++]}");
                        i++;
                        j++;
                    }
                    else
                    {
                        while (i < n)
                        {
                            text.Add($"- {linesOne[i++]}");
                        }

                        while (j < m)
                        {
                            text.Add($"+ {linesTwo[j++]}");
                        }
                    }
                }

                if (pathDest != null)
                {
                    try
                    {
                        File.WriteAllLines(pathDest, text.ToArray());
                    }
                    catch (Exception)
                    {
                        InformationMessages.CantGetAccessToTheFile();
                    }
                }
                else
                {
                    PrintArray(text);
                }
            }
            else if (!File.Exists(pathOne) || !File.Exists(pathTwo))
            {
                InformationMessages.FileNotExist();
            }
            else
            {
                InformationMessages.CantGetAccessToTheFile();
            }
        }


        /// <summary>
        /// Обрабатывает все возможные команды, которые могли поступить от пользователя.
        /// </summary>
        /// <param name="array">Список параметров, которые задаёт пользователь.</param>
        /// <param name="flag">Флаг используется в случае, если максимальная длина команды превысила
        /// допустимое число символов в строке.</param>
        /// <returns>Возвращает true, если команда выполнена успешна или команда слишком длинная и false
        /// иначе.</returns>
        internal static bool ChooseOperation(string[] array, bool flag)
        {
            int n = array.Length;
            if (flag)
                return true;
            try
            {
                if (n == 1 && array[0] == "reset")
                {
                    Reset();
                    return true;
                }

                if (n == 1 && array[0] == "help")
                {
                    Help();
                    return true;
                }

                if (n == 1 && array[0] == "exit")
                {
                    Finished = true;
                    return true;
                }

                if (n == 1 && array[0] == "lsblk")
                {
                    ShowAvailableDisks();
                    return true;
                }

                if (n == 2 && array[0] == "cd")
                {
                    ChangeDirectory(array[1]);
                    return true;
                }

                if (n == 1 && array[0] == "ls")
                {
                    ShowAvailableFiles();
                    return true;
                }

                if (n is >= 2 and <= 3 && array[0] == "read")
                {
                    TryRead(array);
                    return true;
                }

                if (n == 3 && array[0] == "cp")
                {
                    Copy(array[1], array[2]);
                    return true;
                }

                if (n == 3 && array[0] == "mv")
                {
                    Move(array[1], array[2]);
                    return true;
                }

                if (n >= 2 && array[0] == "create")
                {
                    TryCreateFile(array);
                    return true;
                }

                if (n >= 3 && array[0] == "cat")
                {
                    Concatenate(array[1..^1], array[^1]);
                    return true;
                }


                if (n == 2 && array[0] == "rm")
                {
                    Remove(array[1]);
                    return true;
                }

                if (n == 1 && array[0] == "encodings")
                {
                    ShowEncodings();
                    return true;
                }

                if (n >= 1 && array[0] == "mask")
                {
                    TryFindMask(array);
                    return true;
                }

                if (n >= 1 && array[0] == "maskd")
                {
                    TryFindMaskWithSubDirectories(array);
                    return true;
                }

                if (n >= 3 && array[0] == "maskc")
                {
                    CopyFilesToDirectory(array);
                    return true;
                }

                if ((n == 3 || n == 4) && array[0] == "diff")
                {
                    CompareTwoFilesAndPrintDifference(array);
                    return true;
                }
            }
            catch (Exception)
            {
                InformationMessages.SomethingWentWrong();
            }

            return false;
        }


        /// <summary>
        /// Получает тип копирования (перезаписывать или нет по строке).
        /// </summary>
        /// <param name="s">строка с командой</param>
        /// <returns>возвращает тип копирования и -1, если строка s некорректна.</returns>
        private static int GetTypeOfCopyFromString(string s)
        {
            switch (s)
            {
                case "1": return 1;
                case "0": return 0;
                default: return -1;
            }
        }

        /// <summary>
        /// Извлекает из массива параметров маску.
        /// </summary>
        /// <param name="array">Массив параметров, которые задаёт пользователь.</param>
        /// <param name="from">В какой позиции для данной команды по счёту должна находиться маска</param>
        /// <returns>Возвращает маску, если она есть в параметрах и дефолтную (под которую все файлы подходят
        /// иначе.</returns>
        private static string GetMaskFromArray(string[] array, int from = 1)
        {
            if (array.Length <= from)
                return @"(\w+)\.(\w+)";
            return string.Join(' ', array[from..]);
        }

        /// <summary>
        /// Печатает все файлы, которые удовлетворяют данной маске.
        /// </summary>
        /// <param name="array">Массив, в котором содержится информация о файлах.</param>
        /// <param name="needFullName">Флаг отвечающий за то, нужно ли печатать полное имя файла или нет.</param>
        private static void PrintPaths(List<FileInfo> array, bool needFullName = false)
        {
            foreach (FileInfo element in array)
            {
                Console.WriteLine(needFullName ? element.FullName : element.Name);
            }
        }

        /// <summary>
        /// Печатает массив строк.
        /// </summary>
        /// <param name="array">Список, который печатают.</param>
        private static void PrintArray(List<string> array)
        {
            foreach (string element in array)
            {
                Console.WriteLine(element);
            }
        }

        /// <summary>
        /// Очищает консоль.
        /// </summary>
        private static void Reset()
        {
            Console.Clear();
        }
    }
}