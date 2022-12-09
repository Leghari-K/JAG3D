using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
