using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaInformator
{
    
    public sealed class Logger
    {
        private static StringBuilder _sb;
        private static string _filepath = System.IO.Path.GetTempFileName();
        private static TextBox _textbox;

        public Logger() {
            _sb = new StringBuilder();
        }

        public Logger(TextBox box)
        {
            _textbox = box;
            _sb = new StringBuilder();
        }

        public static void AddTextBox(TextBox box)
        {
            _textbox = box;
        }

        public static void Log(string _msg)
        {
            _sb.Append(_msg);
            _textbox.AppendText(_msg);
        }

        public static void Save(string filepath)
        {
            try
            {
                _filepath = filepath;
                File.WriteAllText(_filepath, _sb.ToString());
            }
            catch (Exception e)
            {
                File.WriteAllText(System.IO.Path.GetTempFileName(), _sb.ToString() + "\n" + e.Message);
            }
        }
    }
}
