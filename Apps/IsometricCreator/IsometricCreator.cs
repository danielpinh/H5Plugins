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
    [TransactionAttribute(TransactionMode.Manual)]
    public class IsometricCreator : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            //List Points
            List<XYZ> points = new List<XYZ>();
            List<ElementId> eleIds = new List<ElementId>();           
                       
            //Get Isometric View Template
            string templateName = "H5-MEC-3D-ISO";
            View viewTemplate = new FilteredElementCollector(doc).OfClass(typeof(View)).
                    Cast<View>().Where(v => v.IsTemplate && v.Name.Equals(templateName)).FirstOrDefault();
            ElementId viewtemplateId = viewTemplate.Id;              

            //Current 3D view and its orientation
            View3D view3d;
            View3D current3DView = uidoc.ActiveView as View3D;
            ViewOrientation3D viewOrientation3D = current3DView.GetOrientation();

            //Tag Parameters
            //TagMode tmode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tOrient = TagOrientation.Horizontal;

            //Select Elements in UI            
            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Element, "Selecione os elementos para compor a vista Isométrica");

            //get elements bouding boxes and
            foreach (Reference r in refs)
            {
                BoundingBoxXYZ bb = doc.GetElement(r).get_BoundingBox(uidoc.ActiveView);
                points.Add(bb.Min);
                points.Add(bb.Max);
            }
            BoundingBoxXYZ sb = new BoundingBoxXYZ
            {
                Min = new XYZ(points.Min(p => p.X),
                           points.Min(p => p.Y),
                           points.Min(p => p.Z)),
                Max = new XYZ(points.Max(p => p.X),
                           points.Max(p => p.Y),
                           points.Max(p => p.Z))
            };

            ElementId pipeFittingTag = null;
            ElementId pipeTag1 = null;
            ElementId pipeTag2 = null;
            ElementId pipeAcessoryTag = null;
            ElementId plumbingFixtureTag = null;


            //IDENTIFICANDO SE AS TAGS ESTÃO CARREGADAS NO PROJETO.
            try
            {
                //Coletando as tags
                Collectors myCollector = new Collectors();
                pipeFittingTag = myCollector.PipeFittingsTagsbyFamilyName(doc, "ID-CONEXÕES DE TUBO-ITEM+POSICIONAMENTO");
                if (pipeFittingTag == null)
                {
                    TaskDialog.Show("ERRO!", "A família: \n \nID-CONEXÕES DE TUBO-ITEM+POSICIONAMENTO \n \nnão foi encontrada, carregue-a e tente novamente.");
                    return Result.Failed;
                }
                pipeTag1 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-ITEM+POSICIONAMENTO");
                if (pipeTag1 == null)
                {
                    TaskDialog.Show("ERRO!", "A família: \n \nID-TUBO-ITEM+POSICIONAMENTO \n \nnão foi encontrada, carregue-a e tente novamente.");
                    return Result.Failed;
                }
                pipeTag2 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-TAG+SISTEMA+DIÂMETRO");
                if (pipeTag2 == null)
                {
                    TaskDialog.Show("ERRO!", "A família: \n \nID-TUBO-TAG+SISTEMA+DIÂMETRO \n \nnão foi encontrada, carregue-a e tente novamente.");
                    return Result.Failed;
                }
                pipeAcessoryTag = myCollector.PipeAcessoryTagsbyFamilyName(doc, "ID-ACESSÓRIOS DE TUBO-ITEM+POSICIONAMENTO");
                if (pipeAcessoryTag == null)
                {
                    TaskDialog.Show("ERRO!", "A família: \n \nID-ACESSÓRIOS DE TUBO-ITEM+POSICIONAMENTO \n \nnão foi encontrada, carregue-a e tente novamente.");
                    return Result.Failed;
                }
                plumbingFixtureTag = myCollector.PlumbingFixtureTagsbyFamilyName(doc, "ID-PEÇA HIDROSSANITÁRIA-ITEM+POSICIONAMENTO");
                if (plumbingFixtureTag == null)
                {
                    TaskDialog.Show("ERRO!", "A família: \n \nID-ACESSÓRIOS DE TUBO-ITEM+POSICIONAMENTO \n \nnão foi encontrada, carregue-a e tente novamente.");
                    return Result.Failed;
                }
            }
            catch 
            {                
            }
            
            //LISTA COM TODOS OS ELEMENTOS SELECIONADOS NA UI
            foreach (Reference item in refs)
            {
                Element ele = doc.GetElement(item.ElementId);
                ElementId eleId = ele.Id;
                eleIds.Add(eleId);
            }

            //VERIFICANDO SE TODAS AS FAMÍLIAS SELECIONADAS POSSUEM A LINHA DE 1 mm
            List<string> familiesNameWithoutLine = new List<string>();
            foreach (ElementId eleId in eleIds)
            {
                Element myElement = doc.GetElement(eleId);

                if (myElement.Category.Name != "Tubulação" && myElement.Category.Name != "Sistemas de tubulação")
                {
                    FamilyInstance familyInstance = myElement as FamilyInstance;
                    FamilySymbol familySymbol = familyInstance.Symbol;
                    string familyName = familySymbol.FamilyName.ToString();
                    string familyID = myElement.Id.ToString();
                    Line oneMillimieterLine = null;
                    List<int> lineCount = new List<int>();

                    //OPTIONS
                    Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

                    //COLETANDO A LINHA DE 1 mm DENTRO DA CONEXÃO                                              
                    GeometryElement gE = myElement.get_Geometry(opts);

                    foreach (GeometryInstance gI in gE)
                    {
                        GeometryElement geoElem = gI.GetSymbolGeometry();
                        foreach (var obj in geoElem)
                        {
                            if (obj is Line)
                            {
                                oneMillimieterLine = obj as Line;
                                if (Math.Round(oneMillimieterLine.ApproximateLength, 5) == 0.00328)
                                {
                                    lineCount.Add(1);
                                }
                            }
                        }
                    }
                    if (lineCount.Count == 0)
                    {
                        familiesNameWithoutLine.Add("● ID: " + familyID);
                    }
                }
            }

            if (familiesNameWithoutLine.Count > 0)
            {
                string valor = "";
                foreach (var item in familiesNameWithoutLine)
                {
                    valor += Environment.NewLine + item;
                }

                TaskDialog.Show("ERRO!", String.Format("O Revit não encontrou a linha de 1 mm dentro das seguintes famílias: \n {0} \n \nInsira a linha e tente novamente.", valor));
                return Result.Failed;
            }

            //Get Family Symbol
            ViewFamilyType viewFamilyType3D = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x => ViewFamily.ThreeDimensional == x.ViewFamily);

            //Get Section Box family
            using (TransactionGroup transGroup = new TransactionGroup(doc, "Isometric Create"))
            {
                transGroup.Start("Gerar Isométrico");
                using (Transaction firstTrans = new Transaction(doc))
                {
                    try
                    {
                        firstTrans.Start("first transaction");
                        //Create new Isometric View
                        view3d = View3D.CreateIsometric(doc, viewFamilyType3D.Id);

                        //Set section box around selected elements
                        view3d.SetSectionBox(sb);

                        //isolate selected elements in 3D View
                        view3d.IsolateElementsTemporary(eleIds);

                        //Setting orientation
                        view3d.SetOrientation(viewOrientation3D);

                        //Save Orientation and Lock
                        view3d.SaveOrientationAndLock();

                        //set view template
                        view3d.ViewTemplateId = viewtemplateId;
                        firstTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return Result.Failed;
                    }
                }

                //DIRIGINDO A VISTA ATIVA PARA A VISTA DO ISOMÉTRICO CRIADO
                uidoc.ActiveView = view3d;

                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_SectionBox);
                ICollection<ElementId> sectionElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .WherePasses(filter)
                    .ToElementIds();

                //-------------------------------------------------INSERINDO IDENTIFICADORES 
                using (Transaction secondTrans = new Transaction(doc))
                {
                    try
                    {
                        XYZ point;
                        secondTrans.Start("second transaction");
                        uidoc.ActiveView.HideElementsTemporary(sectionElements);

                        view3d.IsolateElementsTemporary(eleIds);
                        view3d.ConvertTemporaryHideIsolateToPermanent();

                        foreach (ElementId eleId in eleIds)
                        {
                            Element ele = doc.GetElement(eleId);

                            Reference refe = new Reference(ele);
                            string categoryName = ele.Category.Name;

                            if (categoryName == "Tubulação")
                            {
                                LocationCurve lc = ele.Location as LocationCurve;
                                Line lineCurve = lc.Curve as Line;
                                point = lc.Curve.Evaluate(0.5, true);
                                XYZ newPoint = new XYZ(point.X, point.Y, point.Z);
                                IndependentTag tagPipe1 = IndependentTag.Create(doc, pipeTag1, view3d.Id, refe, true, tOrient, newPoint);
                                IndependentTag tagPipe2 = IndependentTag.Create(doc, pipeTag2, view3d.Id, refe, false, tOrient, newPoint);
                            }
                            else if (categoryName.Contains("Acessórios"))
                            {
                                LocationPoint loc = ele.Location as LocationPoint;
                                point = loc.Point;
                                IndependentTag tagAccessoryPipe = IndependentTag.Create(doc, pipeAcessoryTag, view3d.Id, refe, true, tOrient, point);
                            }
                            else if (categoryName.Contains("Peças hidrossanitárias"))
                            {
                                LocationPoint loc = ele.Location as LocationPoint;
                                point = loc.Point;
                                IndependentTag tagAccessoryPipe = IndependentTag.Create(doc, plumbingFixtureTag, view3d.Id, refe, true, tOrient, point);
                            }
                            else if (categoryName.Contains("Conexões"))
                            {
                                LocationPoint loc = ele.Location as LocationPoint;
                                point = loc.Point;
                                IndependentTag tagpipeFitting = IndependentTag.Create(doc, pipeFittingTag, view3d.Id, refe, true, tOrient, point);
                            }
                        }
                        secondTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return Result.Failed;
                    }
                }


                //-------------------------------------------------INSERINDO COTAS
                using (Transaction thirdTrans = new Transaction(doc))
                {
                    thirdTrans.Start("third transaction");

                    foreach (ElementId eleId in eleIds)
                    {
                        Element ele = doc.GetElement(eleId);
                        ReferenceArray referenceArray = new ReferenceArray();
                        XYZ startPoint = null;
                        XYZ endPoint = null;
                        List<XYZ> myPoints = new List<XYZ>();
                        Reference startPointRef = null;
                        Reference endPointRef = null;

                        //-------------------------------CRIANDO COTAS EM CADA UMA DAS TUBULAÇÕES A PARTIR DA COLETA DAS LINHAS DENTRO DAS CONEXÕES DE CADA EXTREMIDADE
                        if (ele.Category.Name == "Tubulação")
                        {
                            LocationCurve lc = ele.Location as LocationCurve;
                            Line linePoint = lc.Curve as Line;
                            XYZ mediuPoint = linePoint.Origin;
                            XYZ mediumPoint = lc.Curve.Evaluate(0.5, true);

                            ConnectorSet connectors = null;
                            if (ele is MEPCurve)
                            {
                                connectors = ((MEPCurve)ele).ConnectorManager.Connectors;
                            }

                            //COLETANDO CADA UM DOS CONECTORES DE CADA TUBULAÇÃO
                            foreach (Connector connector in connectors)
                            {
                                Line oneMillimieterLine = null;

                                //SE ESTIVER CONECTADO...
                                if (connector.IsConnected == true)
                                {
                                    ConnectorSet connectorSet = connector.AllRefs;

                                    foreach (Connector connec in connectorSet)
                                    {
                                        Element myEle = connec.Owner;

                                        if (myEle.Category.Name != "Tubulação" && myEle.Category.Name != "Sistemas de tubulação")
                                        {
                                            FamilyInstance familyInstance = myEle as FamilyInstance;
                                            FamilySymbol familySymbol = familyInstance.Symbol;
                                            string familyName = familySymbol.FamilyName.ToString();
                                            string familyID = myEle.Id.ToString();

                                            //COLETANDO O PONTO DE LOCALIZAÇÃO CENTRAL DA CONEXÃO                                               
                                            LocationPoint locationPoint = myEle.Location as LocationPoint;
                                            XYZ originPoint = locationPoint.Point;
                                            myPoints.Add(originPoint);

                                            //OPTIONS
                                            Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

                                            //COLETANDO A LINHA DE 1 mm DENTRO DA CONEXÃO                                              
                                            GeometryElement gE = myEle.get_Geometry(opts);

                                            foreach (GeometryInstance gI in gE)
                                            {
                                                GeometryElement geoElem = gI.GetSymbolGeometry();
                                                foreach (var obj in geoElem)
                                                {
                                                    if (obj is Line)
                                                    {
                                                        oneMillimieterLine = obj as Line;
                                                        if (Math.Round(oneMillimieterLine.ApproximateLength, 5) == 0.00328)
                                                        {
                                                            try
                                                            {
                                                                startPoint = oneMillimieterLine.GetEndPoint(0);
                                                                startPointRef = oneMillimieterLine.GetEndPointReference(0);
                                                                endPoint = oneMillimieterLine.GetEndPoint(1);
                                                                endPointRef = oneMillimieterLine.GetEndPointReference(1);
                                                            }
                                                            catch
                                                            {
                                                            }
                                                        }
                                                    }
                                                }

                                                double distancebetweenPoints = originPoint.DistanceTo(startPoint);

                                                if (distancebetweenPoints < 0.000001)
                                                {
                                                    referenceArray.Append(endPointRef);
                                                }
                                                else
                                                {
                                                    referenceArray.Append(startPointRef);
                                                }
                                            }
                                        }
                                    }
                                }
                                //SE O TUBO NÃO ESTIVER CONECTADO EM UMA DAS EXTREMIDADES
                                else if (connector.IsConnected == false)
                                {
                                    //COLETANDO O PONTO DE ORIGEM DO CONECTOR DO TUBO QUE ESTÁ DESCONECTADO                                    
                                    XYZ originPoint = connector.Origin;
                                    myPoints.Add(originPoint);
                                    Line pipeLine = null;

                                    //OPÇÕES
                                    Options opts1 = new Options() { View = doc.ActiveView, ComputeReferences = true, IncludeNonVisibleObjects = true };

                                    //COLETANDO A LINHA CENTRAL DO TUBO                                    
                                    GeometryElement geoEle = ele.get_Geometry(opts1);
                                    try
                                    {

                                        foreach (GeometryObject geoObj in geoEle)
                                        {
                                            if (geoObj is Line)
                                            {
                                                pipeLine = geoObj as Line;
                                            }
                                        }
                                        //COMPARANDO OS PONTOS DE REFERÊNCIA COM O PONTO DE ORIGEM DO CONECTOR
                                        startPoint = pipeLine.GetEndPoint(0);
                                        startPointRef = pipeLine.GetEndPointReference(0);
                                        endPoint = pipeLine.GetEndPoint(1);
                                        endPointRef = pipeLine.GetEndPointReference(1);

                                        double distancebetweenPoints = originPoint.DistanceTo(startPoint);

                                        if (distancebetweenPoints < 0.000001)
                                        {
                                            referenceArray.Append(startPointRef);
                                        }
                                        else
                                        {
                                            referenceArray.Append(endPointRef);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                            if (myPoints.Count == 2)
                            {
                                try
                                {
                                    //ESTUDO DOS PONTOS PARA INSERÇÃO DAS COTAS E CRIAÇÃO DOS PLANOS DE REFERÊNCIA 
                                    XYZ point1 = myPoints[0];
                                    XYZ point2 = myPoints[1];
                                    XYZ newMediumPoint = new XYZ();

                                    //TaskDialog.Show("teste", point1.ToString() + Environment.NewLine + point2.ToString() + Environment.NewLine + mediuPoint.ToString());

                                    if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                    {
                                        //TaskDialog.Show("teste", "sou coplanar");

                                        if (Math.Round(mediuPoint.X, 0) == Math.Round(point1.X, 0) && Math.Round(mediuPoint.Y, 0) == Math.Round(point1.Y, 0))
                                        {
                                            //TaskDialog.Show("teste", "x e y igual");
                                            newMediumPoint = new XYZ(mediuPoint.X + 100, mediuPoint.Y + 100, mediuPoint.Z);
                                        }
                                        else if (Math.Round(mediuPoint.X, 0) == Math.Round(point1.X, 0))
                                        {
                                            //TaskDialog.Show("teste", "x igual");
                                            newMediumPoint = new XYZ(mediuPoint.X + 100, mediuPoint.Y, mediuPoint.Z);
                                        }
                                        else if (Math.Round(mediuPoint.Y, 0) == Math.Round(point1.Y, 0))
                                        {
                                            //TaskDialog.Show("teste", "y igual");
                                            newMediumPoint = new XYZ(mediuPoint.X, mediuPoint.Y + 100, mediuPoint.Z);
                                        }
                                        else
                                        {
                                            //TaskDialog.Show("teste", "nenhum dos casos");
                                            newMediumPoint = new XYZ(mediuPoint.X+10, mediuPoint.Y + 10, mediuPoint.Z);
                                        }
                                        Plane myPlane = Plane.CreateByThreePoints(point1, point2, newMediumPoint);
                                        SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                        uidoc.ActiveView.SketchPlane = sktPlane;
                                    }
                                    else
                                    {
                                        //TaskDialog.Show("teste", "não sou coplanar");
                                        //TaskDialog.Show("teste", point1.ToString() + Environment.NewLine + point2.ToString() + Environment.NewLine + mediumPoint.ToString());
                                        
                                        if (Math.Round(point1.X, 0) == Math.Round(point2.X, 0) && Math.Round(point1.Y, 0) == Math.Round(point2.Y, 0))
                                        {
                                            //TaskDialog.Show("teste", "x e y iguais");
                                            newMediumPoint = new XYZ(mediumPoint.X, mediumPoint.Y + 10, mediumPoint.Z);                                         
                                        }
                                        else if (Math.Round(point1.X, 0) == Math.Round(point2.X, 0))
                                        {
                                            //TaskDialog.Show("teste", "x iguais");
                                            newMediumPoint = new XYZ(mediumPoint.X + 10, mediumPoint.Y , mediumPoint.Z);
                                        }
                                        else if (Math.Round(point1.Y, 0) == Math.Round(point2.Y, 0))
                                        {
                                            //TaskDialog.Show("teste", "y iguais");
                                            newMediumPoint = new XYZ(mediumPoint.X, mediumPoint.Y + 10, mediumPoint.Z);
                                        }
                                        else
                                        {
                                            //TaskDialog.Show("teste", "nenhuma das alternativas");
                                            newMediumPoint = mediumPoint;
                                        }

                                        Plane myPlane = Plane.CreateByThreePoints(point1, point2, newMediumPoint);
                                        SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                        uidoc.ActiveView.SketchPlane = sktPlane;
                                    }
                                    Line linebetweenFittings = Line.CreateBound(point1, point2);
                                    Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, linebetweenFittings, referenceArray);
                                    //TaskDialog.Show("Teste", newDimension.ToString());
                                }
                                catch
                                {
                                }
                            }
                        }
                        //--------------------------------------------CASO DUAS CONEXÕES ESTEJAM CONECTADAS ENTRE SI...
                        else if (ele.Category.Name == "Conexões de tubo" || ele.Category.Name == "Equipamento mecânico" || ele.Category.Name == "Acessórios do tubo" || ele.Category.Name == "Peças hidrossanitárias")
                        {                            
                            Element myOwner = null;
                            Line lineObj = null;                         
                            FamilyInstance fs = ele as FamilyInstance;
                            MEPModel mepModel = fs.MEPModel;
                            LocationPoint locationPoint1 = ele.Location as LocationPoint;
                            XYZ originPoint1 = locationPoint1.Point;
                            myPoints.Add(originPoint1);

                            //Options
                            Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

                            //Get profile area                                               
                            GeometryElement gE1 = ele.get_Geometry(opts);

                            try
                            {
                                foreach (GeometryInstance gI in gE1)
                                {
                                    GeometryElement geoElem = gI.GetSymbolGeometry();
                                    foreach (var obj in geoElem)
                                    {
                                        if (obj is Line)
                                        {
                                            lineObj = obj as Line;
                                        }
                                    }
                                    //Coletando os pontos inicial e final da linha
                                    startPoint = lineObj.GetEndPoint(0);
                                    startPointRef = lineObj.GetEndPointReference(0);
                                    endPoint = lineObj.GetEndPoint(1);
                                    endPointRef = lineObj.GetEndPointReference(1);

                                    double distancebetweenPoints = originPoint1.DistanceTo(startPoint);

                                    if (distancebetweenPoints < 0.000001)
                                    {
                                        referenceArray.Append(endPointRef);
                                    }
                                    else
                                    {
                                        referenceArray.Append(startPointRef);
                                    }
                                }
                            }
                            catch 
                            {                                
                            }

                            ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
                            foreach (Connector connector in connectors)
                            {
                                if (connector.IsConnected == true)
                                {
                                    ConnectorSet connectorSet = connector.AllRefs;

                                    foreach (Connector connec in connectorSet)
                                    {
                                        Element myOwnerElement = connec.Owner;
                                        string categoryOwner = myOwnerElement.Category.Name;

                                        if (categoryOwner != "Tubulação" && categoryOwner != "Sistemas de tubulação")
                                        {
                                            myOwner = myOwnerElement;
                                        }
                                    }

                                    if (myOwner != null)
                                    {
                                        LocationPoint locationPoint2 = myOwner.Location as LocationPoint;
                                        XYZ originPoint2 = locationPoint2.Point;
                                        myPoints.Add(originPoint2);

                                        //Get profile area                                               
                                        GeometryElement gE2 = myOwner.get_Geometry(opts);

                                        try
                                        {
                                            foreach (GeometryInstance gI in gE2)
                                            {
                                                GeometryElement geoElem = gI.GetSymbolGeometry();
                                                foreach (var obj in geoElem)
                                                {
                                                    if (obj is Line)
                                                    {
                                                        lineObj = obj as Line;
                                                    }
                                                }

                                                //Coletando os pontos inicial e final da linha
                                                startPoint = lineObj.GetEndPoint(0);
                                                startPointRef = lineObj.GetEndPointReference(0);
                                                endPoint = lineObj.GetEndPoint(1);
                                                endPointRef = lineObj.GetEndPointReference(1);

                                                double distancebetweenPoints = originPoint2.DistanceTo(startPoint);

                                                if (distancebetweenPoints < 0.000001)
                                                {
                                                    referenceArray.Append(endPointRef);
                                                }
                                                else
                                                {
                                                    referenceArray.Append(startPointRef);
                                                }
                                            }
                                        }
                                        catch
                                        {                                            
                                        }
                                    }
                                }
                            }

                            if (myPoints.Count >= 2)
                            {
                                XYZ point1 = myPoints[0];
                                XYZ point2 = myPoints[1];

                                if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                {
                                    XYZ point3 = new XYZ(myPoints[0].X + 20, myPoints[0].Y + 20, myPoints[0].Z);
                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                    uidoc.ActiveView.SketchPlane = sktPlane;
                                }
                                else
                                {
                                    XYZ point3 = new XYZ(myPoints[0].X, myPoints[0].Y + 100, myPoints[1].Z - 50);
                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                    uidoc.ActiveView.SketchPlane = sktPlane;
                                }
                                Line linebetweenFittings = Line.CreateBound(point1, point2);
                                Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, linebetweenFittings, referenceArray);
                            }

                            //FamilyInstance familyInstace1 = ele as FamilyInstance;
                            //FamilySymbol familySymbol1 = familyInstace1.Symbol;
                            //MEPModel mepModel = familyInstace1.MEPModel;
                            //string familyName1 = familySymbol1.FamilyName.ToString();
                            //string familyID1 = ele.Id.ToString();
                            //Line oneMillimieterLine = null;

                            ////COLETANDO O PONTO DE REFERÊNCIA DO ELEMENTO COM BASE NO LOCATION POINT                           
                            //LocationPoint locationPoint1 = ele.Location as LocationPoint;
                            //XYZ originPoint1 = locationPoint1.Point;
                            //myPoints.Add(originPoint1);
                            //List<int> lineCount1 = new List<int>();

                            ////OPTIONS
                            //Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

                            ////COLENTANDO A GEOMETRIA DO ELEMENTO
                            //GeometryElement gE = ele.get_Geometry(opts);

                            //foreach (GeometryInstance gI in gE)
                            //{
                            //    GeometryElement geoElem = gI.GetSymbolGeometry();
                            //    foreach (var obj in geoElem)
                            //    {
                            //        if (obj is Line)
                            //        {
                            //            oneMillimieterLine = obj as Line;
                            //            if (Math.Round(oneMillimieterLine.ApproximateLength, 5) == 0.00328)
                            //            {                                            
                            //                try
                            //                {
                            //                    startPoint = oneMillimieterLine.GetEndPoint(0);
                            //                    startPointRef = oneMillimieterLine.GetEndPointReference(0);
                            //                    endPoint = oneMillimieterLine.GetEndPoint(1);
                            //                    endPointRef = oneMillimieterLine.GetEndPointReference(1);
                            //                }
                            //                catch
                            //                {
                            //                }
                            //            }
                            //        }
                            //    }                             

                            //    double distancebetweenPoints = originPoint1.DistanceTo(startPoint);

                            //    if (distancebetweenPoints < 0.000001)
                            //    {
                            //        referenceArray.Append(endPointRef);
                            //    }
                            //    else
                            //    {
                            //        referenceArray.Append(startPointRef);
                            //    }
                            //}

                            //XYZ originPoint2 = new XYZ();

                            ////PROCURANDO SE A CONEXÃO ESTÁ CONECTADA A OUTRA CONEXÃO...
                            //ConnectorSet connectors = mepModel.ConnectorManager.Connectors;
                            //foreach (Connector connector in connectors)
                            //{
                            //    if (connector.IsConnected == true)
                            //    {
                            //        ConnectorSet connectorSet = connector.AllRefs;

                            //        foreach (Connector connec in connectorSet)
                            //        {
                            //            Element myEle = connec.Owner;

                            //            if (myEle.Category.Name != "Tubulação" && myEle.Category.Name != "Sistemas de tubulação")
                            //            {
                            //                FamilyInstance familyInstance2 = myEle as FamilyInstance;
                            //                FamilySymbol familySymbol2 = familyInstance2.Symbol;
                            //                string familyName2 = familySymbol2.FamilyName.ToString();
                            //                string familyID2 = myEle.Id.ToString();

                            //                //COLETANDO O PONTO DE LOCALIZAÇÃO CENTRAL DA CONEXÃO                                               
                            //                LocationPoint locationPoint2 = myEle.Location as LocationPoint;
                            //                originPoint2 = locationPoint2.Point;                                           
                            //                myPoints.Add(originPoint2);                                            

                            //                //OPTIONS
                            //                Options opts2 = new Options() { View = doc.ActiveView, ComputeReferences = true };

                            //                //COLETANDO A LINHA DE 1 mm DENTRO DA CONEXÃO                                              
                            //                GeometryElement gE2 = myEle.get_Geometry(opts);

                            //                foreach (GeometryInstance gI in gE)
                            //                {
                            //                    GeometryElement geoElem = gI.GetSymbolGeometry();
                            //                    foreach (var obj in geoElem)
                            //                    {
                            //                        if (obj is Line)
                            //                        {
                            //                            oneMillimieterLine = obj as Line;
                            //                            if (Math.Round(oneMillimieterLine.ApproximateLength, 5) == 0.00328)
                            //                            {                                                            
                            //                                //SE A LINHA DE 1 mm FOR ENCONTRADA, COLETAR OS PONTOS INICIAL E FINAL E SUAS RESPECTIVAS REFERÊNCIAS
                            //                                //PARA A CRIAÇÃO DA COTA
                            //                                try
                            //                                {
                            //                                    startPoint = oneMillimieterLine.GetEndPoint(0);
                            //                                    startPointRef = oneMillimieterLine.GetEndPointReference(0);
                            //                                    endPoint = oneMillimieterLine.GetEndPoint(1);
                            //                                    endPointRef = oneMillimieterLine.GetEndPointReference(1);
                            //                                }
                            //                                catch
                            //                                {
                            //                                }
                            //                            }
                            //                        }
                            //                    }                                                

                            //                    double distancebetweenPoints = originPoint2.DistanceTo(startPoint);

                            //                    if (distancebetweenPoints < 0.000001)
                            //                    {
                            //                        referenceArray.Append(endPointRef);
                            //                    }
                            //                    else
                            //                    {
                            //                        referenceArray.Append(startPointRef);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            ////Identificando se existem dois pontos válidos para criação da cota entre eles
                            //if (myPoints.Count == 2)
                            //{
                            //    try
                            //    {
                            //        XYZ point1 = myPoints[0];
                            //        XYZ point2 = myPoints[1];
                            //        XYZ newPoint1 = new XYZ();
                            //        XYZ normalX = new XYZ(1, 0, 0);
                            //        XYZ normalY = new XYZ(0, 1, 0);

                            //        if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                            //        {
                            //            TaskDialog.Show("teste", "sou coplanar");

                            //            Plane myPlane = Plane.CreateByOriginAndBasis(point1, normalX, normalY);
                            //            SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                            //            uidoc.ActiveView.SketchPlane = sktPlane;
                            //        }
                            //        else
                            //        {
                            //            //TaskDialog.Show("teste", "não sou coplanar");
                            //            //TaskDialog.Show("teste", point1.Z.ToString());
                            //            //TaskDialog.Show("teste", point2.Z.ToString());
                            //            //TaskDialog.Show("teste", newPoint1.Z.ToString());

                            //            //if (Math.Round(point1.X, 0) == Math.Round(point2.X, 0) && Math.Round(point1.Y, 0) == Math.Round(point2.Y, 0))
                            //            //{
                            //            //    TaskDialog.Show("teste", "entrei");
                            //            //    newPoint1 = new XYZ(point1.X + 100, point1.Y + 100, point1.Z + 100);
                            //            //    point1 = new XYZ(point1.X, point1.Y, point1.Z + 32);
                            //            //}
                            //            //else if (Math.Round(point1.Z, 0) == Math.Round(point2.Z, 0))
                            //            //{
                            //            //    TaskDialog.Show("teste","entrei");
                            //            //    point1 = new XYZ(point1.X, point1.Y, point1.Z + 32);
                            //            //}
                            //            //else if (Math.Round(point1.X, 0) == Math.Round(point2.X, 0))
                            //            //{
                            //            //    newPoint1 = new XYZ(point1.X + 100, point1.Y, point1.Z + 100);
                            //            //}
                            //            //else if (Math.Round(point1.Y, 0) == Math.Round(point2.Y, 0))
                            //            //{
                            //            //    newPoint1 = new XYZ(point1.X, point1.Y + 100, point1.Z + 100);
                            //            //}
                            //            //else
                            //            //{
                            //            //    newPoint1 = new XYZ(point1.X, point1.Y, point1.Z + 100);
                            //            //}

                            //            //Plane myPlane = Plane.CreateByThreePoints(point1, point2, newPoint1);
                            //            //SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                            //            //uidoc.ActiveView.SketchPlane = sktPlane;

                            //            XYZ point3 = new XYZ(originPoint1.X, originPoint1.Y + 10, originPoint1.Z + 100);
                            //            Plane myPlane = Plane.CreateByThreePoints(originPoint1, originPoint2, point3);
                            //            SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                            //            uidoc.ActiveView.SketchPlane = sktPlane;
                            //        }

                            //        Line linebetweenFittings = Line.CreateBound(point1, point2);
                            //        Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, linebetweenFittings, referenceArray);
                            //    }
                            //    catch
                            //    {
                            //    }
                            //}
                        }
                    }

                    thirdTrans.Commit();
                }
                transGroup.Assimilate();
            }        
            return Result.Succeeded;
        }
    }
}