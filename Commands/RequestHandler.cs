using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace H5Plugins
{
    /// <summary>
    /// This is the class that will implement the IExternalEventHandler interface
    /// to handle all commands started by user action in the MainWindow (modeless dialog) 
    /// as Requests listed in an enumeration used by the Request class.
    /// Also here we will define all the methods that will build the application functionality using the Revit API.
    /// </summary>
    public class RequestHandler : IExternalEventHandler
    {        
        public const string Path = @"V:\Projetos\2108-BIM\Desenvolvimento-BIM\00-TEMPLATES\00-REVIT\05-MECÂNICA\DETALHES TÍPICOS.rvt";

        public Request Request { get; } = new Request(); // instantiating the Request class, which will take the commands by the user by indentifying its RequestId 

        public void Execute(UIApplication app)

        {

            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                /* Based on the command started by the user in the MainWindow, 
                 these Switch cases will use the Take method from the Request class to execute the chosen command by its RequestId. */

                switch (Request.Take())
                {

                    case RequestId.None:
                        {
                            return;  // no request to handle
                        }
                    case RequestId.Leitos:
                        {                            
                            LookupTableMap lk = new LookupTableMap();
                            lk.Leitos(doc);
                            break;
                        }
                    case RequestId.Eletrocalhas:
                        {
                            LookupTableMap lk = new LookupTableMap();
                            lk.Eletrocalhas(doc);
                            break;
                        }
                    case RequestId.Eletrodutos:
                        {
                            LookupTableMap lk = new LookupTableMap();
                            lk.Eletrodutos(doc);
                            break;                           
                        }
                    case RequestId.Perfilados:
                        {
                            LookupTableMap lk = new LookupTableMap();
                            lk.Perfilados(doc);
                            break;
                        }
                    case RequestId.Dutos:
                        {
                            try
                            {
                                LookupTableMap lk = new LookupTableMap();
                                lk.DutosRetangulares(doc);
                                lk.DutosRedondos(doc);
                            }
                            finally
                            {

                            }                         
                            break;                          
                        }
                    case RequestId.DetalhesTipicos:
                        {                          
                            Document opendoc = app.Application.OpenDocumentFile(Path);
                            DetalhesTipicos.DetTip(app, doc, opendoc, uidoc);
                            break;
                        }
                    default:
                        {
                            TaskDialog.Show("Warning", "No valid request has been taken");
                            break;
                        }
                }
            }
            finally
            { 
            
            }
        }

        public string GetName()
        {
            /* This method is needed to identify this event handler. */
            return this.GetType().Name;
        }    
        
    }
     
}
