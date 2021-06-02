using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class AtribuirCodigos : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
           
            string paramName1 = "H5 Código do fabricante";
            string paramName2 = "H5 Código do material";
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            try
            {
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                ElementCategoryFilter collectorFilter1 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
                collector1.WherePasses(collectorFilter1)
                    .WhereElementIsNotElementType()
                    .ToElements();

                //Define code for each cable tray element
                int numeroDeLeitos = 0;
                foreach (Element ct in collector1)

                {
                    string keyHeader = "L##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\1004.3.2 - Leitos_Para_Cabos\Leito_Cabos.csv";


                    if (ct.Name.Contains("LEITO"))
                    {

                        Element cableTray = doc.GetElement(ct.Id);
                        Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByOneHeader(keyHeader, widthCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByOneHeader(keyHeader, widthCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = ct.LookupParameter(paramName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = ct.LookupParameter(paramName2);
                                paramSet2.Set(codint.ToString());
                            }
                            trans.Commit();
                            numeroDeLeitos++;
                        }
                    }
                }

                //ELETROCALHA PERFURADA              
                //Define code for each cable tray element



                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                ElementCategoryFilter collectorFilter2 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
                collector2.WherePasses(collectorFilter2)
                    .WhereElementIsNotElementType()
                    .ToElements();


                int numeroDeEletrocalhasPerf = 0;
                foreach (Element ecp in collector2)
                {
                    if (ecp.Name.Contains("PERFURADA"))
                    {

                        string keyHeader1 = "L##length##millimeters";
                        string keyHeader2 = "A##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";
                        string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\1004.3.3 - Eletrocalhas_para_Cabos\Eletrocalha_Perfurada_Cabos.csv";

                        Element cableTray = doc.GetElement(ecp.Id);
                        Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                        Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {

                                Parameter paramSet1 = ecp.LookupParameter(paramName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = ecp.LookupParameter(paramName2);
                                paramSet2.Set(codint.ToString());

                            }
                            trans.Commit();
                            numeroDeEletrocalhasPerf++;
                        }
                    }

                    //ELETROCALHA LISA              
                    //Define code for each cable tray element
                    FilteredElementCollector collector3 = new FilteredElementCollector(doc);
                    ElementCategoryFilter collectorFilter3 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
                    collector3.WherePasses(collectorFilter3)
                        .WhereElementIsNotElementType()
                        .ToElements();


                    int numeroDeEletrocalhasLisa = 0;
                    foreach (Element ecl in collector3)
                    {

                        if (ecl.Name.Contains("LISA"))
                        {
                            string keyHeader1 = "L##length##millimeters";
                            string keyHeader2 = "A##length##millimeters";
                            string header1 = "CODFAB##other##";
                            string header2 = "COD##other##";
                            string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\1004.3.3 - Eletrocalhas_para_Cabos\Eletrocalha_Lisa_Cabos.csv";


                            Element cableTray = doc.GetElement(ecl.Id);
                            Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                            Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);

                            LookUpTableMapping lktmapping = new LookUpTableMapping();
                            string codfab = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header1, csvPath);
                            string codint = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header2, csvPath);

                            using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                            {
                                trans.Start();
                                {

                                    Parameter paramSet1 = ecl.LookupParameter(paramName1);
                                    paramSet1.Set(codfab.ToString());

                                    Parameter paramSet2 = ecl.LookupParameter(paramName2);
                                    paramSet2.Set(codint.ToString());

                                }
                                trans.Commit();
                                numeroDeEletrocalhasLisa++;
                            }
                        }

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