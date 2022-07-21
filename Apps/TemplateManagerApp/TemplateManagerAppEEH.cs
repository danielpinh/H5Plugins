using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H5Plugins
{
    public class CustomSaveFilePathEEH : IExternalEventHandler
    {

        public void Execute(UIApplication app)
        {
            try
            {
                // Abrindo a caixa de diálogo para escolha da pasta
                var folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Selecione a pasta para salvar o arquivo.";
                folderBrowserDialog.ShowNewFolderButton = true;

                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = folderBrowserDialog.SelectedPath;
                    TemplateManagerAppMVVM.UIDataViewModel.FilePath = folderName;
                }
            }
            catch { }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }


    public class TemplateManagerAppEEH : IExternalEventHandler
    {        
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            SaveAsOptions saveAsOptions = new SaveAsOptions();
            saveAsOptions.MaximumBackups = 3;
            saveAsOptions.OverwriteExistingFile = true;

           //TemplateManagerAppMVVM.UIDataViewModel.FilePath;

            string templateFilePath = @"C:\Users\danie\Desktop\Template.rte";

            string newProjectName = TemplateManagerAppMVVM.UIDataViewModel.FilePath + @"\" + TemplateManagerAppMVVM.UIDataViewModel.FileName + ".rvt";
            string newTemplateProjectName = TemplateManagerAppMVVM.UIDataViewModel.FilePath + @"\" + TemplateManagerAppMVVM.UIDataViewModel.FileName + ".rte";

            if ((bool)TemplateManagerAppMVVM.MainView.Projeto_CheckBox.IsChecked)
            {
                using (var trans = new Transaction(doc))
                {
                    trans.Start("Internal Transaction");
                    try
                    {
                        Document openedDocument = app.Application.NewProjectDocument(templateFilePath);
                        openedDocument.SaveAs(newProjectName, saveAsOptions);
                    }
                    catch { }
                    trans.Commit();
                }

                try
                {
                    UIDocument uiOpenedDoc = app.OpenAndActivateDocument(newProjectName);
                }
                catch { }
            }
            else if((bool)TemplateManagerAppMVVM.MainView.Template_CheckBox.IsChecked)
            {
                using (var trans = new Transaction(doc))
                {
                    trans.Start("Internal Transaction");
                    try
                    {
                        Document openedDocument = app.Application.NewProjectTemplateDocument(templateFilePath);
                        openedDocument.SaveAs(newTemplateProjectName, saveAsOptions);
                    }
                    catch { }
                }

                try
                {
                    UIDocument uiOpenedDoc = app.OpenAndActivateDocument(newTemplateProjectName);
                }
                catch { }
            }
        }
        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
