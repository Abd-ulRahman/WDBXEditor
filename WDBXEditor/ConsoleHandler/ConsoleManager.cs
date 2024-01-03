using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDBXEditor.Reader.FileTypes;
using WDBXEditor.Storage;

namespace WDBXEditor.ConsoleHandler
{
    public static class ConsoleManager
    {
        public static bool ConsoleMode { get; set; } = false;
        public static bool IsClosing { get; set; } = true;

        public static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(string[] args);

        public static void ConsoleMain(string[] args)
        {
            Database.LoadDefinitions();

            if (CommandHandlers.ContainsKey(args[0].ToLower()))
                InvokeHandler(args[0], args.Skip(1).ToArray());

            while (!IsClosing)
            {
                args = Console.ReadLine().Split(' ');
                if (CommandHandlers.ContainsKey(args[0].ToLower()))
                    InvokeHandler(args[0], args.Skip(1).ToArray());
            }

        }

        public static bool InvokeHandler(string command, params string[] args)
        {
            try
            {
                command = command.ToLower();
                if (CommandHandlers.ContainsKey(command))
                {
                    CommandHandlers[command].Invoke(args);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("");
                return false;
            }
        }
        public static string RunCommand(string arguments, bool readOutput)
        {
            var output = string.Empty;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = "cmd.exe",
                    Arguments = "/C " + arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                };

                var proc = Process.Start(startInfo);

                if (readOutput)
                {
                    output = proc.StandardOutput.ReadToEnd();
                }

                proc.WaitForExit(60000);

                return output;
            }
            catch (Exception)
            {
                return output;
            }
        }

        public static void LoadCommandDefinitions()
        {
            //Argument commands
            DefineCommand("-console", LoadConsoleMode);
            DefineCommand("-export", ConsoleCommands.ExportArgCommand);
            DefineCommand("-sqldump", ConsoleCommands.SqlDumpArgCommand);
            DefineCommand("-extract", ConsoleCommands.ExtractCommand);

            //Console commands
            DefineCommand("export", ConsoleCommands.ExportConCommand);
            DefineCommand("sqldump", ConsoleCommands.SqlDumpConCommand);
            DefineCommand("extract", ConsoleCommands.ExtractCommand);
            DefineCommand("load", ConsoleCommands.LoadCommand);
            DefineCommand("help", ConsoleCommands.HelpCommand);
            DefineCommand("exit", delegate { Environment.Exit(0); });
            DefineCommand("restart", Restart_Console);
        }

        public static void LoadConsoleMode(string[] args)
        {
            IsClosing = false;
            ConsoleMode = true;

            Console.WriteLine("   WDBX Editor - Console Mode");
            Console.WriteLine("Type help to see a list of available commands.");
            Console.WriteLine("");

            //Remove argument methods
            foreach (var k in CommandHandlers.Keys.ToList())
                if (k[0] == '-')
                    CommandHandlers.Remove(k);
        }

        [ConsoleHelp("Restart the Console version of the program Typing any thing else than Existing Commands will start GUI", "", "")]
        private static void Restart_Console(string[] args)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath);
            Environment.Exit(0);
        }

        /// <summary>
        /// Converts args into keyvalue pair
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseCommand(string[] args)
        {
            Dictionary<string, string> keyvalues = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (i == args.Length - 1)
                    break;

                string key = args[i].ToLower();
                string value = args[++i];
                if (value[0] == '"' && value[value.Length - 1] == '"')
                    value = value.Substring(1, value.Length - 2);

                keyvalues.Add(key, value);
            }

            return keyvalues;
        }

        private static void DefineCommand(string command, HandleCommand handler)
        {
            CommandHandlers[command.ToLower()] = handler;
        }
    }
}
