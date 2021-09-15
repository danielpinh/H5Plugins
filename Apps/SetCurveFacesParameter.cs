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
    public class SetCurveFacesParameter
    {       
        public void Set(Document doc, List<string> faces)
        {            
            int myIndex = 0;
            double myArea = 0.0;
            Face myFace = null;
            List<double> f1doubleValues = new List<double>();
            List<double> f2doubleValues = new List<double>();
            List<double> f3doubleValues = new List<double>();         

            //Em cada face selecionada na coleção de faces curvas
            foreach (string face in faces)
            {
                foreach (string hashCode in ADFSelectElements.CurveFacesHashCodeString)
                {
                    if (face.Contains(hashCode))
                    {
                        myIndex = ADFSelectElements.CurveFacesHashCodeString.IndexOf(hashCode);
                        myArea = ADFSelectElements.CurveFacesAreaDouble.ElementAt(myIndex);
                        myFace = ADFSelectElements.CurveFacesElementList.ElementAt(myIndex);

                        if (face.Contains("F1"))
                        {
                            f1doubleValues.Add(myArea);
                        }
                        else if (face.Contains("F2"))
                        {
                            f2doubleValues.Add(myArea);
                        }
                        else if (face.Contains("F3"))
                        {
                            f3doubleValues.Add(myArea);
                        }
                    }
                }

            }

            //SOMANDO AS ÁREAS TOTAIS POR TIPO DE FORMA
            double f1totalArea = f1doubleValues.Sum();
            double f2totalArea = f2doubleValues.Sum();
            double f3totalArea = f3doubleValues.Sum();

            //DOUBLE PARA STRING
            string f1totalAreaString = f1totalArea.ToString();
            string f2totalAreaString = f2totalArea.ToString();
            string f3totalAreaString = f3totalArea.ToString();

            //COLETANDO O ELEMENTID DA PEÇA
            ElementId partId = myFace.Reference.ElementId;
            Element myEle = doc.GetElement(partId);

            //INSERINDO OS VALORES NOS PARÂMETROS            
            using (Transaction trans = new Transaction(doc, "Atribuir Parâmetros"))
            {
                trans.Start();
                {
                    Parameter formacurvaF1 = myEle.LookupParameter("H5 Forma Curva F1");
                    formacurvaF1.SetValueString(f1totalAreaString);

                    Parameter formacurvaF2 = myEle.LookupParameter("H5 Forma Curva F2");
                    formacurvaF2.SetValueString(f2totalAreaString);

                    Parameter formacurvaF3 = myEle.LookupParameter("H5 Forma Curva F3");
                    formacurvaF3.SetValueString(f3totalAreaString);
                }
                trans.Commit();
            }
        }         
    }
}