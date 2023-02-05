using EtraExtensions;
using Pastel;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using MagmaMc.JEF;

namespace MonoBleedingCube
{
    public static class Utils
    {

        public static void ProgressBar(ProgressContext progressContext, string[] LoopList, string Text)
        {

            ProgressTask ScanFiles = progressContext.AddTask($"[orange1]{Text}[/]");
            foreach (string Item in LoopList)
            {
                string item = Item.Replace(".exe", "").Replace("./", "");
                Thread.Sleep(100);
                //string FileName = new FileInfo(Item).Name.Replace(new FileInfo(Item).Extension, "");
                ScanFiles.Increment(Math.Ceiling(100.0 / LoopList.Length));
            }
            ScanFiles.Value = 100;
        }

        public static class FileDialog
        {
            public static string GetImage(string ImageFileName)
            {

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = JEF.Utils.ApplicationName;
                dialog.Filter = $"{ImageFileName} (*.png, *.jpg, *.jpeg, *.ico) | *.png;*.jpg;*.jpeg;*.ico";
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                if (DialogResult.OK == dialog.ShowDialog())
                    return (dialog.FileName);
                else 
                    return "";
            }
            public static Bitmap GetBitmap(string ImageFilesName)
            {
                string FileName = GetImage(ImageFilesName);
                if (FileName != "")
                    return new Bitmap(FileName);
                else
                    return null;
            }

            public static Icon GetIcon(string ImageFilesName) => Icon.FromHandle(GetBitmap(ImageFilesName).GetHicon());
    }

        public static List<string> DirectoryConvertList(List<string> SourceList, string Dir)
        {
            List<string> OutputList = new List<string>();
            foreach (string FileName in Directory.GetFiles(Dir))
            {
                if (!SourceList.Contains(FileName))
                    OutputList.Add(FileName);
            }
            foreach (string Foders in Directory.GetDirectories(Dir))
            {
                if (!SourceList.Contains(Foders))
                    OutputList.Add(Foders);
            }
            return OutputList;
        }
        public static List<string> CopyDirectory(string sourceDir, string destinationDir, bool recursive, string[] Excluted = null)
        {
            List<string> Copy_List = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException("Source directory not found: " + directoryInfo.FullName);

            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            if (!new DirectoryInfo(destinationDir).Exists)
            {
                Directory.CreateDirectory(destinationDir);
            }
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                if ((Excluted ?? new string[0]).ToList().Contains(fileInfo.Name))
                    continue;
                string destFileName = Path.Combine(destinationDir, fileInfo.Name);
                fileInfo.CopyTo(destFileName, overwrite: true);
                Copy_List.Add(fileInfo.Name);
            }
            if (recursive)
            {
                DirectoryInfo[] array = directories;
                foreach (DirectoryInfo directoryInfo2 in array)
                {
                    string destinationDir2 = Path.Combine(destinationDir, directoryInfo2.Name);
                    CopyDirectory(directoryInfo2.FullName, destinationDir2, recursive: true, Excluted);
                }
            }
            return Copy_List;
        }
        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        public static void DrawIn(string Base) => _DrawIn(Base);

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        /// <param name="CharDelay">In Milliseconds How Long To Wait To Render A Character</param>
        public static void DrawIn(string Base, byte CharDelay) => _DrawIn(Base, CharDelay);

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        /// <param name="CharDelay">In Milliseconds How Long To Wait To Render A Character</param>
        /// <param name="HideCursor">Hides The Cursor When Rendering</param>
        public static void DrawIn(string Base, byte CharDelay, bool HideCursor) => _DrawIn(Base, CharDelay, HideCursor);


        private static void _DrawIn(string Text, byte CharDelay = 8, bool HideCursor = true)
        {
            bool Visible = Console.CursorVisible;
            Console.CursorVisible = false.If(HideCursor) ?? Visible;
            string[] SplitText = Text.Split(' ');
            foreach (string Sections in SplitText)
            {
                foreach (char Character in Sections)
                {
                    Thread.Sleep(CharDelay);
                    Console.Write(Character);
                }

                Console.Write(" ");
            }
            Console.WriteLine();
            Console.CursorVisible = true.If(HideCursor) ?? Visible;
        }
        public static string CenterText(string Text)
        {
            return String.Format("{0," + ((Console.WindowWidth / 2) + (Text.Length / 2)) + "}", Text);
        }
        public static string Multiply(string x, int y)
        {
            string returned = x;
            for (int i = 0; i < y; i++)
                returned += x;
            return returned;
        }
    }
    public static class Folder
    {
        public static void DeleteDir(DirectoryInfo dir, string FolderPath = "")
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                File.Delete(FolderPath + fileInfo.Name);
                //Debug(FolderPath + fileInfo.Name, "blue", "DeleteDir");
                Thread.Sleep(10);
            }
            DirectoryInfo[] directories = dir.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directories)
            {
                Directory.Delete(FolderPath + directoryInfo.Name, recursive: true);
                //Debug("\t" + FolderPath + directoryInfo.Name, "blue", "DeleteDir");
                Thread.Sleep(10);
            }
        }
    }
    public class Debugger
    {
        private bool Logging = true;

        /// <summary>
        /// Enables Or Disables Logging
        /// </summary>
        public void Enabled(bool @set) => Logging = @set;

        /// <summary>
        /// Gets Logging value
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public bool Enabled() => Logging;


        private class Colors
        {
            public static string Log = "#a9cbe8";
            public static string Warning = "#f0ad69";
            public static string Error = "#e64f3c";
            
            public static string Sender = "#345bf7";

        }
        public void Log(string Message, string Sender)
        {
            if (!Logging) return;

            Console.Write(Sender.Pastel(Colors.Sender) + " - ");
            Console.Write(Message.Pastel(Colors.Log));
        }
        public void Warning(string Message, string Sender)
        {
            if (!Logging) return;

            Console.Write(Sender.ToString().Pastel(Colors.Sender) + " - ");
            Console.Write(Message.Pastel(Colors.Warning));
        }
        public void Error(string Message, string Sender)
        {
            if (!Logging) return;

            Console.Write(Sender.ToString().Pastel(Colors.Sender) + " - ");
            Console.Write(Message.Pastel(Colors.Error));
        }

    }
}
