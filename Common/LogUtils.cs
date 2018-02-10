using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common
{
    public class LogUtils
    {
        private const string Kernel32DllName = "kernel32.dll";

        public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

        [DllImport(Kernel32DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32DllName)]
        private static extern int GetConsoleOutputCP();

        /// Creates a new console instance if the process is not attached to a console already.
        public static void Show()
        {
            if (HasConsole) return;
            AllocConsole();
            InvalidateOutAndError();
        }

        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        public static void Hide()
        {
            if (!HasConsole) return;
            SetOutAndErrorNull();
            FreeConsole();
        }

        public static void Toggle()
        {
            if (HasConsole)
                Hide();
            else
                Show();
        }

        private static void InvalidateOutAndError()
        {
            var type = typeof(Console);
            var _out = type.GetField("_out",
                BindingFlags.Static | BindingFlags.NonPublic);
            var error = type.GetField("_error",
                BindingFlags.Static | BindingFlags.NonPublic);
            var initializeStdOutError = type.GetMethod("InitializeStdOutError",
                BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(_out != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);
            _out.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] {true});
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        public static void Write(string str)
        {
            var rootPath = $"{Environment.CurrentDirectory}/logs/";
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            var path = $"{rootPath}{DateTime.Now:yyyy_MM_dd}.txt";
            try
            {
                using (var log = new StreamWriter(path, true))
                {
                    log.WriteLine("*********************************");
                    log.WriteLine("Time|" + DateTime.Now.ToLongTimeString());
                    log.WriteLine(str);
                    log.WriteLine("*********************************");
                    log.WriteLine();
                    log.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}