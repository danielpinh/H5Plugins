using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
    public class PaintFinalFacesOverrideExternalEventHandler : IExternalEventHandler
    {     
        public void Execute(UIApplication app)
        {            
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            ItemCollection finalFaces;            
            finalFaces = AreaDeFormasMVVM.mainView.FinalsCurveFaceListBox.Items;


            List<Face> myCurveFacesF1 = new List<Face>();
            List<Face> myCurveFacesF2 = new List<Face>();
            List<Face> myCurveFacesF3 = new List<Face>();

            foreach (string finalFace in finalFaces)
            {
                foreach (Face face in ADFSelectElements.CurveFacesElementList)
                {
                    if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F1"))
                    {
                        myCurveFacesF1.Add(face);
                    }
                    else if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F2"))
                    {
                        myCurveFacesF2.Add(face);
                    }
                    else if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F3"))
                    {
                        myCurveFacesF3.Add(face);
                    }
                    {
                        continue;
                    }
                }
            }
            try
            {
                SelectFaces sf = new SelectFaces();
                sf.PaintFaces(doc, uidoc, myCurveFacesF1, "H5-CURVA-F1");
                sf.PaintFaces(doc, uidoc, myCurveFacesF2, "H5-CURVA-F2");
                sf.PaintFaces(doc, uidoc, myCurveFacesF3, "H5-CURVA-F3");
            }
            catch (Exception)
            {
                throw;
            }            
        }
        public string GetName()
        {            
            return this.GetType().Name;
        }          

    }
}
