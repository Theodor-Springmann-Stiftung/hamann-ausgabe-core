using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace HaInformator
{
    public enum cMenuState
    {
        None,
        tView,
        lView,
        aView,
        vView
    }
    public class ContextMenuSelection
    {
        private static Dictionary<cMenuState, List<ToolStripItem>> _groups;
        public Action<object, EventArgs> tNewBtn_Click 
        {
            set 
            {
                _cMenu.Items["tNewBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> tEditBtn_Click
        {
            set
            {
                _cMenu.Items["tEditBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> tAddAttributeBtn_Click
        {
            set
            {
                _cMenu.Items["tAddAttributeBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> tDeleteBtn_Click
        {
            set
            {
                _cMenu.Items["tDeleteBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> tDeleteNodeKeepContentBtn_Click
        {
            set
            {
                _cMenu.Items["tDeleteNodeKeepContentBtn"].Click += new EventHandler(value);
            }
        }

        public Action<object, EventArgs> tSaveBtn_Click
        {
            set
            {
                _cMenu.Items["tSaveBtn"].Click += new EventHandler(value);
            }
        }

        public Action<object, EventArgs> lEditBtn_Click
        {
            set
            {
                _cMenu.Items["lEditBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> lAddAttributeBtn_Click
        {
            set
            {
                _cMenu.Items["lAddAttributeBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> lDeleteBtn_Click
        {
            set
            {
                _cMenu.Items["lDeleteBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> lDeleteNodeKeepContentBtn_Click
        {
            set
            {
                _cMenu.Items["lDeleteNodeKeepContentBtn"].Click += new EventHandler(value);
            }
        }

        public Action<object, EventArgs> aNewBtn_Click
        {
            set
            {
                _cMenu.Items["aNewBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> aEditBtn_Click
        {
            set
            {
                _cMenu.Items["aEditBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> aDeleteBtn_Click
        {
            set
            {
                _cMenu.Items["aDeleteBtn"].Click += new EventHandler(value);
            }
        }
        public Action<object, EventArgs> vEditBtn_Click
        {
            set
            {
                _cMenu.Items["vEditBtn"].Click += new EventHandler(value);
            }
        }

        private static ContextMenuStrip _cMenu;
        private static cMenuState _currState;

        public ContextMenuSelection(ContextMenuStrip cMenu)
        {
            _cMenu = cMenu;
            _createGroups();
        }

        private void _createGroups()
        {
            _groups = new Dictionary<cMenuState, List<ToolStripItem>>(5);
            _groups.Add(
                cMenuState.tView,
                new List<ToolStripItem>()
                {
                    _cMenu.Items["tHeader"],
                    _cMenu.Items["tNewBtn"], //
                    _cMenu.Items["tEditBtn"], //
                    _cMenu.Items["tAddAttributeBtn"], //
                    _cMenu.Items["tDeleteBtn"], //
                    // _cMenu.Items["tDeleteNodeKeepContentBtn"],
                    _cMenu.Items["tSaveBtn"]
                }
            );
            _groups.Add(
                cMenuState.lView,
                new List<ToolStripItem>()
                {
                    _cMenu.Items["lheader"],
                    _cMenu.Items["lEditBtn"], //
                    _cMenu.Items["lAddAttributeBtn"], //
                    _cMenu.Items["lDeleteBtn"], //
                   // _cMenu.Items["lDeleteNodeKeepContentBtn"] //
                }
            );
            _groups.Add(
                cMenuState.aView,
                new List<ToolStripItem>()
                {
                    _cMenu.Items["aheader"],
                    _cMenu.Items["aNewBtn"], //
                    _cMenu.Items["aEditBtn"], //
                    _cMenu.Items["aDeleteBtn"] //
                }
            );
            _groups.Add(
                cMenuState.vView,
                new List<ToolStripItem>()
                {
                    _cMenu.Items["vHeader"],
                    _cMenu.Items["vEditBtn"] //
                }
            );
        }

        public ContextMenuStrip Open(cMenuState state)
        {
            foreach (var i in _cMenu.Items)
            {
                var it = (ToolStripItem)i;
                if (_groups[state].Contains(i))
                    it.Enabled = true;
                else
                    it.Enabled = false;
            }

            return _cMenu;
        }


        public DialogResult ShowInputDialog(ref string input, string title = "")
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.Text = title;
            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public DialogResult ShowDoubleInputDialog(ref string input, ref string value, string title = "")
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 95);
            Form inputBox = new Form();

            inputBox.Text = title;
            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            System.Windows.Forms.TextBox textBox2 = new TextBox();
            textBox2.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox2.Location = new System.Drawing.Point(5, 33);
            textBox2.Text = input;
            inputBox.Controls.Add(textBox2);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 58);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 58);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            value = textBox2.Text;
            return result;
        }

        public DialogResult ShowSaveFileDialog(ref string path)
        {
            SaveFileDialog sFileD = new SaveFileDialog();
            sFileD.DefaultExt = "xml";
            sFileD.Filter = "XML-Datei|*.xml";

            var res = sFileD.ShowDialog();
            path = sFileD.FileName;
            return res;
        }
    }
}
