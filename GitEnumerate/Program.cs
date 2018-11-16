using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using static System.Console;

namespace GitEnumerate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var color = ForegroundColor;

            var folders = Directory.GetDirectories("C:\\Git");

            var length = folders.Max(f => f.Length);

            var verbose = args != null && args.Length > 0 && args[0].ToLower() == "-v";

            foreach (var folder in folders)
            {
                using (var repo = new Repository(folder))
                {
                    var status = repo.RetrieveStatus(new StatusOptions());

                    ForegroundColor = ConsoleColor.White;
                    Write(folder);
                    Write(new string(' ', length - folder.Length + 2));
                    ForegroundColor = ConsoleColor.Green;
                    Write($"+ {status.Added.Count().ToString().PadRight(3)} ");
                    ForegroundColor = ConsoleColor.Yellow;
                    Write($"~ {status.Modified.Count().ToString().PadRight(3)} ");
                    ForegroundColor = ConsoleColor.Red;
                    Write($"~ {status.Removed.Count().ToString().PadRight(3)} ");
                    ForegroundColor = ConsoleColor.Cyan;
                    WriteLine(repo.Head.FriendlyName);

                    if (verbose)
                    {
                        ForegroundColor = ConsoleColor.Green;
                        status.Added.ToList().ForEach(a => WriteLine(a.FilePath));
                        ForegroundColor = ConsoleColor.Yellow;
                        status.Modified.ToList().ForEach(m => WriteLine(m.FilePath));
                        ForegroundColor = ConsoleColor.Red;
                        status.Missing.ToList().ForEach(m => WriteLine(m.FilePath));
                    }
                }
            }

            ForegroundColor = color;
        }
    }
}
