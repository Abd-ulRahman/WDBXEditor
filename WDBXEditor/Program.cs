using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WDBXEditor.ConsoleHandler;

namespace WDBXEditor
{
    class Program
    {
        public static bool PrimaryInstance = false;
        [STAThread]
        static void Main(string[] args)
        {
            SetDllDirectory(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), (Environment.Is64BitProcess ? "x64" : "x86")));

            InstanceManager.InstanceCheck(args); //Check to see if we can run this instance
            InstanceManager.LoadDll("StormLib.dll"); //Loads the correct StormLib library

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Console.WriteLine("Hello World!");
            ConsoleManager.LoadConsoleMode(args);

            for (;;)
            {
                var input = Console.ReadLine();
                args = input.Split(' ');
                System.Console.WriteLine("Got " + input);
                if (args != null && args.Length > 0)
                {
                    ConsoleManager.LoadCommandDefinitions();

                    if (ConsoleManager.CommandHandlers.ContainsKey(args[0].ToLower()))
                    {
                        //CreateConsole(); //Create a new console

                        ConsoleManager.ConsoleMain(args); //Console mode
                    }
                    else
                    {
                        Application.Run(new Main(args)); //Load file(s)
                    }
                }
                else
                {
                    Application.Run(new Main()); //Default
                }
                InstanceManager.Stop();
            }
        }

        public static void CreateConsole()
        {
            if (!AttachConsole(-1)) //Attempt to attach to existing console window
                AllocConsole(); //Create a new console
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);
        [DllImport("kernel32")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);
    }
}
