using MonoBleedingCube;
using Spectre.Console;
using System.Collections.Generic;
using System.IO;
using Color = Spectre.Console.Color;
using BoldText = Spectre.Console.FigletText;
using SimpleConfig = MagmaMc.MagmaSimpleConfig.MagmaSimpleConfig;
using System;
using MagmaMc.JEF;
using System.Threading;
using static MonoBleedingCube.Utils;
using EtraExtensions;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Windows.Forms;
using Pastel;
using System.Linq;
using MagmaMc.GDI;
using Text = MagmaMc.GDI.Text;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MonoBleedingCube;
using Size = System.Drawing.Size;
using System.Reflection;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
[SupportedOSPlatform("windows")]
#endif
namespace MonoBleedingCube
{
    internal class MonoBleedingCube
    {
        [STAThread]
        public static void Main(string[] ConsoleArgs) =>
            new MonoBleedingCube().StartMain(ConsoleArgs);

        static IntPtr ConsoleWindow = JEF.GetConsoleWindow();
        static Debugger Debugger = new Debugger();
        static SimpleConfig Config = new SimpleConfig("MBC.MSC");

        public void StartMain(string[] ConsoleArgs)
        {
            JEF.Windows.RestoreWindow(JEF.GetConsoleWindow());
            Console.CursorVisible = false;
            Console.SetWindowSize(150, 32);
            ConfigSetup();

            if (!(bool)Config.GetValue("DevMode", true, "MonoBleedingCube"))
                JEF.Windows.HideWindow(JEF.GetConsoleWindow());

            SetTitle("MonoBleedingCube", Color.Orange3);

            string Data = (string)Config.GetValue("AppName", Section: "MonoBleedingCube") + "_Data\\MonoBleedingCube-Data\\";
            Files.Setup((string)Config.GetValue("AppName", Section: "MonoBleedingCube"));
            if ((bool)Config.GetValue("DevMode", true, "MonoBleedingCube"))
                DevelopmentMode();
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo((string)Config.GetValue("AppName", Section: "MonoBleedingCube"));
            float sizeX = (float)Config.GetValue("SplashWidth", 1, "MonoBleedingCube");
            float sizeY = (float)Config.GetValue("SplashHeight", 1, "MonoBleedingCube");
            ScreenRenderer Renderer = new ScreenRenderer();
            try
            {
                Bitmap BitmapImage = (Bitmap)Image.FromFile(Data + "Splash.png");
                Renderer.StartRender();
                BitmapImage.SetResolution(sizeX * BitmapImage.HorizontalResolution, sizeY * BitmapImage.VerticalResolution);
                for (int i = 0; i < 10; i++)
                {
                    Renderer.ScreenGraphics.DrawImage(BitmapImage, (Screen.PrimaryScreen.Bounds.Width / 2) - 200, 200);
                    Thread.Sleep(100);
                }
                Renderer.EndRender();
            }
            catch { }
            Task.Run(() =>
            {
                Bitmap BitmapImage = (Bitmap)Image.FromFile(Data + "Splash.png");
                Renderer.StartRender();
                BitmapImage.SetResolution(sizeX * BitmapImage.HorizontalResolution, sizeY * BitmapImage.VerticalResolution);
                for (int i = 0; i < 5; i++)
                {
                    Renderer.ScreenGraphics.DrawImage(BitmapImage, (Screen.PrimaryScreen.Bounds.Width / 2) - 200, 200);
                    Thread.Sleep(100);
                }
                Renderer.EndRender();
            });
            Files.FilesToGame(Config);

            process.Start();
            process.WaitForExit();

        }


        public void ConfigSetup()
        {
            if (!File.Exists(Config.FileName))
                FirstTimeUse();
        }

        public void SetTitle(string @Title, Color color)
        {
            Console.Title = "MonoBleedingCube - " + @Title;

            Console.Clear();
            BoldText Bold_Text = new BoldText(@Title).Centered().Color(color);
            AnsiConsole.Write(Bold_Text);
            AnsiConsole.Write(new Markup("[orange3]Bringing The Edge To The 3D Dimension[/]").Centered());
            AnsiConsole.MarkupLine($"[{color}]" + "=".Multiply(Console.WindowWidth - 1).CenterText() + "[/]");
        }

        public void DevelopmentMode()
        {
            SetTitle("Developer Mode", Color.Orange3);
            while (true)
            {
                string Data = (string)Config.GetValue("AppName", Section: "MonoBleedingCube") + "_Data\\MonoBleedingCube-Data\\";
                bool Devmode = (bool)Config.GetValue("DevMode", true, "MonoBleedingCube");
                string Tab = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(10)
                        .Title("Config")
                        .WrapAround(true)
                        .MoreChoicesText("[grey](Move up and down to reveal more Options)[/]")
                        .AddChoices(new[] {
                            "Add Overwrite", "Add Copy", "Add Zip",
                            "Change Splash", "Install File Extension",
                            (Devmode == true ?  "Disable" : "Enable" ) + " DevMode",
                            "Exit"
                        }));
                if (Tab == "Add Overwrite")
                    MoveFiles("Overwrite", Data);

                else if (Tab == "Add Copy")
                    MoveFiles("Copy", Data);

                else if (Tab == "Add Zip")
                    AddZipFiles();

                else if (Tab == "Change Splash")
                    Utils.FileDialog.GetBitmap("").Save(Data + "\\Splash.png");

                else if (Tab.Contains("DevMode"))
                    Config.SetValue("DevMode", !Devmode, "MonoBleedingCube");

                else if (Tab == "Exit")
                    Environment.Exit(1);
                else if (Tab == "Install File Extension")
                {
                    File.WriteAllText("Add MSC.reg", Resources.Add_MSC);
                    try
                    {
                        Process.Start(new ProcessStartInfo("Add MSC.reg") { UseShellExecute = true });
                    }
                    catch { MessageBox.Show("Failed To Install File Extension,\n Opening Directory"); Process.Start(new ProcessStartInfo(Directory.GetCurrentDirectory()) { UseShellExecute = true }); }

                }
            }
        }

        public void AddZipFiles()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        public void FirstTimeUse()
        {
            SetTitle("First Time Setup", Color.Orange3);
            Thread.Sleep(50);
            "Go Through Automatic Setup".Pastel("#3aa9cf").DrawIn(25, true, false);
            bool GameData_Folder = AnsiConsole.Confirm("");


            SetTitle("Fist Time Setup", Color.Orange3);

            Progress ProgressBar = AnsiConsole.Progress();

            #region Config Setup
            ProgressBar.Start(ProgressContent =>
            {
                string AppName = "";

                List<string> Files_Dir = new List<string>();
                List<string> Folders_Dir = new List<string>();
                foreach (string folder in Directory.GetFiles("./"))
                    Files_Dir.Add(folder.Replace(new FileInfo(folder).Extension, "").Replace("./", ""));
                foreach (string folder in Directory.GetDirectories("./"))
                    Folders_Dir.Add(folder.Replace("./", ""));

                // Files
                Utils.ProgressBar(ProgressContent, Directory.GetFiles("./"), "Indexing Files");

                // Folders
                Utils.ProgressBar(ProgressContent, Directory.GetDirectories("./"), "Indexing Folders");


                ProgressTask ScanForUnity = ProgressContent.AddTask("[orange1]Finding Game[/]");
                foreach (string _FileName in Files_Dir)
                {
                    string FileName = _FileName.Replace(".exe", "").Replace("./", "");
                    if (Folders_Dir.Contains(FileName + "_Data"))
                    {
                        AppName = FileName;
                        break;
                    }

                    ScanForUnity.Increment(Math.Ceiling(100.0 / Files_Dir.Count));
                    Thread.Sleep(100);
                }

                if (AppName == "")
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = "MonoBleedingCube - Setup";
                    dialog.Filter = "Unity Executable|*.exe";
                    dialog.InitialDirectory = Directory.GetCurrentDirectory();
                    dialog.RestoreDirectory = false;
                    dialog.Multiselect = false;
                    if (DialogResult.OK == dialog.ShowDialog())
                        AppName = dialog.SafeFileName.Replace(new FileInfo(dialog.SafeFileName).Extension, "");

                }




                /// File Selection
                /// Kepp Dev Mode Active?
                /// 



                File.WriteAllText(Config.FileName, "");
                Config.SetValue("AppName", AppName, Section: "MonoBleedingCube");
                Config.SetValue("DevMode", "true", Section: "MonoBleedingCube");
                Config.SetValue("Version", Application.ProductVersion, Section: "MonoBleedingCube");
                Config.GetValue("SplashWidth", 1, "MonoBleedingCube");
                Config.GetValue("SplashHeight", 1, "MonoBleedingCube");
            });
            #endregion

            Console.Clear();
        }
        public void MoveFiles(string Dir, string Data)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FileName = Dir + " Selection:";
            openFileDialog.InitialDirectory = "./";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string FileName in openFileDialog.FileNames)
                {
                    // get the file attributes for file or directory
                    FileAttributes attr = File.GetAttributes(FileName);
                    string selectedPath = "";
                    //detect whether its a directory or file
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        selectedPath = new DirectoryInfo(FileName).Name;
                    else
                        selectedPath = new FileInfo(FileName).Name;
                    try
                    {
                        File.Move(selectedPath, Data + $"\\{Dir}\\" + selectedPath);
                    }
                    catch (Exception e) { MessageBox.Show(e.ToString()); }
                }
            }

        }
    }
}