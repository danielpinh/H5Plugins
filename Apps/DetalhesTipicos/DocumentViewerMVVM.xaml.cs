using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class DocumentViewerMVVM : Window, IDisposable
    {

        public ObservableCollection<DocumentViewerViewModel> MainViewModel { get; set; }
        public static DocumentViewerMVVM MainView { get; set; }       

        public DocumentViewerMVVM()
        {
            MainView = this;            
            InitializeComponent();
            InitializeCommands();            
            this.DataContext = this;            
        }

        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }
        
        public void Dispose()
        {
            this.Close();
        }

        private void DetailsGenerateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class DocumentViewerViewModel : INotifyPropertyChanged
    {
        private string nomeDoDetalheTipico { get; set; }
        private bool ischeckedDetalheTipico { get; set; }
        
        public string NomeDoDetalheTipico
        {
            get { return nomeDoDetalheTipico; }
            set
            {
                nomeDoDetalheTipico = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public bool IsCheckedDetalheTipicoName
        {
            get { return ischeckedDetalheTipico; }
            set
            {
                ischeckedDetalheTipico = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }       

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}