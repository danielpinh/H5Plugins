using System;
using System.Collections.Generic;
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
    public partial class SupportManagerMVVM : Window    
    {

        public static SupportManagerMVVM MainView { get; set; } = new SupportManagerMVVM();
        readonly ExternalEvent supportManagerEvent = ExternalEvent.Create(new SupportManagerEEH());
        public SupportManagerViewModel ViewModel { get; set; }

        public SupportManagerMVVM()
        {
            ViewModel = new SupportManagerViewModel();
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

        private void InsertSupport_Click(object sender, RoutedEventArgs e)
        {
            supportManagerEvent.Raise();
        }
    }
    public class SupportManagerViewModel : INotifyPropertyChanged
    {
        private string scheduleName;

        private decimal maximumDistance;

        public string ScheduleName
        {
            get { return scheduleName; }
            set
            {
                scheduleName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        
        public decimal MaximumDistance
        {
            get { return maximumDistance; }
            set
            {
                maximumDistance = value;
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

