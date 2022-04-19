using Asguho.ConfigGenerator;
using Asguho.FolderUtil;
using Asguho.HttpLib;
using Asguho.ShortcutHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices.ComTypes;

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
            var _webClient = new WebClient();
            //copy from instanceTemplate to the new instance
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/InstanceTemplate/instance.cfg", _instanceFolder+"instance.cfg");
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/InstanceTemplate/mmc-pack.json", _instanceFolder+"mmc-pack.json");
            //FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\instanceTemplate\\", _instanceFolder);
        }
        static void downloadMultiMC() {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\")) {
                var _webClient = new WebClient();

                string _myTempDir = System.IO.Directory.GetCurrentDirectory() + "\\temp\\";
                FolderUtil.createIfNone(_myTempDir);

                if (!File.Exists(_myTempDir + "\\_multiMC.zip")) {
                        _webClient.DownloadFile("https://files.multimc.org/downloads/mmc-stable-win32.zip", _myTempDir + "\\_multiMC.zip");
                }

                if (!Directory.Exists(_myTempDir + "\\_multiMC\\")) {
                    ZipFile.ExtractToDirectory(_myTempDir + "\\_multiMC.zip", _myTempDir + "\\_multiMC\\");
                }
                FolderUtil.CopyDirectory(_myTempDir + "\\_multiMC\\MultiMC", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\", true);

                //copy from MultiMCTemplate to the new MuiltiMC folder
                MultiMCConfigcreator.createMultiMCConfig();
                _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/MultiMCInstance/multimc.cfg", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg");
                //FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
                //FolderUtil.copyAllDirectorysFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
            }
        }
        static void downloadMods() {
            var _webClient = new WebClient();
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/1.18.2/mods.txt", Directory.GetCurrentDirectory() + "\\temp\\mods.txt");
            string[] modUrls = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\temp\\mods.txt");
            foreach (string modUrl in modUrls) {
                _webClient.DownloadFile(modUrl, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\mods\\"+modUrl +".jar");
            }
        }
    }
}