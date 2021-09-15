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

    public class ADFSelectElements         
    {
        public static List<Face> FacesElementsList { get; set; } = new List<Face>();
        public static List<Face> PlanarFacesElementList { get; set; } = new List<Face>();
        public static List<Face> CurveFacesElementList { get; set; } = new List<Face>();       
        public static List<string> CurveFacesHashCodeString { get; set; } = new List<string>();
        public static List<string> PlanarFacesHashCodeString { get; set; } = new List<string>();
        public static List<double> CurveFacesAreaDouble { get; set; } = new List<double>();
        public static List<double> PlanarFacesAreaDouble { get; set; } = new List<double>();
        public static List<string> CurveFacesAreaAndHashCodeString { get; set; } = new List<string>();      
        public static List<string> PlanarFacesAreaAndHashCodeString { get; set; } = new List<string>();
        public static Face FaceToPaint { get; set; }        

        public static void GetFaces(Document doc, UIDocument uidoc)        
        {
            
            GetAllFacesOfSelectedElements getAllFaces = new GetAllFacesOfSelectedElements();
            FacesElementsList = getAllFaces.Faces(doc, uidoc);
            Converters cvn = new Converters();          

            //FACES PLANAS
            foreach (Face face in FacesElementsList)
            {
                var faceAreaFeet = face.Area;
                double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);

                if (face is PlanarFace)
                {                    
                    PlanarFacesElementList.Add(face);
                    PlanarFacesHashCodeString.Add(face.GetHashCode().ToString());
                    PlanarFacesAreaDouble.Add(faceAreaMeter);
                    PlanarFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                    if (!AreaDeFormasMVVM.MainView.AllPlanarFacesListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")"))
                    {
                        AreaDeFormasMVVM.MainView.AllPlanarFacesListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                    }
                }
            }

            //FACES CURVAS
            foreach (Face face in FacesElementsList)
            {
                var faceAreaFeet = face.Area;
                double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);

                if (face is RuledFace || face is CylindricalFace || face is HermiteFace)
                {
                    CurveFacesElementList.Add(face);
                    CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                    CurveFacesAreaDouble.Add(faceAreaMeter);
                    CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");

                    if (!AreaDeFormasMVVM.MainView.AllCurveFacesListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")"))
                    {
                        AreaDeFormasMVVM.MainView.AllCurveFacesListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                    }                  
                }
            }
        }
    }    
}









