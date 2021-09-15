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
  
    public class SupportManagerEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            SupportManager sm = new SupportManager();
            sm.Execute(doc, app);         
        }

        public string GetName()
        {
            return this.GetType().Name;
        }

    }
}









