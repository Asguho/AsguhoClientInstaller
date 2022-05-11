﻿using Asguho.ConfigGenerator;
using Asguho.FolderUtil;
using Asguho.HttpLib;
using Asguho.ShortcutHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace AsguhoClientInstaller {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine("Setting up AsguhoClient!");

            List<PathInfo> pathInfos = new List<PathInfo>();
            HttpHelper.GetAllFilePathAndSubDirectory("https://www.asguho.dk/minecraft/client/", pathInfos);
            HttpHelper.PrintAllPathInfo(pathInfos);

            //setup();


            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void setup() {
            FolderUtil.createIfNone(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho");
            downloadMultiMC();
            createInstance("testclient");
            downloadMods();
            createShortcut();
            FolderUtil.deleteTempFolder();
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
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/InstanceTemplate/instance.cfg", _instanceFolder + "instance.cfg");
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/InstanceTemplate/mmc-pack.json", _instanceFolder + "mmc-pack.json");
            //FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\instanceTemplate\\", _instanceFolder);
        }
        static void downloadMultiMC() {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\MultiMC.exe")) {
                var _webClient = new WebClient();

                string _myTempDir = FolderUtil.getTempFolder();
                FolderUtil.createIfNone(_myTempDir);

                if (!File.Exists(_myTempDir + "\\_multiMC.zip")) {
                    _webClient.DownloadFile("https://files.multimc.org/downloads/mmc-stable-win32.zip", _myTempDir + "\\_multiMC.zip");
                }

                if (!Directory.Exists(_myTempDir + "\\_multiMC\\")) {
                    ZipFile.ExtractToDirectory(_myTempDir + "\\_multiMC.zip", _myTempDir + "\\_multiMC\\");
                }
                FolderUtil.CopyDirectory(_myTempDir + "\\_multiMC\\MultiMC", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\", true);
            }
            //copy from MultiMCTemplate to the new MuiltiMC folder
            MultiMCConfigcreator.createMultiMCConfig();
            //_webClient.DownloadFile("https://www.asguho.dk/minecraft/client/MultiMCInstance/multimc.cfg", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg");
            //FolderUtil.copyAllFilesFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
            //FolderUtil.copyAllDirectorysFromFolder(Directory.GetCurrentDirectory() + "\\MultiMCTemplate", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\");
        }
        //static void downloadMods() {
        //    var _webClient = new WebClient();
        //    _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/1.18.2/mods.txt", FolderUtil.getTempFolder()+"mods.txt");
        //    string[] modUrls = File.ReadAllLines(FolderUtil.getTempFolder()+"mods.txt");
        //    Regex regexFile = new Regex("https://modrinth.com/mod/.(<file>)./", RegexOptions.IgnoreCase);
        //    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\");
        //    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\");
        //    foreach (string modUrl in modUrls) {
        //        Console.WriteLine(useRegex(modUrl)+" "+modUrl);
        //        _webClient.DownloadFile(modUrl, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\"+modUrl.Replace("https://cdn.modrinth.com/data/", "").Replace("/version/","-").Replace("/","-"));
        //    }
        //}
        static void downloadMods() {
            var _webClient = new WebClient();
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/1.18.2/mods.txt", FolderUtil.getTempFolder() + "mods.txt");
            string[] modUrls = File.ReadAllLines(FolderUtil.getTempFolder() + "mods.txt");
            Regex regexFile = new Regex("https://modrinth.com/mod/.(<file>)./", RegexOptions.IgnoreCase);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\");
            foreach (string modUrl in modUrls) {
                downloadFileAsync(modUrl, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\" + modUrl.Replace("https://cdn.modrinth.com/data/", "").Replace("/version/", "-").Replace("/", "-"));
            }
        }
        static void downloadFileAsync(string url, string filePath) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetResponse(new AsyncCallback(delegate (IAsyncResult ar) {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
                using (Stream stream = response.GetResponseStream()) {
                    Console.WriteLine("downloading from: " + url);
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                        stream.CopyTo(fileStream);
                    }
                }
            }), null);
        }
        public static bool useRegex(String input) {
            Regex regex = new Regex("https://cdn\\.modrinth\\.com/data/[a-zA-Z]+/versions/[a-zA-Z]+([0-9]+(\\.[0-9]+)+)-(\\d(\\.\\d)+)/([a-zA-Z]+(-[a-zA-Z]+)+)([0-9]+(\\.[0-9]+)+)-(\\d(\\.\\d)+)\\.jar", RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }

    }
}