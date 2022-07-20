using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using Autodesk.Revit.DB.Architecture;
using System.Linq;
using System;
using Autodesk.Revit.UI.Selection;
using H5Plugins;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class Debug : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            return Result.Succeeded;
        }       
    }
}