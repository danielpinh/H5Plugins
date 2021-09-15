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

            //Coletando as tags
            Collectors myCollector = new Collectors();
            ElementId pipeFittingTag = myCollector.PipeFittingsTagsbyFamilyName(doc, "ID-CONEXÕES DE TUBO-ITEM+POSICIONAMENTO");           
            ElementId pipeTag1 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-ITEM+POSICIONAMENTO");
            ElementId pipeTag2 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-TAG+SISTEMA+DIÂMETRO");              
            ElementId pipeAcessoryTag = myCollector.PipeAcessoryTagsbyFamilyName(doc, "ID-ACESSÓRIOS DE TUBO-ITEM+POSICIONAMENTO");
            ElementId plumbingFixtureTag = myCollector.PlumbingFixtureTagsbyFamilyName(doc, "ID-PEÇA HIDROSSANITÁRIA-ITEM+POSICIONAMENTO");

            //creating a list of ElementId for each selected elements
            foreach (Reference item in refs)
            {
                Element ele = doc.GetElement(item.ElementId);
                ElementId eleId = ele.Id;
                eleIds.Add(eleId);
            }

            //Get Family Symbol
            ViewFamilyType viewFamilyType3D = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x => ViewFamily.ThreeDimensional == x.ViewFamily);

            //Get Section Box family
            using (TransactionGroup transGroup = new TransactionGroup(doc, "Isometric Create"))
            {
                transGroup.Start("transaction group");
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

                //Set active view for the new created Isometric View
                uidoc.ActiveView = view3d;

                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_SectionBox);
                ICollection<ElementId> sectionElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .WherePasses(filter)
                    .ToElementIds();             

                //INSERINDO IDENTIFICADORES 
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
                            else if(categoryName.Contains("Conexões"))
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

                //INSERINDO COTAS
                using (Transaction thirdTrans = new Transaction(doc))
                {
                    thirdTrans.Start(" third transaction");
                    try
                    {                     
                        foreach (ElementId eleId in eleIds)
                        {
                            Element ele = doc.GetElement(eleId);
                            ReferenceArray referenceArray = new ReferenceArray();
                            XYZ startPoint = null;
                            XYZ endPoint = null;
                            List<XYZ> myPoints = new List<XYZ>();
                            Element myOwner = null;
                            Line lineObj = null;
                            Reference startPointRef = null;
                            Reference endPointRef = null;

                            //Coletando as conexões, acessórios e peças hidrossanitárias na linha
                            if (ele.Category.Name != "Tubulação")
                            {
                                FamilyInstance fs = ele as FamilyInstance;
                                MEPModel mepModel = fs.MEPModel;                            

                                //Coletando o ponto de localização das conexões, acessórios e peças hidrossanitárias
                                LocationPoint locationPoint1 = ele.Location as LocationPoint;
                                XYZ originPoint1 = locationPoint1.Point;
                                myPoints.Add(originPoint1);                                                      

                                //Options + geometria do elemento
                                Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };                                                                               
                                GeometryElement gE1 = ele.get_Geometry(opts); 

                                //Coletando a linha de 1 mm dentro de cada elemento
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

                                        //Avaliando se um dos pontos da linha coincide com o ponto de localização das conexões
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
                                catch (Exception ex)
                                {
                                    message = ex.Message;
                                }

                                //identificando se as conexões estão conectadas com outras conexões para inserir cotas entre elas
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

                                            if (categoryOwner != "Tubulação")
                                            {
                                                myOwner = myOwnerElement;
                                            }
                                        }
                                        //Se houver alguma conexão conectada a outra conexão, coletar o ponto de localização da conexão e a linha de 1 mm dentro dela
                                        if (myOwner != null )
                                        {                                            
                                            LocationPoint locationPoint2 = myOwner.Location as LocationPoint;
                                            XYZ originPoint2 = locationPoint2.Point;
                                            myPoints.Add(originPoint2);

                                            //Geometria do elemento                                              
                                            GeometryElement gE2 = myOwner.get_Geometry(opts);

                                            //Encontrando a linha de 1mm dentro da conexão que está conectada a outra conexão
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

                                                    //Avaliando se um dos pontos da linha coincide com o ponto de localização das conexões
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
                                            catch (Exception ex)
                                            {
                                                message = ex.Message;
                                            }
                                        }
                                    }                                   
                                }

                                //Identificando se existem dois pontos válidos para criação da cota entre eles
                                if (myPoints.Count >= 2)
                                {

                                    //Se houver, coletar os dois pontos individualmente
                                    XYZ point1 = myPoints[0];
                                    XYZ point2 = myPoints[1];

                                    //Criando um plano que passa pelos 2 pontos como referêcia para inserção da cota
                                    //Se os pontos forem coplanares...
                                    if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                    {
                                        XYZ point3 = new XYZ(myPoints[0].X + 20, myPoints[0].Y + 20, myPoints[0].Z);
                                        Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                        SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                        uidoc.ActiveView.SketchPlane = sktPlane;
                                    }
                                    //Se os pontos NÃO forem coplanares...
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
                            }

                            //Para linha com tubulação sem conexão em uma das extremidades...
                            else if (ele.Category.Name == "Tubulação")
                            {
                                ConnectorSet connectors = null;

                                if (ele is MEPCurve)
                                {
                                    connectors = ((MEPCurve)ele).ConnectorManager.Connectors;
                                }

                                foreach (Connector connector in connectors)
                                {                                    

                                    if (connector.IsConnected == true)
                                    {
                                        ConnectorSet connectorSet = connector.AllRefs;
                                        foreach (Connector connec in connectorSet)
                                        {
                                            Element myEle = connec.Owner;

                                            if (myEle.Category.Name != "Tubulação")
                                            {
                                                //Coletando o ponto de origem da família de conexão                                                
                                                LocationPoint locationPoint = myEle.Location as LocationPoint;
                                                XYZ originPoint = locationPoint.Point;
                                                myPoints.Add(originPoint);

                                                //Options
                                                Options opts = new Options() { View = doc.ActiveView, ComputeReferences = true };

                                                //Get profile area                                               
                                                GeometryElement gE = myEle.get_Geometry(opts);
                                                try
                                                {
                                                    foreach (GeometryInstance gI in gE)
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
                                                catch (Exception ex)
                                                {
                                                    message = ex.Message;                                                   
                                                }                                                                                        
                                            }
                                        }
                                    }
                                    else if (connector.IsConnected == false)
                                    {                                        
                                        XYZ originPoint = connector.Origin;                                        
                                        myPoints.Add(originPoint);

                                        //Options
                                        Options opts1 = new Options() { View = doc.ActiveView, ComputeReferences = true, IncludeNonVisibleObjects = true };
                                        
                                        //Pegando a linha central do tubo                                        
                                        GeometryElement geoEle = ele.get_Geometry(opts1);
                                        try
                                        {
                                            List<Line> myLines = new List<Line>();

                                            foreach (GeometryObject geoObj in geoEle)
                                            {                                                
                                                if (geoObj is Line)
                                                {                                                    
                                                    lineObj = geoObj as Line;
                                                    myLines.Add(lineObj);
                                                                                                      
                                                }                                             
                                            }

                                            lineObj = myLines[1];
                                            //Coletando os pontos inicial e final da linha
                                            startPoint = lineObj.GetEndPoint(0);
                                            startPointRef = lineObj.GetEndPointReference(0);
                                            endPoint = lineObj.GetEndPoint(1);
                                            endPointRef = lineObj.GetEndPointReference(1);                                          
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
                                        catch (Exception ex)
                                        {
                                            message = ex.Message;
                                        }                                       
                                    }
                                }                                                          

                                XYZ point1 = myPoints[0];
                                XYZ point2 = myPoints[1];

                                if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                {
                                    XYZ point3 = new XYZ(myPoints[0].X + 20, myPoints[0].Y + 20, myPoints[0].Z);                                    
                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);         
                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);
                                    uidoc.ActiveView.   SketchPlane = sktPlane;
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
                        }
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return Result.Failed;
                    }

                    thirdTrans.Commit();
                }
                transGroup.Assimilate();
            }

            return Result.Succeeded;
        }
    }
}