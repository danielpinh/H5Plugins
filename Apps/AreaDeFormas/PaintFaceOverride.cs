using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;

namespace H5Plugins
{
    public class PaintFaceOverride
    {       

        public void PaintFace(Document doc, Face face)
        {          
            //Get material to paint surface
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Material));

            Material myMaterial = collector
                .ToElements()
                .Cast<Material>()
                .FirstOrDefault(m => m.Name == "H5-FORMAS-02");

            Material myMaterial2 = collector
                .ToElements()
                .Cast<Material>()
                .FirstOrDefault(m => m.Name == "H5-FORMAS-01");

            View myView = doc.ActiveView;
            ElementId eleId = face.Reference.ElementId;            
            
            ElementId elementId = face.Reference.ElementId;

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            myView.SetElementOverrides(eleId, ogs);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Colorir Face Selecionada");
                try
                {
                    foreach (Face item in ADFSelectElements.PlanarFacesElementList)
                    {
                        doc.Paint(elementId, item, myMaterial2.Id);
                    }

                    foreach (Face item in ADFSelectElements.CurveFacesElementList)
                    {
                        doc.Paint(elementId, item, myMaterial2.Id);
                    }

                    doc.RemovePaint(elementId, face);
                    doc.Paint(elementId, face, myMaterial.Id);                    
                }
                catch (Exception)
                {
                }
                tx.Commit();
            }

        }
    }
}