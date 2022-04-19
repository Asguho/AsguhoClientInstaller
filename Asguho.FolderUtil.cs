using System;
using System.IO;
namespace Asguho.FolderUtil {
    public class FolderUtil {
        public static void createIfNone(string folderPath) {
            if (!Directory.Exists(folderPath)) {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            else {
                Console.WriteLine("the folder: " + folderPath + " already exits");
            }
        }
        public static void copyAllDirectorysFromFolder(string sourcePath, string destPath) {
            Console.WriteLine(sourcePath);
            Console.WriteLine(Directory.GetDirectories(sourcePath));
            foreach (string _targetDirectoriy in Directory.GetDirectories(sourcePath)) {
                Console.WriteLine(_targetDirectoriy);
                if (!File.Exists(destPath + _targetDirectoriy.ToString().Replace(sourcePath, ""))) {
                    CopyDirectory(_targetDirectoriy, destPath + _targetDirectoriy.ToString().Replace(sourcePath, ""), true);
                }
            }
        }
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive) {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);
            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();
            // Create the destination directory
            Directory.CreateDirectory(destinationDir);
            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            // If recursive and copying subdirectories, recursively call this method
            if (recursive) {
                foreach (DirectoryInfo subDir in dirs) {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        public static void copyAllFilesFromFolder(string sourcePath, string destPath) {
            foreach (string _file in Directory.GetFiles(sourcePath)) {
                if (!File.Exists(destPath + _file.ToString().Replace(sourcePath, ""))) {
                    File.Copy(_file, destPath + _file.ToString().Replace(sourcePath, ""));
                }
            }
        }
    }
}
