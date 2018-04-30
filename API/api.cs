using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace app_sys
{
    public class _API
    {
        public const string CRAWLER = "CRAWLER";
        public const string DB_FILE = "DB_FILE";

        public const string TRANSLATER = "TRANSLATOR";

        public const string WORD_LOAD_LOCAL = "WORD_LOAD_LOCAL";
        public const string WORD_DOWNLOAD = "WORD_DOWNLOAD";
    }

    public interface IAPI
    {
        msg Execute(msg msg);
        void Close();
    }

    public class api_Translater : IAPI
    {
        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string[] words = (string[])m.Input;
            Encoding encoding = Encoding.UTF7;
            string input = string.Join("\r\n", words);
            m.Key = input;
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

    public class api_word_Download : IAPI
    {
        static readonly object _lock = new object();
        static List<string> listWordDownload = new List<string>() { };

        public msg Execute(msg m)
        {
            if (m == null || m.Input == null) return m;
            string[] a = (string[])m.Input;

            m.Key = string.Empty;
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

    public class api_word_LocalStore : IAPI
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
            m.Key = string.Empty;
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
}
