using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace DocAcrobat
{
    public partial class Form1 : Form
    {
        double progess = 0;
        double file = 0;

        public Form1()
        {
            InitializeComponent();
            ConvertButton.Enabled = false;

            InitializeBackgroundWorker();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                PathBox.Text = folderBrowserDialog1.SelectedPath;
                ConvertButton.Enabled = true;
            }
        }

        public static bool PrepareWordAutomation()
        {
            ////Wenn man das Programm abbricht, bleiben leider manchmal Wordinstanzen aktiv, die dann Dokumente blokieren. Deswegen kann man das ruig öfter laufen lassen.
            //if (Process.GetProcessesByName("winword").Any())
            //{
  
            //    if (Process.GetProcessesByName("winword").Any())
            //    {
            //        var answer = Helper.SayYesNo("Also es scheint immernoch Word zu laufen. Veilleicht ist das ein verwaister Prozess. Wenn du dir sicher bist, dass du Word nicht laufen hast, dann mach einfach weiter. Weitermachen?");
            //        if (answer == System.Windows.MessageBoxResult.No)
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }

        public static void DocToPdf(string path, Microsoft.Office.Interop.Word.Application appWORD)
        {
            string dir = Path.GetDirectoryName(path) + @"\";
            var file = Path.GetFileName(path);
            var pdfDirectory = Directory.CreateDirectory(dir + @"pdf\").FullName;
            var pdfFile = file.Replace(".docx", ".pdf");
            Document wordDocument = appWORD.Documents.Open(path, ConfirmConversions: false, ReadOnly: true, AddToRecentFiles: false, Visible: true);
            wordDocument.ExportAsFixedFormat((pdfDirectory + pdfFile), WdExportFormat.wdExportFormatPDF);
            wordDocument.Close(false);
            wordDocument = null;
            GC.Collect();
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy != true)
            {
                ConvertButton.Enabled = false;
                OpenButton.Enabled = false;
                StatusLabel.Text = "Konvertiere...";
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void Convert(string paths, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (PrepareWordAutomation())
            {
                var appWORD = new Microsoft.Office.Interop.Word.Application();
                appWORD.Visible = false;
                appWORD.ScreenUpdating = false;

                var files = System.IO.Directory.GetFiles(paths, "*.doc");
                var files2 = System.IO.Directory.GetFiles(paths, "*.docx");
                file = 100.0 / (files.Length + files2.Length);

                foreach (string path in files.Concat(files2))
                {
                    DocToPdf(path, appWORD);
                    progess += file;
                    worker.ReportProgress((int)Math.Round(progess));
                }

                appWORD.Quit(false);
                appWORD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            backgroundWorker.DoWork +=
                new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker_ProgressChanged);
        }

        private void backgroundWorker_DoWork(object sender,
            DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Convert(PathBox.Text, worker, e);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusLabel.Text = ("Das Konvertieren vieler Dateien kann eine Weile in Anspruch nehmen. Fortschritt: " + e.ProgressPercentage.ToString() + "%");
        }


        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                StatusLabel.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                StatusLabel.Text = "Error: " + e.Error.Message;
            }
            else
            {
                StatusLabel.Text = "Done!";
            }

            ConvertButton.Enabled = true;
            OpenButton.Enabled = true;
            progess = 0;
        }
    }
}
