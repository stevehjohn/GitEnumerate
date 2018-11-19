using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace GitEnumerate
{
    public class Program
    {
        private static ConsoleColor _userForegroundColor;
        private static bool _verbose;

        public static void Main(string[] args)
        {
            WriteLine();

            _userForegroundColor = ForegroundColor;

            //var recurse = 1;

            if (args != null && args.Length > 0)
            {
                var argsList = args.ToList().Select(a => a.ToLower()).ToArray();

                for (var i = 0; i < argsList.Length; i++)
                {
                    switch (argsList[i])
                    {
                        case "-v":
                            _verbose = true;
                            break;
                        //case "-r":
                        //    recurse = 1;
                        //    if (argsList.Length - 1 > i)
                        //    {
                        //        if (!argsList[i + 1].StartsWith("-"))
                        //        {
                        //            if (!int.TryParse(argsList[i + 1], out recurse))
                        //            {
                        //                Help($"Unsuitable parameter '{argsList[i + 1]}' supplied for -r option.");
                        //            }

                        //            i++;
                        //        }
                        //    }
                        //    break;
                        case "-h":
                            Help();
                            break;
                        default:
                            Help($"Unknown argument '{argsList[i]}'.");
                            break;
                    }
                }
            }

            EnumerateFolder(Environment.CurrentDirectory);

            ForegroundColor = _userForegroundColor;
        }

        private static void EnumerateFolder(string root)
        {
            ForegroundColor = ConsoleColor.Cyan;
            Write("  Enumerating ");
            ForegroundColor = ConsoleColor.White;
            WriteLine($"{root}\n");

            string[] folders;

            try
            {
                folders = Directory.GetDirectories(root);

                if (folders.Length == 0)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("    No subfolders found.");
                    return;
                }
            }
            catch
            {
                // Probably access denied
                return;
            }

            var count = 0;

            var length = folders.Max(f => f.Length);

            foreach (var folder in folders)
            {
                try
                {
                    using (var repo = new Repository(folder))
                    {
                        var status = repo.RetrieveStatus(new StatusOptions());

                        var displayFolder = folder;

                        if (displayFolder.StartsWith(".\\") || displayFolder.StartsWith(".//"))
                        {
                            displayFolder = displayFolder.Substring(2);
                        }

                        ForegroundColor = ConsoleColor.White;
                        Write($"    {displayFolder}");
                        Write(new string(' ', length - displayFolder.Length + 2));
                        ForegroundColor = ConsoleColor.Green;
                        Write($"+ {status.Added.Count().ToString().PadRight(3)} ");
                        ForegroundColor = ConsoleColor.Yellow;
                        Write($"~ {status.Modified.Count().ToString().PadRight(3)} ");
                        ForegroundColor = ConsoleColor.Red;
                        Write($"~ {status.Missing.Count().ToString().PadRight(3)} ");
                        ForegroundColor = ConsoleColor.Cyan;
                        WriteLine(repo.Head.FriendlyName);

                        if (_verbose)
                        {
                            ForegroundColor = ConsoleColor.Green;
                            status.Added.ToList().ForEach(a => WriteLine($"      + {a.FilePath}"));
                            ForegroundColor = ConsoleColor.Yellow;
                            status.Modified.ToList().ForEach(m => WriteLine($"      ~ {m.FilePath}"));
                            ForegroundColor = ConsoleColor.Red;
                            status.Missing.ToList().ForEach(m => WriteLine($"      - {m.FilePath}"));
                        }

                        count++;
                    }
                }
                catch
                {
                    // Probably not a repo
                }

                if (count == 0)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("    No repositories found.\n");
                }
            }

            WriteLine();
        }

        private static void Help(string message = null)
        {
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("  Git Status Folder Enumerator by Stevö John.");

            if (message != null)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"\n    {message}\n");
            }

            ForegroundColor = ConsoleColor.Cyan;

            WriteLine("  Usage:");
            WriteLine("    GitEnumerate [-v]");
            //WriteLine("    GitEnumerate [-v] [-r [levels]]");
            WriteLine("\n  Where:");
            WriteLine("    -v: Verbose, lists added/changed/deleted files within the repo.");
            //WriteLine("    -r: Recurse, recurse more than one level from the current folder.");
            //WriteLine("    levels: how many levels to recurse if -r specified. Default: 2 if -r specified, 1 if not.");
            WriteLine();

            ForegroundColor = _userForegroundColor;
            Environment.Exit(0);
        }
    }

    //public class FolderInfo
    //{
    //    public string Folder { get; set; }
    //    public int Level { get; set; }

    //    public FolderInfo(string folder, int level)
    //    {
    //        Folder = folder;
    //        Level = level;
    //    }
    //}
}
