using System.IO;
using System.Windows;
using Path = System.IO.Path;

namespace JAG3D
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            var datumPath = Path.Combine("datum.txt");
            var levellingPath = Path.Combine("levelling.txt");

            string datum = File.ReadAllText(datumPath);
            string lavelling = File.ReadAllText(levellingPath);
            InitializeComponent();
            debugText.Text += "Load\n";
            debugText.Text += "Data Import...\n";
            debugText.Text += datum;
            debugText.Text += "\nObservation data\n";
            debugText.Text += lavelling;
        }
    }
}
