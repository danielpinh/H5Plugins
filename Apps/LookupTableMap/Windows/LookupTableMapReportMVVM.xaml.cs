using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class LookupTableMapReportMVVM : Window, IDisposable
    {
        public static ObservableCollection<ReportViewModel> ReportViewModels { get; set; } = new ObservableCollection<ReportViewModel>();
        public static LookupTableMapReportMVVM MainView { get; set; }
        public LookupTableMapReportMVVM()
        {
            MainView = this;
            DataContext = this;

            InitializeComponent();
            InitializeCommands();

            Icon = PngImageSource("H5Plugins.Resources.Head5.png");
        }
        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }
        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDatagrid.ItemsSource = ReportViewModels;
        }
    }

    public class ReportViewModel : INotifyPropertyChanged
    {
        private string _FamilyName { get; set; }
        public string FamilyName
        {
            get { return _FamilyName; }
            set
            {
                _FamilyName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        private string _FamilyId { get; set; }
        public string FamilyId
        {
            get { return _FamilyId; }
            set
            {
                _FamilyId = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        private string _Result { get; set; }
        public string Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
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
