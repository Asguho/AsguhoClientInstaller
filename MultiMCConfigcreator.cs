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
            string[] lines2 = {"Analytics=false", "AnalyticsClientID=", "AutoCloseConsole=", "AutoUpdate=", "CentralModsDir=", "ConsoleFont=", "ConsoleFontSize=", "ConsoleMaxLines=", "ConsoleOverflowStop=", "IconTheme=", "IconsDir=", "InstSortMode=", "InstanceDir=instances", "JProfilerPath=", "JVisualVMPath=", "JavaArchitecture=", "JavaPath=javaw", "JavaTimestamp=", "JavaVersion=", "JsonEditor=", "JvmArgs=", "Language=en_US", "LastHostname=AskeDesktop", "LaunchMaximized=true", "MCEditPath=", "MainWindowGeometry=", "MainWindowState=", "MaxMemAlloc=", "MinMemAlloc=", "MinecraftWinHeight=", "MinecraftWinWidth=", "PagedGeometry=", "PasteEEAPIKey=", "PermGen=", "PostExitCommand=", "PreLaunchCommand=", "ProxyAddr=", "ProxyPass=", "ProxyPort=", "ProxyType=", "ProxyUser=", "RecordGameTime=", "ShowConsole=false", "ShowConsoleOnError=", "ShowGameTime=", "ShowGlobalGameTime=", "ShownNotifications=", "UpdateChannel=", "UseNativeGLFW=", "UseNativeOpenAL=", "WrapperCommand="
             };
            lines2[27] = "MaxMemAlloc=" + getRam().ToString();
            lines2[28] = "MinMemAlloc=" + (Math.Floor(getRam() * 0.20)).ToString();
            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.asguho\\MultiMC\\multimc.cfg", lines2);
        }
    }}

