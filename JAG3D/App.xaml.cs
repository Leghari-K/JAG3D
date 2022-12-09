using System;
using System.Windows;

namespace JAG3D
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            DebugWindow debugWindow = new DebugWindow();
            debugWindow.Show();
        }


    }
}
