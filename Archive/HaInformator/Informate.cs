using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace HaInformator
{
    public class Informate : HaControl
    {
        private ElementSelection _elementSelection;
        private AttrValSelection _attrValSelection;
        private ContextMenuSelection _cMenuSelection;
        private XDocument _doc;

        public Informate(Logger logger, 
                         TreeView tView, 
                         ListBox lView, 
                         ListBox attrLView, 
                         ListBox valLView, 
                         GroupBox gbox, 
                         ContextMenuStrip cMenu)
        {
            this.Description = "Tool zur Darstellung aller Nodes, ihrer Attribute und Werte";
            _cMenuSelection = new ContextMenuSelection(cMenu);
            _attrValSelection = new AttrValSelection(attrLView, valLView, gbox, _cMenuSelection);
            _elementSelection = new ElementSelection(tView, lView, _attrValSelection, _cMenuSelection);
        }

        public override HaControlResult Act(string filepath)
        {
            try
            {
                _doc = XDocument.Load(filepath, LoadOptions.PreserveWhitespace);
                _elementSelection.Load(new Tree(_doc));
                return HaControlResult.OK;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                return HaControlResult.Error;
            }
        }
    }
}
