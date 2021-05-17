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
    public class DetalhesTipicos : IExternalCommand
    {
        public const string Path = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\00-TEMPLATES\00-REVIT\05-MECÂNICA\DETALHES TÍPICOS.rvt";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            //Get UIDocument and Document            

            UIApplication application = commandData.Application;
            NewMethod(application);
            return Result.Succeeded;
        }

        public static void NewMethod(UIApplication application)
        {
            Document opendoc = application.Application.OpenDocumentFile(Path);
            UIDocument uidoc = application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                //Coletando as drafting views do documento externo "Detalhes Típicos"

                //Criando os Filtered Element Collectors
                FilteredElementCollector collector = new FilteredElementCollector(doc);

                //Criando um filtro para as conexões de tubo
                ElementCategoryFilter elementsFilter = new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment);

                //Aplicando os filtros nos seus respectivos Collectors
                //Criando uma lista de conexões de tubo                
                IList<Element> elementsElements = collector.WherePasses(elementsFilter).WhereElementIsNotElementType().ToElements();

                //Lista com os valores dos parâmetros
                var paramValueslist = new List<string>();

                //Coletando os valores dos parâmetros e criando uma lista com eles
                foreach (Element ele in elementsElements)
                {
                    Element typeEle = doc.GetElement(ele.GetTypeId());
                    Parameter param = typeEle.LookupParameter("H5 Detalhe Típico");

                    // Se for nulo, continuar!
                    if (param == null)
                    {
                        continue;
                    }

                    var paramValue = param.AsString();
                    //Adiciona os valores dos parâmetros à lista "paramValuesList"
                    paramValueslist.Add(paramValue);
                }

                var paramValuesNoDups = new HashSet<string>(paramValueslist);

                //Lista de vistas de desenho
                List<View> viewsDrafing = new List<View>();

                //Criando um Filtered Element Collector e aplicando as regras de filtros
                FilteredElementCollector collectorOut = new FilteredElementCollector(opendoc).OfCategory(BuiltInCategory.OST_Views);
                ElementClassFilter filter = new ElementClassFilter(typeof(ViewDrafting));
                collectorOut.WherePasses(filter);
                collectorOut.WhereElementIsNotElementType();

                //Retirando os template views e selecionando os detalhes com base no parâmetro "Modelo"
                foreach (string paramValues in paramValuesNoDups)
                {
                    foreach (View item in collectorOut)
                    {
                        if (!item.IsTemplate && item.Name.Equals(paramValues))
                        {
                            viewsDrafing.Add(item);
                        }

                    }

                }

                // Copiando as vistas de desenho entre projetos
                IEnumerable<ViewDrafting> draftingViews = viewsDrafing.OfType<ViewDrafting>();
                int numDraftingElements =
                    DuplicateViewUtils.DuplicateDraftingViews(opendoc, draftingViews, doc);
                int numDrafting = draftingViews.Count<ViewDrafting>();



                //Coletando a família de folha apropriada: Family Symbol
                FamilySymbol tBlock = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .WhereElementIsElementType()
                    .Cast<FamilySymbol>()
                    .First();

                //Lista de vistas de desenho
                List<View> viewsDrafingIn = new List<View>();

                //Criando um Filtered Element Collector e aplicando as regras de filtros
                FilteredElementCollector collectorIn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
                ElementClassFilter filterIn = new ElementClassFilter(typeof(ViewDrafting));
                collectorIn.WherePasses(filter);
                collectorIn.WhereElementIsNotElementType();

                foreach (string paramValues in paramValuesNoDups)
                {
                    foreach (View item in collectorIn)
                    {
                        if (item.Name.Equals(paramValues))
                        {
                            viewsDrafingIn.Add(item);
                        }

                    }

                }

                int[] numberOfdraftingViews = Enumerable.Range(1, viewsDrafing.Count()).ToArray();
                int a = 0;

                foreach (View item in viewsDrafingIn)
                {

                    using (Transaction trans = new Transaction(doc, "Create Sheet"))
                    {

                        trans.Start();
                        //Criando uma família de folha                       
                        ViewSheet vSheet = ViewSheet.Create(doc, tBlock.Id);
                        vSheet.Name = item.Name.ToString();
                        vSheet.SheetNumber = numberOfdraftingViews[a].ToString();


                        //Get Midpoint
                        BoundingBoxUV outline = vSheet.Outline;
                        double x = (outline.Max.U + outline.Min.U) / 2;
                        double y = (outline.Max.V + outline.Min.V) / 2;
                        XYZ midPoint = new XYZ(x, y, 0);

                        //Criando uma viewport e inserindo o detalhe na folha
                        Viewport vPort = Viewport.Create(doc, vSheet.Id, item.Id, midPoint);
                        string nameDraftingView = item.Name.ToString();
                        a++;

                        trans.Commit();
                    }
                }

                // Mostrando na tela as vista de desenho importadas 
                TaskDialog.Show("Detalhes Típicos",
                    String.Format("Foram criadas {0} folhas com Detalhes Típicos.", viewsDrafing.Count().ToString()));          

            }
            catch (Exception)
            {               
                
            }
        }
    }

}