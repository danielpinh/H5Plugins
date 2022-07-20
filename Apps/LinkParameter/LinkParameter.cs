using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class LinkParameter 
    {
        public static List<string> InstanceParameterNames { get; set; } = new List<string>();
        public static List<string> TypeParameterNames { get; set; } = new List<string>();

        public void FamilyBySelectionLinkParameter(Document doc, UIDocument uidoc)
        {
            //Variables
            var collec = new Collectors();

            using (var trans = new Transaction(doc))
            {
                trans.Start("Link Parameter");

                //Selecting elements in document
                IList<Reference> refs = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                //Parameters selected in UI
                foreach (ParameterViewModel pvm in UISelectedParameters())
                {
                    foreach (Reference refe in refs)
                    {
                        try
                        {

                            Element ele = doc.GetElement(refe.ElementId);
                            FamilyInstance familyInstance = ele as FamilyInstance;

                            string parameterName = pvm.ParameterName;

                            //coletando o Parâmetro "H5 Sistema" do Element
                            Parameter systemParameter = familyInstance.LookupParameter(parameterName);

                            string systemParameterValueString = null;
                            double systemParameterValueDouble = 0;
                            int systemParameterValueInt = 0;

                            //coletando o valor de cada um dos parâmetros 
                            if (systemParameter.StorageType == StorageType.Double)
                            {
                                systemParameterValueDouble = systemParameter.AsDouble();
                            }
                            else if (systemParameter.StorageType == StorageType.Integer)
                            {
                                systemParameterValueInt = systemParameter.AsInteger();
                            }
                            else if (systemParameter.StorageType == StorageType.String)
                            {
                                systemParameterValueString = systemParameter.AsString();
                            }

                            //coletando as famílias aninhadas do FamilyInstance
                            ICollection<ElementId> subComponents1 = familyInstance.GetSubComponentIds();

                            SetParameterAndGetSubFamilyInstanceIds(doc, subComponents1, parameterName, systemParameterValueString, systemParameterValueDouble, systemParameterValueInt);
                        }
                        catch (Exception ex)                       
                        {
                            TaskDialog.Show("debug", ex.Message);
                        }
                    }
                }
                trans.Commit();
            }
        }

        public void FamilyInstanceLinkParameter (Document doc)
        {
            //Variables
            var collec = new Collectors();

            //Categories selected in UI
            IEnumerable<CategoryViewModel> selectedCategories = UISelectedCategories();            

            foreach (CategoryViewModel cvm in selectedCategories)
            {
                var cat = cvm.Category;
                var familyInstances = collec.AllFamilyInstancesOfCategory(doc, cat);

                using (var trans = new Transaction(doc))
                {
                    trans.Start("Link Parameter");

                    //Parameters selected in UI
                    foreach (ParameterViewModel pvm in UISelectedParameters())
                    {
                        foreach (FamilyInstance familyInstance in familyInstances)
                        {
                            try
                            {
                                string parameterName = pvm.ParameterName;

                                //coletando o Parâmetro "H5 Sistema" do Element
                                Parameter systemParameter = familyInstance.LookupParameter(parameterName);

                                string systemParameterValueString = null;
                                double systemParameterValueDouble = 0;
                                int systemParameterValueInt = 0;

                                //coletando o valor de cada um dos parâmetros 
                                if (systemParameter.StorageType == StorageType.Double)
                                {
                                    systemParameterValueDouble = systemParameter.AsDouble();
                                }
                                else if (systemParameter.StorageType == StorageType.Integer)
                                {
                                    systemParameterValueInt = systemParameter.AsInteger();
                                }
                                else if (systemParameter.StorageType == StorageType.String)
                                {
                                    systemParameterValueString = systemParameter.AsString();
                                }

                                //coletando as famílias aninhadas do FamilyInstance
                                ICollection<ElementId> subComponents1 = familyInstance.GetSubComponentIds();

                                SetParameterAndGetSubFamilyInstanceIds(doc, subComponents1, parameterName, systemParameterValueString, systemParameterValueDouble, systemParameterValueInt);
                            }
                            catch { }
                        }
                    }
                    trans.Commit();
                }
            }
        }

        public void SetParameterAndGetSubFamilyInstanceIds(Document doc,
            ICollection<ElementId> subComponents,
            string parameterName,
            string parameterValueString,
            double parameterValueDouble,
            int parameterValueInt)
        {
            if (subComponents.Count > 0)
            {
                foreach (ElementId eleId in subComponents)
                {
                    try
                    {
                        //coletando o Element do ElementId
                        Element element = doc.GetElement(eleId);

                        //coletando o FamilyInstance do Element
                        FamilyInstance familyInstance = element as FamilyInstance;

                        try
                        {
                            //coletando o Parâmetro do Element
                            Parameter parameter = familyInstance.LookupParameter(parameterName);

                            //Alimentando o Parâmetro no Element
                            if (parameter.StorageType == StorageType.Double)
                            {
                                parameter.Set(parameterValueDouble);
                            }
                            else if (parameter.StorageType == StorageType.Integer)
                            {
                                parameter.Set(parameterValueInt);
                            }
                            else if (parameter.StorageType == StorageType.String)
                            {
                                parameter.Set(parameterValueString);
                            }

                        }
                        catch { }

                        //coletando as famílias aninhadas do FamilyInstance
                        ICollection<ElementId> newSubComponents = familyInstance.GetSubComponentIds();

                        SetParameterAndGetSubFamilyInstanceIds(doc,
                            newSubComponents,
                            parameterName,
                            parameterValueString,
                            parameterValueDouble,
                            parameterValueInt);
                    }
                    catch { }
                }
            }
        }
        public void FamilySymbolLinkParameter(Document doc)
        {
            var collec = new Collectors();

            //Categories selected in UI
            IEnumerable<CategoryViewModel> selectedCategories = UISelectedCategories();

            foreach (CategoryViewModel cvm in selectedCategories)
            {
                var cat = cvm.Category;
                var familyInstances = collec.AllFamilyInstancesOfCategory(doc, cat);

                using (var trans = new Transaction(doc))
                {
                    trans.Start("Link Parameter");

                    //Parameters selected in UI
                    foreach (ParameterViewModel pvm in UISelectedParameters())
                    {
                        foreach (FamilyInstance familyInstance in familyInstances)
                        {
                            try
                            {
                                //coletando o FamilySymbol do FamilyInstance
                                FamilySymbol fSymbol = familyInstance.Symbol;

                                string parameterName = pvm.ParameterName;

                                //coletando o Parâmetro "H5 Sistema" do Element
                                Parameter systemParameter = fSymbol.LookupParameter(parameterName);

                                string systemParameterValueString = null;
                                double systemParameterValueDouble = 0;
                                int systemParameterValueInt = 0;

                                //coletando o valor de cada um dos parâmetros 
                                if (systemParameter.StorageType == StorageType.Double)
                                {
                                    systemParameterValueDouble = systemParameter.AsDouble();
                                }
                                else if (systemParameter.StorageType == StorageType.Integer)
                                {
                                    systemParameterValueInt = systemParameter.AsInteger();
                                }
                                else if (systemParameter.StorageType == StorageType.String)
                                {
                                    systemParameterValueString = systemParameter.AsString();
                                }

                                //coletando as famílias aninhadas do FamilyInstance
                                ICollection<ElementId> subComponents1 = familyInstance.GetSubComponentIds();

                                SetParameterAndGetSubFamilySymbolIds(doc, subComponents1, parameterName, systemParameterValueString, systemParameterValueDouble, systemParameterValueInt);
                            }
                            catch { }
                        }
                    }
                    trans.Commit();
                }
            }
        }

        public void SetParameterAndGetSubFamilySymbolIds(Document doc, ICollection<ElementId> subComponents, string parameterName, string parameterValueString, double parameterValueDouble, int parameterValueInt)
        {
            if (subComponents.Count > 0)
            {
                foreach (ElementId eleId in subComponents)
                {
                    try
                    {
                        //coletando o Element do ElementId
                        Element element = doc.GetElement(eleId);

                        //coletando o FamilyInstance do Element
                        FamilyInstance familyInstance = element as FamilyInstance;

                        //coletando o FamilySymbol do Element
                        FamilySymbol fSymbol = familyInstance.Symbol;

                        //coletando o Parâmetro do Element
                        Parameter parameter = fSymbol.LookupParameter(parameterName);

                        //Alimentando o Parâmetro no Element
                        if (parameter.StorageType == StorageType.Double)
                        {
                            parameter.Set(parameterValueDouble);
                        }
                        else if (parameter.StorageType == StorageType.Integer)
                        {
                            parameter.Set(parameterValueInt);
                        }
                        else if (parameter.StorageType == StorageType.String)
                        {
                            parameter.Set(parameterValueString);
                        }

                        //coletando as famílias aninhadas do FamilyInstance
                        ICollection<ElementId> newSubComponents = familyInstance.GetSubComponentIds();

                        SetParameterAndGetSubFamilyInstanceIds(doc, newSubComponents, parameterName, parameterValueString, parameterValueDouble, parameterValueInt);
                    }
                    catch { }
                }
            }
        }
        private static IEnumerable<CategoryViewModel> UISelectedCategories()
        {
            LinkParameterMVVM linkParameterMainView = H5AppExternalApplication.h5App.linkParameterMVVM;
            IEnumerable<CategoryViewModel> selectedCategories = linkParameterMainView.CategoryMainViewModel.Where(ps => ps.IsCheckedCategoryName == true);
            return selectedCategories;
        }
        private static IEnumerable<ParameterViewModel> UISelectedParameters()
        {
            LinkParameterMVVM linkParametermainView = H5AppExternalApplication.h5App.linkParameterMVVM;
            IEnumerable<ParameterViewModel> selectedParameters = linkParametermainView.ParametersMainViewModel.Where(ps => ps.IsCheckedParameterName == true);
            return selectedParameters;
        }
    }

}    



