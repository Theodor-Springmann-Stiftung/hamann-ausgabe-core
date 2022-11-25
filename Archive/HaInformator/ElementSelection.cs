using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaInformator
{
    public class ElementSelection
    {
        private static TreeView _tView;
        private static TreeNodeCollection _trees;
        private static ListBox _lView;

        private static AttrValSelection _attrValSelection;
        private static ContextMenuSelection _cMenuSelection;

        private static FlatCollection _flats; // Liste aller Elemente
        private SelectionState _state; // Ausgewählte BaumElemente

        public ElementSelection(TreeView tView, 
                                ListBox lView, 
                                AttrValSelection selection,
                                ContextMenuSelection cMenuSelection)
        {
            _tView = tView;
            _lView = lView;
            _trees = tView.Nodes;
            _attrValSelection = selection;
            _cMenuSelection = cMenuSelection;

            _tView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._tView_Select);
            _lView.MouseDown += new MouseEventHandler(this._lView_Select);

            _cMenuSelection.tNewBtn_Click = _tNewBtn_Click;
            _cMenuSelection.tEditBtn_Click = _tEditBtn_Click;
            _cMenuSelection.tAddAttributeBtn_Click = _tlAddAttributeBtn_Click;
            _cMenuSelection.tDeleteBtn_Click = _tlDeleteBtn_Click;
            _cMenuSelection.tSaveBtn_Click = _tSaveBtn_Click;

            _cMenuSelection.lEditBtn_Click = _lEditBtn_Click;
            _cMenuSelection.lAddAttributeBtn_Click = _tlAddAttributeBtn_Click;
            _cMenuSelection.lDeleteBtn_Click = _tlDeleteBtn_Click;
        }

        private void _tSaveBtn_Click(object sender, EventArgs e)
        {
            string filepath = "";
            if (_cMenuSelection.ShowSaveFileDialog(ref filepath) == DialogResult.OK)
                _state.SaveTree(filepath);
        }

        private void _tEditBtn_Click(object sender, EventArgs e)
        {
            _ = _EditBtnClick();
            _tView_Select((Tree)_tView.SelectedNode);
        }

        private void _lEditBtn_Click(object sender, EventArgs e)
        {
            var ret = _EditBtnClick();
            _lView_Select(ret);
        }

        private string _EditBtnClick()
        {
            _tView.LabelEdit = true;
            var str = _state.Name;
            if (_cMenuSelection.ShowInputDialog(ref str, "Tag <" + _state.Name + "> bearbeiten") == DialogResult.OK)
                _state.ChangeName(str);
            _tView.LabelEdit = false;
            _lView_Draw();
            return str;
        }

        private void _tlAddAttributeBtn_Click(object sender, EventArgs e)
        {
            _AddAttributeBtn_Click();
        }

        private void _AddAttributeBtn_Click()
        {
            _tView.LabelEdit = true;
            _attrValSelection.AddAttribute();
            _tView.LabelEdit = false;
        }

        private void _tlDeleteBtn_Click(object sender, EventArgs e)
        {
            _tView.LabelEdit = true;
            _state.RemoveElement();
            _tView.LabelEdit = false;
            _lView_Draw();
        }

        private void _tNewBtn_Click(object sender, EventArgs e)
        {
            _tView.LabelEdit = true;
            var str = "";
            if (_cMenuSelection.ShowInputDialog(ref str, "Neuen Tag unter <" + _state.Name + "> einfügen" ) == DialogResult.OK)
                _state.AddElement(str, "");
            _tView.LabelEdit = false;
            _lView_Draw();
        }

        private void _lView_Draw()
        {
            _lView.Items.Clear();
            _flats = new FlatCollection(_trees);
            if (_flats.Keys() != null)
                _lView.Items.AddRange(_flats.Keys());
        }

        private void _tView_Select(object sender, TreeNodeMouseClickEventArgs e)
        {
            _tView.SelectedNode = e.Node;

            _tView_Select((Tree)e.Node);

            if (e.Button == MouseButtons.Right)
                e.Node.ContextMenuStrip = _cMenuSelection.Open(cMenuState.tView);
        }

        private void _lView_Select(object sender, MouseEventArgs e)
        {
            var node = _lView.IndexFromPoint(new Point(e.X, e.Y));

            if (node == -1)
                return;

            //if (node != _lView.SelectedIndex)
                _lView_Select((String)_lView.Items[node]);

            if (e.Button == MouseButtons.Right)
                _lView.ContextMenuStrip = _cMenuSelection.Open(cMenuState.lView);
        }

        private void _tView_Select(Tree tree)
        {
            _lView.ClearSelected();
            if (_state != null)
                _state.Unmark();

            var desc = "Arribute von " + _tView.SelectedNode.Text.ToUpper() + "-Tags in " + _tView.SelectedNode.FullPath;
            _state = new SelectionState(tree);
            _selectNode(desc);
        }

        private void _lView_Select(string sItem)
        {
            _lView.SelectedItem = sItem;

            _tView.CollapseAll();
            if (_state != null)
                _state.Unmark();

            _tView.BeginUpdate();
            _state = new SelectionState(_flats.GetTrees(sItem));

            _tView.EndUpdate();
            _selectNode("Atrribute aller " + sItem + "-Tags");
            _state.Mark();
        }

        public void Load(Tree tree)
        {
            _state = null;
            _trees.Add(tree);
            _lView_Draw();
        }

        private void _selectNode(string desc)
        {
            _attrValSelection.Load(_state, desc);
        }
    }
}
