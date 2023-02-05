using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtraExtensions
{

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
    internal static class Extensions
    {

        // Exenstions
        public static string CenterText(this string Text)
        {
            return String.Format("{0," + ((Console.WindowWidth / 2) + (Text.Length / 2)) + "}", Text);
        }
        public static string Multiply(this string x, int y)
        {
            StringBuilder returned = new StringBuilder(x);
            for (int i = 0; i < y; i++)
                returned.Append(x);
            return returned.ToString();
        }

        public static bool? If(this bool Base, bool IsTrue)
        {
            if (IsTrue)
                return Base;
            else
                return null;
        }

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        public static void DrawIn(this string Base) => _DrawIn(Base);

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        /// <param name="CharDelay">In Milliseconds How Long To Wait To Render A Character</param>
        public static void DrawIn(this string Base, byte CharDelay) => _DrawIn(Base, CharDelay);

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        /// <param name="CharDelay">In Milliseconds How Long To Wait To Render A Character</param>
        /// <param name="HideCursor">Hides The Cursor When Rendering</param>
        public static void DrawIn(this string Base, byte CharDelay, bool HideCursor) => _DrawIn(Base, CharDelay, HideCursor);

        /// <summary>
        /// Draws In Text With A Delay
        /// </summary>
        /// <param name="Base">Base String</param>
        /// <param name="CharDelay">In Milliseconds How Long To Wait To Render A Character</param>
        /// <param name="HideCursor">Hides The Cursor When Rendering</param>
        public static void DrawIn(this string Base, byte CharDelay, bool HideCursor, bool newline) => _DrawIn(Base, CharDelay, HideCursor, newline);


        private static void _DrawIn(string Text, byte CharDelay = 0, bool HideCursor = true, bool newline = true)
        {
            bool Visible = Console.CursorVisible;
            Console.CursorVisible = false.If(HideCursor) ?? Visible;
            string[] SplitText = Text.Split(' ');
            foreach (string Sections in SplitText)
            {
                foreach (char Character in Sections)
                {
                    Thread.Sleep(CharDelay);
                    Console.Write(Character.ToString());
                }
                    Console.Write(" ");
            }
            if (newline)
                Console.WriteLine();
            Console.CursorVisible = true.If(HideCursor) ?? Visible;
        }

    }
}