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
            
            MutiMCHandler multiMCHandler = new MutiMCHandler();
            InstanceHandler instanceHandler = new InstanceHandler("AsguhoClient");

            FolderUtil.deleteTempFolder();
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        //static void downloadFileAsync(string url, string filePath) {
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "GET";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.BeginGetResponse(new AsyncCallback(delegate (IAsyncResult ar) {
        //        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
        //        using (Stream stream = response.GetResponseStream()) {
        //            Console.WriteLine("downloading from: " + url);
        //            using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
        //                stream.CopyTo(fileStream);
        //            }
        //        }
        //    }), null);
        //}

    }
}