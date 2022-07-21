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
    /// <summary>
    /// This is the code behind of the MainWindow control, 
    /// which presents the application main user interface and we want to work as a modeless dialog.
    /// Here we set methods that should be called by other classes to control the window activity.
    /// Here we also set the interactions between the user input through the window controls 
    /// and the classes that build the commands that Revit should receive as requests.
    /// </summary>
    public partial class TemplateManagerAppMVVM : Window, IDisposable
    {
        public static ObservableCollection<TemplateViewModel> TemplateViewModels { get; set; }
        public static TemplateViewModel SelectedTemplateViewModel { get; set; }

        public static UIDataViewModel UIDataViewModel { get; set; }
        public static TemplateManagerAppMVVM MainView { get; set; }

        public static ExternalEvent TemplateManagerAppEEH = ExternalEvent.Create(new TemplateManagerAppEEH());

        public static ExternalEvent CustomSaveFilePathEEH = ExternalEvent.Create(new CustomSaveFilePathEEH());

        public TemplateManagerAppMVVM()
        {
            TemplateViewModels = new ObservableCollection<TemplateViewModel>();
            SelectedTemplateViewModel = new TemplateViewModel();
            UIDataViewModel = new UIDataViewModel();
            MainView = this;
            InitializeComponent();
            InitializeCommands();
        }
        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
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
        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void Dispose()
        {
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            TemplateManagerAppEEH.Raise();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TemplateViewModels.Add(new TemplateViewModel { TemplateName = "Mecânica" });
            TemplateViewModels.Add(new TemplateViewModel { TemplateName = "Elétrica" });
            TemplateViewModels.Add(new TemplateViewModel { TemplateName = "Arquitetura" });
            TemplateViewModels.Add(new TemplateViewModel { TemplateName = "Estruturas" });

            UIDataViewModel.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Projeto_CheckBox.IsChecked = true;

            Templates_ComboBox.ItemsSource = TemplateViewModels;
            Templates_ComboBox.SelectedIndex = 0;
        }

        private void Projeto_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Template_CheckBox.IsChecked = false;
        }

        private void Projeto_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Template_CheckBox.IsChecked = true;
        }

        private void Template_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Projeto_CheckBox.IsChecked = false;
        }

        private void Template_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Projeto_CheckBox.IsChecked = true;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            CustomSaveFilePathEEH.Raise();
        }
    }

    public class TemplateViewModel : INotifyPropertyChanged
    {
        private string _TemplateName { get; set; }

        public string TemplateName
        {
            get { return _TemplateName; }
            set
            {
                _TemplateName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        private string _TemplateFilePath { get; set; }

        public string TemplateFilePath
        {
            get { return _TemplateFilePath; }
            set
            {
                _TemplateFilePath = value;
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
    public class UIDataViewModel : INotifyPropertyChanged
    {
        private string _FileName { get; set; }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        private string _FilePath { get; set; }

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
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
