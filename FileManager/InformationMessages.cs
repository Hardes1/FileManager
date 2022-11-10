using System;
using System.IO;

namespace FileManager
{
    /// <summary>
    /// Класс, который содержит все сообщения, выводимые программой.
    /// </summary>
    public static class InformationMessages
    {
        /// <summary>
        /// Выводит приветствие пользователю на экран.
        /// </summary>
        internal static void Greeetings()
        {
            Console.WriteLine("Приветствую вас в файловом менеджере! Для того чтобы ознакомиться со спиком\nдоступных" +
                              " команд, введите help или же введите любую из допустимых команд.");
        }

        /// <summary>
        /// Выводит прощание с пользователем.
        /// </summary>
        internal static void Goodbye()
        {
            Console.WriteLine("Спасибо, что воспользовались файловым менеджером, до свидания!");
        }

        /// <summary>
        /// Если была введена некорректная команда, то выводит сообщение на экран об этом.
        /// </summary>
        internal static void IncorrectCommand()
        {
            Console.WriteLine("Была введена некорректная команда, попробуйте ещё раз");
        }

        /// <summary>
        /// Печатает список команд, которые доступны пользователю.
        /// </summary>
        internal static void AvailableCommands()
        {
            Console.WriteLine("lsblk - показать информацию обо всех дисках.");
            Console.WriteLine("cd <path> - изменить текущую директорию/диск на <path>.");
            Console.WriteLine(
                "read <path> [encoding] - прочитать весь текст из существующего " +
                "текстового файла, расположенного в <path> " +
                "в кодировке [encoding].");
            Console.WriteLine("cp <path_from> <path_to> - копирование файла, <path_from> - копируемый файл," +
                              "<path_to> имя целевого файла. Он не может быть файлом директории.\nЕсли" +
                              " файл <path_to> уже существует, то программа перезапишет его, если это возможно.");
            Console.WriteLine("mv <path_from> <path_to> - перемещение файла, <path_from> - файл для перемещения," +
                              " <path_to> имя целевого файла. Он не может быть файлом директории.\nЕсли" +
                              " файл <path_to> уже существует, то программа перезапишет его, если это возможно.");
            Console.WriteLine("rm <path> - удалить файл, расположенным в <path>, если это возможно.");
            Console.WriteLine("encodings - список поддерживаемых кодировок.");
            Console.WriteLine("create <path> [encoding] [text] - создаёт файл, который расположен по пути <path>," +
                              "с текстом [text] в кодировке [encoding].");
            Console.WriteLine("cat <path1> <path2> ... <pathn> <path destination> - записывает всё содержимое из" +
                              "файлов <path1>, <path2> ... <pathn> в <path destination>\nи результат конкатенации" +
                              "выводит в консоль.");
            Console.WriteLine("mask [mask] - вывести все файлы в текущей директории по заданной маске." +
                              " По умолчанию выводит все файлы в директории.");
            Console.WriteLine("maskd [mask] - вывести все файлы в текущей директории и в дочерних по заданной маске." +
                              " По умолчанию выводит все файлы в директории и её поддиректориях.");
            Console.WriteLine("maskc <directory_from> <directory_to> <type_of_copy> [mask] - копирует все файлы" +
                              " из папки <directory_from> в папку <directory_to> по заданной маске. <type_of_copy>\n" +
                              "может быть равен 0 или 1. При <type_of_copy>==0 файл не копируется в директорию, " +
                              "если файл с таким названием уже в ней существует, иначе перезаписывает файл.");
            Console.WriteLine("diff <path1> <path2> [path_dest] выводит последовательность действий, которые надо" +
                              " совершить, чтобы превратить файл <path1> в <path2>, по желанию результат операций\n" +
                              "можно записать в файл [path_dest].");
            Console.WriteLine("reset - очистить командную строку.");
            Console.WriteLine("exit - выход из программы.");
            Console.WriteLine("Везде, где необходим параметр <path> допускается как относительный," +
                              " так и абсолютный путь.");
            Console.WriteLine("Более подробно" +
                              "о том, как задавать маску можно прочитать на сайте\n" +
                              "https://docs.microsoft.com/ru-ru/dotnet/standard/base-types/regular-expressions");
            Console.WriteLine("[encoding] - дополнительные параметры, которые можно не вводить.");
        }

        /// <summary>
        /// Показывает список доступных кодировок, с которыми работает файловый менеджер.
        /// </summary>
        internal static void ShowEncodings()
        {
            Console.WriteLine("Список доступных кодировок, поддерживаемых программой:");
            Console.WriteLine("utf-8");
            Console.WriteLine("utf-32");
            Console.WriteLine("ascii");
            Console.WriteLine("utf-16");
            Console.WriteLine("latin-1");
        }


        /// <summary>
        /// Выводит сообщение об ошибке, если был введён некорректный тип копирования.
        /// </summary>
        internal static void ErrorIncorrectTypeOfCopy()
        {
            Console.WriteLine("Тип копирования, который вы выбрали некорректен.");
        }

        /// <summary>
        /// Выводит сообщение об ошибке, связанное с несуществующей директорией
        /// </summary>
        internal static void ErrorIncorrectDirectory()
        {
            Console.WriteLine("Директории, из которой вы хотите скопировать файлы не существует.");
        }

        /// <summary>
        /// Выводит сообщение об ошибке, если данного файла не существует.
        /// </summary>
        internal static void FileNotExist()
        {
            Console.WriteLine("Файла, к которому вы хотите обратиться не существует.");
        }

        /// <summary>
        /// Выводит сообщение об ошибке, если файла, который пытается скопировать пользователь не существует.
        /// </summary>
        internal static void CopiedOrMovedFileNotExist()
        {
            Console.WriteLine("Файла, который вы пытаетесь скопировать или переместить не существует.");
        }

        /// <summary>
        /// Выводит сообщение об ошибке, если директория, с которой пользователь что-то хочет сделать не существует.
        /// </summary>
        internal static void DirectoryNotExist()
        {
            Console.WriteLine("Директории, к которой вы хотите обратиться не существует.");
        }


        /// <summary>
        /// Выводит сообщение об ошибке, если файл, к которому пытается обратиться пользователь, не доступен.
        /// </summary>
        internal static void CantGetAccessToTheFile()
        {
            Console.WriteLine("Не удаётся получить доступ к файлу или директории," +
                              " возможно, сейчас он(а) открыт(а) в другой программе.");
        }


        /// <summary>
        /// Выводит путь директивы, в которой пользователь находится в данный момент.
        /// </summary>
        internal static void PrintCurrenPath()
        {
            Console.Write(Directory.GetCurrentDirectory() + @"$ ");
        }

        /// <summary>
        /// Выводит сообщение об ошибке, если пользователь указал неподдерживаемую кодировку.
        /// </summary>
        internal static void IncorrectEncoding()
        {
            Console.WriteLine("Кодировка, которую вы ввели не существует или не поддерживается.");
        }


        /// <summary>
        /// Выводит сообщение об ошибке, если программа где-то поймала исключение, но оно не подходит
        /// под другие случаи
        /// </summary>
        internal static void SomethingWentWrong()
        {
            Console.WriteLine("Кажется, что-то пошло не так.");
        }


        /// <summary>
        /// Выводит сообщение об ошибке, если было введено некорректное регулярное выражение.
        /// </summary>
        internal static void IncorrectRegularExpression()
        {
            Console.WriteLine("Регулярное выражение, которые вы написали некорректно");
        }
    }
}