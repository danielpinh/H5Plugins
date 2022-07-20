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
    public partial class LinkParameterMVVM : Window , IDisposable
    {
        private static ExternalEvent LinkParameterEEH = ExternalEvent.Create(new LinkParameterExecuteEEH());
        private static ExternalEvent LinkParameterBySelectionEEH = ExternalEvent.Create(new LinkParameterBySelectionEEH());

        public ParameterViewModel ParameterViewModel { get; set; } = new ParameterViewModel();

        public ObservableCollection<ParameterViewModel> ParametersMainViewModel { get; set; } 
        public ObservableCollection<CategoryViewModel> CategoryMainViewModel { get; set; }

        public static LinkParameterMVVM MainView { get; set; }

        public LinkParameterMVVM(ExternalEvent externalEvent)
        {
            DataContext = this;
            MainView = this;
            ParametersMainViewModel = new ObservableCollection<ParameterViewModel>();
            CategoryMainViewModel = new ObservableCollection<CategoryViewModel>();
            externalEvent.Raise();
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
            this.Icon = H5AppExternalApplication.h5App.PngImageSource("H5Plugins.Resources.Head5.png");

        }
        public void Dispose()
        {     
              this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LinkParameterEEH.Raise();
        }

        private void SelectAllButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainView.ParametersMainViewModel)
            {
                try
                {
                    item.IsCheckedParameterName = true;
                }
                catch 
                {                   
                }
            }            
        }

        private void SelectAllButton_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in MainView.ParametersMainViewModel)
            {
                if (item.IsCheckedParameterName == true)
                {
                    item.IsCheckedParameterName = false;
                }
                else
                {
                    continue;
                }
            }            
        }

        private void ClearSelectionButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in ParametersMainViewModel)
            {
                item.IsCheckedParameterName = false;
            }

            SelectAllButton.IsChecked = false;
        }

        private void InstanceParameterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LinkParameterMVVM.MainView.ParametersMainViewModel.Clear();
            TypeParameterCheckBox.IsChecked = false;
            if (InstanceParameterCheckBox.IsChecked == true)
            {
                foreach (string paramName in LinkParameter.InstanceParameterNames)
                {
                    LinkParameterMVVM.MainView.ParametersMainViewModel.Add(new ParameterViewModel { ParameterName = paramName });
                }
            }
        }

        private void TypeParameterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LinkParameterMVVM.MainView.ParametersMainViewModel.Clear();
            InstanceParameterCheckBox.IsChecked = false;
            if (TypeParameterCheckBox.IsChecked == true)
            {                
                foreach (string paramName in LinkParameter.TypeParameterNames)
                {
                    LinkParameterMVVM.MainView.ParametersMainViewModel.Add(new ParameterViewModel { ParameterName = paramName });
                }
            }
        }

        private void InstanceParameterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LinkParameterMVVM.MainView.ParametersMainViewModel.Clear();
        }

        private void TypeParameterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LinkParameterMVVM.MainView.ParametersMainViewModel.Clear();
        }

        private void FindCategoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string currentText = FindCategoryTextBox.Text;
            string currenteTextUpper = currentText.ToUpper();
            string currenteTextLower = currentText.ToLower();

            var newFilter = from filter in CategoryMainViewModel
                              let categoryName = filter.CategoryName
                              where
                               categoryName.StartsWith(currenteTextLower)
                               || categoryName.StartsWith(currenteTextUpper)
                               || categoryName.Contains(currentText)
                              select filter;

           categoriesListBox.ItemsSource = newFilter;
        }

        private void SelectFamilies_Click(object sender, RoutedEventArgs e)
        {
            LinkParameterBySelectionEEH.Raise();
            this.Close();
        }
    }
    public class ParameterViewModel : INotifyPropertyChanged
    {      
        private string parameterName { get; set; }
        private bool isCheckedParameterName { get; set; }
        private bool isInstanceParameter { get; set; }
        private bool isTypeParameter { get; set; }

        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                parameterName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }    
        
        public bool IsCheckedParameterName
        {
            get { return isCheckedParameterName; }
            set
            {
                isCheckedParameterName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public bool IsInstanceParameter
        {
            get { return isInstanceParameter; }
            set
            {
                isInstanceParameter = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public bool IsTypeParameter
        {
            get { return isTypeParameter; }
            set
            {
                isTypeParameter = value;
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

    public class CategoryViewModel : INotifyPropertyChanged
    {
        private string categoryName { get; set; }
        private bool isCheckedCategoryName { get; set; }
        private string searchText { get; set; }
        private string categoriesSelected { get; set; }
        private Category category { get; set; }

        public Category Category
        {
            get { return category; }
            set
            {
                category = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public bool IsCheckedCategoryName
        {
            get { return isCheckedCategoryName; }
            set
            {
                isCheckedCategoryName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string CategoriesSelected
        {
            get { return categoriesSelected; }
            set
            {
                categoriesSelected = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
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




