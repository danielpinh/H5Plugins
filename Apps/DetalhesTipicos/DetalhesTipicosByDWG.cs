using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using Revit.SDK.Samples.DuplicateViews.CS;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class DetalhesTipicosByDWG
    {
        public static void AllElements(Document doc, UIDocument uidoc)
        {
            try
            {
                //COLLECTOR
                List<Element> allFamilyInstances = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements()
                .ToList();               

                //LISTA COM OS VALORES DOS PARÂMETROS "DETALHE TÍPICO"
                List<string> paramValueslist = new List<string>();

                //BUSCANDO O VALOR NO PARÂMETRO "DETALHE TÍPICO"
                foreach (Element ele in allFamilyInstances)
                {
                    Element typeEle = doc.GetElement(ele.GetTypeId());
                    try
                    {
                        Parameter param = typeEle.LookupParameter("H5 Detalhe Típico");
                        //SE O PARÂMETRO FOR NULO PARA O ELEMENTO, CONTINUAR A BUSCA...
                        if (param != null)
                        {
                            var paramValue = param.AsString();                            
                            paramValueslist.Add(paramValue);
                        }
                    }
                    catch (Exception)
                    {                        
                    }                                          
                }

                //ELIMINANDO OS PARÂMETROS DUPLICADOS COM HASHSET
                var paramValuesNoDups = new HashSet<string>(paramValueslist);

                List<string> paramValueslistFinal = new List<string>();
                //CRIANDO UMA NOVA LISTA COM OS VALORES DO HASHSET
                foreach (string item in paramValuesNoDups)
                {
                    paramValueslistFinal.Add(item);
                    //DocumentViewerMVVM.MainView.MainViewModel.Add(new DocumentViewerViewModel { NomeDoDetalheTipico = item.ToString() });                     
                }

                //COLETANDO UMA VISTA DE DESENHO
                ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(x => x.ViewFamily == ViewFamily.Drafting);

                //COLETANDO O ID DA VISTA DE DESENHO
                ElementId viewFamilyTypeId = viewFamilyType.Id;

                //DWG OPTIONS
                DWGImportOptions dwgOptions = new DWGImportOptions();
                dwgOptions.ThisViewOnly = true;
                dwgOptions.Placement = ImportPlacement.Centered;
                dwgOptions.OrientToView = true;
                dwgOptions.AutoCorrectAlmostVHLines = true;
                dwgOptions.Unit = ImportUnit.Millimeter;

                //ID DO VÍNCULO
                ElementId linkId = ElementId.InvalidElementId;

                //COLETANDO A FAMÍLIA DE FOLHA PARA INSERÇÃO DO DETALHA NA VISTA
                FamilySymbol tBlock = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .WhereElementIsElementType()
                    .Cast<FamilySymbol>()
                    .First();

                //CONTADORES
                int a = 0;
                int[] numberOfdraftingViews = Enumerable.Range(1, paramValueslist.Count()).ToArray();

                //ADICIONANDO BARRA DE PROGRESSO AO PROCESSO DE CRIAÇÃO DAS VISTAS
                using (var progressBar = new ProgressBar("Preparando os Detalhes Típicos..."))
                {
                    progressBar.RunString<string>(paramValueslistFinal, (paramValue) =>
                    {                       
                        //CAMINHO PARA A PASTA QUE CONTÉM OS DETALHES TÍPICOS
                        string detalhesTipicosPath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\MECÂNICA\PRODUZIDO\FABRICADO\DETALHE TÍPICO\GERAL\" + paramValue + ".dwg";

                        try
                        {
                            using (TransactionGroup transactionGroup = new TransactionGroup(doc, "Detalhes Típicos"))

                            {
                                transactionGroup.Start();

                                using (Transaction trans = new Transaction(doc, "Criar folhas"))
                                {
                                    trans.Start();

                                    //CRIANDO UMA NOVA VISTA DE DESENHO PARA INSERIR O DETALHE TÍPICO
                                    View myView = ViewDrafting.Create(doc, viewFamilyTypeId);
                                    myView.Name = paramValue;

                                    //INSERINDO O LINK NA VISTA
                                    doc.Link(detalhesTipicosPath, dwgOptions, myView, out linkId);

                                    //CRIANDO UMA FAMÍLIA DE FOLHA                      
                                    ViewSheet vSheet = ViewSheet.Create(doc, tBlock.Id);
                                    vSheet.Name = paramValue.ToString();
                                    vSheet.SheetNumber = numberOfdraftingViews[a].ToString();

                                    //COLETANDO O PONTO CENTRAL DA VISTA PARA INSERÇÃO DO DETALHE NA FOLHA
                                    BoundingBoxUV outline = vSheet.Outline;
                                    double x = (outline.Max.U + outline.Min.U) / 2;
                                    double y = (outline.Max.V + outline.Min.V) / 2;
                                    XYZ midPoint = new XYZ(x, y, 0);

                                    //CRIANDO UMA VIEWPORT PARA INSERIR O DETALHE NA FOLHA
                                    Viewport vPort = Viewport.Create(doc, vSheet.Id, myView.Id, midPoint);
                                    a++;
                                    trans.Commit();
                                }
                                transactionGroup.Assimilate();
                            }
                        }
                        catch
                        {
                        }
                       
                   });
                }
            }

            catch (Exception)
            {

            } 
            
        }        
    }
}