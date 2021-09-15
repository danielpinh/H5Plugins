using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace H5Plugins
{
    /// <summary>
    /// This is the class that will implement the IExternalEventHandler interface
    /// to handle all commands started by user action in the MainWindow (modeless dialog) 
    /// as Requests listed in an enumeration used by the Request class.
    /// Also here we will define all the methods that will build the application functionality using the Revit API.
    /// </summary>
    public class AreaDeFormasEEH : IExternalEventHandler
    {
        public static AreaDeFormasMVVM MainView { get; set; } = new AreaDeFormasMVVM();
        public void Execute(UIApplication app)
        {            
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            ADFSelectElements.FacesElementsList.Clear();
            ADFSelectElements.PlanarFacesElementList.Clear();
            ADFSelectElements.CurveFacesElementList.Clear();
            ADFSelectElements.CurveFacesHashCodeString.Clear();
            ADFSelectElements.PlanarFacesHashCodeString.Clear();
            ADFSelectElements.CurveFacesAreaDouble.Clear();
            ADFSelectElements.PlanarFacesAreaDouble.Clear();
            ADFSelectElements.CurveFacesAreaAndHashCodeString.Clear();
            ADFSelectElements.PlanarFacesAreaAndHashCodeString.Clear();

            ADFSelectElements.GetFaces(doc, uidoc);

            //MainViewModel myModel = new MainViewModel();
            //myModel.CountAllCurveFaces = AreaDeFormasMVVM.mainView.AllCurveFacesListBox.Items.Count.ToString();
        }
        public string GetName()
        {            
            return this.GetType().Name;
        }          

    }   
    
}
