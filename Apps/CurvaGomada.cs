using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]


    public class CurvaGomada : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;         

            Collectors myCollector = new Collectors();
            string comprimentoEquivalente = "H5 Comprimento Equivalente";
            string diametroNominal = "H5 Diâmetro";

            try
            {
                List<Element> myPipes = myCollector.AllPipes(doc);

                foreach (Element pipe in myPipes)
                {
                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter comprimentoEquivalenteParam = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                            Parameter diametroNominalParam = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

                            string comprimentoEquivalenteParamString = comprimentoEquivalenteParam.AsValueString();                            
                            string diametroNominalParamString = diametroNominalParam.AsValueString();
                            try
                            {
                                string novoComprimento = comprimentoEquivalenteParamString.Replace(".", "");
                                if (novoComprimento != null)
                                {
                                    Parameter paramSet1 = pipe.LookupParameter(comprimentoEquivalente);
                                    paramSet1.SetValueString(novoComprimento.ToString());
                                }
                                else
                                {
                                    Parameter paramSet1 = pipe.LookupParameter(comprimentoEquivalente);
                                    paramSet1.SetValueString(comprimentoEquivalenteParamString.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                message = ex.Message;
                            }    
                            
                            Parameter paramSet2 = pipe.LookupParameter(diametroNominal);

                            if (paramSet2 == null)
                            {
                                TaskDialog.Show("ERRO!", "O Parâmetro H5 Diâmetro não foi encontrado nos tubos do projeto." + Environment.NewLine + Environment.NewLine + "Adicione o parâmetro e tente novamente.");
                                break;
                            }
                            else
                            {
                                try
                                {
                                    paramSet2.SetValueString(diametroNominalParamString);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                                
                            }

                            
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }           

        return Result.Succeeded;
        }
    }
}



