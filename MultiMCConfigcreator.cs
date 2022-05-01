using System;
using System.IO;
using System.Management;
using System.Net;

namespace Asguho.ConfigGenerator {
    public class MultiMCConfigcreator {
        public static int getRam() {
            ManagementObjectSearcher Search = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
            foreach (ManagementObject Mobject in Search.Get()) {
                double Ram_Bytes = (Convert.ToDouble(Mobject["TotalPhysicalMemory"]));
                int ram_MegaBytes = Convert.ToInt32(Math.Floor(Ram_Bytes / 1048576));
                Console.WriteLine("RAM Size in Mega Bytes: {0}", ram_MegaBytes);
                return ram_MegaBytes;
            }
            return 8000;
        }
        public static void createMultiMCConfig() {
            var _webClient = new WebClient();
            string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg";
            _webClient.DownloadFile("https://www.asguho.dk/minecraft/client/MultiMCInstance/multimc.cfg", configPath);
            string[] configString = File.ReadAllLines(configPath);
            for (int i = 0; i < configString.Length; i++) {
                configString[i]=configString[i].Replace("[MAXRAM]", getRam().ToString());
                configString[i]=configString[i].Replace("[MINRAM]", (Math.Floor(getRam() * 0.20)).ToString());
            }
            File.WriteAllLines(configPath, configString);
        }
    }}

