using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{   
    public class Collectors
    {
        public List<Element> ConduitsByFamilyTypeName(Document doc, string familyType)
        {           
            var elementList = new List<Element>();
            
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element ele in collector)
            {
                if (ele.Name.Contains(familyType))
                {
                    {
                        elementList.Add(ele);
                    }
                }

            }
            return elementList;
        }

        public List<Element> CableTraysByFamilyTypeName(Document doc, string familyType)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element ele in collector)
            {
                if (ele.Name.Contains(familyType))
                {
                    {
                        elementList.Add(ele);
                    }
                }

            }
            return elementList;
        }

        public List<Element> RetangularDuctsByFamilyTypeName(Document doc, string familyType)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element duct in collector)
            {
                if (duct.Name.Contains(familyType))
                {
                    {
                        elementList.Add(duct);
                    }
                }

            }
            return elementList;
        }

        public List<Element> FlexRoundDuctsByFamilyTypeName(Document doc, string familyType)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element duct in collector)
            {
                if (duct.Name.Contains(familyType))
                {
                    {
                        elementList.Add(duct);
                    }
                }

            }
            return elementList;
        }

        public Element HangerByFamilyName(Document doc, string familyName)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_CableTrayFitting);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (FamilyInstance hanger in collector)
            {
                FamilySymbol eleSymbol = hanger.Symbol; 

                if (eleSymbol.FamilyName.Contains(familyName))
                {
                    {
                        elementList.Add(eleSymbol);
                    }
                }

            }
            return elementList.FirstOrDefault();
        }

    }
   
}