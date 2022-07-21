using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using Autodesk.Revit.DB.Architecture;
using System.Linq;
using System;
using Autodesk.Revit.UI.Selection;
using H5Plugins;
using Autodesk.Revit.ApplicationServices;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class Debug : IExternalCommand
    {

        //public static double DoubleEspecificExternalWallWidth { get; set; } = 0;
        //public static double DoubleRevExternalWallWidth { get; set; } = 0;
        //public static double DoubleSpecificExternalWallPlusDoubleSpecificRevExternalWall { get; set; } = DoubleEspecificExternalWallWidth + DoubleRevExternalWallWidth;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilySymbol> notLoad = new List<FamilySymbol>();

            List<FamilySymbol> families = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            foreach (var item in families)
            {
                try
                {
                    Family family = item.Family;

                    if (family.IsEditable)
                    {

                    }
                    else
                    {
                        notLoad.Add(item);
                    }

                }
                catch
                {
                }

            }

            TaskDialog.Show("debug", notLoad.Count.ToString());



            return Result.Succeeded;
        }
    }
}