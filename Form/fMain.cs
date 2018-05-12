using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using FarsiLibrary.Win;
using System.Diagnostics;
using Gma.System.MouseKeyHook;
using PdfiumViewer;
using System.Threading;
using Gecko;

namespace appel
{
    public class fMain : Form, IFORM
    {
        const bool allow_hookMouseWheel = false;

        #region [ VARIABLE: CONST ... ]

        NotifyIcon notifyIcon;
        public EventHandler onNotifyClick;
        public bool isVisiable = false;
        oNode document_Opening = null;

        const int width_tab_Left = 269;
        const int width_tab_Right = 369;

        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ UI LAYOUT ]

        Label ui_lbl_form_backgrounc_Init = new Label()
        {
            AutoSize = false,
            Dock = DockStyle.None,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
            Location = new Point(0, 0),
        };

        #region [ CONTRACTOR UI: HEADER ]

        Panel ui_toolbar_Header = new Panel() { Dock = DockStyle.Top, Height = 32, Visible = false };

        IconButton ui_btn_app_left_ShowHide;
        IconButton ui_btn_app_right_ShowHide;

        IconButton ui_btn_app_Exit;
        IconButton ui_btn_app_Max;
        IconButton ui_btn_app_Min;

        IconButton ui_cat_btn_pathOpen;
        IconButton ui_cat_btn_pathRemove;
        IconButton ui_cat_btn_pathSetting;

        Label ui_lbl_path_doc_current;

        #endregion

        #region [ CONTRACTOR UI: FOOTER ]

        Panel ui_toolbar_Footer = new Panel() { Dock = DockStyle.Bottom, Height = 32, Visible = false };

        #endregion

        #region [ CONTRACTOR UI: AREA LEFT ]

        Splitter ui_splitter_Left = new Splitter()
        {
            Dock = DockStyle.Left,
            MinExtra = 0,
            MinSize = width_tab_Left,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };


        FATabStrip ui_tab_Left = new FATabStrip()
        {
            Dock = DockStyle.Left,
            Width = width_tab_Left,
            Tag = width_tab_Left,
            Margin = new Padding(0),
            AlwaysShowClose = false,
            AlwaysShowMenuGlyph = false,
            RightToLeft = RightToLeft.Yes,
            Visible = false,
        };

        //☆★☐☑⧉✉⦿⦾⚠⚿⛑✕✓⥀✖↭☊⦧▷◻◼⟲≔☰⚒❯►❚❚❮⟳⚑⚐✎✛
        //🕮🖎✍⦦☊🕭🔔🗣🗢🖳🎚🏷🖈🎗🏱🏲🗀🗁🕷🖒🖓👍👎♥♡♫♪♬♫🎙🎖🗝●◯⬤⚲☰⚒🕩🕪❯►❮⟳⚐🗑✎✛🗋🖫⛉ ⛊ ⛨⚏★☆
        FATabStripItem ui_tab_Tag = new FATabStripItem() { Title = "⛊", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_Content = new FATabStripItem() { Title = "☰", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_Folder = new FATabStripItem() { Visible = false, Title = "☘", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_Search = new FATabStripItem() { Title = "⚲", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_Media = new FATabStripItem() { Title = "►", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_Noti = new FATabStripItem() { Title = "⚑", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };


        System.Windows.Forms.TreeView ui_cat_treeView;


        FlowLayoutPanel ui_tag_listItems = new FlowLayoutPanel()
        {
            Visible = false,
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = true,
            Padding = new Padding(7, 0, 0, 0),
            RightToLeft = RightToLeft.Yes,
            BorderStyle = System.Windows.Forms.BorderStyle.None,
            Margin = new Padding(0),
        };
        FlowLayoutPanel ui_content_listItems = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            RightToLeft = RightToLeft.Yes,
            BorderStyle = System.Windows.Forms.BorderStyle.None,
            Margin = new Padding(0),
        };
        FlowLayoutPanel ui_find_listItems = new FlowLayoutPanel()
        {
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0),
            RightToLeft = RightToLeft.Yes,
            BorderStyle = System.Windows.Forms.BorderStyle.None,
            Margin = new Padding(0),
        };

        #endregion

        #region [ CONTRACTOR UI: AREA CENTER ]

        FATabStrip ui_tab_Center = new FATabStrip()
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            AlwaysShowClose = false,
            Visible = false,
        };
        FATabStripItem ui_tab_Help = new FATabStripItem() { Title = "Help", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_log_Tab = new FATabStripItem() { Title = "Log", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };

        // log, help
        TextBox ui_log_Text;
        IconButton ui_log_btnClean;
        IconButton ui_log_btnCopy;

        #endregion

        #region [ CONTRACTOR UI: AREA RIGHT ]

        Splitter ui_splitter_Right = new Splitter()
        {
            Dock = DockStyle.Right,
            MinExtra = 0,
            MinSize = width_tab_Right,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        FATabStrip ui_tab_Right = new FATabStrip()
        {
            Dock = DockStyle.Right,
            Width = width_tab_Right,
            Tag = width_tab_Right,
            Margin = new Padding(0),
            AlwaysShowClose = false,
            AlwaysShowMenuGlyph = false,
            Visible = false,
        };

        FATabStripItem ui_tab_detail_Speak = new FATabStripItem() { Title = "Speak", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_detail_Word = new FATabStripItem() { Title = "Word", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_detail_Sentence = new FATabStripItem() { Title = "Sentence", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_detail_Grammar = new FATabStripItem() { Title = "Grammar", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };
        FATabStripItem ui_tab_detail_Bookmark = new FATabStripItem() { Title = "Mark", CanClose = false, Padding = new Padding(0), Margin = new Padding(0), BackColor = Color.White, };

        FlowLayoutPanel ui_speak_listItems = new FlowLayoutPanel()
        {
            Visible = false,
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = true,
            Padding = new Padding(0, 0, 17, 0),
            RightToLeft = RightToLeft.No,
            BorderStyle = System.Windows.Forms.BorderStyle.None,
            Margin = new Padding(0),
        };

        FlowLayoutPanel ui_word_listItems = new FlowLayoutPanel()
        {
            Visible = false,
            AutoScroll = true,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = true,
            Padding = new Padding(0, 0, 17, 0),
            RightToLeft = RightToLeft.No,
            BorderStyle = System.Windows.Forms.BorderStyle.None,
            Margin = new Padding(0),
        };

        #endregion

        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ FROM DRAW UI: CONTROLS, ICONS ... ]

        void f_show_label_Background_Init()
        {
            this.Controls.Add(ui_lbl_form_backgrounc_Init);
            ui_lbl_form_backgrounc_Init.Width = this.Width;
            ui_lbl_form_backgrounc_Init.Height = this.Height;
            ui_lbl_form_backgrounc_Init.BringToFront();
        }

        public void f_hide_label_Background(int timeOut_FadeIn = 100)
        {
            ui_tab_Left.Visible = true;
            ui_tab_Right.Visible = true;
            ui_tab_Center.Visible = true;

            ui_toolbar_Header.Visible = true;
            ui_toolbar_Footer.Visible = true;

            ui_tag_listItems.Visible = true;
            ui_word_listItems.Visible = true;
            ui_speak_listItems.Visible = true;

            setTimeout.Delay(timeOut_FadeIn, () =>
            {
                ui_lbl_form_backgrounc_Init.crossThreadPerformSafely(() =>
                {
                    ui_lbl_form_backgrounc_Init.SendToBack();
                });
            });
        }

        public void f_form_init_UI()
        {
            notifyIcon = new NotifyIcon()
            {
                Icon = Resources.icon,
                Text = "IFC Amiss v1.0",
                Visible = true,
            };
            notifyIcon.MouseClick += (se, ev) => { if (onNotifyClick != null) onNotifyClick.Invoke(this, new EventArgs()); };


            ui_splitter_Left.SplitterMoved += (se, ev) => { ui_tab_Left.Tag = ui_tab_Left.Width; };
            ui_splitter_Right.SplitterMoved += (se, ev) => { ui_tab_Right.Tag = ui_tab_Right.Width; };
            ui_tab_Center.TabStripItemSelectionChanged += f_tab_Center_TabStripItemSelectionChanged;
            ui_tab_Center.TabStripItemClosed += f_tab_Center_TabStripItemClosed;
            ui_tab_Center.TabStripItemClosing += f_tab_Center_TabStripItemClosing;



            this.Controls.AddRange(new Control[] {

                ui_tab_Center,

                ui_splitter_Left,ui_tab_Left,
                new Label(){ Dock = DockStyle.Left, AutoSize = false, Width = 2 },

                ui_splitter_Right,ui_tab_Right,

                ui_toolbar_Header,
                //new Label(){ Dock = DockStyle.Top, AutoSize = false, Height = 2 },

                ui_toolbar_Footer,

                new Label(){ Dock = DockStyle.Top, AutoSize = false, Height = 1, BackColor = Color.Orange },
                new Label(){ Dock = DockStyle.Bottom, AutoSize = false, Height = 1 , BackColor = Color.Orange},
                new Label(){ Dock = DockStyle.Left, AutoSize = false, Width = 1, BackColor = Color.Orange },
                new Label(){ Dock = DockStyle.Right, AutoSize = false, Width = 1 , BackColor = Color.Orange},
            });


            ui_tab_Left.Items.AddRange(new FATabStripItem[] {
                ui_tab_Media,
                ui_tab_Tag,
                ui_tab_Content,
                ui_tab_Folder,
                ui_tab_Search,
                ui_tab_Noti,
            });

            ui_tab_Center.Items.AddRange(new FATabStripItem[] {
                ui_log_Tab,
                ui_tab_Help,
            });

            ui_toolbar_Header.MouseMove += new MouseEventHandler(this.f_form_move_MouseDown);
            ui_toolbar_Footer.MouseMove += new MouseEventHandler(this.f_form_move_MouseDown);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            f_form_header_initIcons();

            f_cat_treeView_Init();
            f_tag_init_UI();
            f_log_init_UI();
            f_area_right_initUI();

            f_show_label_Background_Init();

            f_hook_mouse_Open();
        }


        void f_log_init_UI()
        {
            ui_log_Text = new TextBox() { Dock = DockStyle.Fill, ScrollBars = ScrollBars.Both, BorderStyle = System.Windows.Forms.BorderStyle.None, Multiline = true };
            Panel box = new Panel() { Dock = DockStyle.Top, Height = 32, BackColor = Color.White };
            ui_log_Tab.Controls.AddRange(new Control[] { ui_log_Text, box });

            ui_log_btnClean = new IconButton(18) { IconType = IconType.ios_trash_outline, Dock = DockStyle.Right };
            ui_log_btnCopy = new IconButton(20) { IconType = IconType.ios_copy_outline, Dock = DockStyle.Right };

            box.Controls.AddRange(new Control[]{
                new Label() { Dock = DockStyle.Right, AutoSize = false, Width = 7 },
                ui_log_btnCopy ,
                new Label() { Dock = DockStyle.Right, AutoSize = false, Width = 7 },
                ui_log_btnClean,
                new Label() { Dock = DockStyle.Right, AutoSize = false, Width = 7  },
            });
        }

        void f_form_header_initIcons()
        {
            ui_lbl_path_doc_current = new Label()
            {
                Text = @"D:\Projects\data_el2\articles-IT\net framework\.NET FRAMEWORK 4.0 INTRODUCTION 2018.txt",
                Dock = DockStyle.Fill,
                AutoSize = false,
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            ui_lbl_path_doc_current.MouseMove += f_form_move_MouseDown;

            ui_btn_app_Exit = new IconButton() { IconType = IconType.close_circled, Dock = DockStyle.Right };
            ui_btn_app_Max = new IconButton(20) { IconType = IconType.ios_browsers_outline, Dock = DockStyle.Right };
            ui_btn_app_Min = new IconButton(20) { IconType = IconType.minus, Dock = DockStyle.Right };
            ui_btn_app_right_ShowHide = new IconButton(20) { IconType = IconType.ios_more, Dock = DockStyle.Right };

            ui_btn_app_left_ShowHide = new IconButton(22) { IconType = IconType.navicon_round, Dock = DockStyle.Left };
            ui_cat_btn_pathOpen = new IconButton() { IconType = IconType.ios_folder_outline, Dock = DockStyle.Left, ToolTipText = "Open folder" };
            ui_cat_btn_pathRemove = new IconButton(19) { IconType = IconType.trash_b, Dock = DockStyle.Left };
            ui_cat_btn_pathSetting = new IconButton() { IconType = IconType.ios_gear_outline, Dock = DockStyle.Left, ToolTipText = "Setting" };


            ui_toolbar_Header.Controls.AddRange(new Control[] {
                ui_lbl_path_doc_current,

                new Label() { Dock = DockStyle.Left, AutoSize = false, Width = 7 },
                //ui_cat_btn_pathRemove,
                ui_cat_btn_pathOpen,
                new Label() { Dock = DockStyle.Left, AutoSize = false, Width = 7 },
                ui_cat_btn_pathSetting,
                new Label() { Dock = DockStyle.Left, AutoSize = false, Width = 7 },
                ui_btn_app_left_ShowHide,
                new Label() { Dock = DockStyle.Left, AutoSize = false, Width = 7 },

                ui_btn_app_right_ShowHide,
                ui_btn_app_Min,
                ui_btn_app_Max,
                ui_btn_app_Exit,
            });
            ui_btn_app_left_ShowHide.Click += (se, ev) =>
            {
                if (ui_tab_Left.Width == 0 && (int)ui_tab_Left.Tag == 0) ui_tab_Left.Tag = width_tab_Left;

                if (ui_tab_Left.Width == 0)
                {
                    ui_tab_Left.Width = (int)ui_tab_Left.Tag;
                    ui_tab_Left.Tag = ui_tab_Left.Width;
                }
                else
                {
                    ui_tab_Left.Tag = ui_tab_Left.Width;
                    ui_tab_Left.Width = 0;
                }
            };
            ui_btn_app_right_ShowHide.Click += (se, ev) =>
            {
                if (ui_tab_Right.Width == 0 && (int)ui_tab_Right.Tag == 0) ui_tab_Right.Tag = width_tab_Left;

                if (ui_tab_Right.Width == 0)
                {
                    ui_tab_Right.Width = (int)ui_tab_Right.Tag;
                    ui_tab_Right.Tag = ui_tab_Right.Width;
                    ui_cat_treeView.Enabled = true;
                }
                else
                {
                    ui_tab_Right.Tag = ui_tab_Right.Width;
                    ui_tab_Right.Width = 0;
                    ui_cat_treeView.Enabled = false;
                }
            };
            ui_btn_app_Exit.Click += (se, ev) => { this.Close(); };
            ui_btn_app_Max.Click += (se, ev) =>
            {
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;
                else
                    this.WindowState = FormWindowState.Maximized;
            };
            ui_btn_app_Min.Click += (se, ev) =>
            {
                this.WindowState = FormWindowState.Minimized;
            };

            ui_cat_btn_pathOpen.Click += (se, ev) =>
            {
                using (var fol = new FolderBrowserDialog())
                {
                    DialogResult result = fol.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string path = fol.SelectedPath.Trim();
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (string.IsNullOrEmpty(path)) return;
                            path = path.Trim().ToLower();

                            if (api_settingApp.get_checkExistFolder(path) == false)
                            {
                                app.postMessageToService(new msg() { API = _API.SETTING_APP, KEY = _API.SETTING_APP_KEY_UPDATE_FOLDER, Input = path });
                            }
                        }
                    }
                }
            };
        }

        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ FORM: INIT, FREE RESOURCE, WNDPROC ... ]

        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case 0x84: //NC_HITTEST:
        //            this.Activate();
        //            break;
        //        //case MessageHelper.WM_USER:
        //        //    MessageBox.Show("Message recieved: " + m.WParam + " - " + m.LParam);
        //        //    break;
        //        //case MessageHelper.WM_COPYDATA:
        //        //    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
        //        //    Type mytype = mystr.GetType();
        //        //    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
        //        //    MessageBox.Show(mystr.lpData);
        //        //    break;
        //    }
        //    base.WndProc(ref m);
        //}

        public fMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = app.app_name;
            this.Icon = Resources.icon;
            //this.Font = new Font("Arial", 17, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.None;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        /*////////////////////////////////////////////////////////////////////////*/

        public void f_form_freeResource()
        {
            f_hook_mouse_Close();
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Icon = null;
            }
        }


        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ TREE CATEGORY ]

        void f_cat_treeView_Init()
        {
            //ui_cat_treeView = new NoHScrollTree() { Dock = DockStyle.Fill, BorderStyle = System.Windows.Forms.BorderStyle.None };
            ui_cat_treeView = new System.Windows.Forms.TreeView() { Dock = DockStyle.Fill, BorderStyle = System.Windows.Forms.BorderStyle.None, BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND };


            ui_cat_treeView.NodeMouseClick += f_cat_treeView_NodeMouseClick;
            ui_tab_Folder.Controls.AddRange(new Control[] {
                ui_cat_treeView ,
                new Label(){ Dock = DockStyle.Bottom, AutoSize = false, Height = 1 , BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
                new Label(){ Dock = DockStyle.Left, AutoSize = false, Width = 1, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
                new Label(){ Dock = DockStyle.Right, AutoSize = false, Width = 1 , BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
            });
            string[] paths = api_settingApp.get_listFolder();
            f_cat_treeView_addNewNodeRoot_Loading(paths);
        }

        private void f_cat_treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node.Tag == null) return;

            oNode item = (oNode)node.Tag;
            if (item.type == oNodeType.NONE) return;

            if (item.type == oNodeType.FOLDER)
            {
                if (node.Checked == false)
                    f_cat_treeView_addNewNodeChilds_Loading(node);
            }
            else
                f_doc_viewContent(item);

            if (node.Checked == false)
                node.Checked = true;
        }

        void f_cat_treeView_addNewNodeRoot_Loading(string[] paths)
        {
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    string name = Path.GetFileName(path);
                    ui_cat_treeView.Nodes.Add(new TreeNode()
                    {
                        Checked = false,
                        Text = name,
                        Tag = new oNode() { name = name, path = path, anylatic = false, type = oNodeType.FOLDER }
                    });
                }
            }
        }

        void f_cat_treeView_addNewNodeChilds_Loading(TreeNode node)
        {
            oNode item = (oNode)node.Tag;
            if (!File.Exists(item.path)) return;

            string[] paths = Directory.GetDirectories(item.path);
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    string name = Path.GetFileName(path);
                    node.Nodes.Add(new TreeNode()
                    {
                        Checked = false,
                        Text = name,
                        Tag = new oNode() { name = name, path = path, anylatic = false, type = oNodeType.FOLDER }
                    });
                }
            }

            paths = "*.txt|*.pdf|*.html|*.htm|*.pptx|*.ppt|*.docx|*.doc|*.xlsx|*.xls"
                .Split('|')
                .SelectMany(filter => System.IO.Directory.GetFiles(item.path, filter))
                .ToArray();
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    string name = Path.GetFileName(path).ToLower();
                    node.Nodes.Add(new TreeNode()
                    {
                        Checked = false,
                        Text = name,
                        Tag = new oNode() { name = name, path = path, anylatic = false }
                    });
                }
            }

        }

        #endregion

        #region [ TAG ]

        void f_tag_init_UI()
        {
            ui_splitter_Left.MinSize = width_tab_Left;

            var toolbar = new Panel()
            {
                AutoScroll = true,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Height = 27
            };

            ui_tab_Tag.Controls.AddRange(new Control[] {
                ui_tag_listItems,
                toolbar,
                new Label(){ Dock = DockStyle.Bottom, AutoSize = false, Height = 1 , BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
                new Label(){ Dock = DockStyle.Left, AutoSize = false, Width = 1, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
                new Label(){ Dock = DockStyle.Right, AutoSize = false, Width = 1 , BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR },
            });

            var txt_search = new TextBox()
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Width = 99,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Right,
                Location = new Point(ui_tag_listItems.Width - 104, 5),
                RightToLeft = System.Windows.Forms.RightToLeft.No,
                Height = 20,
            };

            var btn_add = new IconButton(21) { IconType = IconType.android_add, Dock = DockStyle.Left };
            var btn_edit = new IconButton(21) { IconType = IconType.edit, Dock = DockStyle.Left };
            var btn_remove = new IconButton(19) { IconType = IconType.trash_a, Dock = DockStyle.Left };

            toolbar.Controls.AddRange(new Control[] {
                btn_remove,
                new Label(){ Dock = DockStyle.Left, Width = 5, AutoSize = false },
                btn_edit,
                new Label(){ Dock = DockStyle.Left, Width = 5, AutoSize = false },
                btn_add,

                txt_search,
            });

            btn_add.Click += (se, ev) =>
            {
                string s = Microsoft.VisualBasic.Interaction.InputBox("Please input name tag:", "Create new tag", "", -1, -1);
                s = s.Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    ui_tag_listItems.Controls.Add(new Label()
                    {
                        Text = s,
                        BackColor = Color.WhiteSmoke,
                        TextAlign = ContentAlignment.MiddleCenter,
                        RightToLeft = RightToLeft.No,
                    });
                }
            };
            btn_edit.Click += (se, ev) =>
            {

            };
            btn_remove.Click += (se, ev) =>
            {

            };
        }

        public void f_tag_Reload(string[] tags)
        {
            ui_tag_listItems.Controls.Clear();
            for (int i = 0; i < tags.Length; i++)
            {
                var lbl = new Label()
                {
                    Text = tags[i],
                    AutoSize = true,
                    BackColor = Color.WhiteSmoke,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = font_content_P,
                    Margin = new Padding(7, 5, 1, 5),
                    Padding = new Padding(1, 3, 1, 3),
                    BorderStyle = System.Windows.Forms.BorderStyle.None,
                    RightToLeft = RightToLeft.No,
                };
                ui_tag_listItems.Controls.Add(lbl);
                lbl.MouseHover += (se, ev) => { lbl.BackColor = Color.Orange; };
                lbl.MouseLeave += (se, ev) => { lbl.BackColor = Color.WhiteSmoke; };
                lbl.Click += (se, ev) =>
                {
                    lbl.BackColor = Color.Orange;
                };
            }
        }

        void f_tag_package_Binding(long[] items)
        {
            string name = string.Empty;
            //ui_tag_listItems.Controls.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                oNode node = api_nodeStore.Get(items[i]);

                //var lbl = new Label()
                //{
                //    Text = name,
                //    AutoSize = true,
                //    BackColor = Color.WhiteSmoke,
                //    TextAlign = ContentAlignment.MiddleCenter,
                //    Font = font_content_P,
                //    Margin = new Padding(7, 5, 1, 5),
                //    Padding = new Padding(1, 3, 1, 3),
                //    BorderStyle = System.Windows.Forms.BorderStyle.None,
                //    RightToLeft = RightToLeft.No,
                //}; 
                var lbl = new uiItemLabel(node,
                    IconType.ios_book_outline)
                {
                    //BackColor = Color.WhiteSmoke,
                    Height = 22,
                    Margin = new Padding(7, 5, 1, 5),
                };

                ui_tag_listItems.Controls.Add(lbl);
                //lbl.MouseHover += (se, ev) => { lbl.BackColor = Color.Orange; };
                //lbl.MouseLeave += (se, ev) => { lbl.BackColor = Color.WhiteSmoke; };
                //lbl.Click += (se, ev) =>
                //{
                //    lbl.BackColor = Color.Orange;
                //    ((fMain)this.FindForm()).f_doc_viewContent(items[i]);
                //};
            }
        }

        void f_tag_book_Binding(long[] items)
        {
            string name = string.Empty;
            //ui_tag_listItems.Controls.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                oNode node = api_nodeStore.Get(items[i]);
                //var lbl = new Label()
                //{
                //    Text = name,
                //    AutoSize = true,
                //    BackColor = Color.WhiteSmoke,
                //    TextAlign = ContentAlignment.MiddleCenter,
                //    Font = font_content_P,
                //    Margin = new Padding(7, 5, 1, 5),
                //    Padding = new Padding(1, 3, 1, 3),
                //    BorderStyle = System.Windows.Forms.BorderStyle.None,
                //    RightToLeft = RightToLeft.No,
                //}; 
                var lbl = new uiItemLabel(node,
                    IconType.ios_book_outline)
                {
                    //BackColor = Color.WhiteSmoke,
                    Height = 22,
                    Margin = new Padding(7, 5, 1, 5),
                };

                ui_tag_listItems.Controls.Add(lbl);
                //lbl.MouseHover += (se, ev) => { lbl.BackColor = Color.Orange; };
                //lbl.MouseLeave += (se, ev) => { lbl.BackColor = Color.WhiteSmoke; };
                //lbl.Click += (se, ev) =>
                //{
                //    lbl.BackColor = Color.Orange;
                //    ((fMain)this.FindForm()).f_doc_viewContent(items[i]);
                //};
            }
        }
        #endregion

        #region [ DOCUMENT ]

        private void f_tab_Center_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {
        }

        private void f_tab_Center_TabStripItemClosed(object sender, EventArgs e)
        {
            var tab = ui_tab_Center.SelectedItem;
            if (tab != null && tab.Tag != null)
            {
                oNode node = (oNode)tab.Tag;
                ui_lbl_path_doc_current.Text = node.path;
            }
        }

        private void f_tab_Center_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            if (e.Item != null && e.Item.Tag != null)
            {
                oNode node = (oNode)e.Item.Tag;
                document_Opening = node;
                ui_lbl_path_doc_current.Text = node.path;
            }
        }


        static readonly System.Drawing.Font font_content_H1 = new System.Drawing.Font("Arial", 17, FontStyle.Bold);
        static readonly System.Drawing.Font font_content_H2 = new System.Drawing.Font("Arial", 15, FontStyle.Bold);
        static readonly System.Drawing.Font font_content_H3 = new System.Drawing.Font("Arial", 13, FontStyle.Bold);
        static readonly System.Drawing.Font font_content_H5 = new System.Drawing.Font("Arial", 11, FontStyle.Bold);
        static readonly System.Drawing.Font font_content_P = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
        static readonly List<string> content_listHeading2 = new List<string>() {
                "A.", "B.", "C.", "D.", "E.", "F.",
                "I.", "V.",
                "1.", "2.", "3.", "4.", "5.", "6.", "7.", "8.", "9."
            };
        static readonly List<string> content_listHeading3 = new List<string>() { "II.", "VI.", "IV.", "10.", "11.", "12.", "13.", "14.", "15." };
        static readonly List<string> content_listHeading4 = new List<string>() { "III." };
        public Control[] f_doc_buildContentControl(string[] a)
        {
            string si = string.Empty;
            List<Control> list = new List<Control>();
            GrowLabel growLabel;
            bool isHeading = false;
            for (int i = a.Length - 1; i > 0; i--)
            {
                si = a[i];
                if (si.Length == 0) continue;

                isHeading = false;
                if (si[0] == '■') isHeading = true;
                if (si.Length > 1 && content_listHeading2.IndexOf(si.Substring(0, 2)) != -1) isHeading = true;
                else if (si.Length > 2 && content_listHeading3.IndexOf(si.Substring(0, 3)) != -1) isHeading = true;
                else if (si.Length > 3 && content_listHeading4.IndexOf(si.Substring(0, 4)) != -1) isHeading = true;

                if (isHeading)
                {
                    growLabel = new GrowLabel()
                    {
                        Text = si,
                        Font = ((si[0] == 'I' || si[0] == 'V' || si[0] == 'X') == true ? font_content_H2 : font_content_H3),
                        Dock = DockStyle.Top
                    };
                    list.AddRange(new Control[]{
                            new Label() { AutoSize = false, Height = 10, Dock = DockStyle.Top },
                            growLabel
                        });
                    continue;
                }

                switch (si[0])
                {
                    default:
                        growLabel = new GrowLabel()
                        {
                            Text = si,
                            Font = font_content_P,
                            Dock = DockStyle.Top
                        };
                        break;
                        //case '■':
                        //    growLabel = new GrowLabel()
                        //    {
                        //        Text = si.Substring(1).Trim(),
                        //        Font = font_content_H3,
                        //        Dock = DockStyle.Top
                        //    };
                        //    break;
                        //case '¦':
                        //    break;
                        //case '⌐': // begin UL_OL 
                        //    break;
                        //case '•': // LI 
                        //    break;
                        //case '□': // LI LI 
                        //    break;
                        //case '▫': // LI LI LI 
                        //    break;
                        //case '┘': // end UL_OL 
                        //    break;
                }
                list.AddRange(new Control[]{
                            new Label() { AutoSize = false, Height = 10, Dock = DockStyle.Top },
                            growLabel
                        });
            }

            list.AddRange(new Control[]{
                            new Label() { AutoSize = false, Height = 20, Dock = DockStyle.Top },
                            new GrowLabel() {
                                    Text = a[0],
                                    Font = font_content_H1,
                                    Dock = DockStyle.Top,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                },
                            new Label() { AutoSize = false, Height = 20, Dock = DockStyle.Top },
                        });

            return list.ToArray();
        }

        public void f_doc_viewContent(oNode doc)
        {
            if (doc == null) return;

            #region // exist in tabs

            foreach (FATabStripItem ti in ui_tab_Center.Items)
            {
                if (ti.Tag != null)
                {
                    oNode node = (oNode)ti.Tag;
                    switch (doc.type)
                    {
                        case oNodeType.PACKAGE_ARTICLE:
                            if (node.name == doc.name)
                            {
                                ui_tab_Center.SelectedItem = ti;
                                document_Opening = doc;
                                return;
                            }
                            break;
                        default:
                            if (node.path == doc.path)
                            {
                                ui_tab_Center.SelectedItem = ti;
                                document_Opening = doc;
                                return;
                            }
                            break;
                    }
                }
            }

            #endregion

            PdfViewer pdfViewer = null;
            Panel box_text = null;
            Panel toolbar;
            FATabStripItem tab = null;
            ui_lbl_path_doc_current.Text = doc.path;
            string tit = doc.name;
            if (tit != null && tit.Length > 20) tit = tit.Substring(0, 18) + "...";
            string[] a = new string[] { };
            string text = string.Empty;
            string si;
            GrowLabel growLabel = null;
            GeckoWebBrowser browser = null;
            Cursor = Cursors.WaitCursor;
            ui_tab_Left.Enabled = false;

            switch (doc.type)
            {
                case oNodeType.PDF:
                    if (!File.Exists(doc.path)) return;
                    #region [ PDF ]

                    pdfViewer = new PdfViewer()
                    {
                        Dock = DockStyle.Fill,
                        ShowToolbar = false,
                        Document = null,
                        BackColor = Color.White,
                        ZoomMode = PdfViewerZoomMode.FitWidth,
                        BorderStyle = System.Windows.Forms.BorderStyle.None,
                        ShowBookmarks = false,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Visible = false,
                    };

                    if (pdfViewer.Document != null)
                        pdfViewer.Document.Dispose();
                    PdfDocument pdf = null;
                    try { pdf = PdfDocument.Load(this, doc.path); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (pdf == null) return;
                    pdfViewer.Document = pdf;

                    tab = new FATabStripItem(tit, null)
                    {
                        BackColor = Color.White,
                        Padding = new Padding(0),
                        Margin = new Padding(0),
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    tab.Controls.AddRange(new Control[]{
                            pdfViewer,
                    });
                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
                case oNodeType.YOUTUBE:
                    #region [ YOUTUBE ]

                    tab = new FATabStripItem(doc.title, null)
                    {
                        //Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        //AutoScroll = true,
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    browser = new GeckoWebBrowser();
                    browser.Dock = DockStyle.Fill;
                    //browser.DisableWmImeSetContext = true;
                    browser.DomContextMenu += f_browser_DomContextMenu;
                    browser.DocumentCompleted += f_browser_DocumentCompleted;
                    tab.Controls.Add(browser);
                    browser.EnableDefaultFullscreen();

                    //browser.Retargeted += (s, e) =>
                    //{
                    //    var ch = e.Request as Gecko.Net.Channel;
                    //    Console.WriteLine("Retargeted: url: " + e.Uri + ", contentType: " + ch.ContentType + ", top: " + e.DomWindowTopLevel);
                    //};

                    //open.Click += (s, a) =>
                    //{
                    //    nsIFilePicker filePicker = Xpcom.CreateInstance<nsIFilePicker>("@mozilla.org/filepicker;1");
                    //    filePicker.Init(browser.Window.DomWindow, new nsAString("hello"), nsIFilePickerConsts.modeOpen);
                    //    //filePicker.AppendFilter(new nsAString("png"), new nsAString("*.png"));
                    //    //filePicker.AppendFilter(new nsAString("html"), new nsAString("*.html"));
                    //    if (nsIFilePickerConsts.returnOK == filePicker.Show())
                    //    {
                    //        using (nsACString str = new nsACString())
                    //        {
                    //            filePicker.GetFileAttribute().GetNativePathAttribute(str);
                    //            browser.Navigate(str.ToString());
                    //        }
                    //    }
                    //};

                    //// Popup window management.
                    //browser.CreateWindow += (s, e) =>
                    //{
                    //    // A naive popup blocker, demonstrating popup cancelling.
                    //    Console.WriteLine("A popup is trying to show: " + e.Uri);
                    //    if (e.Uri.StartsWith("http://annoying-site.com"))
                    //    {
                    //        e.Cancel = true;
                    //        Console.WriteLine("A popup is blocked: " + e.Uri);
                    //        return;
                    //    }

                    //    // For <a target="_blank"> and window.open() without specs(3rd param),
                    //    // e.Flags == GeckoWindowFlags.All, and we load it in a new tab;
                    //    // otherwise, load it in a popup window, which is maximized by default.
                    //    // This simulates firefox's behavior.
                    //    if (e.Flags == GeckoWindowFlags.All)
                    //        e.WebBrowser = AddTab();
                    //    else
                    //    {
                    //        var wa = System.Windows.Forms.Screen.GetWorkingArea(this);
                    //        e.InitialWidth = wa.Width;
                    //        e.InitialHeight = wa.Height;
                    //    }
                    //};

                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
                case oNodeType.LINK:
                    #region [ LINK ]

                    tab = new FATabStripItem(doc.title, null)
                    {
                        //Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        //AutoScroll = true,
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    browser = new GeckoWebBrowser();
                    browser.Dock = DockStyle.Fill;
                    //browser.DisableWmImeSetContext = true;
                    browser.DomContextMenu += f_browser_DomContextMenu;
                    browser.DocumentCompleted += f_browser_DocumentCompleted;
                    tab.Controls.Add(browser);
                    browser.EnableDefaultFullscreen();

                    //browser.Retargeted += (s, e) =>
                    //{
                    //    var ch = e.Request as Gecko.Net.Channel;
                    //    Console.WriteLine("Retargeted: url: " + e.Uri + ", contentType: " + ch.ContentType + ", top: " + e.DomWindowTopLevel);
                    //};

                    //open.Click += (s, a) =>
                    //{
                    //    nsIFilePicker filePicker = Xpcom.CreateInstance<nsIFilePicker>("@mozilla.org/filepicker;1");
                    //    filePicker.Init(browser.Window.DomWindow, new nsAString("hello"), nsIFilePickerConsts.modeOpen);
                    //    //filePicker.AppendFilter(new nsAString("png"), new nsAString("*.png"));
                    //    //filePicker.AppendFilter(new nsAString("html"), new nsAString("*.html"));
                    //    if (nsIFilePickerConsts.returnOK == filePicker.Show())
                    //    {
                    //        using (nsACString str = new nsACString())
                    //        {
                    //            filePicker.GetFileAttribute().GetNativePathAttribute(str);
                    //            browser.Navigate(str.ToString());
                    //        }
                    //    }
                    //};

                    //// Popup window management.
                    //browser.CreateWindow += (s, e) =>
                    //{
                    //    // A naive popup blocker, demonstrating popup cancelling.
                    //    Console.WriteLine("A popup is trying to show: " + e.Uri);
                    //    if (e.Uri.StartsWith("http://annoying-site.com"))
                    //    {
                    //        e.Cancel = true;
                    //        Console.WriteLine("A popup is blocked: " + e.Uri);
                    //        return;
                    //    }

                    //    // For <a target="_blank"> and window.open() without specs(3rd param),
                    //    // e.Flags == GeckoWindowFlags.All, and we load it in a new tab;
                    //    // otherwise, load it in a popup window, which is maximized by default.
                    //    // This simulates firefox's behavior.
                    //    if (e.Flags == GeckoWindowFlags.All)
                    //        e.WebBrowser = AddTab();
                    //    else
                    //    {
                    //        var wa = System.Windows.Forms.Screen.GetWorkingArea(this);
                    //        e.InitialWidth = wa.Width;
                    //        e.InitialHeight = wa.Height;
                    //    }
                    //};

                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
                case oNodeType.TEXT:
                    if (!File.Exists(doc.path)) return;
                    #region [ TEXT ]

                    box_text = new Panel()
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        AutoScroll = true,
                        Name = doc.path,
                        Visible = false,
                    };

                    tab = new FATabStripItem(tit, null)
                    {
                        //Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        //AutoScroll = true,
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    text = File.ReadAllText(doc.path);
                    a = text.Split(new char[] { '\r', '\n' }).Where(x => x.Length > 0).ToArray();

                    box_text.Controls.AddRange(f_doc_buildContentControl(a));
                    tab.Controls.Add(box_text);

                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
                case oNodeType.DOCX:
                    if (!File.Exists(doc.path)) return;
                    #region [ DOCX ]

                    box_text = new Panel()
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        AutoScroll = true,
                        Name = doc.path,
                        Visible = false,
                    };

                    tab = new FATabStripItem(tit, null)
                    {
                        //Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        //AutoScroll = true,
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    try
                    {
                        // Extract text from an input file.
                        DocxToText dtt = new DocxToText(doc.path);
                        text = dtt.ExtractText();
                        a = text.Split(new char[] { '\r', '\n' }).Where(x => x.Length > 0).ToArray();
                    }
                    catch
                    {
                        return;
                    }

                    box_text.Controls.AddRange(f_doc_buildContentControl(a));
                    tab.Controls.Add(box_text);

                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
                case oNodeType.PACKAGE_ARTICLE:
                    #region [ BOOK_ARTICLE ]

                    box_text = new Panel()
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(15, 0, 0, 0),
                        BackColor = Color.White,
                        AutoScroll = true,
                        Name = doc.path,
                        Visible = false,
                    };

                    tit = doc.title;
                    tab = new FATabStripItem(tit, null)
                    {
                        BackColor = Color.White,
                    };
                    ui_tab_Center.AddTab(tab, true);
                    tab.Tag = doc;

                    text = doc.content;
                    a = text.Split(new char[] { '\r', '\n' }).Where(x => x.Length > 0).ToArray();

                    for (int i = a.Length - 1; i > 0; i--)
                    {
                        si = a[i];
                        if (si.Length == 0) continue;

                        switch (si[0])
                        {
                            case '■':
                                growLabel = new GrowLabel()
                                {
                                    Text = si,
                                    Font = font_content_H3,
                                    Dock = DockStyle.Top
                                };
                                break;
                            //case '¦':
                            //    break;
                            //case '⌐': // begin UL_OL 
                            //    break;
                            //case '•': // LI 
                            //    break;
                            //case '□': // LI LI 
                            //    break;
                            //case '▫': // LI LI LI 
                            //    break;
                            //case '┘': // end UL_OL 
                            //    break;
                            default:
                                growLabel = new GrowLabel()
                                {
                                    Text = si,
                                    Font = font_content_P,
                                    Dock = DockStyle.Top
                                };
                                break;
                        }

                        //box_text.Controls.AddRange(new Control[]{  
                        //    new Label() { AutoSize = false, Height = 10, Dock = DockStyle.Top },
                        //    growLabel
                        //});
                        box_text.Controls.AddRange(new Control[]{
                            new Label() { AutoSize = false, Height = 10, Dock = DockStyle.Top },
                            growLabel
                        });
                    }

                    box_text.Controls.AddRange(new Control[]{
                            new Label() { AutoSize = false, Height = 20, Dock = DockStyle.Top },
                            new GrowLabel() {
                                    Text = a[0],
                                    Font = font_content_H1,
                                    Dock = DockStyle.Top,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                },
                            new Label() { AutoSize = false, Height = 20, Dock = DockStyle.Top },
                        });

                    tab.Controls.Add(box_text);

                    #endregion
                    #region [ TOOLBAR ]
                    toolbar = new Panel()
                    {
                        Dock = DockStyle.Bottom,
                        Height = 27,
                        BackColor = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND,
                    };
                    tab.Controls.AddRange(new Control[]{
                            toolbar,
                            //new Label(){ AutoSize = false, Dock = DockStyle.Top, BackColor = Color.Gray, Height = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                            new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
                    });
                    #endregion
                    break;
            }

            if (tab != null)
            {
                document_Opening = doc;
                if (box_text != null) box_text.Visible = true;

                if (browser != null)
                {
                    switch (doc.type) {
                        case oNodeType.LINK:
                            browser.Navigate(doc.path);
                            break;
                    }
                }

                if (pdfViewer != null)
                    setTimeout.Delay(300, () => { pdfViewer.crossThreadPerformSafely(() => { pdfViewer.Visible = true; }); });

                if (!string.IsNullOrEmpty(text))
                {
                    setTimeout.Delay(1, () =>
                    {
                        f_ui_word_request_AnalyticText(text);
                    });
                }

                app.postMessageToService(new msg() { API = _API.SETTING_APP, KEY = _API.SETTING_APP_KEY_UPDATE_NODE_OPENING, Input = doc });
            }

            ui_tab_Left.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void f_browser_DomContextMenu(object sender, DomMouseEventArgs e)
        {
            //contextMenu.Show(browser, PointToClient(new Point(e.ScreenX, e.ScreenY)));
            e.Handled = true;
        }

        private void f_browser_DocumentCompleted(object sender, EventArgs e)
        {
            //if (browser.Url.ToString() == m_url)
            //{
            //    foreach (var a in browser.Document.GetElementsByTagName("A").Cast<GeckoAnchorElement>())
            //    {
            //        a.Style.CssText = "display:none";
            //        break;
            //    }

            //    foreach (var img in browser.Document.GetElementsByTagName("IMG").Cast<GeckoImageElement>())
            //    {
            //        if (img.Src == "http://web20office.com/web/images/web20office.png")
            //        {
            //            img.Style.CssText = "display:none";
            //            bg.SendToBack();
            //            break;
            //        }
            //    }
            //}
            //else {
            //    foreach (var a in browser.Document.GetElementsByTagName("DIV"))
            //    {
            //        if (a.Id == "mainTop2") {
            //            a.SetAttribute("style", "display:none;");
            //            break;
            //        }
            //    }

            //}
        }

        #endregion

        #region [ AREA RIGHT ]

        void f_area_right_initUI()
        {
            ui_tab_Right.Items.AddRange(new FATabStripItem[] {
                ui_tab_detail_Word,
                ui_tab_detail_Speak ,
                ui_tab_detail_Sentence,
                ui_tab_detail_Grammar,
                ui_tab_detail_Bookmark,
            });

            ui_tab_detail_Word.Controls.AddRange(new Control[] {
                ui_word_listItems,
                new Label(){ AutoSize = false, Dock = DockStyle.Left, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                new Label(){ AutoSize = false, Dock = DockStyle.Right, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Width = 1 },
                new Label(){ AutoSize = false, Dock = DockStyle.Bottom, BackColor = _CONST.TAB_ACTIVE_BORDER_COLOR, Height = 1 },
            });
        }

        #region [ WORD ]

        void f_ui_word_request_AnalyticText(string s)
        {
            oWordContent wo = api_word_LocalStore.f_analytic_Text(s);
            if (wo != null && wo.words.Length > 0)
            {
                app.postMessageToService(new msg() { API = _API.WORD_LOAD_LOCAL, Input = wo });
            }
        }

        #endregion

        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ TRANSLATE ]

        public void f_translate_onMessage(msg m)
        {
            if (m == null || m.Output == null) return;


            app.postMessageToService(new msg() { API = _API.TRANSLATER, Input = new string[] { "hello", "send message" } });

        }

        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ FORM MOVE ]

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void f_form_move_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        #region [ HOOK MOUSE: MOUSE WHEEL, RUN SCROLLBAR ... ]

        void f_hook_mouse_wheel_runScrollBar(MouseEventArgs e)
        {
            int type = 0, x_mouse = e.X, x_app = this.Location.X, x_content = x_app + ui_tab_Left.Width, x_right = x_content + ui_tab_Center.Width;
            if (x_mouse > x_app && x_mouse < x_right)
            {
                if (x_mouse < x_content)
                {
                    type = 1;// area left
                }
                else if (x_mouse < x_right)
                {
                    type = 2; // area center 
                }
                else
                {
                    type = 3; // area right
                }
            }

            if (type == 0) return;
            switch (type)
            {
                case 1:
                    if (ui_tab_Left.SelectedItem != null)
                    {
                        switch (ui_tab_Left.SelectedItem.Title)
                        {
                            case "Tag":
                                ui_tag_listItems.Focus();
                                break;
                            case "Folder":
                                if (ui_cat_treeView != null)
                                    ui_cat_treeView.Focus();
                                break;
                            case "Content":
                                ui_content_listItems.Focus();
                                break;
                            case "Find":
                                ui_find_listItems.Focus();
                                break;
                        }
                    }
                    break;
                case 2:
                    if (ui_tab_Center != null && ui_tab_Center.SelectedItem != null)
                    {
                        if (document_Opening != null && document_Opening.type == oNodeType.TEXT)
                        {
                            //handle = ui_tab_Center.SelectedItem.Handle;
                            ui_tab_Center.SelectedItem.Controls[0].Focus();
                        }
                        else
                            ui_tab_Center.SelectedItem.Focus();
                    }
                    break;
                case 3:
                    break;
            }
        }

        void f_hook_mouse_Open()
        {
            if (allow_hookMouseWheel)
                f_hook_mouse_SubscribeGlobal();
        }

        void f_hook_mouse_Close()
        {
            if (allow_hookMouseWheel)
                f_hook_mouse_Unsubscribe();
        }

        /*////////////////////////////////////////////////////////////////////////*/

        private IKeyboardMouseEvents hook_events;

        private void f_hook_mouse_SubscribeApplication()
        {
            f_hook_mouse_Unsubscribe();
            f_hook_mouse_Subscribe(Hook.AppEvents());
        }

        private void f_hook_mouse_SubscribeGlobal()
        {
            f_hook_mouse_Unsubscribe();
            f_hook_mouse_Subscribe(Hook.GlobalEvents());
        }

        private void f_hook_mouse_Subscribe(IKeyboardMouseEvents events)
        {
            hook_events = events;
            //m_Events.KeyDown += OnKeyDown;
            //m_Events.KeyUp += OnKeyUp;
            //m_Events.KeyPress += HookManager_KeyPress;

            //m_Events.MouseUp += OnMouseUp;
            //m_Events.MouseClick += OnMouseClick;
            //m_Events.MouseDoubleClick += OnMouseDoubleClick;

            //m_Events.MouseMove += HookManager_MouseMove;

            //m_Events.MouseDragStarted += OnMouseDragStarted;
            //m_Events.MouseDragFinished += OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //m_Events.MouseWheelExt += f_hook_mouse_HookManager_MouseWheelExt;
            //else
            hook_events.MouseWheel += f_hook_mouse_HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //m_Events.MouseDownExt += HookManager_Supress;
            //else
            //m_Events.MouseDown += OnMouseDown;
        }

        private void f_hook_mouse_Unsubscribe()
        {
            if (hook_events == null) return;
            //m_Events.KeyDown -= OnKeyDown;
            //m_Events.KeyUp -= OnKeyUp;
            //m_Events.KeyPress -= HookManager_KeyPress;

            //m_Events.MouseUp -= OnMouseUp;
            //m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            //m_Events.MouseMove -= HookManager_MouseMove;

            //m_Events.MouseDragStarted -= OnMouseDragStarted;
            //m_Events.MouseDragFinished -= OnMouseDragFinished;

            //if (checkBoxSupressMouseWheel.Checked)
            //m_Events.MouseWheelExt -= f_hook_mouse_HookManager_MouseWheelExt;
            //else
            hook_events.MouseWheel -= f_hook_mouse_HookManager_MouseWheel;

            //if (checkBoxSuppressMouse.Checked)
            //m_Events.MouseDownExt -= HookManager_Supress;
            //else
            //m_Events.MouseDown -= OnMouseDown;

            hook_events.Dispose();
            hook_events = null;
        }

        private void f_hook_mouse_HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(string.Format("Wheel={0:000}", e.Delta));
            f_hook_mouse_wheel_runScrollBar(e);
        }

        private void f_hook_mouse_HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            //Debug.WriteLine(string.Format("Wheel={0:000}", e.Delta)); 
            //Debug.WriteLine("Mouse Wheel Move Suppressed.\n");
            e.Handled = true;
            //e.Handled = true; // true: break event at here, stop mouse wheel at here
        }

        /////////////////////////////////////////////////////////////


        #endregion

        /*////////////////////////////////////////////////////////////////////////*/

        #region [ API RESPONSE ]

        public void f_api_word_LocalStore_responseMsg(object sender, threadMsgEventArgs e)
        {
            msg m = e.Message;
            if (m == null) return;
            string[] a = null;
            oWord wo = null;
            switch (m.API)
            {
                case _API.WORD_LOAD_LOCAL:
                    #region
                    a = (string[])m.Output.Data;
                    Label[] a_lbl = new Label[a.Length];
                    for (int i = 0; i < a.Length; i++)
                    {
                        a_lbl[i] = new Label()
                        {
                            Text = a[i],
                            AutoSize = true,
                            BackColor = Color.WhiteSmoke,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = font_content_P,
                            Margin = new Padding(7, 5, 1, 5),
                            Padding = new Padding(1, 3, 1, 3),
                            BorderStyle = System.Windows.Forms.BorderStyle.None,
                            RightToLeft = System.Windows.Forms.RightToLeft.No,
                        };

                        //a_lbl[i].MouseHover += (se, ev) => { a_lbl[i].BackColor = Color.Orange; };
                        //a_lbl[i].MouseLeave += (se, ev) => { a_lbl[i].BackColor = Color.WhiteSmoke; };
                        //a_lbl[i].Click += (se, ev) =>
                        //{
                        //    a_lbl[i].BackColor = Color.Orange;
                        //};
                    }

                    ui_word_listItems.crossThreadPerformSafely(() =>
                    {
                        ui_word_listItems.Controls.Clear();
                        ui_word_listItems.Controls.AddRange(a_lbl);
                    });

                    #endregion
                    break;
                case _API.WORD_DOWNLOAD:
                    #region



                    #endregion
                    break;
            }
        }

        public void f_api_folder_Analytic_responseMsg(object sender, threadMsgEventArgs e)
        {
            msg m = e.Message;
            if (m == null && m.Output == null) return;

        }

        public void f_api_settingApp_responseMsg(object sender, threadMsgEventArgs e)
        {
            msg m = e.Message;
            if (m == null && m.Output == null) return;

        }

        public void f_api_crawler_responseMsg(object sender, threadMsgEventArgs e)
        {
            msg m = e.Message;
            if (m == null && m.Output == null) return;
            if (string.IsNullOrEmpty(m.Log)) return;
            string s = ui_log_Text.Text;

            ui_log_Text.crossThreadPerformSafely(() =>
            {
                ui_log_Text.Text = m.Log + Environment.NewLine + s;
            });
        }

        public void api_responseMsg(object sender, threadMsgEventArgs e)
        {
            msg m = e.Message;
            if (m == null || m.Output == null) return;
            string s = string.Empty;

            switch (m.API)
            {
                case _API.CRAWLER:
                    switch (m.KEY)
                    {
                        case _API.CRAWLER_KEY_REQUEST_LINK:
                            oLink link = (oLink)m.Input;
                            ui_log_Text.crossThreadPerformSafely(() =>
                            {
                                ui_log_Text.Text += Environment.NewLine + m.Log + Environment.NewLine;
                            });
                            break;
                        case _API.CRAWLER_KEY_COMPLETE:
                            ui_log_Text.crossThreadPerformSafely(() =>
                            {
                                ui_log_Text.Text += Environment.NewLine + m.Log + Environment.NewLine;
                            });
                            break;
                    }
                    break;
                case _API.YOUTUBE:
                    switch (m.KEY) {
                        case _API.YOUTUBE_INFO:

                            break;
                    }
                    break;
            }
        }

        public void api_initMsg(msg m)
        {
            if (m == null || m.Output == null) return;
            string s = string.Empty;

            switch (m.API)
            {
                case _API.SETTING_APP:
                    switch (m.KEY)
                    {
                        case _API.SETTING_APP_KEY_INT:
                            f_tag_package_Binding(api_settingApp.get_package());
                            f_tag_book_Binding(api_settingApp.get_book());
                            break;
                    }
                    break;
            }
        }
        #endregion
    }
}
