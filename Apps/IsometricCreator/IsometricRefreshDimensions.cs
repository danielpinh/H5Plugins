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
    public class IsometricRefreshDimensions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            //List Points            
            List<ElementId> eleIds = new List<ElementId>();  
            
            //Current 3D view       
            View3D current3DView = uidoc.ActiveView as View3D;
            ElementId currentViewId = current3DView.Id;

            //Get all dimensions in view and delete          
            FilteredElementCollector collectDimensions = new FilteredElementCollector(doc, currentViewId);     
            ElementCategoryFilter dimensionFilter = new ElementCategoryFilter(BuiltInCategory.OST_Dimensions);
            List<Element> dimensionsList = collectDimensions.WherePasses(dimensionFilter)
                .WhereElementIsNotElementType()
                .ToElements()
                .ToList();

            using (Transaction trans = new Transaction(doc, "Deletar Cotas"))
            {
                trans.Start();
                {
                    foreach (Element item in dimensionsList)
                    {
                        ElementId eleId = item.Id;
                        doc.Delete(eleId);
                    }
                }
                trans.Commit();
            }      


            //Collecting all elements in view            
            FilteredElementCollector eleCollec = new FilteredElementCollector(doc, currentViewId);
            List<Element> elementsList = eleCollec
                .WhereElementIsNotElementType()
                .ToElements()
                .ToList();

            
            using (TransactionGroup transGroup = new TransactionGroup(doc, "Isometric Create"))
            {
                transGroup.Start("transaction group");                               

                //INSERTING DIMENSIONS 
                using (Transaction thirdTrans = new Transaction(doc))
                {
                    thirdTrans.Start(" third transaction");
                    try
                    {                      
                        foreach (Element ele in elementsList)
                        {                            
                            if (ele.Category.Name == "Conexões de tubo" || ele.Category.Name == "Equipamento mecânico" || ele.Category.Name == "Acessórios do tubo") 
                            {                              
                                ReferenceArray referenceArray = new ReferenceArray();
                                XYZ startPoint = null;
                                XYZ endPoint = null;
                                List<XYZ> myPoints = new List<XYZ>();
                                Element myOwner = null;
                                Line lineObj = null;
                                Reference startPointRef = null;
                                Reference endPointRef = null;

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
                                catch (Exception ex)
                                {
                                    message = ex.Message;
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

                                            if (categoryOwner != "Tubulação")
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
                                            catch (Exception ex)
                                            {
                                                message = ex.Message;
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
                            }
                            else if (ele.Category.Name == "Tubulação")
                            {
                                
                                ReferenceArray referenceArray = new ReferenceArray();
                                XYZ startPoint = null;
                                XYZ endPoint = null;
                                List<XYZ> myPoints = new List<XYZ>();
                                //Element myOwner = null;
                                Line lineObj = null;
                                Reference startPointRef = null;
                                Reference endPointRef = null;

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
                        }
                        thirdTrans.Commit();
                    }                  
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return Result.Failed;
                    }                    
                }
                transGroup.Assimilate();
            }
            return Result.Succeeded;
        }
    }
}