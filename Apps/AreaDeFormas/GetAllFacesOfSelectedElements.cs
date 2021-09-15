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
    public class GetAllFacesOfSelectedElements
    {     
        public List<Face> Faces(Document doc, UIDocument uidoc)
        {                                       
            //Select Elements in UI            
            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Element, "Selecione os elementos para tirar a área de formas");

            //List id of elements selected
            List<ElementId> eleIds = new List<ElementId>();
            List<Face> elementFaces = new List<Face>();

            Converters cvn = new Converters();

            //Options
            Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

            //creating a list of ElementId for each selected elements
            foreach (Reference item in refs)
            {
                Element ele = doc.GetElement(item.ElementId);               
                ElementId eleId = ele.Id;
                eleIds.Add(eleId);
            }

            //Get material to paint surface
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Material));

            Material myMaterial = collector
                .ToElements()
                .Cast<Material>()
                .FirstOrDefault(m => m.Name == "H5-FORMAS-01");

            if (myMaterial == null)
            {
                TaskDialog.Show("ERRO!", "ERRO! Adicione os materiais e tente novamente.");
            }

            List<string> areaFaces = new List<string>();

            foreach (ElementId eleId in eleIds)
            {
                Element myEle = doc.GetElement(eleId);

                //PEÇAS 
                if (myEle.Category.Name == "Peças") 
                {
                    GeometryElement gE = myEle.get_Geometry(opts);
                    foreach (var item in gE)
                    {
                        Solid solidObj2 = item as Solid;
                        FaceArray faceArray = solidObj2.Faces;
                        foreach (Face face in faceArray)
                        {
                            double faceArea = face.Area;
                            ElementId materialFaceId = face.MaterialElementId;
                            Element elementMaterial = doc.GetElement(materialFaceId);
                            Material material = elementMaterial as Material;

                            if (face is PlanarFace)
                            {
                                Plane facePlane = face.GetSurface() as Plane;
                                XYZ normalPlaneFace = facePlane.Normal;                
                                
                                //SE A FACE JÀ ESTIVER PINTADA, CLASSFIQUE-A CONFORME O MATERIAL
                                if ((doc.IsPainted(eleId, face) && (!material.Name.Contains("FORMAS"))))
                                {                                    
                                    var faceAreaFeet = face.Area;
                                    double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);                                    
                                    if (material.Name.ToString() == "H5-PLANA-F1")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1"))
                                        {
                                            AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1");
                                            ADFSelectElements.PlanarFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.PlanarFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            ADFSelectElements.PlanarFacesElementList.Add(face);
                                            ADFSelectElements.PlanarFacesAreaDouble.Add(faceAreaMeter);
                                        }                                                                                    
                                    }
                                    else if (material.Name.ToString() == "H5-PLANA-F2")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2"))
                                        {
                                            AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2");
                                            ADFSelectElements.PlanarFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.PlanarFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            ADFSelectElements.PlanarFacesElementList.Add(face);
                                            ADFSelectElements.PlanarFacesAreaDouble.Add(faceAreaMeter);
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-PLANA-F3")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3"))
                                        {
                                            AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3");
                                            ADFSelectElements.PlanarFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.PlanarFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            ADFSelectElements.PlanarFacesElementList.Add(face);                                            
                                            ADFSelectElements.PlanarFacesAreaDouble.Add(faceAreaMeter);                                       
                                        }
                                    }                                   
                                }
                                else
                                {
                                    /*if (normalPlaneFace.Z != 1 && normalPlaneFace.Z != -1)*/
                                    {
                                        using (Transaction tx = new Transaction(doc))
                                        {
                                            tx.Start("Colorir Face");
                                            try
                                            {
                                                doc.RemovePaint(eleId, face);
                                                doc.Paint(eleId, face, myMaterial.Id);
                                                elementFaces.Add(face);
                                            }
                                            catch (Exception)
                                            {
                                                continue;
                                            }
                                            tx.Commit();
                                        }
                                    }
                                }
                            }
                            else if (face is CylindricalFace)
                            {
                                //SE A FACE JÀ ESTIVER PINTADA, CLASSFIQUE-A CONFORME O MATERIAL
                                if ((doc.IsPainted(eleId, face) && (!material.Name.Contains("FORMAS"))))
                                {
                                    var faceAreaFeet = face.Area;
                                    double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);
                                    if (material.Name.ToString() == "H5-CURVA-F1")
                                    {                                      
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1"))
                                        {                                            
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F2")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F3")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3");
                                        }
                                    }                                                                      
                                }
                                else
                                {
                                    using (Transaction tx = new Transaction(doc))
                                    {
                                        tx.Start("Colorir Face");
                                        try
                                        {
                                            doc.RemovePaint(eleId, face);
                                            doc.Paint(eleId, face, myMaterial.Id);
                                            elementFaces.Add(face);
                                        }
                                        catch (Exception)
                                        {
                                            continue;
                                        }
                                        tx.Commit();
                                    }

                                }
                                
                            }

                            else if (face is RuledFace)
                            {
                                //SE A FACE JÀ ESTIVER PINTADA, CLASSFIQUE-A CONFORME O MATERIAL
                                if ((doc.IsPainted(eleId, face) && (!material.Name.Contains("FORMAS"))))
                                {
                                    var faceAreaFeet = face.Area;
                                    double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);
                                    if (material.Name.ToString() == "H5-CURVA-F1")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F2")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F3")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3");
                                        }
                                    }
                                }
                                else
                                {
                                    using (Transaction tx = new Transaction(doc))
                                    {
                                        tx.Start("Colorir Face");
                                        try
                                        {
                                            doc.RemovePaint(eleId, face);
                                            doc.Paint(eleId, face, myMaterial.Id);
                                            elementFaces.Add(face);
                                        }
                                        catch (Exception)
                                        {
                                            continue;
                                        }
                                        tx.Commit();
                                    }

                                }
                            }
                            else if (face is HermiteFace)
                            {
                                //SE A FACE JÀ ESTIVER PINTADA, CLASSFIQUE-A CONFORME O MATERIAL
                                if ((doc.IsPainted(eleId, face) && (!material.Name.Contains("FORMAS"))))
                                {
                                    var faceAreaFeet = face.Area;
                                    double faceAreaMeter = Math.Round(cvn.AreaFeettoMeter(faceAreaFeet), 3);
                                    if (material.Name.ToString() == "H5-CURVA-F1")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F1");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F2")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F2");
                                        }
                                    }
                                    else if (material.Name.ToString() == "H5-CURVA-F3")
                                    {
                                        if (!AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Contains(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3"))
                                        {
                                            ADFSelectElements.CurveFacesElementList.Add(face);
                                            ADFSelectElements.CurveFacesHashCodeString.Add(face.GetHashCode().ToString());
                                            ADFSelectElements.CurveFacesAreaDouble.Add(faceAreaMeter);
                                            ADFSelectElements.CurveFacesAreaAndHashCodeString.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ")");
                                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(faceAreaMeter + " m² (" + face.GetHashCode().ToString() + ") - F3");
                                        }
                                    }
                                }
                                else
                                {
                                    using (Transaction tx = new Transaction(doc))
                                    {
                                        tx.Start("Colorir Face");
                                        try
                                        {
                                            doc.RemovePaint(eleId, face);
                                            doc.Paint(eleId, face, myMaterial.Id);
                                            elementFaces.Add(face);
                                        }
                                        catch (Exception)
                                        {
                                            continue;
                                        }
                                        tx.Commit();
                                    }

                                }
                            }
                        }
                    }                        
                }
                else
                {
                    TaskDialog.Show("ERRO!", "ERRO! Por favor, selecione apenas elementos das categorias de Peças");
                }             
            }            
            return elementFaces;
        }

    }
    
}