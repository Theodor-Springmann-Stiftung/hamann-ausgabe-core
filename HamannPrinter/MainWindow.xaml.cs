using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using static HamannPrinter.XMLMerger;

namespace HamannPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public MainWindow()
        {
            InitializeComponent();
            RegisterDocs.IsChecked = false;
            VolumeDocs.IsChecked = false;
            StartYearTextBox.Text = "1751";
            EndYearTextBox.Text = "1764";
            XmlFileBox.Text = @"C:\Users\simon\Desktop\Printer\XML"; // DEV
            OutputDirBox.Text = @"C:\Users\simon\Desktop\Printer\Ausg"; // DEV
        }

        private void SingleDocChanged(object sender, RoutedEventArgs e)
        {
            /*Wenn das Erstellen von Einzelbrief.Worddokumenten nicht ausgewählt ist, wird das Feld für das Erstellen von entpsrechenden Pdf Dateien deaktiviert.*/
        }

        private void VolDocChanged(object sender, RoutedEventArgs e)
        {
            /*Wenn das Erstellen von Band.Worddokumenten nicht ausgewählt ist, wird das Feld für das Erstellen von entpsrechenden Pdf Dateien deaktiviert.*/
        }

        private void CommDocChanged(object sender, RoutedEventArgs e)
        {
            /*Wenn das Erstellen von Kommentar.Worddokumenten nicht ausgewählt ist, wird das Feld für das Erstellen von entpsrechenden Pdf Dateien deaktiviert.*/
  
        }

        private void XmlFileButton_Click(object sender, RoutedEventArgs e)
        {
            /*Wenn der such button für die xml-quell-datein ausgewählt wird, wird ein entsprechende 
             * windows forms dialog gestartet. Das Ergebenis wird in der nebenstehenden Textbox angezeigt.*/
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    XmlFileBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void OutputDirButton_Click(object sender, RoutedEventArgs e)
        {/*wenn der suchen button für das ausgabe verzeichnis geklickt wird, startet ein entsprechender windows forms dialog.
            das ergebnis der user eingaben wird in der nebenstehenden textbox gespeichert.*/
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    OutputDirBox.Text = dialog.SelectedPath;
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*Routine für "Dokumente ezeugen"-Button */
            if (CheckInput())
            {
                /*startet Konsole für Fehlerausgabe. Die entpsrechenden Meldungen werden auch in einer Log-Datei im Ausgabeverzeichnis gespeichert.*/
                AllocConsole();
                this.IsEnabled = false;
                Confix confix = MakeConfix();
                if (!File.Exists(confix.HamannXmlPath))
                {
                    Warn("Upsi!\nDie erstellte Datei \"" + confix.HamannXmlPath + "\" existiert nicht.");
                    this.IsEnabled = true;
                }
                else
                {
                    var parser = new Parser();
                    parser.MakeDocuments(confix);
                    this.IsEnabled = true;
                    //this.Close();
                }
            }
        }

        private bool CheckInput()
        {
            if (LetterDocs.IsChecked == true || VolumeDocs.IsChecked == true)
            {
                if (GetYears().Item1 <= GetYears().Item2)
                {
                    if (CheckPaths())
                        return true;
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Warn("Kein zu generierender Hamannband ausgewählt!");
                    return false;
                }
            }
            else if (RegisterDocs.IsChecked == true)
            {
                if (CheckPaths())
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                Warn("Kein zu generierender Dokumententyp ausgewählt!");
                return false;
            }
        }

        private bool CheckPaths()
        {
            string xmlPath = XmlFileBox.Text;
            string outPath = OutputDirBox.Text;
            if (!Directory.Exists(xmlPath))
            {
                Warn("Kein existierendes verzeichnis ausgewählt!");
                return false;
            }
            else
            {
                if (!Directory.Exists(outPath))
                {
                    Warn("Kein existierendes Ausgabeverzeichnis ausgewählt!");
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        private void Warn(string message)
        {
            System.Windows.MessageBox.Show(message,
                "Confirmation",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation
            );
        }

        private Confix MakeConfix()
        {
            var confix = new Confix()
            {
                Editionsrichtlinien = CheckForEditionsRichtlinien(),
                LettersDocx = LetterDocs.IsChecked,
                VolumeDocx = VolumeDocs.IsChecked,
                RegisterDocx = RegisterDocs.IsChecked,
                HamannXmlPath = GetHAMANNxml(),
                OutputPath = MakePath(OutputDirBox.Text),
                Years = GetYears(),
            };
            return confix;
        }

        string CheckForEditionsRichtlinien()
        {
            string path = MakePath(XmlFileBox.Text);
            string file = path + "Editionsrichtlinien.docx";
            int stupidCounter = 0;
            while (!File.Exists(file))
            {
                if (stupidCounter > 2)
                {
                    Warn("Das dauert jetzt zu lange. Um das noch einmal zusammenzufassen: in "+path+ " muss eine Datei namens \"Editionsrichtlinien.docx\" liegen, die hinten an die Banddokumente angefügt wird.\nDie ist nicht da oder hat einen falschen Namen oder etwas funktionier nicht. Wir beenden das besser.");
                    Application.Current.Shutdown();
                    System.Environment.Exit(0);
                }
                Warn("\"Editionsrichtlinien.docx\" im Verzeichnis "+path+" nicht gefunden. \nDiese Datei ist aber unbedingt nötig um Bände zu erstellen.\n(Du willst keine Bände erstellen? Pech, sie ist trotzdem nötig, weil Fritz nicht programmieren kann.)\nBitte die Datei in das Verzeichnis legen und dann auf \"Ok\" klicken.");
                ++stupidCounter;
            }
            return file;
        }

        string GetHAMANNxml()
        {
            string path = MakePath(XmlFileBox.Text);
            string file = path + "HAMANN.xml";
            if (File.Exists(file))
            {
                var answer = System.Windows.MessageBox.Show("HAMANN.xml gefunden. \nZuletzt bearbeitet: " + File.GetLastWriteTime(file) + "\n\nSoll diese Datei verwendet werden, ohne eine neue aus den Einzeldokumenten zusammenzusetzen?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes) return file;
                File.Delete(file);
            }
            return new Concatinator(MakePath(XmlFileBox.Text)).HamannXmlFile;
        }

        private string MakePath(string path)
        {
            if (!path.EndsWith(@"\")) path += @"\";
            return path;
        }
        
        public (int, int) GetYears()
        {
            return (int.Parse(StartYearTextBox.Text), int.Parse(EndYearTextBox.Text));
        }

        private void TextBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void EndYearTextBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }
    }
}
