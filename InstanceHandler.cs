using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AsguhoClientInstaller {
    public class InstanceHandler {

        public InstanceHandler(string instanceName) {
            this.InstanceName = instanceName;
            createInstance();
            ShortcutHelper.CreateShortcut(InstanceName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.asguho\MultiMC\MultiMC.exe", "-l " + InstanceName, "launcher for " + InstanceName);
        }

        private string InstanceName { get; }


        private void createInstance() {
            string _instanceFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\" + InstanceName + "\\";
            FolderUtil.createIfNone(_instanceFolder);
            FolderUtil.createIfNone(_instanceFolder + ".minecraft");
            var _webClient = new WebClient();

            //download files from server
            List<DownloadableFile> filesToDownload= HttpHelper.GetDownloadableFiles(InstanceName);

            foreach (var fileToDownload in filesToDownload) {
                _webClient.DownloadFile(fileToDownload.fileUrl, fileToDownload.filePath);

                if (fileToDownload.filePath == _instanceFolder+"\\mods.txt") {
                    downloadMods();
                }
            }
        }

        private void downloadMods() {
            WebClient _webClient = new WebClient();
            string[] modUrls = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\" + InstanceName + "\\" + "mods.txt");
            Regex regexFile = new Regex("https://modrinth.com/mod/.(<file>)./", RegexOptions.IgnoreCase);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\");
            foreach (string modUrl in modUrls) {
                _webClient.DownloadFile(modUrl, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\instances\\testclient\\.minecraft\\mods\\" + modUrl.Replace("https://cdn.modrinth.com/data/", "").Replace("/version/", "-").Replace("/", "-"));
            }
        }
    }   
}
