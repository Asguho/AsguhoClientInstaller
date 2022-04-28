using System;
using System.IO;
using System.Management;

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
            string[] lines ={
            "Analytics=false",
            "AnalyticsClientID=",
            "AnalyticsSeen=",
            "JavaPath=javaw",
            "Language=en_US",
            "LastHostname=",
            "MainWindowGeometry=",
            "MainWindowState=",
            "MaxMemAlloc=1024",
            "MinMemAlloc=512",
            "ShownNotifications="
            };
            string[] lines2 ={
                "Analytics=false", "AnalyticsClientID=", "AutoCloseConsole=", "AutoUpdate=", "CentralModsDir=", "ConsoleFont=", "ConsoleFontSize=", "ConsoleMaxLines=", "ConsoleOverflowStop=", "IconTheme=", "IconsDir=", "InstSortMode=", "InstanceDir=instances", "JProfilerPath=", "JVisualVMPath=", "JavaPath=javaw", "JsonEditor=", "JvmArgs=", "Language=en_US", "LastHostname=", "LaunchMaximized=", "MCEditPath=", "MainWindowGeometry=", "MainWindowState=", "MaxMemAlloc=1024", "MinMemAlloc=512", "MinecraftWinHeight=", "MinecraftWinWidth=", "PagedGeometry=", "PasteEEAPIKey=", "PermGen=18", "PostExitCommand=", "PreLaunchCommand=", "ProxyAddr=", "ProxyPass=", "ProxyPort=", "ProxyType=", "ProxyUser=", "RecordGameTime=", "ShowConsole=", "ShowConsoleOnError=", "ShowGameTime=", "ShowGlobalGameTime=", "ShownNotifications=", "UpdateChannel=", "UseNativeGLFW=", "UseNativeOpenAL=", "WrapperCommand="            
            };
            //lines[9] = "MaxMemAlloc=" + getRam().ToString();
            //lines[10] = "MinMemAlloc=" + (Math.Floor(getRam() * 0.20)).ToString();
            lines2[25] = "MaxMemAlloc=" + getRam().ToString();
            lines2[26] = "MinMemAlloc=" + (Math.Floor(getRam() * 0.20)).ToString();
            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg", lines2);
        }
    }
}

