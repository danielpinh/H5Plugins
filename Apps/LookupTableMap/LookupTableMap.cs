using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class LookupTableMap
    {
        readonly Collectors Collector = new Collectors();
        readonly string ParamName1 = "H5 Código do fabricante";
        readonly string ParamName2 = "H5 Código do material";

        private DataAcess DataAcess { get; } = new DataAcess();
        
        public void Leitos(Document doc)
        {
            ////LEITO-CABOS-AC         
            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();
                List<Element> leitos = Collector.CableTraysByFamilyTypeName(doc, "LEITO");
                string lookuptablename = "Leito_Cabos.csv";
                string csvPath = DataAcess.GetLookupTable(lookuptablename);

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = leitos.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = leitos.Count();

                foreach (Element element in leitos)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "L##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
                        Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByOneHeader(keyHeader, widthCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByOneHeader(keyHeader, widthCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(ParamName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }

        public void Septos(Document doc)
        {
            //SEPTOS        
            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();
                List<Element> septos = Collector.CableTraysByFamilyTypeName(doc, "SEPTO");

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = septos.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = septos.Count();

                string lookuptablename = "Leito_Cabos_Septo_Divisor.csv";
                string csvPath = DataAcess.GetLookupTable(lookuptablename);

                foreach (Element element in septos)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "A##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
                        Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByOneHeader(keyHeader, heightCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByOneHeader(keyHeader, heightCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(ParamName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }               

            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }

        public void Perfilados(Document doc)
        {
            ////PERFILADOS         
            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();
                List<Element> perfilados = Collector.CableTraysByFamilyTypeName(doc, "PERFILADO");

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = perfilados.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = perfilados.Count();

                string lookuptablename = "Perfilado_Perfurado.csv";
                string csvPath = DataAcess.GetLookupTable(lookuptablename);

                foreach (Element element in perfilados)
                {
                    try
                    {
                        string keyHeader1 = "L##length##millimeters";
                        string keyHeader2 = "A##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";
                        string paramName1 = "H5 Código do fabricante";
                        string paramName2 = "H5 Código do material";

                        Element cableTray = doc.GetElement(element.Id);
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

                                Parameter paramSet1 = element.LookupParameter(paramName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(paramName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }
        public void Eletrocalhas(Document doc)
        {
            //ELETROCALHA-CABOS-AC-PERFURADA            
            //Define code for each cable tray element        
            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>(); 

                //ProgressBar
                List<Element> eletrocalhasPerfuradas = Collector.CableTraysByFamilyTypeName(doc, "PERFURADA");
                List<Element> eletrocalhasLisa = Collector.CableTraysByFamilyTypeName(doc, "LISA");

                string lookuptablename = "Eletrocalha_Perfurada_Cabos.csv";
                string csvPath = DataAcess.GetLookupTable(lookuptablename);

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = eletrocalhasPerfuradas.Count() + eletrocalhasLisa.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = eletrocalhasPerfuradas.Count() + eletrocalhasLisa.Count();

                foreach (Element element in eletrocalhasPerfuradas)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader1 = "L##length##millimeters";
                        string keyHeader2 = "A##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
                        Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                        Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {

                                Parameter paramSet1 = element.LookupParameter(ParamName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //ELETROCALHA-CABOS-AC-LISA               
                //Define code for each cable tray element
                lookuptablename = "Eletrocalha_Lisa_Cabos.csv";
                csvPath = DataAcess.GetLookupTable(lookuptablename);

                foreach (Element element in eletrocalhasLisa)
                {
                    try
                    {
                        string keyHeader1 = "L##length##millimeters";
                        string keyHeader2 = "A##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
                        Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
                        Parameter heightCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByTwoHeaders(keyHeader1, keyHeader2, widthCableTray.AsValueString(), heightCableTray.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {

                                Parameter paramSet1 = element.LookupParameter(ParamName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }

            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }

        public void Eletrodutos(Document doc)
        {
            //ELETRODUTO-ACG-RÍGIDO         
            //Define code for each cable tray element

            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();

                //ProgressBar
                List<Element> eletrodutacgRig = Collector.ConduitsByFamilyTypeName(doc, "ACG-RÍGIDO");
                List<Element> eletrodutoacgFlex = Collector.ConduitsByFamilyTypeName(doc, "ACG-FLEXÍVEL");
                List<Element> eletrodutopeadFlex = Collector.ConduitsByFamilyTypeName(doc, "PEAD-FLEXÍVEL");

                string lookuptablename = "Eletroduto_Rígido_Aço.csv";
                string csvPath = DataAcess.GetLookupTable(lookuptablename);
                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = eletrodutacgRig.Count() + eletrodutoacgFlex.Count() + eletrodutopeadFlex.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = eletrodutacgRig.Count() + eletrodutoacgFlex.Count() + eletrodutopeadFlex.Count();

                foreach (Element element in eletrodutacgRig)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        //string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
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

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //ELETRODUTO-ACG-FLEXÍVEL         
                //Define code for each cable tray element
                lookuptablename = "Eletroduto_Flexível_Aço.csv";
                csvPath = DataAcess.GetLookupTable(lookuptablename);

                foreach (Element element in eletrodutoacgFlex)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element cableTray = doc.GetElement(element.Id);
                        Parameter diameterConduit = cableTray.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codfab = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(ParamName1);
                                paramSet1.Set(codfab.ToString());

                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //ELETRODUTO-PEAD-FLEXÍVEL         
                //Define code for each cable tray element

                lookuptablename = "Eletroduto_Flexível_PEAD.csv";
                csvPath = DataAcess.GetLookupTable(lookuptablename);

                foreach (Element element in eletrodutopeadFlex)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        //string header1 = "CODFAB##other##";
                        string header2 = "COD##other##";

                        Element eletroduto = doc.GetElement(element.Id);
                        Parameter diameterConduit = eletroduto.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        //string codfab = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header1, csvPath);
                        string codint = lktmapping.LookupByOneHeader(keyHeader, diameterConduit.AsValueString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                //Parameter paramSet1 = ele.LookupParameter(paramName1);
                                //paramSet1.Set(codfab.ToString());
                                Parameter paramSet2 = element.LookupParameter(ParamName2);
                                paramSet2.Set(codint.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;

        }
        public void Dutos(Document doc)
        {
            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();

                //DUTOS RETANGULARES RÍGIDOS/HVAC         
                //Define code for each duct

                //ProgressBar
                List<Element> retangularDucts = Collector.RetangularDuctsByFamilyTypeName(doc, "DUTO RETANGULAR");
                List<Element> roundDucts = Collector.FlexRoundDuctsByFamilyTypeName(doc, "ALUDEC");

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = retangularDucts.Count() + roundDucts.Count();
                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = retangularDucts.Count() + roundDucts.Count();

                foreach (Element element in retangularDucts)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "LADO##length##millimeters";
                        string header1 = "ESPESSURAMSG##length##millimeters";
                        string header2 = "ESPESSURAMM##length##millimeters";
                        string header3 = "MASSALINEAR##length##millimeters";
                        string header4 = "DESC##other##";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\07-AVAC\00-ITENS COMERCIAIS\07-DUTO RETANGULAR RÍGIDO\DUTOS.csv";
                        string paramEspessuraMM = "H5 Espessura da chapa (mm)";
                        string paramEspessuraMSG = "H5 Espessura da chapa (ponto)";
                        string paramMassa = "H5 Massa por área de chapa";
                        string paramdescMat = "H5 Descrição do material";

                        Element retangularDuct = doc.GetElement(element.Id);
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
                                Parameter paramSet1 = element.LookupParameter(paramEspessuraMM);
                                paramSet1.SetValueString(espessuraMM);
                                Parameter paramSet2 = element.LookupParameter(paramEspessuraMSG);
                                paramSet2.SetValueString(espessuraMSG);
                                Parameter paramSet3 = element.LookupParameter(paramMassa);
                                paramSet3.SetValueString(mass);
                                Parameter paramSet4 = element.LookupParameter(paramdescMat);
                                paramSet4.Set(desc);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //DUTOS REDONDOS FLEXÍVEIS/HVAC         
                //Define code for each duct
                foreach (Element element in roundDucts)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "ESPESSURAMSG##length##millimeters";
                        string header2 = "ESPESSURAMM##length##millimeters";
                        string header3 = "MASSALINEAR##length##millimeters";
                        string header4 = "DESC##other##";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\07-AVAC\00-ITENS COMERCIAIS\22-DUTO FLEXÍVEL CIRCULAR\DUTO FLEXÍVEL CIRCULAR-MULTIVAC.csv";
                        string paramEspessuraMM = "H5 Espessura da chapa (mm)";
                        string paramEspessuraMSG = "H5 Espessura da chapa (ponto)";
                        string paramMassa = "H5 Massa por área de chapa";
                        string paramdescMat = "H5 Descrição do material";

                        Element roundDuct = doc.GetElement(element.Id);
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
                                Parameter paramSet1 = element.LookupParameter(paramEspessuraMM);
                                paramSet1.SetValueString(espessuraMM);
                                Parameter paramSet2 = element.LookupParameter(paramEspessuraMSG);
                                paramSet2.SetValueString(espessuraMSG);
                                Parameter paramSet3 = element.LookupParameter(paramMassa);
                                paramSet3.SetValueString(mass);
                                Parameter paramSet4 = element.LookupParameter(paramdescMat);
                                paramSet4.Set(desc);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });
                            }
                            trans.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }


                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }

        public void LookupTableMapReportMVVM_Closed(object sender, EventArgs e)
        {
            LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Value=0;
        }

        public void Tubos(Document doc)
        {
            try
            {
                //ProgressBar
                List<Element> pipeListGrooved = Collector.PipeByFamilyTypeName(doc, "GROOVED");
                List<Element> pipeList40S = Collector.PipeByFamilyTypeName(doc, "40S");
                List<Element> pipeList10S = Collector.PipeByFamilyTypeName(doc, "10S");
                List<Element> pipeListsch80 = Collector.PipeByFamilyTypeName(doc, "SCH80");
                List<Element> pipeListSTD = Collector.PipeByFamilyTypeName(doc, "STD");
                List<Element> pipeListsch40 = Collector.PipeByFamilyTypeName(doc, "SCH40");

                //TUBOS SISTEMA GROOVED     
                //Define code for each tube
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();

                LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = pipeListGrooved.Count() +
                    pipeList40S.Count() +
                    pipeList10S.Count() +
                    pipeListsch80.Count() +
                    pipeListSTD.Count() +
                    pipeListsch40.Count();

                LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = pipeListGrooved.Count() +
                    pipeList40S.Count() +
                    pipeList10S.Count() +
                    pipeListsch80.Count() +
                    pipeListSTD.Count() +
                    pipeListsch40.Count();


                foreach (Element element in pipeListGrooved)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-Grooved.csv";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }
                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //TUBO AÇO INOX 40S                   
                //Define code for each tube
                foreach (Element element in pipeList40S)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-Aço_Inox-SCH40S.csv";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //TUBO AÇO INOX 10S
                //Define code for each tube

                foreach (Element element in pipeList10S)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-Aço_Inox-SCH10S.csv";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //TUBOS SCH80     
                //Define code for each tube

                foreach (Element element in pipeListsch80)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\MECÂNICA\MATERIAPRIMA\TUBO\GERAL\TUBOS-SCH80-R0(0).CSV";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //TUBOS SCH40     
                //Define code for each tube

                foreach (Element element in pipeListsch40)
                {
                    try
                    {

                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\MECÂNICA\MATERIAPRIMA\TUBO\GERAL\TUBOs-SCH40-R0(0).CSV";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //TUBOS STD     
                //Define code for each tube

                foreach (Element element in pipeListSTD)
                {
                    try
                    {
                        //LookupMapping parameters
                        string keyHeader = "DN##length##millimeters";
                        string header1 = "COD##other##";
                        string header2 = "PESOUNIT##mass##kilograms";
                        string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\MECÂNICA\MATERIAPRIMA\TUBO\GERAL\TUBOS-STD-R0(0).CSV";
                        string codMat = "H5 Código do material";
                        string massaLinear = "H5 Massa";

                        Element pipeElement = doc.GetElement(element.Id);
                        string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                        string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                        using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(codMat);
                                paramSet1.Set(codMatValue);
                                Parameter paramSet2 = element.LookupParameter(massaLinear);
                                paramSet2.SetValueString(massaLinearValue);

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                ////TUBOS PVC-3 METROS   
                ////Define code for each tube
                //List<Element> pipeListPVC3 = myCollector.PipeByFamilyTypeName(doc, "3 METROS");
                //foreach (Element pp in pipeListPVC3)
                //{
                //    //LookupMapping parameters
                //    string keyHeader = "DN##length##millimeters";
                //    string header1 = "COD##other##";
                //    string header2 = "PESOUNIT##mass##kilograms";
                //    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-PVC-Marrom-3metros.csv";
                //    string codMat = "H5 Código do material";
                //    string massaLinear = "H5 Massa";

                //    Element pipeElement = doc.GetElement(pp.Id);
                //    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                //    LookUpTableMapping lktmapping = new LookUpTableMapping();
                //    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                //    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                //    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                //    {
                //        trans.Start();
                //        {
                //            Parameter paramSet1 = pp.LookupParameter(codMat);
                //            paramSet1.Set(codMatValue);
                //            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                //            paramSet2.SetValueString(massaLinearValue);
                //        }
                //        trans.Commit();
                //    }
                //}

                ////TUBOS PVC-6 METROS   
                ////Define code for each tube
                //List<Element> pipeListPVC6 = myCollector.PipeByFamilyTypeName(doc, "6 METROS");
                //foreach (Element pp in pipeListPVC6)
                //{
                //    //LookupMapping parameters
                //    string keyHeader = "DN##length##millimeters";
                //    string header1 = "COD##other##";
                //    string header2 = "PESOUNIT##mass##kilograms";
                //    string csvPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-TUBO\Tubos-PVC-Marrom-6metros.csv";
                //    string codMat = "H5 Código do material";
                //    string massaLinear = "H5 Massa";

                //    Element pipeElement = doc.GetElement(pp.Id);
                //    string pipeDiameter = pipeElement.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                //    LookUpTableMapping lktmapping = new LookUpTableMapping();
                //    string codMatValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header1, csvPath);
                //    string massaLinearValue = lktmapping.LookupByOneHeader(keyHeader, pipeDiameter.ToString(), header2, csvPath);

                //    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                //    {
                //        trans.Start();
                //        {
                //            Parameter paramSet1 = pp.LookupParameter(codMat);
                //            paramSet1.Set(codMatValue);
                //            Parameter paramSet2 = pp.LookupParameter(massaLinear);
                //            paramSet2.SetValueString(massaLinearValue);
                //        }
                //        trans.Commit();
                //    }
                //}             

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }

        //SISTEMAS
        public void Sistemas(Document doc)
        {            
            string systemAbreviationValue;
            string systemNameValue;

            //ProgressBar

            //EQUIPAMENTOS MECÂNICOS (EXCETO PARAFUSOS, PORCAS, ARRUELAS, CHUMBADORES)   
            //Define code for each mechanical equipment
            List<string> elementsToNotCollect = new List<string>();
            elementsToNotCollect.Add("PORCA");
            elementsToNotCollect.Add("PARAFUSO");
            elementsToNotCollect.Add("ARRUELA");
            elementsToNotCollect.Add("CHUMBADOR");
            elementsToNotCollect.Add("BARRA");

            List<Element> mechanicalList = Collector.MechanicalEquipmentsWithoutNames(doc, elementsToNotCollect);
            List<FamilyInstance> elbowS = Collector.PipeFittingsByFamilyName(doc, "GOMADA");
            List<Element> valvesList = Collector.AllPipeAcessories(doc);
            List<Element> pipeList = Collector.AllPipes(doc);

            LookupTableMapMVVM.MainView.LookupTable_ProgressBar.Maximum = mechanicalList.Count() + elbowS.Count() + valvesList.Count() + pipeList.Count();
            LookupTableMapMVVM.ProgressBarViewModel.ProgressBarMaxValue = mechanicalList.Count() + elbowS.Count() + valvesList.Count() + pipeList.Count();

            try
            {
                //Clean collection
                LookupTableMapReportMVVM.ReportViewModels.Clear();
                List<ReportViewModel> reportViewModels = new List<ReportViewModel>();

                string systemParameter = "H5 Sistema";
                string keyHeader = "ABREVIATURA##other##";
                string header1 = "NOME##other##";
                string csvPath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\05-MECÂNICA\00-MATÉRIA-PRIMA\00-SISTEMAS\Sistemas-utf-8.csv";

                foreach (Element element in mechanicalList)
                {
                    try
                    {
                        systemAbreviationValue = element.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString();

                        if (systemAbreviationValue != null)
                        {
                            string substringValue = systemAbreviationValue.Substring(0, 2);

                            LookUpTableMapping lktmapping = new LookUpTableMapping();
                            systemNameValue = lktmapping.LookupByOneHeader(keyHeader, substringValue.ToString(), header1, csvPath);

                            using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                            {
                                trans.Start();
                                {
                                    Parameter paramSet1 = element.LookupParameter("H5 Sistema");
                                    paramSet1.Set(systemNameValue);

                                    reportViewModels.Add(new ReportViewModel
                                    {
                                        FamilyName = " " + element.Name + " ",
                                        FamilyId = " " + element.Id.ToString() + " ",
                                        Result = " Parâmetro alterado com sucesso! "
                                    });

                                }
                                trans.Commit();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //CURVA GOMADA  
                //Define code for each mechanical equipment
                foreach (FamilyInstance element in elbowS)
                {
                    Element myEle = element as Element;

                    try
                    {
                        systemAbreviationValue = element.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();
                        if (systemAbreviationValue != null)
                        {
                            LookUpTableMapping lktmapping = new LookUpTableMapping();
                            systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);

                            using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                            {
                                trans.Start();
                                {
                                    Parameter paramSet1 = myEle.LookupParameter("H5 Sistema");
                                    paramSet1.Set(systemNameValue);

                                    reportViewModels.Add(new ReportViewModel
                                    {
                                        FamilyName = " " + element.Name + " ",
                                        FamilyId = " " + element.Id.ToString() + " ",
                                        Result = " Parâmetro alterado com sucesso! "
                                    });

                                }
                                trans.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;

                }

                //TUBOS
                foreach (Element element in pipeList)
                {
                    try
                    {
                        Element myEle = doc.GetElement(element.Id);
                        systemAbreviationValue = myEle.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();

                        if (systemAbreviationValue != null)
                        {
                            LookUpTableMapping lktmapping = new LookUpTableMapping();
                            systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);

                            using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                            {
                                trans.Start();
                                {
                                    Parameter paramSet1 = element.LookupParameter("H5 Sistema");
                                    paramSet1.Set(systemNameValue);

                                    reportViewModels.Add(new ReportViewModel
                                    {
                                        FamilyName = " " + element.Name + " ",
                                        FamilyId = " " + element.Id.ToString() + " ",
                                        Result = " Parâmetro alterado com sucesso! "
                                    });

                                }
                                trans.Commit();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;


                }

                //PIPES
                //var encoding = Encoding.UTF8;
                //StreamReader stream = new StreamReader(csvPath, encoding);
                foreach (Element element in valvesList)
                {
                    try
                    {
                        Element myEle = doc.GetElement(element.Id);
                        systemAbreviationValue = myEle.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();

                        LookUpTableMapping lktmapping = new LookUpTableMapping();
                        systemNameValue = lktmapping.LookupByOneHeader(keyHeader, systemAbreviationValue.ToString(), header1, csvPath);
                        using (Transaction trans = new Transaction(doc, "Atribuir Sistemas"))
                        {
                            trans.Start();
                            {
                                Parameter paramSet1 = element.LookupParameter(systemParameter);
                                paramSet1.Set(systemNameValue.ToString());

                                reportViewModels.Add(new ReportViewModel
                                {
                                    FamilyName = " " + element.Name + " ",
                                    FamilyId = " " + element.Id.ToString() + " ",
                                    Result = " Parâmetro alterado com sucesso! "
                                });

                            }
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        reportViewModels.Add(new ReportViewModel
                        {
                            FamilyName = " " + element.Name + " ",
                            FamilyId = " " + element.Id.ToString() + " ",
                            Result = " Erro: " + ex.Message + " "
                        });
                    }

                    LookupTableMapMVVM.ProgressBarViewModel.ProgressBarValue++;
                }

                //Sorting View Model Data
                if (reportViewModels.Count > 0)
                {
                    IOrderedEnumerable<ReportViewModel> orderedCollec = reportViewModels.OrderBy(x => x.FamilyName);
                    foreach (ReportViewModel item in orderedCollec)
                    {
                        LookupTableMapReportMVVM.ReportViewModels.Add(item);
                    }
                }
                else
                {
                    LookupTableMapReportMVVM.ReportViewModels.Add(new ReportViewModel { FamilyId = "null", FamilyName = "null", Result = "null" });
                }
            }
            catch { }

            //Final Report
            LookupTableMapReportMVVM lookupTableMapReportMVVM = new LookupTableMapReportMVVM();
            lookupTableMapReportMVVM.Show();
            lookupTableMapReportMVVM.Closed += LookupTableMapReportMVVM_Closed;
        }
    }
}



