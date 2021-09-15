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
    public class ClearPaintedFaces
    {     
        public void Clear(Document doc, UIDocument uidoc)
        {                                       
            //Select Elements in UI            
            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Element, "Selecione as faces para retirar a pintura");

            //List id of elements selected
            List<ElementId> eleIds = new List<ElementId>();                     
           
            //creating a list of ElementId for each selected elements
            foreach (Reference item in refs)
            {
                Element ele = doc.GetElement(item.ElementId);
                ElementId eleId = ele.Id;
                eleIds.Add(eleId);               
            }

            //Options
            Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };
           
            try
            {
                foreach (ElementId eleId in eleIds)
                {                  
                    Element myEle = doc.GetElement(eleId);
                    //PEÇAS 
                    if (myEle.Category.Name == "Peças")
                    {
                        using (TransactionGroup tgroup = new TransactionGroup(doc))
                        {
                            tgroup.Start("Limpar todas as faces");
                            GeometryElement gE = myEle.get_Geometry(opts);
                            foreach (var item in gE)
                            {
                                Solid solidObj2 = item as Solid;
                                FaceArray faceArray = solidObj2.Faces;                                
                                foreach (Face face in faceArray)
                                {
                                    using (Transaction tx = new Transaction(doc))
                                    {
                                        tx.Start("Descolorir Faces");
                                        try
                                        {
                                            doc.RemovePaint(eleId, face);                                            
                                        }
                                        catch
                                        {
                                        }
                                        tx.Commit();
                                    }
                                }
                            }
                            tgroup.Assimilate();  
                        }
                    }
                    else
                    {
                        TaskDialog.Show("ERRO!", "ERRO! Por favor, selecione faces válidas");
                    }
                }
            }
            catch 
            {                
            }         
            //creating a list of ElementId for each selected elements
            foreach (Reference item in refs)
            {
                Element myEle = doc.GetElement(item.ElementId);
                ElementId eleId = myEle.Id;
                eleIds.Add(eleId);
                try
                {
                    //ZERANDO VALORES DOS PARÂMETROS
                    using (Transaction trans = new Transaction(doc, "Atribuir Parâmetros"))
                    {
                        trans.Start();
                        {
                            Parameter formacurvaF1 = myEle.LookupParameter("H5 Forma Curva F1");
                            formacurvaF1.SetValueString("0 m²");
                            Parameter formacurvaF2 = myEle.LookupParameter("H5 Forma Curva F2");
                            formacurvaF2.SetValueString("0 m²");
                            Parameter formacurvaF3 = myEle.LookupParameter("H5 Forma Curva F3");
                            formacurvaF3.SetValueString("0 m²");
                            Parameter formaplanaF1 = myEle.LookupParameter("H5 Forma Plana F1");
                            formaplanaF1.SetValueString("0 m²");
                            Parameter formaplanaF2 = myEle.LookupParameter("H5 Forma Plana F2");
                            formaplanaF2.SetValueString("0 m²");
                            Parameter formaplanaF3 = myEle.LookupParameter("H5 Forma Plana F3");
                            formaplanaF3.SetValueString("0 m²");
                        }
                        trans.Commit();
                    }
                }
                catch
                {
                }
            }           
        }

    }
    
}