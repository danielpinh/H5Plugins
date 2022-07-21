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
    /// <summary>
    /// This class implements the IExternalCommand interface and contains the command to execute the application, 
    /// which will be called by the button created in the application class.
    /// </summary>

    [Transaction(TransactionMode.Manual)] // setting transactions to manual in order to associate them with our add-in commands, if needed.

    public class DetalhesTipicosEC : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            H5Plugins.H5AppExternalApplication.h5App.uiApp = app;
            H5Plugins.H5AppExternalApplication.h5App.ShowDetalhesTipicosUI();            
            return Result.Succeeded;
        }
    }
}
       
    

  
       
    



