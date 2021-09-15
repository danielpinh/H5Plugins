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
    public class LookupTableMap
    {
        Collectors myCollector = new Collectors();
        string paramName1 = "H5 Código do fabricante";
        string paramName2 = "H5 Código do material";

        public void Leitos(Document doc)
        {
            ////LEITO-CABOS-AC         
            try
            {
                List<Element> leitos = myCollector.CableTraysByFamilyTypeName(doc, "LEITO");

                foreach (Element ct in leitos)
                {
                    //LookupMapping parameters
                    string keyHeader = "L##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\01-LEITOS PARA CABOS\Leito_Cabos.csv";

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
                    }
                }
            }
            finally
            {

            }
        }

        public void Perfilados(Document doc)
        {
            ////PERFILADOS         
            try
            {
                List<Element> perfilados = myCollector.CableTraysByFamilyTypeName(doc, "PERFILADO");
                foreach (Element ele in perfilados)
                {
                    string keyHeader1 = "L##length##millimeters";
                    string keyHeader2 = "A##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\04-FERRAGENS EM GERAL\Perfilado_Perfurado.csv";
                    string paramName1 = "H5 Código do fabricante";
                    string paramName2 = "H5 Código do material";

                    Element cableTray = doc.GetElement(ele.Id);
                    Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);
                    Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);

                    string newheigthCableTray = null;

                    //Ajuste para perfilados menores que 25 
                    if (heightCableTray.AsValueString() == "25")
                    {
                        newheigthCableTray = "19";
                    }
                    else
                    {
                        newheigthCableTray = heightCableTray.AsValueString();
                    }

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codfab = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), newheigthCableTray, header1, csvPath);
                    string codint = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), newheigthCableTray, header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {

                            Parameter paramSet1 = ele.LookupParameter(paramName1);
                            paramSet1.Set(codfab.ToString());

                            Parameter paramSet2 = ele.LookupParameter(paramName2);
                            paramSet2.Set(codint.ToString());

                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }
        }
        public void Eletrocalhas(Document doc)
        {
            //ELETROCALHA-CABOS-AC-PERFURADA            
            //Define code for each cable tray element        
            try
            {
                List<Element> eletrocalhasPerfuradas = myCollector.CableTraysByFamilyTypeName(doc, "PERFURADA");
                foreach (Element ecp in eletrocalhasPerfuradas)
                {
                    //LookupMapping parameters
                    string keyHeader1 = "L##length##millimeters";
                    string keyHeader2 = "A##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\02-ELETROCALHAS PARA CABOS\Eletrocalha_Perfurada_Cabos.csv";

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
                    }
                }

                //ELETROCALHA-CABOS-AC-LISA               
                //Define code for each cable tray element
                List<Element> eletrocalhasLisa = myCollector.CableTraysByFamilyTypeName(doc, "LISA");
                foreach (Element ecl in eletrocalhasLisa)
                {
                    string keyHeader1 = "L##length##millimeters";
                    string keyHeader2 = "A##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\02-ELETROCALHAS PARA CABOS\Eletrocalha_Lisa_Cabos.csv";


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
                    }
                }

            }
            finally
            {

            }
        }

        public void Eletrodutos(Document doc)
        {

            try
            {
                //ELETRODUTO-ACG-RÍGIDO         
                //Define code for each cable tray element
                List<Element> eletrodutacgRig = myCollector.ConduitsByFamilyTypeName(doc, "ACG-RÍGIDO");
                foreach (Element ele in eletrodutacgRig)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    //string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\03-ELETRODUTOS E CONEXÕES\Eletroduto_Rígido_Aço.csv";

                    Element cableTray = doc.GetElement(ele.Id);
                    Parameter diameterConduit = cableTray.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    //string codfab = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header1, csvPath);
                    string codint = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header2, csvPath);


                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            //Parameter paramSet1 = ele.LookupParameter(paramName1);
                            //paramSet1.Set(codfab.ToString());

                            Parameter paramSet2 = ele.LookupParameter(paramName2);
                            paramSet2.Set(codint.ToString());
                        }
                        trans.Commit();
                    }
                }

                //ELETRODUTO-ACG-FLEXÍVEL         
                //Define code for each cable tray element
                List<Element> eletrodutoacgFlex = myCollector.ConduitsByFamilyTypeName(doc, "ACG-FLEXÍVEL");
                foreach (Element ele in eletrodutoacgFlex)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\03-ELETRODUTOS E CONEXÕES\Eletroduto_Flexível_Aço.csv";

                    Element cableTray = doc.GetElement(ele.Id);
                    Parameter diameterConduit = cableTray.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codfab = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header1, csvPath);
                    string codint = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = ele.LookupParameter(paramName1);
                            paramSet1.Set(codfab.ToString());

                            Parameter paramSet2 = ele.LookupParameter(paramName2);
                            paramSet2.Set(codint.ToString());
                        }
                        trans.Commit();
                    }
                }

                //ELETRODUTO-PEAD-FLEXÍVEL         
                //Define code for each cable tray element
                List<Element> eletrodutopeadFlex = myCollector.ConduitsByFamilyTypeName(doc, "PEAD-FLEXÍVEL");
                foreach (Element ele in eletrodutopeadFlex)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    //string header1 = "CODFAB##other##";
                    string header2 = "COD##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\00-ITENS COMERCIAIS\03-ELETRODUTOS E CONEXÕES\Eletroduto_Flexível_PEAD.csv";

                    Element cableTray = doc.GetElement(ele.Id);
                    Parameter diameterConduit = cableTray.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    //string codfab = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header1, csvPath);
                    string codint = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            //Parameter paramSet1 = ele.LookupParameter(paramName1);
                            //paramSet1.Set(codfab.ToString());
                            Parameter paramSet2 = ele.LookupParameter(paramName2);
                            paramSet2.Set(codint.ToString()); ;
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }
        }
        public void DutosRetangulares(Document doc)
        {
            try
            {
                //DUTOS RETANGULARES RÍGIDOS/HVAC         
                //Define code for each duct
                List<Element> retangularDucts = myCollector.RetangularDuctsByFamilyTypeName(doc, "DUTO RETANGULAR");
                foreach (Element drt in retangularDucts)
                {
                    //LookupMapping parameters
                    string keyHeader = "LADO##length##millimeters";
                    string header1 = "ESPESSURAMSG##length##millimeters";
                    string header2 = "ESPESSURAMM##length##millimeters";
                    string header3 = "MASSALINEAR##length##millimeters";
                    string header4 = "DESC##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\07-AVAC\00-ITENS COMERCIAIS\07-DUTO RETANGULAR RÍGIDO\DUTOS.csv";
                    string paramEspessuraMM = "H5 Espessura da chapa (mm)";
                    string paramEspessuraMSG = "H5 Espessura da chapa (ponto)";
                    string paramMassa = "H5 Massa por área de chapa";
                    string paramdescMat = "H5 Descrição do material";

                    Element retangularDuct = doc.GetElement(drt.Id);
                    string heightDuct = retangularDuct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsValueString();
                    string widthDuct = retangularDuct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsValueString();
                    int biggestsize = 0;

                    int heightInt = int.Parse(heightDuct);
                    int widthInt = int.Parse(widthDuct);

                    if (heightInt > widthInt)
                    {
                        biggestsize = heightInt;
                    }
                    else
                    {
                        biggestsize = widthInt;
                    }

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string espessuraMSG = lktmapping.LookupByOneHeader(keyHeader, biggestsize.ToString(), header1, csvPath);
                    string espessuraMM = lktmapping.LookupByOneHeader(keyHeader, biggestsize.ToString(), header2, csvPath);
                    string mass = lktmapping.LookupByOneHeader(keyHeader, biggestsize.ToString(), header3, csvPath);
                    string desc = lktmapping.LookupByOneHeader(keyHeader, biggestsize.ToString(), header4, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = drt.LookupParameter(paramEspessuraMM);
                            paramSet1.SetValueString(espessuraMM);
                            Parameter paramSet2 = drt.LookupParameter(paramEspessuraMSG);
                            paramSet2.SetValueString(espessuraMSG);
                            Parameter paramSet3 = drt.LookupParameter(paramMassa);
                            paramSet3.SetValueString(mass);
                            Parameter paramSet4 = drt.LookupParameter(paramdescMat);
                            paramSet4.Set(desc);
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }
        }
        public void DutosRedondos(Document doc)
        {
            try
            {
                //DUTOS REDONDOS FLEXÍVEIS/HVAC         
                //Define code for each duct
                List<Element> roundDucts = myCollector.FlexRoundDuctsByFamilyTypeName(doc, "ALUDEC");
                foreach (Element drd in roundDucts)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "ESPESSURAMSG##length##millimeters";
                    string header2 = "ESPESSURAMM##length##millimeters";
                    string header3 = "MASSALINEAR##length##millimeters";
                    string header4 = "DESC##other##";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\07-AVAC\00-ITENS COMERCIAIS\22-DUTO FLEXÍVEL CIRCULAR\DUTO FLEXÍVEL CIRCULAR-MULTIVAC.csv";
                    string paramEspessuraMM = "H5 Espessura da chapa (mm)";
                    string paramEspessuraMSG = "H5 Espessura da chapa (ponto)";
                    string paramMassa = "H5 Massa por área de chapa";
                    string paramdescMat = "H5 Descrição do material";

                    Element roundDuct = doc.GetElement(drd.Id);
                    string diameterDuct = roundDuct.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string espessuraMSG = lktmapping.LookupByOneHeader(keyHeader, diameterDuct.ToString(), header1, csvPath);
                    string espessuraMM = lktmapping.LookupByOneHeader(keyHeader, diameterDuct.ToString(), header2, csvPath);
                    string mass = lktmapping.LookupByOneHeader(keyHeader, diameterDuct.ToString(), header3, csvPath);
                    string desc = lktmapping.LookupByOneHeader(keyHeader, diameterDuct.ToString(), header4, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = drd.LookupParameter(paramEspessuraMM);
                            paramSet1.SetValueString(espessuraMM);
                            Parameter paramSet2 = drd.LookupParameter(paramEspessuraMSG);
                            paramSet2.SetValueString(espessuraMSG);
                            Parameter paramSet3 = drd.LookupParameter(paramMassa);
                            paramSet3.SetValueString(mass);
                            Parameter paramSet4 = drd.LookupParameter(paramdescMat);
                            paramSet4.Set(desc);
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }
        }
        public void Tubos(Document doc)
        {
            try
            {
                //TUBOS SCH80     
                //Define code for each tube
                List<Element> pipeListsch80 = myCollector.PipeByFamilyTypeName(doc, "SCH80");
                foreach (Element pp in pipeListsch80)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "COD##other##";
                    string header2 = "PESOUNIT##mass##kilograms";    
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-Sch80.csv";
                    string codMat = "H5 Código do material";
                    string massaLinear = "H5 Massa";                 

                    Element pipeElement = doc.GetElement(pp.Id);
                    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);                    

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = pp.LookupParameter(codMat);
                            paramSet1.Set(codMatValue);
                            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                            paramSet2.SetValueString(massaLinearValue);                            
                        }
                        trans.Commit();
                    }
                }

                //TUBOS SCH40     
                //Define code for each tube
                List<Element> pipeListsch40 = myCollector.PipeByFamilyTypeName(doc, "SCH40");
                foreach (Element pp in pipeListsch40)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "COD##other##";
                    string header2 = "PESOUNIT##mass##kilograms";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-Sch40.csv";
                    string codMat = "H5 Código do material";
                    string massaLinear = "H5 Massa";

                    Element pipeElement = doc.GetElement(pp.Id);
                    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = pp.LookupParameter(codMat);
                            paramSet1.Set(codMatValue);
                            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                            paramSet2.SetValueString(massaLinearValue);
                        }
                        trans.Commit();
                    }
                }

                //TUBOS PVC-3 METROS   
                //Define code for each tube
                List<Element> pipeListPVC3 = myCollector.PipeByFamilyTypeName(doc, "3 METROS");
                foreach (Element pp in pipeListPVC3)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "COD##other##";
                    string header2 = "PESOUNIT##mass##kilograms";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-PVC-Marrom-3metros.csv";
                    string codMat = "H5 Código do material";
                    string massaLinear = "H5 Massa";

                    Element pipeElement = doc.GetElement(pp.Id);
                    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = pp.LookupParameter(codMat);
                            paramSet1.Set(codMatValue);
                            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                            paramSet2.SetValueString(massaLinearValue);
                        }
                        trans.Commit();
                    }
                }

                //TUBOS PVC-6 METROS   
                //Define code for each tube
                List<Element> pipeListPVC6 = myCollector.PipeByFamilyTypeName(doc, "6 METROS");
                foreach (Element pp in pipeListPVC6)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "COD##other##";
                    string header2 = "PESOUNIT##mass##kilograms";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-PVC-Marrom-6metros.csv";
                    string codMat = "H5 Código do material";
                    string massaLinear = "H5 Massa";

                    Element pipeElement = doc.GetElement(pp.Id);
                    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = pp.LookupParameter(codMat);
                            paramSet1.Set(codMatValue);
                            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                            paramSet2.SetValueString(massaLinearValue);
                        }
                        trans.Commit();
                    }
                }

                //TUBOS STD     
                //Define code for each tube
                List<Element> pipeListSTD = myCollector.PipeByFamilyTypeName(doc, "STD");
                foreach (Element pp in pipeListSTD)
                {
                    //LookupMapping parameters
                    string keyHeader = "DN##length##millimeters";
                    string header1 = "COD##other##";
                    string header2 = "PESOUNIT##mass##kilograms";
                    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-STD.csv";
                    string codMat = "H5 Código do material";
                    string massaLinear = "H5 Massa";

                    Element pipeElement = doc.GetElement(pp.Id);
                    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter paramSet1 = pp.LookupParameter(codMat);
                            paramSet1.Set(codMatValue);
                            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                            paramSet2.SetValueString(massaLinearValue);
                        }
                        trans.Commit();
                    }
                }
            }
            finally
            {

            }
        }
        //SISTEMAS
        public void Sistemas(Document doc)
        {            
            string systemAbreviationValue;
            string systemNameValue;

            try
            {
                string systemParameter = "H5 Sistema";
                string keyHeader = "ABREVIATURA##other##";
                string header1 = "NOME##other##";
                string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-SISTEMAS\Sistemas-utf-8.csv";

                //EQUIPAMENTOS MECÂNICOS (EXCETO PARAFUSOS, PORCAS, ARRUELAS, CHUMBADORES)   
                //Define code for each mechanical equipment
                List<string> elementsToNotCollect = new List<string>();
                elementsToNotCollect.Add("PORCA");
                elementsToNotCollect.Add("PARAFUSO");
                elementsToNotCollect.Add("ARRUELA");
                elementsToNotCollect.Add("CHUMBADOR");
                elementsToNotCollect.Add("BARRA");

                List<Element> mechanicalList = myCollector.MechanicalEquipmentsWithoutNames(doc, elementsToNotCollect);
                foreach (Element me in mechanicalList)
                {
                    try
                    {
                        systemAbreviationValue = me.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString();

                        if (systemAbreviationValue != null)
                        {
                            try
                            {
                                string substringValue = systemAbreviationValue.Substring(0, 2);

                                LookUpTableMapping lktmapping = new LookUpTableMapping();
                                systemNameValue = lktmapping.LookupByOneHeader(keyHeader, substringValue.ToString(), header1, csvPath);

                                using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                                {
                                    trans.Start();
                                    {
                                        Parameter paramSet1 = me.LookupParameter("H5 Sistema");
                                        paramSet1.Set(systemNameValue);
                                    }
                                    trans.Commit();
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }

                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }

                //CURVA GOMADA  
                //Define code for each mechanical equipment
                List<FamilyInstance> elbowS = myCollector.PipeFittingsByFamilyName(doc, "GOMADA");
                foreach (FamilyInstance elbow in elbowS)
                {
                    Element myEle = elbow as Element;

                    try
                    {
                        systemAbreviationValue = elbow.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();
                        if (systemAbreviationValue != null)
                        {
                            try
                            {                               
                                LookUpTableMapping lktmapping = new LookUpTableMapping();
                                systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);

                                using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                                {
                                    trans.Start();
                                    {
                                        Parameter paramSet1 = myEle.LookupParameter("H5 Sistema");
                                        paramSet1.Set(systemNameValue);
                                    }
                                    trans.Commit();
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }            
                

                //TUBOS
                List<Element> pipeList = myCollector.AllPipes(doc);
                foreach (Element pipe in pipeList)
                {
                    try
                    {
                        Element myEle = doc.GetElement(pipe.Id);
                        systemAbreviationValue = myEle.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();

                        if (systemAbreviationValue != null)
                        {
                            try
                            {

                                LookUpTableMapping lktmapping = new LookUpTableMapping();
                                systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);

                                using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                                {
                                    trans.Start();
                                    {
                                        Parameter paramSet1 = pipe.LookupParameter("H5 Sistema");
                                        paramSet1.Set(systemNameValue);
                                    }
                                    trans.Commit();
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                        }

                    }
                    catch (Exception)
                    {

                        continue;
                    }

                }

                //PIPES
                //var encoding = Encoding.UTF8;
                //StreamReader stream = new StreamReader(csvPath, encoding);

                List<Element> valvesList = myCollector.AllPipeAcessories(doc);
                foreach (Element pa in valvesList)
                {
                    try
                    {
                        Element myEle = doc.GetElement(pa.Id);
                        systemAbreviationValue = myEle.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);
                        using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = pa.LookupParameter(systemParameter);
                                paramSet1.Set(systemNameValue.ToString());
                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                
            }
            finally
            {

            }
        }       
    }
}



