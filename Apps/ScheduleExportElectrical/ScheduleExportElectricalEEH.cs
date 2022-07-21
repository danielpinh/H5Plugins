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
using System.Collections.ObjectModel;

namespace H5Plugins
{
    /// <summary>
    /// This is the class that will implement the IExternalEventHandler interface
    /// to handle all commands started by user action in the MainWindow (modeless dialog) 
    /// as Requests listed in an enumeration used by the Request class.
    /// Also here we will define all the methods that will build the application functionality using the Revit API.
    /// </summary>
    public class ScheduleExportElectricalEEH : IExternalEventHandler
    {        
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;            

            //COLETANDO TODAS AS TABELAS DO PROJETO
            FilteredElementCollector scheduleCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule));

            foreach (ViewSchedule vs in scheduleCollector)
            {
                if (!(vs.IsTitleblockRevisionSchedule) && !(vs.IsTemplate))
                {
                    ScheduleExportElectricalMVVM.MainView.MainViewModel.Add(new ScheduleExportElectricalViewModel { ScheduleName = vs.Name });                       
                }                
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }

    }
    public class ScheduleExportElectricalSaveEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {        
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            ScheduleExportElectrical se = new ScheduleExportElectrical();          
            se.ScheduleExport(doc);         
        }

        public string GetName()
        {
            return this.GetType().Name;
        }

    }    
}









