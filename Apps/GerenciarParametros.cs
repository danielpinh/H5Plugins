using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autodesk.Revit.ApplicationServices;


namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class GerenciarParametros : IExternalCommand
    {        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            if (!doc.IsFamilyDocument)
            {
                TaskDialog.Show("Tente novamente", "Infelizmente, há algo de errado. \n\nTente esse comando dentro de um arquivo de família (.rfa). \n");
                return Result.Failed;
            }
            else
            {     
                Application app = uiapp.Application;

                string sharedParameterFilePath = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\00-TEMPLATES\00-REVIT\02-ELÉTRICA\02-PARÂMETROS COMPARTILHADOS\PARÂMETROS-COMPARTILHADOS-ELÉTRICA.txt";

                app.SharedParametersFilename = sharedParameterFilePath;
                FamilyManager familyManager = doc.FamilyManager;

                // get all shared parameters from shared parameters file
                DefinitionFile definitionFile = app.OpenSharedParameterFile();
                DefinitionGroups definitionGroups = definitionFile.Groups;

                using (Transaction trans = new Transaction(doc, "Adicionar Parâmetros"))
                {
                    trans.Start();
                    {
                        foreach (DefinitionGroup definitionGroup in definitionGroups)
                        {

                            foreach (Definition definitiOn in definitionGroup.Definitions)
                            {
                                if (definitiOn != null && definitiOn.Name == "H5 Classificação 01")
                                {
                                    ExternalDefinition externalDef = definitiOn as ExternalDefinition;
                                    familyManager.AddParameter(externalDef, BuiltInParameterGroup.PG_IDENTITY_DATA, true);
                                }
                            }
                        }
                    }
                    trans.Commit();
                }
                return Result.Succeeded;
            }                
        }
    }    
}
