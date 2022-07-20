using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace H5Plugins
{
    public class LookupTableMappingEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            var collectors = new Collectors();
            PopulateUICategoriesListBox(doc);
        }
        public void PopulateUICategoriesListBox(Document doc)
        {
            try
            {
                Categories categories = doc.Settings.Categories;

                ObservableCollection<RvtFamilyCategory> fCats = new ObservableCollection<RvtFamilyCategory>();

                foreach (Category cat in categories)
                {
                    RvtFamilyCategory fcat = new RvtFamilyCategory(cat, cat.Name);
                    fCats.Add(fcat);
                }

                List<ElementType> fSymbols = new FilteredElementCollector(doc)
                  .WhereElementIsElementType()
                  .OfCategory(BuiltInCategory.OST_CableTray)
                  .Cast<ElementType>()
                  .ToList();

                ObservableCollection<RvtFamilySymbol> rvtFamilySymbols = new ObservableCollection<RvtFamilySymbol>();

                foreach (var item in fSymbols)
                {
                    RvtFamilySymbol rvtFamilySymbol = new RvtFamilySymbol(item, item.Name);
                    rvtFamilySymbols.Add(rvtFamilySymbol);
                }

                LookupTableMappingMVVM.LookupTableViewModels.Add(new LookupTableViewModel {
                    LookupColumn = "{LookupColumn}",
                    Default = "{Default}",
                    LookupTableName = "{LookupName}",
                    LookupValue1 = "{LookupValue1}",
                    LookupValue2 = "{LookupValue2}",
                    Categories = null,
                    SelectedSymbol = null,
                    SelectedCategoryName = "{Category}",
                    SelectedSymbolName = "{FamilySymbol}",
                    SelectedParameterName = "{ParameterName}",
                    SelectedCategory = null,
                    Symbols = null,
                    Parameters = null,
                    SelectedParameter = null
                });              
            }
            catch (Exception ex)
            {
                TaskDialog.Show("debug", ex.Message);
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }

    public class NewLookupTableEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            var collectors = new Collectors();
            PopulateUICategoriesListBox(doc);
        }
        public void PopulateUICategoriesListBox(Document doc)
        {
            try
            {
                Categories categories = doc.Settings.Categories;

                ObservableCollection<RvtFamilyCategory> fCats = new ObservableCollection<RvtFamilyCategory>();

                foreach (Category cat in categories)
                {
                    RvtFamilyCategory fcat = new RvtFamilyCategory(cat, cat.Name);
                    fCats.Add(fcat);
                }

                List<ElementType> fSymbols = new FilteredElementCollector(doc)
                  .WhereElementIsElementType()
                  .OfCategory(BuiltInCategory.OST_CableTray)
                  .Cast<ElementType>()
                  .ToList();

                ObservableCollection<RvtFamilySymbol> rvtFamilySymbols = new ObservableCollection<RvtFamilySymbol>();

                foreach (var item in fSymbols)
                {
                    RvtFamilySymbol rvtFamilySymbol = new RvtFamilySymbol(item, item.Name);
                    rvtFamilySymbols.Add(rvtFamilySymbol);
                }

                LookupTableMappingMVVM.LookupTableViewModels.Add(new LookupTableViewModel
                {
                    LookupColumn = "{LookupColumn}",
                    Default = "{Default}",
                    LookupTableName = "TESTE",
                    LookupValue1 = "{LookupValue1}",
                    LookupValue2 = "{LookupValue2}",
                    Categories = null,
                    SelectedSymbol = null,
                    SelectedCategoryName = "{Category}",
                    SelectedSymbolName = "{FamilySymbol}",
                    SelectedParameterName = "{ParameterName}",
                    SelectedCategory = null,
                    Symbols = null,
                    Parameters = null,
                    SelectedParameter = null
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("debug", ex.Message);
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
