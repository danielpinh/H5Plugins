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


            //var collector = new FilteredElementCollector(doc).OfClass(typeof(IndependentTag)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
            //List<ElementId> iDs = new List<ElementId>();
            //TaskDialog.Show("Teste", collector.Count().ToString());

            //foreach (Element it in collector)
            //{             
            //    Parameter par = it.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM);
            //    ElementId eleId = it.Id;
            //    string parString = par.AsValueString();                

            //    if (parString == "ID-TUBO-POSICIONAMENTO+ITEM")
            //    {
            //        iDs.Add(eleId);
            //    }                

            //}

            //TaskDialog.Show("Teste", iDs.Count().ToString());

            

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
            ElementId pipeFittingTag = myCollector.PipeFittingsTagsbyFamilyName(doc, "ID-CONEXÕES DE TUBO-POSICIONAMENTO+ITEM");
           
            ElementId pipeTag1 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-POSICIONAMENTO+ITEM");

            ElementId pipeTag2 = myCollector.PipeTagsbyFamilyName(doc, "ID-TUBO-NÚMERO DA LINHA+SISTEMA+DIÂMETRO");
            
            ElementId pipeAcessoryTag = myCollector.PipeAcessoryTagsbyFamilyName(doc, "ID-ACESSÓRIOS DE TUBO-POSICIONAMENTO+ITEM");           

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

                using (Transaction secondTrans = new Transaction(doc))
                {
                    try
                    {
                        XYZ point;
                        secondTrans.Start("second transaction");
                        uidoc.ActiveView.HideElementsTemporary(sectionElements);

                        view3d.ConvertTemporaryHideIsolateToPermanent();

                        foreach (ElementId eleId in eleIds)
                        {
                            Element ele = doc.GetElement(eleId);
                            
                            Reference refe = new Reference(ele);
                            string categoryName = ele.Category.Name;
                           
                            if (ele.Category.Name == "Tubulação")
                            {
                                Parameter diameterParam = ele.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                                string diameterParamString = diameterParam.AsValueString().ToString();

                                int diameterValue = int.Parse(diameterParamString) / 55;

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

                //get only selected pipes and inserting dimensions on its
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

                                ReferenceArray referenceArray = new ReferenceArray();
                                ModelCurve modelCurve;
                                Line line;

                                //check if points are coplanars
                                if (Math.Round(point1.Z, 4) == Math.Round(point2.Z, 4))
                                {
                                    XYZ point3 = new XYZ(locationPoints[0].X + 20, locationPoints[0].Y + 20, locationPoints[0].Z);

                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                    uidoc.ActiveView.SketchPlane = sktPlane;

                                    XYZ point1a = new XYZ(point1.X, point1.Y, point1.Z);
                                    XYZ point2a = new XYZ(point2.X, point2.Y, point2.Z);

                                    line = Line.CreateBound(point1, point2);
                                    modelCurve = doc.Create.NewModelCurve(line, sktPlane);

                                    referenceArray.Append(modelCurve.GeometryCurve.GetEndPointReference(1));
                                    referenceArray.Append(modelCurve.GeometryCurve.GetEndPointReference(0));
                                }
                                else
                                {
                                    XYZ point3 = new XYZ(locationPoints[0].X, locationPoints[0].Y + 100, locationPoints[1].Z - 50);

                                    Plane myPlane = Plane.CreateByThreePoints(point1, point2, point3);
                                    SketchPlane sktPlane = SketchPlane.Create(doc, myPlane);

                                    uidoc.ActiveView.SketchPlane = sktPlane;

                                    XYZ point1a = new XYZ(point1.X, point1.Y, point1.Z);
                                    XYZ point2a = new XYZ(point2.X, point2.Y, point2.Z);

                                    line = Line.CreateBound(point1, point2);
                                    modelCurve = doc.Create.NewModelCurve(line, sktPlane);

                                    referenceArray.Append(modelCurve.GeometryCurve.GetEndPointReference(1));
                                    referenceArray.Append(modelCurve.GeometryCurve.GetEndPointReference(0));
                                }

                                Dimension newDimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
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