using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using H5Plugins;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace H5Plugins
{
    /// <summary>
    /// This is the class that will implement the IExternalEventHandler interface
    /// to handle all commands started by user action in the MainWindow (modeless dialog) 
    /// as Requests listed in an enumeration used by the Request class.
    /// Also here we will define all the methods that will build the application functionality using the Revit API.
    /// </summary>
    public class LinkParameterEEH : IExternalEventHandler
    {
        public Definition Definition = null;
        public ElementBinding Binding = null;        

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            BindingMap bindingMap = doc.ParameterBindings;
            DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();      

            iterator.Reset();
            
            //coletando todos os parâmetros no projeto
            while(iterator.MoveNext())
            {

                Binding binding = iterator.Current as Binding;
                string bindingName = iterator.Key.Name;               

                if (binding is InstanceBinding)
                {
                    LinkParameter.InstanceParameterNames.Add(bindingName);
                }
                else
                {
                    LinkParameter.TypeParameterNames.Add(bindingName);
                }
            }

            //populando a ListBox com as categorias existentes no projeto
            PopulateUICategoriesListBox(doc);
        }
        public void PopulateUICategoriesListBox (Document doc)
        {
            Categories categories = doc.Settings.Categories;

            foreach (Category cat in categories)
            {
                if (cat.AllowsBoundParameters)
                {
                    LinkParameterMVVM.MainView.CategoryMainViewModel.Add(new CategoryViewModel { 
                        CategoryName = cat.Name,
                        Category = cat,
                    });
                }
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }       
    }
    public class LinkParameterExecuteEEH : IExternalEventHandler
    {        
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            LinkParameter lp = new LinkParameter();

            if (LinkParameterMVVM.MainView.ParameterViewModel.IsTypeParameter == true)
            {
                lp.FamilySymbolLinkParameter(doc);
            }
            else if (LinkParameterMVVM.MainView.ParameterViewModel.IsInstanceParameter == true)
            {
                lp.FamilyInstanceLinkParameter(doc);
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
    public class LinkParameterBySelectionEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            LinkParameter lp = new LinkParameter();
            lp.FamilyBySelectionLinkParameter(doc, uidoc);
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}









