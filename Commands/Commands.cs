using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using Revit.SDK.Samples.DuplicateViews.CS;
using System.Windows;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class Commands : IExternalCommand
    {
        

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Application Documents            
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;            
            Document doc = uidoc.Document;
         
            #region ORDENAR CORTES

            OrdenarCortes oc = new OrdenarCortes();
            oc.OrdCor(doc, uidoc);

            #endregion

            #region PARÂMETROS COMPARTILHADOS

            ParametrosCompartilhados pc = new ParametrosCompartilhados();
            pc.ParCom(uidoc, doc);

            #endregion

            #region ABOUT

            About ab = new About();
            ab.Abt();

            #endregion

            return Result.Succeeded;

        }
    }
}


