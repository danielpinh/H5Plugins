using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class VedaJunta : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;             
       

            //Get first the cable tray to insert the hanger and after pick the hoster family
            try
            {
                //Pick Object

                Selection sel = uiapp.ActiveUIDocument.Selection;

                Reference pickedObj = sel.PickObject(ObjectType.Element);

                //Get options and set their value to ActiveView to get_Geometry method
                Options opts = new Options();
                opts.View = doc.ActiveView;

                //Pick Object in Active View
                ElementId elementId = pickedObj.ElementId;
                Element element = doc.GetElement(elementId);

                //Get profile area
                FamilySymbol fS = element as FamilySymbol;
                FamilyInstance fI = element as FamilyInstance;
                GeometryElement gE = fI.get_Geometry(opts);

                double volumeFeet = 0;
                double volume = 0;
                double profileAreafeet = 0;
                double profileArea = 0;
                double areaFeetdoMeter = 10.764;
                double volumeFeetdoMeter = 35.31466;

                List<PlanarFace> planarFaces = new List<PlanarFace>();



                foreach (var item in gE)
                {
                    GeometryInstance gI = item as GeometryInstance;
                    GeometryElement geoElemTmp = gI.GetSymbolGeometry();

                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Solid solidObj2 = geomObjTmp as Solid;                    

                        FaceArray faceArray = solidObj2.Faces;
                        foreach (Face face in faceArray)
                        {
                            if (face is PlanarFace)
                            {
                                planarFaces.Add(face as PlanarFace);
                                break;
                            }

                        }

                        //Convert area, feet do meter
                        profileAreafeet = planarFaces.First().Area;
                        profileArea = profileAreafeet / areaFeetdoMeter;

                        TaskDialog.Show("teste", profileArea.ToString());

                        //Convert volume, feet do meter
                        volumeFeet = solidObj2.Volume;
                        volume = volumeFeet / volumeFeetdoMeter;

                        break;
                    }
                }          
                               
                double linearLength = Math.Round((volume / profileArea),2);                
                using (var t = new Transaction(doc))
                {
                    t.Start("VedaJunta");
                    Parameter param = element.LookupParameter("H5 Comprimento");
                    param.SetValueString(linearLength.ToString());
                    t.Commit();
                }               
            }

            finally
            {
               
            }
            return Result.Succeeded;
        }
    }
   
}