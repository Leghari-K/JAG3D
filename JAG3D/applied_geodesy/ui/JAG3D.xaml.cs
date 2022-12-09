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
using System.Windows.Shapes;

namespace JAG3D.applied_geodesy.ui
{
    /// <summary>
    /// Interaction logic for JAG3D.xaml
    /// </summary>
    public partial class JAG3D : Window
    {
        public JAG3D()
        {
            InitializeComponent();
            this.Title = "JAG3D . Least-Squares Adjustment & Deformation Analysis .";
        }
    }
}