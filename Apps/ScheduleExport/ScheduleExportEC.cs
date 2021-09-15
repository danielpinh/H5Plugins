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
    /// <summary>
    /// This class implements the IExternalCommand interface and contains the command to execute the application, 
    /// which will be called by the button created in the application class.
    /// </summary>

    [Transaction(TransactionMode.Manual)] // setting transactions to manual in order to associate them with our add-in commands, if needed.
    class ScheduleExportEC : IExternalCommand
    {     
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ClearData();
            //EXIBINDO A JANELA NA UI
            ScheduleExportMVVM.MainView.Show();
            return Result.Succeeded;
        }

        private static void ClearData()
        {
            //LIMPANDO OS VALORES QUE SERÃO EXIBIDOS NA UI
            ScheduleExportMVVM.MainView.ViewModel.ScheduleName = null;
            ScheduleExportMVVM.MainView.ViewModel.MyReviewValue = null;
            foreach (var item in ScheduleExportMVVM.MainView.MainViewModel)
            {
                item.IsCheckedScheduleName = false;
            }
        }
    }

}









