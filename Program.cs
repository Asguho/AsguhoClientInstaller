using Asguho.ConfigGenerator;
using Asguho.FolderUtil;
using Asguho.ShortcutHelper;
using Asguho.HttpLib;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

namespace AsguhoClientInstaller {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine("Setting up AsguhoClient!");

            List<PathInfo> pathInfos = new List<PathInfo>();
            HttpHelper.GetAllFilePathAndSubDirectory("https://www.asguho.dk/minecraft/client/", pathInfos);
            HttpHelper.PrintAllPathInfo(pathInfos);

            setup();


            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void setup() {
            FolderUtil.createIfNone(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho");
            downloadMultiMC();
            createInstance("testclient");
            createShortcut();
        }
        static void createShortcut() {
            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetDescription("My Description");
            link.SetPath(@"C:\Users\aske\AppData\Roaming\.asguho\MultiMC\MultiMC.exe");
            link.SetArguments("-l testclient");

            // save it
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(Path.Combine(desktopPath, "MyLink.lnk"), false);
        }

        static void createInstance(string _instancesName) {
            string _instanceFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\" + _instancesName + "\\";
            FolderUtil.createIfNone(_instanceFolder);

            //copy from instanceTemplate to the new instance
            FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\instanceTemplate\\", _instanceFolder);
        }
        static void downloadMultiMC() {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\")) {
                string _myTempDir = System.IO.Directory.GetCurrentDirectory() + "\\temp\\";
                FolderUtil.createIfNone(_myTempDir);

                if (!File.Exists(_myTempDir + "\\_multiMC.zip")) {
                    using (var _webClient = new WebClient()) {
                        _webClient.DownloadFile("https://files.multimc.org/downloads/mmc-stable-win32.zip", _myTempDir + "\\_multiMC.zip");
                    }
                }

                if (!Directory.Exists(_myTempDir + "\\_multiMC\\")) {
                    ZipFile.ExtractToDirectory(_myTempDir + "\\_multiMC.zip", _myTempDir + "\\_multiMC\\");
                }
                FolderUtil.CopyDirectory(_myTempDir + "\\_multiMC\\MultiMC", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\", true);

                //copy from MultiMCTemplate to the new MuiltiMC folder
                MultiMCConfigcreator.createMultiMCConfig();
                //FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
                //FolderUtil.copyAllDirectorysFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
            }
        }

    }
}