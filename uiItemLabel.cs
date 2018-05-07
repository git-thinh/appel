using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace appel
{
    public class uiItemLabel : Panel
    {
        private IconButton _icon;
        private Label _text;
        private Label _state;
        private IconButton _exit;
        private oNode _node;
        private oNode[] items = new oNode[] { }; 
        private bool isDetailOpened = false;

        FlowLayoutPanel _detail;

        int wi_text = 0, wi_header = 0;
        public uiItemLabel(oNode node, IconType iconType = IconType._none)
        {
            _node = node;
            string text = node.title;

            _detail = new FlowLayoutPanel()
            {
                Visible = false,
                AutoScroll = true,
                //Dock = DockStyle.Fill,
                BackColor = Color.White,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = true,
                Padding = new Padding(7, 0, 0, 0),
                RightToLeft = RightToLeft.Yes,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0),
            };
            _exit = new IconButton() { IconType = IconType.close_circled, Visible = false };
            _text = new Label() { Text = text, AutoSize = true, TextAlign = ContentAlignment.TopLeft };
            _state = new Label() { Dock = DockStyle.Left, AutoSize = false, BackColor = Color.Orange, Width = 1 };

            using (Graphics g = CreateGraphics())
            {
                SizeF size = g.MeasureString(text, _text.Font);
                wi_text = (int)Math.Ceiling(size.Width);
            }

            if (iconType != IconType._none)
            {
                _icon = new IconButton(19) { IconType = iconType, Location = new Point(3, 5), Width = 19 };
                this.Controls.Add(_icon);

                wi_header = wi_text + _icon.Width + _state.Width;
                _text.Location = new Point(_icon.Width + _state.Width, 5);
            }
            else
            {
                wi_header = wi_text + _state.Width;
                _text.Location = new Point(_state.Width, 5);
            }

            this.Width = wi_header;

            this.Controls.AddRange(new Control[] { _text, _state, _exit, _detail });
            _text.Click += _text_Click;
            _exit.Click += _text_Click;
            this.Click += _text_Click;

            this.HandleCreated += (se, ev) =>
            { 
                if (wi_header > this.Parent.Width)
                {
                    wi_header = this.Parent.Width;
                    this.Width = wi_header;
                }
            };
        }

        private void _text_Click(object sender, EventArgs e)
        {
            switch (_node.type)
            {
                case oNodeType.PACKAGE:
                    #region [ PACKAGE ]
                    if (isDetailOpened)
                        detail_Hide();
                    else
                    {
                        if (items.Length == 0)
                        {
                            if (File.Exists(_node.path))
                            {
                                var dicItems = new Dictionary<string, string>();
                                using (var fileStream = File.OpenRead(_node.path))
                                    items = Serializer.Deserialize<Dictionary<string, string>>(fileStream)
                                        .Select(x => new oNode()
                                        {
                                            anylatic = false,
                                            content = x.Value,
                                            name = x.Key,
                                            path = _node.path,
                                            title = x.Value.Split(new char[] { '\r', '\n' })[0],
                                            type = oNodeType.PACKAGE_ARTICLE
                                        }).ToArray();

                            }
                        }
                        if (items.Length > 0)
                        {
                            detail_Show();
                            _detail.Controls.Clear();
                            for (int i = 0; i < items.Length; i++)
                            {
                                var lbl = new Label()
                                {
                                    Text = "• " + items[i].title,
                                    AutoSize = true,
                                    //BackColor = Color.WhiteSmoke,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                    Margin = new Padding(7, 5, 1, 5),
                                    Padding = new Padding(1, 3, 1, 3),
                                    BorderStyle = BorderStyle.None,
                                    RightToLeft = RightToLeft.No,
                                    Tag = items[i],
                                };
                                lbl.Click += (se, ev) =>
                                {
                                    lbl.BackColor = Color.Orange;
                                    ((fMain)this.FindForm()).f_doc_viewContent((oNode)((Label)se).Tag);
                                };
                                _detail.Controls.Add(lbl);
                            }
                        }
                    }
                    #endregion
                    break;
                default:
                    this.BackColor = Color.Orange;
                    ((fMain)this.FindForm()).f_doc_viewContent(_node);
                    break;
            }
        } //end

        void detail_Show()
        {
            isDetailOpened = true;

            foreach (Control ci in this.Parent.Controls)
                ci.Visible = false;

            this.Visible = true;
            ((Panel)this.Parent).AutoScroll = false;
            this.Tag = this.Location;
            this.Width = this.Parent.Width - 15;
            this.Height = this.Parent.Height - 15;
            this.Location = new Point(0, 0);

            _detail.Location = new Point(0, 24);
            _detail.Width = this.Width;
            _detail.Height = this.Height - 24;
            _detail.Visible = true;
            //_detail.BackColor = Color.Green;
            _detail.BringToFront();

            if (_icon != null)
            {
                _icon.InActiveColor = Color.Orange;
            }

            _exit.Location = new Point(_detail.Width - 25, 3);
            _exit.Visible = true;

        } // end

        void detail_Hide()
        {
            isDetailOpened = false;

            foreach (Control ci in this.Parent.Controls)
                ci.Visible = true;

            this.Location = (Point)this.Tag;
            _detail.Visible = false;
            ((Panel)this.Parent).AutoScroll = true;

            if (_icon != null)
            {
                this.Width = _text.Width + _icon.Width + _state.Width;
            }
            else
            {
                this.Width = _text.Width + _state.Width;
            }

            this.Height = 24; 
        }// end

    }
}
