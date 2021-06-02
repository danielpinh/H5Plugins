using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class VerificarInclinacao : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {          
            //Document initialization 
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Get all pipes on active view         
            FilteredElementCollector collectorPipes = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter1 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
            collectorPipes.WherePasses(collectorFilter1)
                .WhereElementIsNotElementType()
                .ToElementIds();   
            
            //Get slope of each pipe and applying new overrides 
            foreach (Element item in collectorPipes)
            {
                ResetElementColor(doc, item.Id);
                double slopeValue = item.get_Parameter(BuiltInParameter.RBS_PIPE_SLOPE).AsDouble();
                Console.WriteLine(slopeValue.ToString());        
               
                if (slopeValue > 0.01)
                {                    
                    ChangeElementColor(doc, item.Id);
                }
            }            

            return Result.Succeeded;
        }
        static void ChangeElementColor(Document doc, ElementId id)
        {
            //Define collor
            Color color = new Color((byte)255, (byte)50, (byte)50);          

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceForegroundPatternColor(color);          

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Change Element Color");               
                //Set Element Overrides
                doc.ActiveView.SetElementOverrides(id, ogs);
                tx.Commit();
            }
        }
        static void ResetElementColor(Document doc, ElementId id)
        {            
            OverrideGraphicSettings resetElementOverrides = new OverrideGraphicSettings();        

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Reset Element Color");
                //Reset Element Overrides                
                doc.ActiveView.SetElementOverrides(id, resetElementOverrides);
                tx.Commit();
            }

        }
    }

    
}