using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class OrdenarCortes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<View> viewsDrafing = new List<View>();

            try
            {
                ////Collect all sections in active document
                //FilteredElementCollector collectorSections = new FilteredElementCollector(doc);                        
                //ElementCategoryFilter filterSections = new ElementCategoryFilter(BuiltInCategory.OST_Views);

                //collectorSections.WherePasses(filterSections)
                //    .WhereElementIsNotElementType()
                //    .ToElements();

                //IEnumerable<View> sectionViews =
                //          from View vs in collectorSections
                //          where vs.ViewType.ToString().Equals("Section")
                //          where vs.IsTemplate == false
                //          select vs;

                //Collect all sheets in active document and sort them by sheet number

                Dictionary<int, ViewSheet> viewSheetsbynumber = new Dictionary<int, ViewSheet>();                                        

                FilteredElementCollector collectorSheets = new FilteredElementCollector(doc);
                ElementCategoryFilter filterSheets = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);

                collectorSheets.WherePasses(filterSheets)
                    .WhereElementIsNotElementType()
                    .ToElements();

                foreach (ViewSheet sheet in collectorSheets)
                {
                    int sheetNumber = int.Parse(sheet.SheetNumber);
                    viewSheetsbynumber.Add(sheetNumber, sheet);
                }

                List<ISet<ElementId>> viewId = new List<ISet<ElementId>>(); 

                foreach (KeyValuePair<int, ViewSheet> dicKeys in viewSheetsbynumber.OrderBy(key => key.Key))
                {
                    viewId.Add(dicKeys.Value.GetAllPlacedViews());                    
                }

                List<ElementId> sectionsId = new List<ElementId>();

                foreach (HashSet<ElementId> hashSet in viewId)
                {
                    foreach (ElementId eleid in hashSet)
                    {
                        sectionsId.Add(eleid);
                    }
                    
                }

                List<Element> sectionViews = new List<Element>();

                foreach (ElementId item in sectionsId)
                {
                    sectionViews.Add(doc.GetElement(item));
                }

                char sectionLetter = 'A';

                foreach (View item in sectionViews)
                {                                        
                    using (Transaction trans = new Transaction(doc, "Criar folhas"))
                    {
                        trans.Start();
                        if (item.ViewType.ToString().Equals("Section"))
                        {
                            Parameter param1 = item.LookupParameter("Nome da vista");
                            Parameter param2 = item.LookupParameter("Número de detalhe");
                            Parameter param3 = item.LookupParameter("Título da página");
                            param1.Set($"CORTE " + sectionLetter.ToString() + "-" + sectionLetter.ToString());
                            param2.Set(sectionLetter.ToString());
                            param3.Set("CORTE");
                            sectionLetter++;                
                        }                      
                        trans.Commit();
                    }
                }                 
             
            }
            finally
            {

            }
            return Result.Succeeded;
        }
       
    }
   
}