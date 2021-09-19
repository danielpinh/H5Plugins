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

    public class ClearFacesEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                ADFSelectElements.FacesElementsList.Clear();
                ADFSelectElements.PlanarFacesElementList.Clear();
                ADFSelectElements.CurveFacesElementList.Clear();
                ADFSelectElements.CurveFacesHashCodeString.Clear();
                ADFSelectElements.PlanarFacesHashCodeString.Clear();
                ADFSelectElements.CurveFacesAreaDouble.Clear();
                ADFSelectElements.PlanarFacesAreaDouble.Clear();
                ADFSelectElements.CurveFacesAreaAndHashCodeString.Clear();
                ADFSelectElements.PlanarFacesAreaAndHashCodeString.Clear();
            }
            catch
            {
            }

            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            ClearPaintedFaces cpf = new ClearPaintedFaces();
            cpf.Clear(doc, uidoc);

        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class SelectCurveFaceEEH : IExternalEventHandler
    {
        public static string typeofFace;
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            SelectFaces sf = new SelectFaces();
            sf.MoveCurveFacesBySelection(doc, uidoc, typeofFace);
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class SelectPlanarFaceEEH : IExternalEventHandler
    {
        public static string typeofFace;
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            SelectFaces sf = new SelectFaces();
            sf.MovePlanarFacesBySelection(doc, uidoc, typeofFace);
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class SetCurveFaceParametersEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            SetFaceParameters sfp = new SetFaceParameters();
            sfp.SetCurveFaces(doc, AreaDeFormasMVVM.CurveFaceListFinalString);
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class SetPlanarFaceParametersEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            SetFaceParameters sfp = new SetFaceParameters();
            sfp.SetPlanarFaces(doc, AreaDeFormasMVVM.PlanarFaceListFinalString);
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class PaintFaceOverrideEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            PaintFaceOverride pto = new PaintFaceOverride();
            pto.PaintFace(doc, ADFSelectElements.FaceToPaint);
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }
    public class PaintFinalCurveFacesOverrideEEH : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            ItemCollection finalCurveFaces;
            finalCurveFaces = AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items;

            List<Face> myCurveFacesF1 = new List<Face>();
            List<Face> myCurveFacesF2 = new List<Face>();
            List<Face> myCurveFacesF3 = new List<Face>();

            foreach (string finalFace in finalCurveFaces)
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
                sf.PaintFaces(doc, myCurveFacesF1, "H5-CURVA-F1");
                sf.PaintFaces(doc, myCurveFacesF2, "H5-CURVA-F2");
                sf.PaintFaces(doc, myCurveFacesF3, "H5-CURVA-F3");
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
    public class PaintFinalPlanarFacesOverrideEEH : IExternalEventHandler
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
