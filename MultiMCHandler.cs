using System;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;

namespace AsguhoClientInstaller {
    public class MutiMCHandler {

        public MutiMCHandler() {
            downloadMultiMC();
            createMultiMCConfig();
        }

        private static void downloadMultiMC() {
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
                FolderUtil.createIfNone(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho");
                FolderUtil.CopyDirectory(_myTempDir + "\\_multiMC\\MultiMC", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\", true);
            }
        }
        private static void createMultiMCConfig() {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg")) {
                var _webClient = new WebClient();
                string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg";
                _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/MultiMCInstance/multimc.cfg", configPath);
                string[] configString = File.ReadAllLines(configPath);
                int _ram = getRam();
                for (int i = 0; i < configString.Length; i++) {
                    configString[i] = configString[i].Replace("[MAXRAM]", _ram.ToString());
                    configString[i] = configString[i].Replace("[MINRAM]", (Math.Floor(_ram * 0.20)).ToString());
                }
                File.WriteAllLines(configPath, configString);
            }
        }
        private static int getRam() {
            ManagementObjectSearcher Search = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
            foreach (ManagementObject Mobject in Search.Get()) {
                double Ram_Bytes = (Convert.ToDouble(Mobject["TotalPhysicalMemory"]));
                int ram_MegaBytes = Convert.ToInt32(Math.Floor(Ram_Bytes / 1048576));
                Console.WriteLine("RAM Size in Mega Bytes: {0}", ram_MegaBytes);
                return ram_MegaBytes;
            }
            return 8000;
        }

    }
}

