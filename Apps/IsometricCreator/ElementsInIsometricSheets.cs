using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autodesk.Revit.ApplicationServices;


namespace H5Plugins
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class ElementsInIsometricSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {                 
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            //Collecting all sheets in project
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>();

            foreach (ViewSheet vs in collector)
            {
                
                //Get ElementId of sheet
                ElementId viewsheetId = vs.Id;
                //Get sheet name and sheet number
                Parameter sheetNameParam = vs.get_Parameter(BuiltInParameter.SHEET_NAME);
                Parameter sheetNumberParam = vs.get_Parameter(BuiltInParameter.SHEET_NUMBER);
                Parameter sheetClientNumberParam = vs.LookupParameter("Número do cliente");
                

                //Parse parameters to string
                string sheetNameParamString = sheetNameParam.AsString();                
                string sheetNumberParamString = sheetNumberParam.AsString();
                string sheetClientNumberParamString = sheetClientNumberParam.AsString();
                

                if (sheetNumberParamString.Contains("DI") || sheetNameParamString.Contains("DI"))
                {
                    //Get all elements placed in this viewsheet >> viewports
                    FilteredElementCollector eleCollec = new FilteredElementCollector(doc, viewsheetId);

                    foreach (Element item in eleCollec)
                    {
                        if (item is Viewport)
                        {
                            //Get viewports located in view
                            Viewport myViewport = item as Viewport;
                            ElementId myViewId = myViewport.ViewId;
                            Element myViewElement = doc.GetElement(myViewId);

                            //Get all elements in each viewport
                            IList<Element> eleCollec2 = new FilteredElementCollector(doc, myViewId)
                                .WhereElementIsNotElementType()
                                .ToElements();

                            foreach (var ele in eleCollec2)
                            {
                                try
                                {
                                    //Set values in the "H5 Isométrico" parameter
                                    Parameter isometricoParam = ele.LookupParameter("H5 Isométrico");
                                    if (isometricoParam != null)
                                    {
                                        using (Transaction t = new Transaction(doc, "Transaction"))
                                        {
                                            t.Start("Isométrico");
                                            isometricoParam.Set(sheetClientNumberParamString);
                                            t.Commit();
                                        }
                                    }
                                    else
                                    {
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    message = ex.Message;
                                }
                            }
                        }
                    }
                }
                else
                {
                    TaskDialog.Show("ERRO!", "ERRO!" +Environment.NewLine + "A folha: " + sheetNumberParamString + "-" + sheetNameParamString + ", não contém isométricos. Tente novamente.");
                }
            }

            return Result.Succeeded;
        }    


    }
}
