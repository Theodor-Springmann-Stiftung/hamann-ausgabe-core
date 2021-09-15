using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HaInformator
{
    public class AttrValSelection
    {
        private static ListBox _attrLView;
        private static ListBox _valLView;
        private static GroupBox _gBox;

        private static ContextMenuSelection _cMenuSelection;

        private static SelectionState _state;
        private static AttributeTree _currAttributeTree;

        public AttrValSelection(ListBox attrLView, 
                                ListBox valLview, 
                                GroupBox gBox,
                                ContextMenuSelection contextMenuSelection)
        {
            _attrLView = attrLView;
            _valLView = valLview;
            _gBox = gBox;
            _cMenuSelection = contextMenuSelection;

            _attrLView.MouseDown += new MouseEventHandler(_attrLView_Select);
            _valLView.MouseDown += new MouseEventHandler(_valLView_Select);

            _cMenuSelection.aNewBtn_Click =_aAddAttributeBtn_Click;
            _cMenuSelection.aEditBtn_Click = _aEditBtn_Click;
            _cMenuSelection.aDeleteBtn_Click = _aDeleteBtn_Click;

            _cMenuSelection.vEditBtn_Click = _vEditBtn_Click;
        }

        private void _aAddAttributeBtn_Click(object sender, EventArgs e)
        {
            AddAttribute();
        }

        public void AddAttribute()
        {
            var str = "name";
            var value = "wert";
            if (_cMenuSelection.ShowDoubleInputDialog(ref str, ref value) == DialogResult.OK)
                _state.AddAttribute(str, value);
            Load(_state);
        }

        private void _aEditBtn_Click(object sender, EventArgs e)
        {
            EditAttribute();
        }

        private void EditAttribute()
        {
            var str = (String)_attrLView.SelectedItem;
            if (_cMenuSelection.ShowInputDialog(ref str) == DialogResult.OK)
                _state.EditAttribute((String)_attrLView.SelectedItem, str);
            Load(_state);
        }

        private void _vEditBtn_Click(object sender, EventArgs e)
        {
            EditValue();
        }

        private void EditValue()
        {
            var str = (String)_valLView.SelectedItem;
            if (_cMenuSelection.ShowInputDialog(ref str) == DialogResult.OK)
                _state.EditValue((String)_attrLView.SelectedItem, (String)_valLView.SelectedItem, str);
            Load(_state);
        }

        private void _aDeleteBtn_Click(object sender, EventArgs e)
        {
            RemoveAttribute();
        }

        private void RemoveAttribute()
        {
            _state.RemoveAttribute((string)_attrLView.SelectedItem);
            Load(_state);
        }

        private void _attrLView_Select(object sender, MouseEventArgs e)
        {
            var node = _attrLView.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));

            if (node == -1)
            {
                _valLView.Items.Clear();
                return;
            }
            else
                _attrLView.SelectedIndex = node;

            _update_valLView();

            if (e.Button == MouseButtons.Right)
                _attrLView.ContextMenuStrip = _cMenuSelection.Open(cMenuState.aView);
        }

        private void _valLView_Select(object sender, MouseEventArgs e)
        {
            var node = _valLView.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));

            if (node == null || node == -1)
            {
                return;
            }
            else
                _valLView.SelectedIndex = node;


            if (e.Button == MouseButtons.Right && _valLView.Items.Count > 0)
                _valLView.ContextMenuStrip = _cMenuSelection.Open(cMenuState.vView);
        }
        
        public void Load(SelectionState state, string description = "")
        {
            _state = state;
            _currAttributeTree = _state.GetAttributeTree();
            if (!String.IsNullOrWhiteSpace(description))
                _gBox.Text = description;
            _draw();
        }

        private void _draw()
        {
            _update_attrLView();
            _update_valLView();
        }

        private void _update_attrLView()
        {
            _attrLView.Items.Clear();
            if (_currAttributeTree != null && _currAttributeTree.Any())
                _attrLView.Items.AddRange(_currAttributeTree.Keys());
            if (_attrLView.Items.Count > 0)
            {
                _attrLView.Enabled = true;
                _attrLView.SelectedIndex = 0;
            }
            else
                _attrLView.Enabled = false;
            _attrLView.Update();
        }

        private void _update_valLView()
        {
            _valLView.Items.Clear();
            if (_currAttributeTree != null && _attrLView.Items.Count > 0)
                _valLView.Items.AddRange(_currAttributeTree.GetValue((String)_attrLView.SelectedItem).Keys.ToArray());
            if (_valLView.Items.Count > 0)
            {
                _valLView.Enabled = true;
            }
            else
                _valLView.Enabled = false;
            _valLView.Update();
        }
    }
}
