﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WDBXEditor.ConsoleHandler;
using Button = System.Windows.Forms.Button;

namespace WDBXEditor
{
    class Program
    {
        //static string[] _args = _args ?? new string[] { "-console" };
        public static bool PrimaryInstance = false;
        [STAThread]
        static void Main(string[] args)
        {
            InstanceManager.InstanceCheck(args); //Check to see if we can run this instance
            SetDllDirectory(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), (Environment.Is64BitProcess ? "x64" : "x86")));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.WriteLine("Hello World!");
            ConsoleManager.LoadConsoleMode(args);
            for (; ; )
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