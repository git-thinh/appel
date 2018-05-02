using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using ProtoBuf;
using System.Net;
using HtmlAgilityPack;
using System.Net.Sockets;
using System.Web;
using System.Threading;

namespace appel
{
    public class _API
    {
        public const string SETTING_APP = "SETTING_APP";
        public const string SETTING_APP_KEY_UPDATE_FOLDER = "SETTING_APP_KEY_UPDATE_FOLDER";
        public const string SETTING_APP_KEY_UPDATE_SIZE = "SETTING_APP_KEY_UPDATE_SIZE";
        public const string SETTING_APP_KEY_UPDATE_NODE_OPENING = "SETTING_APP_KEY_UPDATE_NODE_OPENING";

        public const string CRAWLER = "CRAWLER";
        public const string CRAWLER_KEY_REGISTER_PATH = "CRAWLER_KEY_REGISTER_PATH";
        public const string CRAWLER_KEY_REQUEST_LINK = "CRAWLER_KEY_REQUEST_LINK";
        public const string CRAWLER_KEY_CONVERT_HTML_TO_TEXT = "CRAWLER_KEY_CONVERT_HTML_TO_TEXT";
        public const string CRAWLER_KEY_CONVERT_PACKAGE_TO_TEXT = "CRAWLER_KEY_CONVERT_PACKAGE_TO_TEXT";
        public const string CRAWLER_KEY_COMPLETE = "CRAWLER_KEY_COMPLETE";

        public const string CONTENT = "CONTENT";
        public const string CONTENT_KEY_ANALYTIC = "CONTENT_KEY_ANALYTIC";
        public const string CONTENT_KEY_EDIT_ROLE = "CONTENT_KEY_EDIT_ROLE";


        public const string FOLDER_ANYLCTIC = "FOLDER_ANYLCTIC";


        public const string TRANSLATER = "TRANSLATOR";
        public const string WORD_LOAD_LOCAL = "WORD_LOAD_LOCAL";
        public const string WORD_DOWNLOAD = "WORD_DOWNLOAD";
    }

    public interface IMAIN
    {
        void api_responseMsg(msg m);
    }

    public interface IAPI
    {
        msg Execute(msg msg);
        void Close();
    }

    public class api_base
    {
        static readonly object _lock = new object();
        static Queue<msg> cache = new Queue<msg>();
        static System.Threading.Timer timer = null;
        static IMAIN main = null;

        public api_base()
        {
            if (main == null)
                main = app.get_Main();
            if (timer == null)
            {
                timer = new System.Threading.Timer(new System.Threading.TimerCallback((obj) =>
                {
                    lock (_lock)
                    {
                        if (cache.Count > 0)
                        {
                            msg m = cache.Dequeue();
                            if (main != null) main.api_responseMsg(m);
                        }
                    }
                }), main, 100, 100);
            }
        }

        public void f_responseToMain(msg m)
        {
            lock (_lock) cache.Enqueue(m);
        }
    }

    public class api_fetch : api_base, IAPI
    {
        static readonly object _lock = new object();
        static Dictionary<string, List<string>> dicUrl = new Dictionary<string, List<string>>();

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;


            //HttpWebRequest w = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //w.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
            //w.BeginGetResponse(asyncResult =>
            //{
            //    HttpWebResponse rs = (HttpWebResponse)w.EndGetResponse(asyncResult); //add a break point here 
            //    StreamReader sr = new StreamReader(rs.GetResponseStream(), System.Text.Encoding.UTF8);
            //    s = sr.ReadToEnd();
            //    sr.Close();
            //    rs.Close();
            //    s = HttpUtility.HtmlDecode(s);
            //}, w);

            m.Output.Ok = true;
            m.Output.Data = null;
            return m;
        }

        public void Close()
        {
        }
    }

    public class api_crawler : api_base, IAPI
    {
        static readonly object lockHtml = new object();
        static Dictionary<string, string> dicHtml = new Dictionary<string, string>();

        static readonly object lockUri = new object();
        static List<oLink> listUri = new List<oLink>();

        static int crawlCounter = 0;

        static readonly object lockContent = new object();
        static Dictionary<string, string> dicContent = new Dictionary<string, string>();
        static int contentCounter = 0;

        public api_crawler()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                | (SecurityProtocolType)3072
                | (SecurityProtocolType)0x00000C00
                | SecurityProtocolType.Tls;
        }

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string url_input = string.Empty;
            HttpWebRequest w;
            HtmlDocument doc = new HtmlDocument();
            switch (m.KEY)
            {
                case _API.CRAWLER_KEY_REGISTER_PATH:
                    #region
                    url_input = (string)m.Input;
                    url_input = url_input.ToLower();
                    m.Log = "CRAWLER_KEY_REGISTER_PATH: " + url_input;

                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 
                    //    | (SecurityProtocolType)3072 
                    //    | (SecurityProtocolType)0x00000C00 
                    //    | SecurityProtocolType.Tls;

                    w = (HttpWebRequest)WebRequest.Create(new Uri(url_input));
                    w.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
                    w.BeginGetResponse(asyncResult =>
                    {
                        HttpWebResponse rs = (HttpWebResponse)w.EndGetResponse(asyncResult); //add a break point here 
                        StreamReader sr = new StreamReader(rs.GetResponseStream(), Encoding.UTF8);
                        string htm = sr.ReadToEnd();
                        sr.Close();
                        rs.Close();

                        if (!string.IsNullOrEmpty(htm))
                        {
                            htm = HttpUtility.HtmlDecode(htm);

                            string url = rs.ResponseUri.ToString();
                            string[] url_crawled;
                            lock (lockHtml)
                            {
                                if (dicHtml.ContainsKey(url) == false)
                                {
                                    string htm_format = format_HTML(htm);
                                    dicHtml.Add(url, htm_format); 
                                }
                                url_crawled = dicHtml.Keys.ToArray();
                            }

                            doc.LoadHtml(htm);

                            lock (lockUri)
                            {
                                var url_new = doc.DocumentNode
                                    .SelectNodes("//a")
                                    .Select(p => new oLink()
                                    {
                                        crawled = false,
                                        uri = url,
                                        url = p.GetAttributeValue("href", string.Empty).ToLower(),
                                        text = p.InnerText
                                    })
                                    .Where(x =>
                                            x.url.IndexOf(x.uri) == 0
                                            && !listUri.Any(z => z.url == x.url)
                                            && x.url != url
                                            && x.text != string.Empty)
                                    .GroupBy(x => x.url)
                                    .Select(x => x.First())
                                    .ToArray();

                                listUri.Add(new oLink() { crawled = true, url = url, uri = url, text = string.Empty });
                                if (url_new.Length > 0)
                                {
                                    listUri.AddRange(url_new);
                                    Execute(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_REQUEST_LINK, Input = url_new[0] });
                                }
                            }
                        }
                    }, w);
                    #endregion
                    break;
                case _API.CRAWLER_KEY_REQUEST_LINK:
                    #region
                    Interlocked.Increment(ref crawlCounter);

                    oLink link = (oLink)m.Input;
                    m.Log = "CRAWLER_KEY_REQUEST_LINK: " + link.url;

                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 
                    //    | (SecurityProtocolType)3072 
                    //    | (SecurityProtocolType)0x00000C00 
                    //    | SecurityProtocolType.Tls;

                    w = (HttpWebRequest)WebRequest.Create(new Uri(link.url));
                    w.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
                    w.BeginGetResponse(asyncResult =>
                    {
                        HttpWebResponse rs = (HttpWebResponse)w.EndGetResponse(asyncResult); //add a break point here 
                        StreamReader sr = new StreamReader(rs.GetResponseStream(), Encoding.UTF8);
                        string htm = sr.ReadToEnd();
                        sr.Close();
                        rs.Close();

                        if (!string.IsNullOrEmpty(htm))
                        {
                            htm = HttpUtility.HtmlDecode(htm);

                            string url = rs.ResponseUri.ToString();
                            string[] url_crawled;

                            lock (lockHtml)
                            {
                                if (dicHtml.ContainsKey(url) == false)
                                {
                                    string htm_format = format_HTML(htm);
                                    dicHtml.Add(url, htm_format);
                                    Interlocked.Increment(ref contentCounter);

                                    f_responseToMain(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_COMPLETE, Log = m.KEY + ": " + contentCounter.ToString() + " = " + url });
                                }
                                url_crawled = dicHtml.Keys.ToArray();
                            }

                            /////////////////////////////////////////
                            //f_responseToMain(m);

                            doc.LoadHtml(htm);

                            lock (lockUri)
                            {
                                var url_new = doc.DocumentNode
                                    .SelectNodes("//a")
                                    .Select(p => new oLink()
                                    {
                                        uri = link.uri,
                                        crawled = false,
                                        url = p.GetAttributeValue("href", string.Empty).ToLower(),
                                        text = p.InnerText
                                    })
                                    .Where(x =>
                                            x.url.IndexOf(x.uri) == 0
                                            && !listUri.Any(z => z.url == x.url)
                                            && x.url != url
                                            && x.text != string.Empty)
                                    .GroupBy(x => x.url)
                                    .Select(x => x.First())
                                    .ToArray();


                                //var div_con = doc.DocumentNode 
                                //    .SelectNodes("//article")
                                //    .ToArray();
                                //if (div_con.Length > 0)
                                //{
                                //    string con = div_con[0].InnerHtml;
                                //    lock (lockContent)
                                //    {
                                //        if (dicContent.ContainsKey(url) == false)
                                //            dicContent.Add(url, con);
                                //    }
                                //    Interlocked.Increment(ref contentCounter);

                                //    f_responseToMain(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_REQUEST_LINK, Log = con });
                                //}

                                int index = listUri.IndexOf(link);
                                if (index != -1) listUri[index].crawled = true;

                                if (url_new.Length > 0)
                                    listUri.AddRange(url_new);

                                var li = listUri.Where(x => x.crawled == false).Take(1).SingleOrDefault();
                                if (li == null || contentCounter == 50)
                                {
                                    f_responseToMain(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_COMPLETE, Log = "FINISH CWRALER ..." });
                                    lock (lockHtml)
                                    {
                                        using (var file = File.Create("crawler.bin"))
                                            Serializer.Serialize<Dictionary<string, string>>(file, dicHtml);
                                    }
                                }
                                else
                                {
                                    Execute(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_REQUEST_LINK, Input = li });
                                }
                            }
                        }
                    }, w);
                    #endregion
                    break;
                case _API.CRAWLER_KEY_CONVERT_PACKAGE_TO_TEXT:
                    #region
                    string path_package = (string)m.Input;
                    if (!string.IsNullOrEmpty(path_package) && File.Exists(path_package))
                    {
                        using (var fileStream = File.OpenRead(path_package))
                        {
                            var dic = Serializer.Deserialize<Dictionary<string, string>>(fileStream);
                            foreach (var kv in dic)
                            {
                                string s = kv.Value;

                            }
                        }
                    }
                    #endregion
                    break;
            }

            m.Output.Ok = true;
            m.Output.Data = null;
            return m;
        }

        public void Close()
        {
        }

        public static string getHtml(string url)
        {
            string s = string.Empty;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072 | SecurityProtocolType.Tls;

            //using (WebClient w = new WebClient())
            //{ 
            //    w.Encoding = Encoding.UTF7;
            //    s = w.DownloadString(url);
            //}

            //HttpWebRequest w = (HttpWebRequest)WebRequest.Create(url);
            //w.Method = "GET";
            //w.KeepAlive = false;
            //WebResponse rs = w.GetResponse();
            //StreamReader sr = new StreamReader(rs.GetResponseStream(), System.Text.Encoding.UTF8);
            //s = sr.ReadToEnd();
            //sr.Close();
            //rs.Close();

            //            var uri = new Uri(url);
            //            string req =
            //@"GET " + uri.PathAndQuery + @" HTTP/1.1
            //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36
            //Host: " + uri.Host + @"
            //Accept: */*
            //Accept-Encoding: gzip, deflate
            //Connection: Keep-Alive 
            //";
            //            var requestBytes = Encoding.UTF8.GetBytes(req);
            //            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //            socket.Connect(uri.Host, 80);
            //            if (socket.Connected)
            //            {
            //                socket.Send(requestBytes);
            //                var responseBytes = new byte[socket.ReceiveBufferSize];
            //                socket.Receive(responseBytes);
            //                s = Encoding.UTF8.GetString(responseBytes);
            //            }
            //            s = HttpUtility.HtmlDecode(s);
            //result = CleanHTMLFromScript(result);

            //HttpWebRequest w = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //w.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";
            //w.BeginGetResponse(asyncResult =>
            //{
            //    HttpWebResponse rs = (HttpWebResponse)w.EndGetResponse(asyncResult); //add a break point here 
            //    StreamReader sr = new StreamReader(rs.GetResponseStream(), System.Text.Encoding.UTF8);
            //    s = sr.ReadToEnd();
            //    sr.Close();
            //    rs.Close();
            //    s = HttpUtility.HtmlDecode(s);
            //}, w);

            return s;
        }

        private static string format_HTML(string s)
        {
            string si = string.Empty;
            s = Regex.Replace(s, @"<script[^>]*>[\s\S]*?</script>", string.Empty);
            s = Regex.Replace(s, @"<style[^>]*>[\s\S]*?</style>", string.Empty);
            s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            s = Regex.Replace(s, @"(?s)(?<=<!--).+?(?=-->)", string.Empty).Replace("<!---->", string.Empty);

            //s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            //s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            s = Regex.Replace(s, @"</?(?i:embed|object|frameset|frame|iframe|meta|link)(.|\n|\s)*?>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            string tagName = string.Empty, tagVal = string.Empty;
            foreach (var node in doc.DocumentNode.SelectNodes("//*"))
            {
                if (node.InnerText == null || node.InnerText.Trim().Length == 0)
                {
                    node.Remove();
                    continue;
                }

                tagName = node.Name.ToUpper();
                if (tagName == "A")
                    tagVal = node.GetAttributeValue("href", string.Empty);
                else if (tagName == "IMG")
                    tagVal = node.GetAttributeValue("src", string.Empty);

                node.Attributes.RemoveAll();

                if (tagVal != string.Empty)
                {
                    if (tagName == "A") node.SetAttributeValue("href", tagVal);
                    else if (tagName == "IMG") node.SetAttributeValue("src", tagVal);
                }
            }

            si = doc.DocumentNode.OuterHtml;
            //string[] lines = si.Split(new char[] { '\r', '\n' }, StringSplitOptions.None).Where(x => x.Trim().Length > 0).ToArray();
            string[] lines = si.Split(new char[] { '\r', '\n' }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            si = string.Join(Environment.NewLine, lines);
            return si;
        }
    }

    public class api_settingApp : api_base, IAPI
    {
        const string file_name = "setting.bin";
        static readonly object _lock = new object();
        static oSetting _setting = new oSetting() { };

        public api_settingApp()
        {
            if (File.Exists(file_name))
            {
                lock (_lock)
                {
                    using (var file = File.OpenRead(file_name))
                    {
                        _setting = Serializer.Deserialize<oSetting>(file);
                        _setting.list_folder = new List<string>() { @"E:\data_el2\articles-IT\w2ui" };
                    }
                }
            }
        }

        public static bool get_checkExistFolder(string fol)
        {
            lock (_lock)
                return _setting.list_folder.IndexOf(fol) != -1;
        }

        public static string[] get_listFolder()
        {
            lock (_lock)
                return _setting.list_folder.ToArray();
        }

        public static oNode get_nodeOpening()
        {
            lock (_lock)
                return _setting.node_opening;
        }

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            bool hasUpdate = false;

            switch (m.KEY)
            {
                case _API.SETTING_APP_KEY_UPDATE_FOLDER:
                    #region
                    string fol = (string)m.Input;
                    if (!string.IsNullOrEmpty(fol))
                    {
                        fol = fol.ToLower().Trim();
                        lock (_lock)
                        {
                            if (_setting.list_folder.IndexOf(fol) == -1)
                            {
                                _setting.list_folder.Add(fol);
                                hasUpdate = true;
                                app.postMessageToService(new msg() { API = _API.FOLDER_ANYLCTIC, Input = fol });
                            }
                        }
                    }
                    #endregion
                    break;
                case _API.SETTING_APP_KEY_UPDATE_NODE_OPENING:
                    oNode node = (oNode)m.Input;
                    lock (_lock)
                        _setting.node_opening = node;
                    hasUpdate = true;
                    break;
                case _API.SETTING_APP_KEY_UPDATE_SIZE:
                    oAppSize app_size = (oAppSize)m.Input;
                    lock (_lock)
                    {
                        _setting.app_size = app_size;
                        hasUpdate = true;
                    }
                    break;
            }

            if (hasUpdate)
            {
                using (var file = File.Create(file_name))
                {
                    Serializer.Serialize<oSetting>(file, _setting);
                }
            }
            m.Output.Ok = hasUpdate;
            m.Output.Data = hasUpdate;
            return m;
        }

        public void Close()
        {
            using (var file = File.Create(file_name))
            {
                Serializer.Serialize<oSetting>(file, _setting);
            }
        }
    }

    public class api_Translater : api_base, IAPI
    {
        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string[] words = (string[])m.Input;
            Encoding encoding = Encoding.UTF7;
            string input = string.Join("\r\n", words);
            m.KEY = input;
            m.Output.Ok = false;

            ////string temp = HttpUtility.UrlEncode(input.Replace(" ", "---"));
            //string temp = HttpUtility.UrlEncode(input);
            ////temp = temp.Replace("-----", "%20");

            //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", temp, "en|vi");

            //string s = String.Empty;
            //using (WebClient webClient = new WebClient())
            //{
            //    webClient.Encoding = encoding;
            //    s = webClient.DownloadString(url);
            //}
            //string ht = HttpUtility.HtmlDecode(s);

            //string result = String.Empty;
            //int p = s.IndexOf("id=result_box");
            //if (p > 0)
            //    s = "<span " + s.Substring(p, s.Length - p);
            //p = s.IndexOf("</div>");
            //if (p > 0)
            //{
            //    s = s.Substring(0, p);
            //    s = s.Replace("<br>", "¦");
            //    s = HttpUtility.HtmlDecode(s);
            //    result = Regex.Replace(s, @"<[^>]*>", String.Empty);
            //}
            //if (result != string.Empty)
            //{
            //    string[] rs = result.Split('¦').Select(x => x.Trim()).ToArray();
            //    m.Output = new MsgOutput() { Ok = true, Data = rs, Total = rs.Length };
            //    
            //    lock (lockResponse)
            //    {
            //        if (dicResponses.ContainsKey(m.Key))
            //            dicResponses[m.Key] = m;
            //        else
            //            dicResponses.Add(m.Key, m);
            //    }
            //}
            //else
            //{
            //    m.Output = new MsgOutput() { Data = "Can not translate" };
            //}

            //postMessageToFormUI(m.Key);

            m.Output.Ok = true;
            m.Output.Data = words;
            return m;
        }
        public void Close() { }
    }

    public class api_word_Download : api_base, IAPI
    {
        static readonly object _lock = new object();
        static List<string> listWordDownload = new List<string>() { };

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string[] a = (string[])m.Input;

            m.KEY = string.Empty;
            m.Output.Ok = false;

            ////////https://s3.amazonaws.com/audio.oxforddictionaries.com/en/mp3/you_gb_1.mp3
            ////////https://ssl.gstatic.com/dictionary/static/sounds/oxford/you--_gb_1.mp3


            ////////https://ssl.gstatic.com/dictionary/static/sounds/20160317/hello--_us_1.mp3
            ////////https://ssl.gstatic.com/dictionary/static/sounds/20160317/you--_us_1.mp3

            ////////https://ssl.gstatic.com/dictionary/static/sounds/20160317/ok--_us_1.mp3
            ////////https://ssl.gstatic.com/dictionary/static/sounds/20160317/you--_gb_1.mp3
            ////////https://ssl.gstatic.com/dictionary/static/sounds/20160317/ok--_gb_1.mp3

            //////if (!NetworkInterface.GetIsNetworkAvailable())
            //////{
            //////    // Network does not available.

            //////}

            //////var ping = new System.Net.NetworkInformation.Ping();
            //////var result = ping.Send("www.google.com");
            //////if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
            //////{

            //////}



            //////////string temp = HttpUtility.UrlEncode(input.Replace(" ", "---"));
            ////////string temp = HttpUtility.UrlEncode(input);
            //////////temp = temp.Replace("-----", "%20");

            ////////string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", temp, "en|vi");

            ////////string s = String.Empty;
            ////////using (WebClient webClient = new WebClient())
            ////////{
            ////////    webClient.Encoding = encoding;
            ////////    s = webClient.DownloadString(url);
            ////////}
            ////////string ht = HttpUtility.HtmlDecode(s);

            ////////string result = String.Empty;
            ////////int p = s.IndexOf("id=result_box");
            ////////if (p > 0)
            ////////    s = "<span " + s.Substring(p, s.Length - p);
            ////////p = s.IndexOf("</div>");
            ////////if (p > 0)
            ////////{
            ////////    s = s.Substring(0, p);
            ////////    s = s.Replace("<br>", "¦");
            ////////    s = HttpUtility.HtmlDecode(s);
            ////////    result = Regex.Replace(s, @"<[^>]*>", String.Empty);
            ////////}
            ////////if (result != string.Empty)
            ////////{
            ////////    string[] rs = result.Split('¦').Select(x => x.Trim()).ToArray();
            ////////    m.Output = new MsgOutput() { Ok = true, Data = rs, Total = rs.Length };
            ////////    
            ////////    lock (lockResponse)
            ////////    {
            ////////        if (dicResponses.ContainsKey(m.Key))
            ////////            dicResponses[m.Key] = m;
            ////////        else
            ////////            dicResponses.Add(m.Key, m);
            ////////    }
            ////////}
            ////////else
            ////////{
            ////////    m.Output = new MsgOutput() { Data = "Can not translate" };
            ////////}

            ////////postMessageToFormUI(m.Key);

            m.Output.Ok = true;
            //m.Output.Data = wo.words;
            return m;
        }
        public void Close() { }
    }

    public class api_word_LocalStore : api_base, IAPI
    {
        static readonly object _lock = new object();
        static Dictionary<string, oWord> dicWord = new Dictionary<string, oWord>();

        public static oWordContent f_analytic_Text(string s)
        {
            string[] sentences = s.Split(new char[] { '\n', '\r', '.' }).Where(x => x != string.Empty).ToArray();

            string text = Regex.Replace(s, "[^0-9a-zA-Z]+", " ").ToLower();
            text = Regex.Replace(text, "[ ]{2,}", " ").ToLower();
            oWordCount[] aword = text.Split(' ').Where(x => x.Length > 3)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => new oWordCount() { word = x.Key, count = x.Count() })
                .ToArray();

            return new oWordContent() { sentences = sentences, words = aword };
        }


        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            oWordContent wo = (oWordContent)m.Input;
            m.KEY = string.Empty;
            m.Output.Ok = false;

            string[] a_con = wo.words.Select(x => x.word.ToLower()).Distinct().ToArray();

            string[] a_local = new string[] { };
            lock (_lock)
                a_local = dicWord.Keys.ToArray();

            a_local = a_local.Where(x => a_con.Any(o => o == x)).ToArray();

            string[] a_down;

            if (a_local.Length == 0)
                a_down = a_con;
            else
                a_down = a_local.Where(x => !a_con.Any(o => o == x)).ToArray();

            if (a_down.Length > 0)
                app.postMessageToService(new msg() { API = _API.WORD_DOWNLOAD, Input = a_down });

            m.Output.Ok = true;
            m.Output.Data = a_con;

            return m;
        }
        public void Close() { }
    }

    public class api_folder_Analytic : api_base, IAPI
    {
        static readonly object _lock = new object();
        static List<string> listWordDownload = new List<string>() { };

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string path = (string)m.Input;
            if (Directory.Exists(path))
            {

                m.KEY = string.Empty;
                m.Output.Ok = false;
            }

            m.Output.Ok = true;
            //m.Output.Data = wo.words;
            return m;
        }
        public void Close() { }
    }

}
