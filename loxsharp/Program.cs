using System;
using System.IO;

namespace Lox_
{
    class Program
    {
        private static bool _hadError;

        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: lox# [script]");
                Environment.Exit(exitCode: 64);
            }
            else if (args.Length == 1)
            {
                RunFile(path: args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                _hadError = false;
            }
        }

        private static void RunFile(string path)
        {
            var sourceCode = File.ReadAllText(path);
            Run(sourceCode);

            // Indicate an error in the exit code.           
            if (_hadError)
                Environment.Exit(exitCode: 65);
        }

        private static void Run(string sourceCode)
        {
            var scanner = new Scanner(sourceCode);
            var tokens = scanner.ScanTokens();

            // For now, just print the tokens.        
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine(
                "[line " + line + "] Error" + where + ": " + message);
            _hadError = true;
        }
    }
}
