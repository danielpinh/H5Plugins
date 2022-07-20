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
    public partial class ScheduleExportMechanicalMVVM : Window , IDisposable
    {
        private static ExternalEvent scheduleExportMechanicalSaveEEH = ExternalEvent.Create(new ScheduleExportMechanicalSaveEEH());      

        public ObservableCollection<ScheduleExportMechanicalViewModel> MainViewModel { get; set; }
        public ScheduleExportMechanicalViewModel ViewModel { get; set; }

        public static ScheduleExportMechanicalMVVM MainView { get; set; }

        public ScheduleExportMechanicalMVVM(ExternalEvent externalEvent)
        {
            MainViewModel = new ObservableCollection<ScheduleExportMechanicalViewModel>();
            ViewModel = new ScheduleExportMechanicalViewModel();
            MainView = this;            
            externalEvent.Raise();            
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
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{            
        //    e.Cancel = true;
        //    this.Visibility = System.Windows.Visibility.Hidden;
        //}        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            scheduleExportMechanicalSaveEEH.Raise();
        }

        private void ClearSelectionButton_Checked(object sender, RoutedEventArgs e)
        {           
            foreach (var item in MainViewModel)
            {
                item.IsCheckedScheduleName = false;
            }

            SelectAllButton.IsChecked = false;
        }

        private void SelectAllButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                item.IsCheckedScheduleName = true;
            }
            ClearSelectionButton.IsChecked = false;
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
              this.Close();
        }           

        private void scheduleExportListBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                if (item.IsCheckedScheduleName == false)
                {
                    SelectAllButton.IsChecked = false;
                }
            }
        }

        private void scheduleExportListBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                if (item.IsCheckedScheduleName == false)
                {
                    SelectAllButton.IsChecked = false;
                }
            }
        }

        private void scheduleExportListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in MainViewModel)
            {
                if (item.IsCheckedScheduleName == false)
                {
                    SelectAllButton.IsChecked = false;
                }
            }
        }
    }
    public class ScheduleExportMechanicalViewModel : INotifyPropertyChanged
    {      
        private string scheduleName { get; set; }
        private bool isCheckedScheduleName { get; set; }

        private string myReviewValue;

        private string myDocumentValue;
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
        public string MyDocumentValue
        {
            get { return myDocumentValue; }
            set
            {
                myDocumentValue = value;
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




