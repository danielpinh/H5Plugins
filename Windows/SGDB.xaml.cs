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
using Autodesk.Revit.UI;


namespace H5Plugins
{
    /// <summary>
    /// This is the code behind of the MainWindow control, 
    /// which presents the application main user interface and we want to work as a modeless dialog.
    /// Here we set methods that should be called by other classes to control the window activity.
    /// Here we also set the interactions between the user input through the window controls 
    /// and the classes that build the commands that Revit should receive as requests.
    /// </summary>
    public partial class DetalhesTipicosWindow : Window
    {

        ExternalEvent externalEvent = ExternalEvent.Create(new DTExternalEventHandler());
        public DetalhesTipicosWindow()
        {
            /* The initialization of the MainWindow will be called by the ShowWindow method in the ExternalApplication class, executed by the ExternalCommand class,
             and will take the RequestHandler and the ExternalEvent already instantiated by the application as arguments. */

            InitializeComponent();
            InitializeCommands();

            button.Click += comando01;

        }
        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }
       
        private void comando01(object sender, RoutedEventArgs e)
        {            
            externalEvent.Raise();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
