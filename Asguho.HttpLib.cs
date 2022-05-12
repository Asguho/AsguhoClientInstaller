using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace AsguhoClientInstaller {
    public static class HttpHelper {
        public static string ReadHtmlContentFromUrl(string url) {
            string html = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
                html = reader.ReadToEnd();
            }
            //Console.WriteLine(html);
            return html;
        }

        public static void GetAllFilePathAndSubDirectory(string baseUrl, List<PathInfo> pathInfos) {
            Uri baseUri = new Uri(baseUrl.TrimEnd('/'));
            string rootUrl = baseUri.GetLeftPart(UriPartial.Authority);


            Regex regex = new Regex("alt.*?></td><td><a href=\"(http:|https:)?(?<url>.*?)\">(?<name>.*?)</a></td><td align=\"right\">(?<date>.*?)  </td>", RegexOptions.IgnoreCase);

            string html = ReadHtmlContentFromUrl(baseUrl);
            //Files         
            MatchCollection matches = regex.Matches(html);
            if (matches.Count != 0) {
                foreach (Match match in matches) {
                    if (match.Success) {
                        if (match.Groups["url"].Value.StartsWith("/")) {
                            return; //parent directory
                        }
                        if (match.Groups["url"].Value.EndsWith("/")) { //its a directory
                            //Console.WriteLine($"added dir: {match.Groups["url"]}\t filename: {match.Groups["name"]}\t date: {match.Groups["date"]}");
                            var dirInfo = new PathInfo(baseUrl + match.Groups["url"], true, match.Groups["date"].ToString());
                            GetAllFilePathAndSubDirectory(dirInfo.AbsoluteUrlStr, dirInfo.Childs);
                            pathInfos.Add(dirInfo);
                        }
                        else { // its a file
                            //Console.WriteLine($"added file: {match.Groups["url"]}\t filename: {match.Groups["name"]}\t date: {match.Groups["date"]}");
                            pathInfos.Add(new PathInfo(baseUrl + match.Groups["url"], false, match.Groups["date"].ToString()));
                        }
                    }
                }
            }
        }


        public static void PrintAllPathInfo(List<PathInfo> pathInfos) {
            pathInfos.ForEach(f => {
                Console.WriteLine($"Url: {f.AbsoluteUrlStr}\tDate: {f.Date}");
                PrintAllPathInfo(f.Childs);
            });
        }

        public static List<DownloadableFile> GetDownloadableFiles(string instanceName) {
            List<PathInfo> pathInfos = new List<PathInfo>();
            List<DownloadableFile> downloadableFiles = new List<DownloadableFile>();
            GetAllFilePathAndSubDirectory("https://www.asguho.dk/minecraft/client/"+instanceName+"/", pathInfos);
            PrintAllPathInfo(pathInfos);
            downloadableFilesCrawler("https://www.asguho.dk/minecraft/client/" + instanceName + "/", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\.asguho\\MultiMC\\instances\\{instanceName}\\",  downloadableFiles, pathInfos);
            return downloadableFiles;
        }

        private static void downloadableFilesCrawler(string urlpath, string loaclpath, List<DownloadableFile> downloadableFiles, List<PathInfo> pathInfos) {
            foreach (var pathInfo in pathInfos) {
                string localPath = pathInfo.AbsoluteUrlStr
                    .Replace(urlpath, loaclpath)
                    .Replace("_", ".")
                    .Replace("/", "\\");
                DateTime localDateTime = new FileInfo(localPath).LastWriteTime;
                DateTime remoteDateTime = pathInfo.DateTime;

                //Console.WriteLine($"Url: {pathInfo.AbsoluteUrlStr}\tpath: {localPath}");
                foreach (var item in pathInfo.Childs) {
                    Console.WriteLine(item.AbsoluteUrlStr);
                }
                //Console.WriteLine($"local: {localDateTime}\nremote: {remoteDateTime}");
                if (DateTime.Compare(localDateTime, remoteDateTime) < 0) {
                    //Console.WriteLine($"{pathInfo.AbsoluteUrlStr} is newer than {localPath}");
                    if (pathInfo.IsDir) {
                        downloadableFilesCrawler(urlpath, loaclpath, downloadableFiles, pathInfo.Childs);
                    }
                    else {
                        downloadableFiles.Add(new DownloadableFile(localPath, pathInfo.AbsoluteUrlStr));
                    }
                }
            }
        }
    }
    public class DownloadableFile {
        public DownloadableFile(string filePath, string fileurl) {
            this.filePath = filePath;
            this.fileUrl = fileurl;
        }
        public string filePath { get; }
        public string fileUrl { get; }
    }


    public class PathInfo {
        public PathInfo(string absoluteUri, bool isDir, string date) {
            AbsoluteUrl = new Uri(absoluteUri);
            IsDir = isDir;
            Childs = new List<PathInfo>();
            Date = date;
        }

        public Uri AbsoluteUrl { get; set; }

        public string Date { get; }

        public DateTime DateTime => toDateTime();

        public string AbsoluteUrlStr => AbsoluteUrl.ToString();

        public string RootUrl => AbsoluteUrl.GetLeftPart(UriPartial.Authority);

        public string RelativeUrl => AbsoluteUrl.PathAndQuery;

        public string Query => AbsoluteUrl.Query;

        public bool IsDir { get; set; }
        public List<PathInfo> Childs { get; set; }


        public override string ToString() {
            return String.Format("{0} IsDir {1} ChildCount {2} AbsUrl {3}", RelativeUrl, IsDir, Childs.Count, AbsoluteUrlStr);
        }
        private DateTime toDateTime() {
            //Console.WriteLine(Date);
            Regex regex = new Regex("(?<year>[0-9]+)-(?<month>[0-9]+)-(?<day>[0-9]+) (?<hour>[0-9]+):(?<minute>[0-9]+)", RegexOptions.IgnoreCase);
            Match matche = regex.Match(Date);

            if (matche.Success) {
                //convert string to int
                int year = int.Parse(matche.Groups["year"].ToString());
                int month = int.Parse(matche.Groups["month"].ToString());
                int day = int.Parse(matche.Groups["day"].ToString());
                int hour = int.Parse(matche.Groups["hour"].ToString());
                int minute = int.Parse(matche.Groups["minute"].ToString());

                //Console.WriteLine($"{year}-{month}-{day} {hour}:{minute}");

                DateTime _dateTime = new DateTime(year, month, day, hour, minute, 0);
                return _dateTime;
            }
            return DateTime.MinValue;
        }
    }
}
