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
        public List<Element> PipeByFamilyTypeName(Document doc, string typeName)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element pipe in collector)
            {
                if (pipe.Name.Contains(typeName))
                {
                    elementList.Add(pipe);
                }
            }
            return elementList;
        }

        public List<Element> AllMechanicalEquipments(Document doc)
        {
            var elementList = new List<Element>();
            var elementList2 = new List<Element>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element meq in collector)
            {
                elementList.Add(meq);
            }
            return elementList;

        }

        public List<Element> AllPipeAcessories(Document doc)
        {
            var elementList = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter collectorFilter = new ElementCategoryFilter(BuiltInCategory.OST_PipeAccessory);
            collector.WherePasses(collectorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element pipeAcess in collector)
            {
                elementList.Add(pipeAcess);
            }
            return elementList;
        }

        public ElementId PipeTagsbyFamilyName(Document doc, string familyName)
        {
            FamilySymbol tagfamilySymbol = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_PipeTags)
                .Cast<FamilySymbol>()
                .First(x => x.FamilyName == familyName);
            return tagfamilySymbol.Id;         
        }

        public ElementId PipeFittingsTagsbyFamilyName(Document doc, string familyName)
        {
            FamilySymbol tagfamilySymbol = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_PipeFittingTags)
                .Cast<FamilySymbol>()
                .First(x => x.FamilyName == familyName);
            return tagfamilySymbol.Id;
        }

        public ElementId PipeAcessoryTagsbyFamilyName(Document doc, string familyName)
        {
            FamilySymbol tagfamilySymbol = new FilteredElementCollector(doc)
           .WhereElementIsElementType()
           .OfCategory(BuiltInCategory.OST_PipeAccessoryTags)
           .Cast<FamilySymbol>()
           .First(x => x.FamilyName == familyName);
            return tagfamilySymbol.Id;
        }

    }
}