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
    public partial class LookupTableMappingMVVM : Window, IDisposable
    {
        public static LookupTableMappingMVVM MainView { get; set; }
        public static ObservableCollection<LookupTableViewModel> LookupTableViewModels { get; set; }
        public static ExternalEvent LookupTableMappingEEH { get; set; } = ExternalEvent.Create(new LookupTableMappingEEH());
        public static ExternalEvent NewLookupTableEEH { get; set; } = ExternalEvent.Create(new NewLookupTableEEH());
        public static LookupTableViewModel SelectedLookupTableViewModel { get; set; } = new LookupTableViewModel();
        public LookupTableMappingMVVM()
        {
            LookupTableViewModels = new ObservableCollection<LookupTableViewModel>();

            InitializeComponent();
            InitializeCommands();
            this.DataContext = this;
            MainView = this;

            LookupTables_ListBox.ItemsSource = LookupTableViewModels;
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
            LookupTableMappingEEH.Raise();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }      

        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void AddLookup_Button_Click(object sender, RoutedEventArgs e)
        {
            CreateNewLookupTable();
        }

        private void DeleteLookup_Button_Click(object sender, RoutedEventArgs e)
        {
            LookupTableMappingConfirmMVVM lookupTableMappingConfirmMVVM = new LookupTableMappingConfirmMVVM();
            lookupTableMappingConfirmMVVM.ShowDialog();
        }

        private void CreateNewLookupTable()
        {
            NewLookupTableEEH.Raise();
        }

        private void LookupTables_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedLookupTableViewModel = LookupTables_ListBox.SelectedItem as LookupTableViewModel;
            LookupColumn_TextBlock.DataContext = SelectedLookupTableViewModel;
            LookupDefault_TextBlock.DataContext = SelectedLookupTableViewModel;
            LookupCategory_TextBlock.DataContext = SelectedLookupTableViewModel;
            LookupFamilyType_TextBlock.DataContext = SelectedLookupTableViewModel;
            LookupParameter_TextBlock.DataContext = SelectedLookupTableViewModel;
        }

        private void EditLookup_Button_Click(object sender, RoutedEventArgs e)
        {
            LookupTableMappingEditingMVVM lookupTableMappingEditingMVVM = new LookupTableMappingEditingMVVM();
            lookupTableMappingEditingMVVM.Show();
        }
    }
    public class RvtFamilyCategory
    {
        public Category Category { get; }
        public string CategoryName { get; }

        public RvtFamilyCategory(Category category, string categoryName)
        {
            Category = category;
            CategoryName = categoryName;
        }
    }
    public class RvtFamilySymbol
    {
        public ElementType FamilySymbol { get; }
        public string FamilySymbolName { get; }

        public RvtFamilySymbol(ElementType familySymbol, string fSymbolName)
        {
            FamilySymbol = familySymbol;
            FamilySymbolName = fSymbolName;
        }
    }
    public class LookupTableViewModel : ViewBaseModel
    {
        public string LookupTableName { get; set; }
        public Parameter SelectedParameter { get; set; }
        public string SelectedParameterName { get; set; }
        public ObservableCollection<Parameter> Parameters { get; set; }
        public string LookupColumn { get; set; }
        public string Default { get; set; }
        public string LookupValue1 { get; set; }
        public string LookupValue2 { get; set; }
        public RvtFamilySymbol SelectedSymbol { get; set; }
        public string SelectedSymbolName { get; set; }
        public string SelectedCategoryName { get; set; }
        public RvtFamilyCategory SelectedCategory { get; set; }
        public ObservableCollection<RvtFamilyCategory> Categories { get; set; }
        public ObservableCollection<RvtFamilySymbol> Symbols { get; set; }
    }
}
