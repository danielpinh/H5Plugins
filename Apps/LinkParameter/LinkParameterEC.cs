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

namespace H5Plugins
{ 

    [Transaction(TransactionMode.Manual)]    
    class LinkParameterEC : IExternalCommand
    {     
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {                                           
                UIApplication app = commandData.Application;                
                H5AppExternalApplication.h5App.uiApp = app;
                H5AppExternalApplication.h5App.ShowLinkParameterlUI();              
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("ERRO!", message);
            }
            return Result.Succeeded;
        }      
    }
}









