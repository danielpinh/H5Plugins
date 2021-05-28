using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class AtribuirCodigos : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {         
            
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;            

            try
            {

                string keyHeader = "L##length##millimeters";
                string header2 = "CODFAB##other##";
                string header3 = "COD##other##";

                FilteredElementCollector collectorCableTrays = new FilteredElementCollector(doc);
                ElementCategoryFilter cableTrays = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
                collectorCableTrays.WherePasses(cableTrays)
                    .WhereElementIsNotElementType()
                    .Cast<Element>();

                //Define code for each cable tray element
                int amountOfElements = 0;
                foreach (Element ct in collectorCableTrays)
                {
                    Element cableTray = doc.GetElement(ct.Id);
                    Parameter widthCableTray = cableTray.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);

                    LookUpTableMapping lktmapping = new LookUpTableMapping();
                    string codfab = lktmapping.LookupByHeader(keyHeader, widthCableTray.AsValueString(), header2);
                    string codint = lktmapping.LookupByHeader(keyHeader, widthCableTray.AsValueString(), header3);

                    using (Transaction trans = new Transaction(doc, "Atribuir Códigos"))
                    {
                        trans.Start();
                        {
                            Parameter param1 = ct.LookupParameter("H5 Código do material");
                            param1.Set(codint.ToString());

                            Parameter param2 = ct.LookupParameter("H5 Código do fabricante");
                            param2.Set(codfab.ToString());
                        }

                        trans.Commit();
                        amountOfElements++;
                    }
                }            
            }
            finally
            {

            }
            return Result.Succeeded;
        }

    }
   
}