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
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GalaSoft.MvvmLight;

namespace H5Plugins
{
    public partial class ScheduleExportMVVM : Window, IDisposable
    {            
        public static ScheduleExportMVVM MainView { get; set; } = new ScheduleExportMVVM();
        readonly ExternalEvent scheduleExportEvent = ExternalEvent.Create(new ScheduleExportEEH());
        readonly ExternalEvent scheduleExportSaveEvent = ExternalEvent.Create(new ScheduleExportSaveEEH());
        public ObservableCollection<ScheduleExportViewModel> MainViewModel { get; set; }
        public ScheduleExportViewModel ViewModel { get; set; }
        
        public ScheduleExportMVVM()
        {            
            
            MainViewModel = new ObservableCollection<ScheduleExportViewModel>();            
            ViewModel = new ScheduleExportViewModel();            
            scheduleExportEvent.Raise();
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            scheduleExportSaveEvent.Raise();           
        }

        private void ClearSelectionButton_Checked(object sender, RoutedEventArgs e)
        {           
            foreach (var item in MainViewModel)
            {
                item.IsCheckedScheduleName = false;
            }           
        }

        private void SelectAllButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                item.IsCheckedScheduleName = true;
            }
        }

        private void SelectAllButton_UnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                item.IsCheckedScheduleName = false;
            }
        }
        private void ScheduleExportMVVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }
        public void Dispose()
        {
            scheduleExportSaveEvent.Dispose();
            scheduleExportEvent.Dispose();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            scheduleExportSaveEvent.Dispose();
            scheduleExportEvent.Dispose();
            ViewModel.ScheduleName = null;
            ViewModel.MyReviewValue = null;
            ViewModel.MyReviewValue = null;
        }
    }

    public class ScheduleExportViewModel : INotifyPropertyChanged
    {      
        private string scheduleName { get; set; }
        private bool isCheckedScheduleName { get; set; }

        private string myReviewValue;


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
       
        public bool IsCheckedScheduleName
        {
            get { return isCheckedScheduleName; }
            set
            {
                isCheckedScheduleName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string MyReviewValue
        {
            get { return myReviewValue; }
            set
            {
                myReviewValue = value;
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




