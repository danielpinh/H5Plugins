using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

namespace H5Plugins
{    
    public partial class LookupTableMappingEditingMVVM : Window, IDisposable
    {
        public static LookupTableMappingEditingMVVM MainView { get; set; }
        public static LookupTableViewModel LookupTableViewModel { get; set; } = new LookupTableViewModel();
        public LookupTableMappingEditingMVVM()
        {
            LookupTableViewModel = LookupTableMappingMVVM.SelectedLookupTableViewModel;

            MainView = this;
            InitializeComponent();
            InitializeCommands();
            this.DataContext = this;
        }
        private void InitializeCommands()
        {
            this.ShowInTaskbar = true;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
        }
        public void Dispose()
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }      

        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DeleteLookup_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }  
}
