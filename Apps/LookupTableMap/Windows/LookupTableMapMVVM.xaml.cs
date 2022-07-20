using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class LookupTableMapMVVM : Window, IDisposable
    {
        public static ProgressBarViewModel ProgressBarViewModel { get; set; }
        public bool IsClosed { get; private set; }

        private LookupTableMapEEH m_Handler;
        private ExternalEvent m_ExEvent;
        public static LookupTableMapMVVM MainView { get; set; }
        public LookupTableMapMVVM (ExternalEvent exEvent, LookupTableMapEEH handler)
        {
            /* The initialization of the MainWindow will be called by the ShowWindow method in the ExternalApplication class, executed by the ExternalCommand class,
             and will take the RequestHandler and the ExternalEvent already instantiated by the application as arguments. */

            ProgressBarViewModel = new ProgressBarViewModel();
            MainView = this;
            InitializeComponent();
            InitializeCommands();
            m_Handler = handler;
            m_ExEvent = exEvent;
            Icon = PngImageSource("H5Plugins.Resources.Head5.png");

            LookupTable_ProgressBar.Value = 0;
        }
        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }
       
        public static void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        private void Command01(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Leitos);
        }
        private void Command02(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Eletrocalhas);
        }
        private void Command03(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Eletrodutos);
        }
        private void Command04(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Perfilados);
        }
        private void Command05(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Dutos);
        }
        private void Command06(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Tubos);
        }
        private void Command07(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Sistemas);
        }
        private void Command08(object sender, RoutedEventArgs e)
        {
            MakeRequest(RequestId.Septos);
        }
        private void MakeRequest(RequestId request)
        {
            /* As seen above this method is used to handle and raise the commands requested by the user as external events for Revit. */

            m_Handler.Request.Make(request); // uses the Make method of the Request class instantiated in the RequestHandler class to identify the command started by the user
            m_ExEvent.Raise(); // raises the command requested as an external event for Revit and the Execute method in the handler can finally be done
        }
        public void Dispose()
        {
            this.Close();
        }
        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            return decoder.Frames[0];
        }

    }

    public class ProgressBarViewModel : INotifyPropertyChanged
    {
        private int _ProgressBarMaxValue { get; set; }

        public int ProgressBarMaxValue
        {
            get { return _ProgressBarMaxValue; }
            set
            {
                _ProgressBarMaxValue = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        private int _ProgressBarValue { get; set; }

        public int ProgressBarValue
        {
            get { return _ProgressBarValue; }
            set
            {
                _ProgressBarValue = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            System.Windows.Forms.Application.DoEvents();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;            
        }
    }

}
