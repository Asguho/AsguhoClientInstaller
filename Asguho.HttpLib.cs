using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Asguho.HttpLib {
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
            //Dir
            //MatchCollection matchesDir = regexDir.Matches(html);
            //if (matchesDir.Count != 0) {
            //    foreach (Match match in matchesDir) {
            //        if (match.Success) {
            //            if (match.Groups["dir"].ToString() != "/minecraft/") {
            //                Console.WriteLine("added dir: " + match.Groups["dir"].ToString());
            //                var dirInfo = new PathInfo(baseUrl + match.Groups["dir"], true);
            //                //GetAllFilePathAndSubDirectory(dirInfo.AbsoluteUrlStr, dirInfo.Childs);
            //                pathInfos.Add(dirInfo);
            //            }
            //        }
            //    }
            //}
        }


        public static void PrintAllPathInfo(List<PathInfo> pathInfos) {
            pathInfos.ForEach(f => {
                Console.WriteLine($"Url: {f.AbsoluteUrlStr}\tDate: {f.Date}");
                PrintAllPathInfo(f.Childs);
            });
        }

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

        public string AbsoluteUrlStr => AbsoluteUrl.ToString();

        public string RootUrl => AbsoluteUrl.GetLeftPart(UriPartial.Authority);

        public string RelativeUrl => AbsoluteUrl.PathAndQuery;

        public string Query => AbsoluteUrl.Query;

        public bool IsDir { get; set; }
        public List<PathInfo> Childs { get; set; }


        public override string ToString() {
            return String.Format("{0} IsDir {1} ChildCount {2} AbsUrl {3}", RelativeUrl, IsDir, Childs.Count, AbsoluteUrlStr);
        }
    }
}
