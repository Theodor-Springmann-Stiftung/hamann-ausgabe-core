using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HaInformator
{
    public partial class HaProgram : Form
    {
        private static Informate _informate;

        public HaProgram()
        {
            InitializeComponent();
            _informate = new Informate(new Logger(LogBox),
                                       NodeTree,
                                       NodeList,
                                       AttrList,
                                       ValList,
                                       ArribValDesc,
                                       cMenu);
            Description.Text = _informate.Description;
        }

        private void InFileBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                InFilepath.Text = openFileDialog.FileName;
        }

        private void GoBtn_Click(object sender, EventArgs e)
        {
            GoBtn.Enabled = false;
            if (_informate.Act(InFilepath.Text) == HaControl.HaControlResult.OK)
            {
                
            }
            GoBtn.Enabled = true;
        }

        private void SaveLogBtn_Click(object sender, EventArgs e)
        {

        }

        private void LogBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
