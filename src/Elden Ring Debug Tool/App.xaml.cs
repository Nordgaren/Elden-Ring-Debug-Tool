using Bluegrams.Application;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Properties.Settings? Settings;

        public App()
        {
            var args = Environment.GetCommandLineArgs();
#if DEBUG
            //args = new[] { "", @"G:\Steam\steamapps\common\ELDEN RING 1.03.3\Game\regulation.bin.bnd" };
#endif
            if (args.Length > 1)
                ProcessRegulationAndExit(args);

            PortableJsonSettingsProvider.SettingsFileName = "ERDebug.settings.json";
            PortableJsonSettingsProvider.SettingsDirectory = Environment.CurrentDirectory;
            PortableJsonSettingsProvider.ApplyProvider(Elden_Ring_Debug_Tool.Properties.Settings.Default);
            Settings = Elden_Ring_Debug_Tool.Properties.Settings.Default;

            //Global
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;

            //WPF specific - setting this event as handled can prevent crashes
            Dispatcher.UnhandledException += WpfExceptionHandler;
        }

        private void ProcessRegulationAndExit(string[] args)
        {
            var buffer = new byte[4];
            using (var fs = File.OpenRead(args[1]))
            {
                fs.Read(buffer, 0, buffer.Length);
            }

            if (CheckIfPossiblyEncrypted(buffer))
            {
                if (!File.Exists($"{args[1]}.PreDecrypt.bak"))
                    File.Copy(args[1], $"{args[1]}.PreDecrypt.bak");

                var decryptedReg = SFUtil.DecryptERRegulation(args[1]);
                decryptedReg.Write(args[1]);
                MessageBox.Show("Regulation file decrypted", "BND Decrypted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (BND4.IsRead(args[1], out BND4 bnd))
            {
                if (!File.Exists($"{args[1]}.PreEncrypt.bak"))
                    File.Copy(args[1], $"{args[1]}.PreEncrypt.bak");

                SFUtil.EncryptERRegulation(args[1], bnd);
                MessageBox.Show("Regulation file encrypted", "BND Encrypted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Drag a regulation bin onto this exe to encrypt or decrypt it.", "Elden Ring Debug Tool Command Line Help", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Environment.Exit(0);
        }

        private bool CheckIfPossiblyEncrypted(byte[] buffer)
        {
            var magic = Encoding.ASCII.GetString(buffer);
            return magic != "BND4" && magic != "DCX\0";
        }

        void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = (Exception)e.ExceptionObject;
                LogException(exception);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void WpfExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogException(e.Exception);
            }
            catch (Exception ex)
            {
                //Ignore
            }

        }

        private readonly object _logFileLock = new object();
        private void LogException(Exception exception)
        {
            lock (_logFileLock)
            {
                var logFile = Environment.CurrentDirectory + @"\log.txt";

                //Log retention: at most 2 days. Can up this, but don't want to risk creating a 10GB log file when shit goes wrong.
                //Or when it is never cleared. Use NLog? 
                var createDate = File.GetCreationTime(logFile);
                var clearDate = createDate.AddDays(2);
                if (DateTime.Now > clearDate)
                {
                    File.Delete(logFile);
                }

                var sb = new StringBuilder();
                //Log the date and time 
                sb.Append($"{DateTime.Now:ddd, dd MMM yyy HH':'mm':'ss 'GMT'}\n");
                //Log the error
                sb.Append($"{exception.Message}\n\n");

                var count = 0;
                var innerException = exception.InnerException;
                while (innerException != null)
                {
                    sb.Append($"Inner Exception {count}\n");
                    sb.Append($"{ innerException.Message}\n\n");
                    innerException = innerException.InnerException;
                    count++;
                }

                sb.Append($"{exception.StackTrace}\n\n\n");
                File.AppendAllText(logFile, sb.ToString());
            }
        }
    }
}
