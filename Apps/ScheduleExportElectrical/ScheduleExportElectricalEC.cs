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
    class ScheduleExportElectricalEC : IExternalCommand
    {     
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                //EXIBINDO A JANELA NA UI  
                //ClearData();                              
                UIApplication app = commandData.Application;                
                H5Plugins.H5AppExternalApplication.h5App.uiApp = app;                
                H5Plugins.H5AppExternalApplication.h5App.ShowScheduleExportElectricalUI();              
            }
            catch (Exception ex)
            {
                /* The message users should see in case of any error when trying to start the Add-In. */
                message = ex.Message;
                TaskDialog.Show("ERRO!", message);
                //return Result.Failed;
            }
            return Result.Succeeded;
        }
        //private static void ClearData()
        //{
        //    //LIMPANDO OS VALORES QUE SERÃO EXIBIDOS NA UI
        //    ScheduleExportMVVM.ViewModel.ScheduleName = null;
        //    ScheduleExportMVVM.MainView.ViewModel.MyReviewValue = null;
        //    foreach (var item in ScheduleExportMVVM.MainView.MainViewModel)
        //    {
        //        item.IsCheckedScheduleName = false;
        //    }
        //}

    }
}









