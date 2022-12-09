using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JAG3D.applied_geodesy.ui
{
    /// <summary>
    /// Interaction logic for MenuBuilder.xaml
    /// </summary>
    public partial class MenuBuilder : Page
    {
        public MenuBuilder()
        {
            InitializeComponent();
        }

        private void MenuItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("About Dialog");
        }
    }
}
