using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using H5Plugins;

namespace H5Plugins
{   
    [Transaction(TransactionMode.Manual)]

    public class LookupTableMappingEC : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            H5AppExternalApplication.h5App.uiApp = app;
            H5AppExternalApplication.h5App.ShowLookupTableMappingUI();

            return Result.Succeeded;
        }
    }
}









