using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class ConnectConduit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;           
            ElementId idType;

            //Pick current level
            var level = uidoc.ActiveView.GenLevel;
            if (level == null)
            {
                message = "Wrong Active View";
                return Result.Failed;
            }

            //Retrieve the Conduit Type and get the first type of collection:
            ConduitType conduitType = new FilteredElementCollector(doc)
              .OfClass(typeof(ConduitType))
              .OfType<ConduitType>()
              .FirstOrDefault();

            if (null == conduitType)
            {
                message = "Não foi encontrado nenhum tipo de eletroduto";
                return Result.Failed;
            }

            idType = conduitType.Id;

            //Get connectors location
            try
            {
                //Pick Object
                IList<Reference> pickedObj = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                //Display if null
                if (pickedObj == null)
                {
                    TaskDialog.Show("Erro", "Por favor, selecione os elementos corretamente e tente outra vez.");
                }

                var myPoints = new List<XYZ>();

                //Get element connectors
                ConnectorSet connectors = null;
                
                foreach (Reference ele in pickedObj)
                {
                    ElementId eleId = ele.ElementId;
                    Element element = doc.GetElement(eleId);

                    if (element is FamilyInstance)
                    {
                        MEPModel m = ((FamilyInstance)element).MEPModel;
                        if (null != m
                        && null != m.ConnectorManager)
                        {
                            connectors = m.ConnectorManager.Connectors;
                        }
                    }

                    //Get location of connectors                    
                    foreach (Connector connector in connectors)
                    {
                        if (connector.Domain == Domain.DomainCableTrayConduit)
                        {
                            myPoints.Add(connector.Origin);
                        }                        
                    }
                }                
                
                XYZ startPoint = myPoints.First();
                XYZ endPoint = myPoints.Last();

                //Creating conduits between connectors
                using (var t = new Transaction(doc))
                {
                    t.Start("Connecting Conduits");

                    var pipe = Conduit.Create(doc,
                        idType,
                        startPoint,
                        endPoint,
                        level.Id);               

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