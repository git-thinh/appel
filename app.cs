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

namespace appel
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "Everything"),
    PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class app
    {
        #region [ VARIABLE ]

        const string word_Path = @"D:\Projects\el_word";
        public const string app_name = "English v1.0";
        static fMain fmain;

        static Dictionary<string, IthreadMsg> dicService = null;
        static object lockResponse = new object();
        static Dictionary<string, msg> dicResponses = null;

        static List<string> listFolderPath = new List<string>() { 
            @"D:\Projects\data_el2",
            @"D:\EL",
        };
        
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

        public static void notification_Show(string msg, int duration_ = 0)
        {
            if (fmain != null) fmain.Invoke((MethodInvoker)delegate() { new fNotificationMsg(msg, duration_).Show(); });
        }

        public static string postMessageToService(msg m)
        {
            if (m == null) return string.Empty;
            if (m.Key == string.Empty)
                m.Key = Guid.NewGuid().ToString();

            if (dicService.ContainsKey(m.API)) dicService[m.API].Execute(m);

            return m.Key;
        }

        public static void RUN()
        {
            dicResponses = new Dictionary<string, msg>();
            dicService = new Dictionary<string, IthreadMsg>();
            fmain = new fMain();

            f_tag_loadFile();

            //||| WORD  
            dicService.Add(_API.WORD_LOAD_LOCAL, new threadMsg(new api_word_LocalStore(), fmain.f_api_word_responseMsg));

            //var word_Dowload = new threadMsg(new api_word_Download());
            //word_Dowload.OnMessage += (se, ev) => fmain.f_word_responseMsgFromService(ev.Message);
            //dicService.Add(_api.WORD_DOWNLOAD, word_Dowload);

            //||| CRAWLER


            //||| TRANSLATOR 
            //var msgTranslater = new threadMsg(new api_Translater());
            //msgTranslater.OnMessage += (se, ev) => fmain.f_translate_onMessage(ev.Message);
            //dicService.Add(_api.TRANSLATER, msgTranslater);

            //||| MAIN
            fmain.Shown += main_Shown;
            fmain.FormClosing += main_Closing;
            fmain.onNotifyClick += (se, ev) =>
            {
                if (fmain.isVisiable)
                {
                    fmain.isVisiable = false;
                    fmain.Hide();
                }
                else
                {
                    fmain.isVisiable = true;
                    fmain.Show();
                    fmain.Activate();
                }
            };

            Application.Run(fmain);
        }
         

        private static void main_Closing(object sender, FormClosingEventArgs e)
        {
            if (fmain == null) return;

            var confirmResult = MessageBox.Show("Are you sure to exit this application ?",
                                           "Confirm Exit!",
                                           MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                foreach (var kv in dicService)
                    if (kv.Value != null)
                        kv.Value.Stop();

                fmain.f_form_freeResource();

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

        static void f_main_Load()
        {
            fmain.Left = 0;
            fmain.Top = 0;
            fmain.Width = Screen.PrimaryScreen.WorkingArea.Width;
            fmain.Height = Screen.PrimaryScreen.WorkingArea.Height;

            //fmain.WindowState = FormWindowState.Maximized;
            fmain.f_form_init_UI();
            fmain.f_hide_label_Background(100);

            setTimeout.Delay(300, () =>
            {
                fmain.crossThreadPerformSafely(() =>
                {
                    fmain.f_tag_Reload(listTag.ToArray());

                    //fmain.f_doc_viewContent(new oNode()
                    //{
                    //    path = @"D:\EL\tu-vung-hay-gap-nhat-trong-ky-thi-toeic (1).pdf",
                    //    name = "Test PDF",
                    //    root = false,
                    //    type = oNodeType.PDF
                    //});

                    fmain.f_doc_viewContent(new oNode()
                    {
                        path = @"D:\Projects\data_el2\w2ui-demos-introduction.txt",
                        name = "Test TEXT",
                        root = false,
                        type = oNodeType.TEXT
                    });

                });
            });

            //app.notification_Show("Hi, " + app.app_name, 3000);
        }

        #region [ FOLDER ]

        public static string[] f_folder_getAll()
        {
            return listFolderPath.ToArray();
        }

        public static bool f_folder_Add(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            path = path.Trim().ToLower();

            if (listFolderPath.IndexOf(path) == -1)
            {
                listFolderPath.Add(path);
                return true;
            }
            else return false;
        }

        public static void f_folder_Analytic(string[] paths)
        {

        }

        #endregion

        #region [ TAG ]

        static List<string> listTag = new List<string>() { };

        public static void f_tag_loadFile()
        {
            if (File.Exists("tag.txt"))
                listTag = File.ReadAllText("tag.txt").ToLower().Split(',').Select(x => x.Trim())
                    .Distinct().ToList();
        }

        public static string[] f_tag_getAll()
        {
            return listTag.ToArray();
        }

        public static bool f_tag_AddNew(string tag)
        {
            tag = tag.Trim().ToLower();
            if (listTag.IndexOf(tag) == -1)
            {
                listTag.Add(tag);
                return true;
            }
            return false;
        }


        #endregion
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

