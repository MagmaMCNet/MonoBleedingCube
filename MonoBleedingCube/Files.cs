using MagmaMc.MagmaSimpleConfig;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MonoBleedingCube
{
    class Files
    {
        public MagmaSimpleConfig Config { get; private set; }
        public string Dir { get; private set; }

        public bool Usable { get; private set; } = false;

        public Files(MagmaSimpleConfig config)
        {
            Config = config;
            Dir = (string)Config.GetValue("AppName", Section: "MonoBleedingCube") + "_Data\\MonoBleedingCube-Data\\";
        }

        public void Setup()
        {
            try
            {
                Usable = true;
                DirectoryInfo Game = new DirectoryInfo((string)Config.GetValue("AppName", Section: "MonoBleedingCube") + "_Data");
                DirectoryInfo MBC_Data = Game.CreateSubdirectory("MonoBleedingCube-Data");
                MBC_Data.CreateSubdirectory("Copy");
                MBC_Data.CreateSubdirectory("ZIP");
                MBC_Data.CreateSubdirectory("Overwrite");
            }
            catch { }
        }


        public void FilesToGame(MagmaSimpleConfig Config)
        {
            if (!Usable) return;

            Utils.CopyDirectory(Dir + "Copy\\", "./", true);
            Utils.CopyDirectory(Dir + "Overwrite\\", "./", true);

            try
            {
                Directory.CreateDirectory(Dir + "ZIP\\Temp\\");
            }
            catch { }
            foreach (FileInfo FileName in new DirectoryInfo(Dir+"ZIP\\").GetFiles())
            {
                try
                {
                    ZipFile.ExtractToDirectory(FileName.FullName, Dir + "ZIP\\Temp\\");
                } catch { }
                Utils.CopyDirectory(Dir + "ZIP\\Temp\\", "./", true);
            }
        }
        public void FilesToStore(string Destination)
        {
            if (!Usable) return;

            Utils.CopyDirectory(Dir + "ZIP\\Temp\\", "./", true);
        }
    }
}
