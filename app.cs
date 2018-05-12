using System;
using System.Security.Permissions;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using Gecko;

namespace appel
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "Everything"),
    PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class app
    {
        #region [ VARIABLE ]
        
        public const string app_name = "English v1.0";
        static fMain main;
        static fMedia media;

        static Dictionary<string, IthreadMsg> dicService = null;
        static object lockResponse = new object();
        static Dictionary<string, msg> dicResponses = null;


        #endregion

        static app()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
            {
                Assembly asm = null;
                string comName = ev.Name.Split(',')[0];
                string resourceName = @"Kit\DLL\" + comName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                resourceName = typeof(app).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Debug.WriteLine(resourceName);
                    }
                    else
                    {
                        byte[] buffer = new byte[stream.Length];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                ms.Write(buffer, 0, read);
                            buffer = ms.ToArray();
                        }
                        asm = Assembly.Load(buffer);
                    }
                }
                return asm;
            };
        }


        #region [ APP ]

        public static IFORM get_Main()
        {
            return main;
        }

        public static void notification_Show(string msg, int duration_ = 0)
        {
            if (main != null) main.Invoke((MethodInvoker)delegate () { new fNotificationMsg(msg, duration_).Show(); });
        }

        public static string postMessageToService(msg m)
        {
            if (m == null) return string.Empty;
            if (m.KEY == string.Empty)
                m.KEY = Guid.NewGuid().ToString();

            if (dicService.ContainsKey(m.API)) dicService[m.API].Execute(m);

            return m.KEY;
        }

        public static void RUN()
        {
            f_gecko_Init();

            //string s = api_crawler.getHtml("https://dictionary.cambridge.org/grammar/british-grammar/");
            //return;

            dicResponses = new Dictionary<string, msg>();
            dicService = new Dictionary<string, IthreadMsg>();
            main = new fMain();

            dicService.Add(_API.WORD_LOAD_LOCAL, new threadMsg(new api_word_LocalStore()));
            dicService.Add(_API.SETTING_APP, new threadMsg(new api_settingApp(), main.f_api_settingApp_responseMsg));
            dicService.Add(_API.FOLDER_ANYLCTIC, new threadMsg(new api_folder_Analytic()));
            dicService.Add(_API.CRAWLER, new threadMsg(new api_crawler()));
            dicService.Add(_API.YOUTUBE, new threadMsg(new api_youtube()));
            dicService.Add(_API.MP3, new threadMsg(new api_mp3()));

            //||| MAIN
            main.Shown += main_Shown;
            main.FormClosing += main_Closing;
            main.onNotifyClick += (se, ev) =>
            {
                if (main.isVisiable)
                {
                    main.isVisiable = false;
                    main.Hide();
                }
                else
                {
                    main.isVisiable = true;
                    main.Show();
                    main.Activate();
                }
            };

            f_media_Init();

            Application.Run(main);
        }


        private static void main_Closing(object sender, FormClosingEventArgs e)
        {
            if (main == null) return;

            var confirmResult = MessageBox.Show("Are you sure to exit this application ?",
                                           "Confirm Exit!",
                                           MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                f_gecko_Stop();

                foreach (var kv in dicService)
                    if (kv.Value != null)
                        kv.Value.Stop();

                main.f_form_freeResource();

                // wait for complete threads, free resource
                //Thread.Sleep(200);

                //Application.ExitThread();
                //Application.Exit();
            }
            else
                e.Cancel = true;
        }

        private static void main_Shown(object sender, EventArgs e)
        {
            //fmain.Left = Screen.PrimaryScreen.WorkingArea.Width - 1024;
            //fmain.Width = 999;
            //fmain.Top = 40;
            //if (Screen.PrimaryScreen.WorkingArea.Height > 800)
            //    fmain.Height = 750;
            //else
            //    fmain.Height = Screen.PrimaryScreen.WorkingArea.Height - 50;
            f_main_Load();
        }

        #endregion
        
        #region [ GECKO ]

        static void f_gecko_Stop() {
            //Xpcom.Shutdown();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void f_gecko_Init()
        {
            Xpcom.Initialize("bin");
            GeckoPreferences.User["extensions.blocklist.enabled"] = false;
            // Uncomment the follow line to enable error page
            GeckoPreferences.User["browser.xul.error_pages.enabled"] = true;
            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
            GeckoPreferences.User["full-screen-api.enabled"] = true;

            //GeckoPreferences.User["media.navigator.enabled"] = true;
            //GeckoPreferences.User["media.navigator.permission.disabled"] = true; // enable Access to video & audio
        }

        #endregion

        #region [ MEDIA ]

        static void f_media_Init() {
            media = new fMedia();

        }
        
        #endregion

        static void f_main_Load()
        {
            main.Left = 0;
            main.Top = 0;
            main.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            main.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

            //fmain.WindowState = FormWindowState.Maximized;
            main.f_form_init_UI();
            main.f_hide_label_Background(100);

            //postMessageToService(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_REGISTER_PATH, Input = "https://dictionary.cambridge.org/grammar/british-grammar/" });
            //postMessageToService(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_CONVERT_PACKAGE_TO_HTML, Input = @"E:\data_el2\crawler.raw.bin" });
            //postMessageToService(new msg() { API = _API.CRAWLER, KEY = _API.CRAWLER_KEY_CONVERT_PACKAGE_TO_TEXT, Input = @"E:\data_el2\crawler.htm.bin" });

            setTimeout.Delay(300, () =>
            {
                //fmain.crossThreadPerformSafely(() =>
                //{
                //    //fmain.f_tag_Reload(api_settingApp.);

                //    //fmain.f_doc_viewContent(new oNode()
                //    //{
                //    //    path = @"D:\EL\tu-vung-hay-gap-nhat-trong-ky-thi-toeic (1).pdf",
                //    //    name = "Test PDF",
                //    //    root = false,
                //    //    type = oNodeType.PDF
                //    //});

                //    //fmain.f_doc_viewContent(new oNode()
                //    //{
                //    //    path = @"w2ui-demos-introduction.txt",
                //    //    name = "Test TEXT",
                //    //    root = false,
                //    //    type = oNodeType.TEXT
                //    //});

                //});
            });

            //app.notification_Show("Hi, " + app.app_name, 3000);

            //postMessageToService(new msg() { API = _API.YOUTUBE, KEY = _API.YOUTUBE_INFO, Input = @"nIwU-9ZTTJc" }); 

            main.f_doc_viewContent(new oNode()
            {
                title = "Youtube",
                path = @"https://r6---sn-8qj-i5oed.googlevideo.com/videoplayback?sparams=dur%2Cei%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cpcm2cms%2Cpl%2Cratebypass%2Crequiressl%2Csource%2Cexpire&source=youtube&dur=234.289&initcwndbps=1042500&ei=D6z2WoDyNsuxgQPB4ruYCQ&expire=1526136943&mn=sn-8qj-i5oed%2Csn-i3beln7z&mm=31%2C29&pl=20&mime=video%2Fmp4&id=o-AGrT4c_bkr_PhBjaEQ0PO5gaO48aQ7SIah-UsmwA-OKr&ipbits=0&requiressl=yes&ip=14.177.123.70&mv=m&mt=1526115221&ms=au%2Crdu&signature=0F512CC247A9EF8206EBA5523608B894CD05ACF9.9CA3E2B2E40EEB2EA70A592CAB1952E7BE7668D6&ratebypass=yes&c=WEB&lmt=1510741625396835&key=yt6&itag=22&pcm2cms=yes&fvip=6",
                type = oNodeType.LINK
            });

            //http://www.youtube.com/api/timedtext?lang=en&v=9fEurt2OZ0I
            //const string m_url = @"http://web20office.com/crm/demo/system/login.php?r=/crm/demo";
            //const string m_url = @"file://///G:\_EL\Document\data_el2\articles-IT\w2ui\docs\form.html";
            //const string m_url = @"file:///G:/data_el2/articles-IT/w2ui/docs/form.html";
            //const string m_url = @"file:///C:/1.pdf";
            //const string m_url = @"http://english.com/youtube.html";
            //const string m_url = @"https://yendifplayer.com/demo/";
            //const string m_url = @"https://yendifplayer.com/demo/youtube-setup.html";
        }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            app.RUN();
        }
    }
}

