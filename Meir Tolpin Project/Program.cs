using Meir_Tolpin_Project.Connection;
using Meir_Tolpin_Project.Controls;
using Meir_Tolpin_Project.Interface;
using MouseKeyboardLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Meir_Tolpin_Project
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AllocConsole();
            
            ConnectionInterface form = new ConnectionInterface();
            Application.Run(form);

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
