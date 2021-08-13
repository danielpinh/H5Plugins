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
            TagMode tmode = TagMode.TM_ADDBY_CATEGORY;
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
            BoundingBoxXYZ sb = new BoundingBoxXYZ();
            sb.Min = new XYZ(points.Min(p => p.X),
                           points.Min(p => p.Y),
                           points.Min(p => p.Z));
            sb.Max = new XYZ(points.Max(p => p.X),
                           points.Max(p => p.Y),
                           points.Max(p => p.Z));

            Collectors myCollector = new Collectors();

            ElementId pipeFittingTag = myCollector.PipeFittingsTagsbyFamilyName(doc, "ID-CONEXÕES DE TUBO-ITEM+POSICIONAMENTO");
           
            ElementId pipeTag1 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-ITEM+POSICIONAMENTO");

            ElementId pipeTag2 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-TAG+SISTEMA+DIÂMETRO");
            
            ElementId pipeAcessoryTag = myCollector.PipeAcessoryTagsbyFamilyName(doc, "ID-ACESSÓRIOS DE TUBO-ITEM+POSICIONAMENTO");           

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


                //INSERTING TAGS 
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
                                //Parameter diameterParam = ele.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                                //string diameterParamString = diameterParam.AsValueString().ToString();

                                //int diameterValue = int.Parse(diameterParamString) / 55;

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

                //INSERTING DIMENSIONS
                using (Transaction thirdTrans = new Transaction(doc))
                {
                    thirdTrans.Start(" third transaction");
                    
                    try
                    {
                        foreach (ElementId eleId in eleIds)
                        {
                            List<XYZ> locationPoints = new List<XYZ>();
                            Element ele = doc.GetElement(eleId);
                            ConnectorSet connectors = null;
                            ReferenceArray referenceArray = new ReferenceArray();                       

                            if (ele.Category.Name == "Tubulação")
                            {                               
                                if (ele is MEPCurve)
                                {
                                    connectors = ((MEPCurve)ele).ConnectorManager.Connectors;
                                }                                                           

                                foreach (Connector connector in connectors)
                                {
                                    ConnectorSet connectorSet = connector.AllRefs;

                                    if (connector.IsConnected == true)
                                    {

                                        foreach (Connector connec in connectorSet)
                                        {

                                            Element myEle = connec.Owner;
                                            if ((myEle.Category.Name == "Tubulação") is false)
                                            {                                                                            
                                                Location location = myEle.Location;
                                                LocationPoint locationPoint = location as LocationPoint;
                                                XYZ point = locationPoint.Point;
                                                locationPoints.Add(point);                                           
                                            }
                                        }
                                    }
                                    else
                                    {
                                        XYZ emptyConnec = connector.Origin;
                                        locationPoints.Add(emptyConnec);
                                    }
                                }                               
                                                        
                                XYZ point1 = locationPoints[0];
                                XYZ point2 = locationPoints[1];                                
                                ModelCurve modelCurve;
                                Line linebetweenFittings = null;
                                Line shortLineinFitting = null;

                                
                                //check if points are coplanars 
                                if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                {                                    

                                    foreach (Connector connector in connectors)
                                    {
                                        ConnectorSet connectorSet = connector.AllRefs;

                                        if (connector.IsConnected == true)
                                        {

                                            foreach (Connector connec in connectorSet)
                                            {

                                                Element myEle = connec.Owner;
                                                if ((myEle.Category.Name == "Tubulação") is false)
                                                {
                                                    //Pega a localização da conexão atual e compara os dois
                                                    //location points das conexões que foram coletadas
                                                    //anteriormente
                                                    
                                                    //Location point atual
                                                    Location location = myEle.Location;
                                                    LocationPoint locationPoint = location as LocationPoint;
                                                    XYZ locationPointOrigin = locationPoint.Point;

                                                    //compara com os location points das conexões anteriores                                                    
                                                    double distancebetweenfittingPoints = Math.Round(locationPointOrigin.DistanceTo(point1), 4);

                                                    if (distancebetweenfittingPoints <= 0.00001)
                                                    {                                                        
                                                        linebetweenFittings = Line.CreateBound(locationPointOrigin, point2);
                                                    }
                                                    else
                                                    {                                                        
                                                        linebetweenFittings = Line.CreateBound(locationPointOrigin, point1);
                                                    }

                                                    //Coletando um novo ponto muito próximo ao locationPoint da conexão sobre a linha que liga elas duas
                                                    XYZ intermediatePoint = linebetweenFittings.Evaluate(0.005, true);

                                                    //nova linha muito curta com origem na conexão
                                                    shortLineinFitting = Line.CreateBound(locationPointOrigin, intermediatePoint);

                                                    //Criando um plano de referência para criar a linha de modelo
                                                    XYZ point3 = new XYZ(locationPoints[0].X + 20, locationPoints[0].Y + 20, locationPoints[0].Z);
                                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                                    uidoc.ActiveView.SketchPlane = sktPlane;

                                                    modelCurve = doc.Create.NewModelCurve(shortLineinFitting, sktPlane);

                                                    //Pegando as referências e os pontos da linha de modelo que foi criada
                                                    //ponto inicial
                                                    Reference startpointReference = modelCurve.GeometryCurve.GetEndPointReference(0);
                                                    XYZ startPoint = modelCurve.GeometryCurve.GetEndPoint(0);
                                                    //ponto final
                                                    Reference endpointReference = modelCurve.GeometryCurve.GetEndPointReference(1);
                                                    XYZ endPoint = modelCurve.GeometryCurve.GetEndPoint(1);

                                                    //comparando os dois pontos da linha de modelo com a origem de inserção da conexão
                                                    double distanceBetweenPoints = startPoint.DistanceTo(locationPointOrigin);

                                                    if (distanceBetweenPoints <= 0.0001)
                                                    {
                                                        referenceArray.Append(startpointReference);
                                                    }
                                                    else
                                                    {
                                                        referenceArray.Append(endpointReference);
                                                    }
                                                }                                                                                      
                                            }
                                        }
                                        else
                                        {
                                            XYZ emptyConnec = connector.Origin;                                          

                                            //compara com os location points da lista anterior (tubo + conexão) com o ponto vigente (emptyConnec)                                                    
                                            double distancebetweenfittingPoints = Math.Round(emptyConnec.DistanceTo(point1), 4);

                                            if (distancebetweenfittingPoints <= 0.00001)
                                            {
                                                linebetweenFittings = Line.CreateBound(emptyConnec, point2);
                                            }
                                            else
                                            {
                                                linebetweenFittings = Line.CreateBound(emptyConnec, point1);
                                            }

                                            //Coletando um novo ponto muito próximo ao locationPoint da conexão sobre a linha que liga elas duas
                                            XYZ intermediatePoint = linebetweenFittings.Evaluate(0.005, true);

                                            //nova linha muito curta com origem na conexão
                                            shortLineinFitting = Line.CreateBound(emptyConnec, intermediatePoint);

                                            //Criando um plano de referência para criar a linha de modelo
                                            XYZ point3 = new XYZ(locationPoints[0].X + 20, locationPoints[0].Y + 20, locationPoints[0].Z);
                                            Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                            SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                            uidoc.ActiveView.SketchPlane = sktPlane;

                                            modelCurve = doc.Create.NewModelCurve(shortLineinFitting, sktPlane);

                                            //Pegando as referências e os pontos da linha de modelo que foi criada
                                            //ponto inicial
                                            Reference startpointReference = modelCurve.GeometryCurve.GetEndPointReference(0);
                                            XYZ startPoint = modelCurve.GeometryCurve.GetEndPoint(0);
                                            //ponto final
                                            Reference endpointReference = modelCurve.GeometryCurve.GetEndPointReference(1);
                                            XYZ endPoint = modelCurve.GeometryCurve.GetEndPoint(1);

                                            //comparando os dois pontos da linha de modelo com a origem de inserção da conexão
                                            double distanceBetweenPoints = startPoint.DistanceTo(emptyConnec);

                                            if (distanceBetweenPoints <= 0.0001)
                                            {
                                                referenceArray.Append(startpointReference);
                                            }
                                            else
                                            {
                                                referenceArray.Append(endpointReference);
                                            }
                                        }
                                    }                                                                      

                                }

                                //if the points are not coplanars
                                else
                                {
                                    foreach (Connector connector in connectors)
                                    {
                                        ConnectorSet connectorSet = connector.AllRefs;

                                        if (connector.IsConnected == true)
                                        {

                                            foreach (Connector connec in connectorSet)
                                            {

                                                Element myEle = connec.Owner;
                                                if ((myEle.Category.Name == "Tubulação") is false)
                                                {
                                                    //Pega a localização da conexão atual e compara os dois
                                                    //location points das conexões que foram coletadas
                                                    //anteriormente

                                                    //Location point atual
                                                    Location location = myEle.Location;
                                                    LocationPoint locationPoint = location as LocationPoint;
                                                    XYZ locationPointOrigin = locationPoint.Point;

                                                    //compara com os location points das conexões anteriores                                                    
                                                    double distancebetweenfittingPoints = Math.Round(locationPointOrigin.DistanceTo(point1), 4);

                                                    if (distancebetweenfittingPoints <= 0.00001)
                                                    {
                                                        linebetweenFittings = Line.CreateBound(locationPointOrigin, point2);
                                                    }
                                                    else
                                                    {
                                                        linebetweenFittings = Line.CreateBound(locationPointOrigin, point1);
                                                    }

                                                    //Coletando um novo ponto muito próximo ao locationPoint da conexão sobre a linha que liga elas duas
                                                    XYZ intermediatePoint = linebetweenFittings.Evaluate(0.005, true);

                                                    //nova linha muito curta com origem na conexão
                                                    shortLineinFitting = Line.CreateBound(locationPointOrigin, intermediatePoint);

                                                    //Criando um plano de referência para criar a linha de modelo
                                                    XYZ point3 = new XYZ(locationPoints[0].X, locationPoints[0].Y + 100, locationPoints[1].Z - 50);
                                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                                    uidoc.ActiveView.SketchPlane = sktPlane;

                                                    modelCurve = doc.Create.NewModelCurve(shortLineinFitting, sktPlane);

                                                    //Pegando as referências e os pontos da linha de modelo que foi criada
                                                    //ponto inicial
                                                    Reference startpointReference = modelCurve.GeometryCurve.GetEndPointReference(0);
                                                    XYZ startPoint = modelCurve.GeometryCurve.GetEndPoint(0);
                                                    //ponto final
                                                    Reference endpointReference = modelCurve.GeometryCurve.GetEndPointReference(1);
                                                    XYZ endPoint = modelCurve.GeometryCurve.GetEndPoint(1);

                                                    //comparando os dois pontos da linha de modelo com a origem de inserção da conexão
                                                    double distanceBetweenPoints = startPoint.DistanceTo(locationPointOrigin);

                                                    if (distanceBetweenPoints <= 0.0001)
                                                    {
                                                        referenceArray.Append(startpointReference);
                                                    }
                                                    else
                                                    {
                                                        referenceArray.Append(endpointReference);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {                                           
                                            XYZ emptyConnec = connector.Origin;

                                            //compara com os location points da lista anterior (tubo + conexão) com o ponto vigente (emptyConnec)                                                    
                                            double distancebetweenfittingPoints = Math.Round(emptyConnec.DistanceTo(point1), 4);

                                            if (distancebetweenfittingPoints <= 0.00001)
                                            {
                                                linebetweenFittings = Line.CreateBound(emptyConnec, point2);
                                            }
                                            else
                                            {
                                                linebetweenFittings = Line.CreateBound(emptyConnec, point1);
                                            }

                                            //Coletando um novo ponto muito próximo ao locationPoint da conexão sobre a linha que liga elas duas
                                            XYZ intermediatePoint = linebetweenFittings.Evaluate(0.005, true);

                                            //nova linha muito curta com origem na conexão
                                            shortLineinFitting = Line.CreateBound(emptyConnec, intermediatePoint);

                                            //Criando um plano de referência para criar a linha de modelo
                                            XYZ point3 = new XYZ(locationPoints[0].X + 20, locationPoints[0].Y + 20, locationPoints[0].Z);
                                            Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                            SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                            uidoc.ActiveView.SketchPlane = sktPlane;

                                            modelCurve = doc.Create.NewModelCurve(shortLineinFitting, sktPlane);

                                            //Pegando as referências e os pontos da linha de modelo que foi criada
                                            //ponto inicial
                                            Reference startpointReference = modelCurve.GeometryCurve.GetEndPointReference(0);
                                            XYZ startPoint = modelCurve.GeometryCurve.GetEndPoint(0);
                                            //ponto final
                                            Reference endpointReference = modelCurve.GeometryCurve.GetEndPointReference(1);
                                            XYZ endPoint = modelCurve.GeometryCurve.GetEndPoint(1);

                                            //comparando os dois pontos da linha de modelo com a origem de inserção da conexão
                                            double distanceBetweenPoints = startPoint.DistanceTo(emptyConnec);

                                            if (distanceBetweenPoints <= 0.0001)
                                            {
                                                referenceArray.Append(startpointReference);
                                            }
                                            else
                                            {
                                                referenceArray.Append(endpointReference);
                                            }
                                        }
                                    }
                                }
                                    
                                Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, shortLineinFitting, referenceArray);
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