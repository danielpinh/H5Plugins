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
using Autodesk.Revit.ApplicationServices;

namespace H5Plugins
{
    public class SelectFaces
    {
        public static List<Face> MySelectedFaces = new List<Face>();

        public void PaintFaces(Document doc, List<Face> facesList, string materialName)
        {
            //COLETANDO O MATERIAL PARA PINTAR AS FACES SELECIONADAS
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Material));
            Material myMaterial = collector
                .ToElements()
                .Cast<Material>()
                .FirstOrDefault(m => m.Name == materialName);

            View activeView = doc.ActiveView;
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();


            foreach (Face face in facesList)
            {
                ElementId eleId = face.Reference.ElementId;
                activeView.SetElementOverrides(eleId, ogs);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Descolorir Face");
                    try
                    {
                        doc.RemovePaint(eleId, face);                        
                    }
                    catch (Exception)
                    {
                    }
                    tx.Commit();
                }

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Colorir Face Selecionada");
                    try
                    {                        
                        doc.Paint(eleId, face, myMaterial.Id);
                    }
                    catch (Exception)
                    {
                    }
                    tx.Commit();
                }
            }
        }
        public void MoveCurveFacesBySelection (Document doc, UIDocument uidoc, string typeOfFace)
        {
            MySelectedFaces.Clear();
            //SELECIONA AS FACES NA INTERFACE DO USUÁRIO

            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Face, "Selecione a face");            
            Element myPartElement;
            ElementId eleId;           
            
            //PARA CADA REFERÊNCIA SELECIONADA...FAÇA
            foreach (Reference pickedRef in refs)
            {
                //COLETE A FACE DA REFERÊNCIA
                GeometryObject geoObj = doc.GetElement(pickedRef).GetGeometryObjectFromReference(pickedRef);
                Face myFace = geoObj as Face;                

                //ADICIONANDO AS FACES SELECIONADAS À PROPRIEDADE QUE CONTÉM AS FACES SELECIONADAS
                MySelectedFaces.Add(myFace);

                //RETORNE A PEÇA CUJA FACE PERTENCE                
                myPartElement = doc.GetElement(pickedRef);
                eleId = myPartElement.Id;
            }           

            //ADICIONANDO AS FACES SELECIONADAS A LISTBOX DE FACES VÁLIDAS
            //PARA CADA FACE SELECIONADA E PARA CADA FACE DENTRE TODAS AS FACES QUE O ELEMENTO POSSUI, SE A FACE SELECIONADA ESTÁ DENTRE AS TODAS AS FACES, REMOVA ELA 
            //DA LISTA DE FACES DESCARTADAS E ADICIONE A LISTA QUE CONTÉM AS FACES VÁLIDAS          
            foreach (string item in ADFSelectElements.CurveFacesAreaAndHashCodeString)
            {
                
                foreach (Face face in MySelectedFaces)
                {
                    string hash = face.GetHashCode().ToString();                    
                    if (item.Contains(hash))
                    {
                        List<int> myList = new List<int>();
                        AreaDeFormasMVVM.MainView.AllCurveFacesListBox.Items.Remove(item);
                        foreach (string myFace in AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items)
                        {
                            if (myFace.Contains(hash.ToString())) myList.Add(1);
                        }
                        if (myList.Count > 0)
                        {
                            myList.Clear();
                        }
                        else
                        {
                            AreaDeFormasMVVM.MainView.FinalsCurveFaceListBox.Items.Add(item + " - " + typeOfFace);
                        }
                        myList.Clear();
                    }                    
                }
            }            
        }
        public void MovePlanarFacesBySelection(Document doc, UIDocument uidoc, string typeOfFace)
        {            
            MySelectedFaces.Clear();
            //SELECIONA AS FACES NA INTERFACE DO USUÁRIO       
            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Face, "Selecione a face");            
            Element myPartElement;
            ElementId eleId;

            //PARA CADA REFERÊNCIA SELECIONADA...FAÇA
            foreach (Reference pickedRef in refs)
            {
                //COLETE A FACE DA REFERÊNCIA
                GeometryObject geoObj = doc.GetElement(pickedRef).GetGeometryObjectFromReference(pickedRef);
                Face myFace = geoObj as Face;

                //ADICIONANDO AS FACES SELECIONADAS À PROPRIEDADE QUE CONTÉM AS FACES SELECIONADAS
                MySelectedFaces.Add(myFace);

                //RETORNE A PEÇA CUJA FACE PERTENCE                
                myPartElement = doc.GetElement(pickedRef);
                eleId = myPartElement.Id;
            }
            
            //ADICIONANDO AS FACES SELECIONADAS A LISTBOX DE FACES VÁLIDAS
            //PARA CADA FACE SELECIONADA E PARA CADA FACE DENTRE TODAS AS FACES QUE O ELEMENTO POSSUI, SE A FACE SELECIONADA ESTÁ DENTRE AS TODAS AS FACES, REMOVA ELA 
            //DA LISTA DE FACES DESCARTADAS E ADICIONE A LISTA QUE CONTÉM AS FACES VÁLIDAS          
            foreach (string item in ADFSelectElements.PlanarFacesAreaAndHashCodeString)
            {               

                foreach (Face face in MySelectedFaces)
                {
                    string hash = face.GetHashCode().ToString();
                    if (item.Contains(hash))
                    {
                        List<int> myList = new List<int>();
                        AreaDeFormasMVVM.MainView.AllPlanarFacesListBox.Items.Remove(item);
                        foreach (string myFace in AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items)
                        {                            
                            if (myFace.Contains(hash.ToString())) myList.Add(1);                            
                        }
                        if (myList.Count > 0)
                        {                            
                            myList.Clear();
                        }
                        else
                        {
                            AreaDeFormasMVVM.MainView.FinalsPlanarFaceListBox.Items.Add(item + " - " + typeOfFace);                            
                        }
                        myList.Clear();
                    }

                }
            }

        }
    }    
}