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
    public class PaintFinalPlanarFacesOverrideExternalEventHandler : IExternalEventHandler
    {     
        public void Execute(UIApplication app)
        {            
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            ItemCollection finalPlanarFaces;            
            finalPlanarFaces = AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items;

            List<Face> myPlanarFacesF1 = new List<Face>();
            List<Face> myPlanarFacesF2 = new List<Face>();
            List<Face> myPlanarFacesF3 = new List<Face>();

            foreach (string finalFace in finalPlanarFaces)
            {
                foreach (Face face in ADFSelectElements.PlanarFacesElementList)
                {
                    if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F1"))
                    {
                        myPlanarFacesF1.Add(face);
                    }
                    else if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F2"))
                    {
                        myPlanarFacesF2.Add(face);
                    }
                    else if (finalFace.Contains(face.GetHashCode().ToString()) && finalFace.Contains("F3"))
                    {
                        myPlanarFacesF3.Add(face);
                    }
                    {
                        continue;
                    }
                }
            }
            try
            {
                SelectFaces sf = new SelectFaces();
                sf.PaintFaces(doc, myPlanarFacesF1, "H5-PLANA-F1");
                sf.PaintFaces(doc, myPlanarFacesF2, "H5-PLANA-F2");
                sf.PaintFaces(doc, myPlanarFacesF3, "H5-PLANA-F3");
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
