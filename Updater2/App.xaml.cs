﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Updater2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string exepath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        public static bool openingDS4W;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mwd = new MainWindow();
            for (int i=0, arlen = e.Args.Length; i < arlen; i++)
            {
                string temp = e.Args[i];
                if (temp.Contains("-skipLang"))
                    mwd.downloadLang = false;
                else if (temp.Equals("-autolaunch"))
                    mwd.autoLaunchDS4W = true;
            }

            mwd.Show();
        }

        public App()
        {
            //Console.WriteLine(CultureInfo.CurrentCulture);
            this.Exit += (s, e) =>
                {
                    string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                    if (!openingDS4W && File.Exists(exepath + "\\Update Files\\DS4Windows\\DS4Updater.exe")
                        && FileVersionInfo.GetVersionInfo(exepath + "\\Update Files\\DS4Windows\\DS4Updater.exe").FileVersion.CompareTo(version) != 0)
                    {
                        File.Move(exepath + "\\Update Files\\DS4Windows\\DS4Updater.exe", exepath + "\\DS4Updater NEW.exe");
                        Directory.Delete(exepath + "\\Update Files", true);
                        StreamWriter w = new StreamWriter(exepath + "\\UpdateReplacer.bat");
                        w.WriteLine("@echo off"); // Turn off echo
                        w.WriteLine("@echo Attempting to replace updater, please wait...");
                        w.WriteLine("@ping -n 4 127.0.0.1 > nul"); //Its silly but its the most compatible way to call for a timeout in a batch file, used to give the main updater time to cleanup and exit.
                        w.WriteLine("@del \"" + exepath + "\\DS4Updater.exe" + "\"");
                        w.WriteLine("@ren \"" + exepath + "\\DS4Updater NEW.exe" + "\" \"DS4Updater.exe\"");
                        w.WriteLine("@DEL \"%~f0\""); // Attempt to delete myself without opening a time paradox.
                        w.Close();

                        Process.Start(exepath + "\\UpdateReplacer.bat");
                    }
                    else if (File.Exists(exepath + "\\DS4Updater NEW.exe"))
                        File.Delete(exepath + "\\DS4Updater NEW.exe");
                    if (!openingDS4W && Directory.Exists(exepath + "\\Update Files"))
                        Directory.Delete(exepath + "\\Update Files", true);
                };
        }
    }
}
