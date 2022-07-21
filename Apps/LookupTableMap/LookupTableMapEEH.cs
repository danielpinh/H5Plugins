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
    public class LookupTableMapEEH : IExternalEventHandler
    {              
        public LookupTableMapRequest Request { get; } = new LookupTableMapRequest(); // instantiating the Request class, which will take the commands by the user by indentifying its RequestId 

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
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Leitos(doc);
                            break;
                        }
                    case RequestId.Eletrocalhas:
                        {
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Eletrocalhas(doc);
                            break;
                        }
                    case RequestId.Eletrodutos:
                        {
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Eletrodutos(doc);
                            break;                           
                        }
                    case RequestId.Perfilados:
                        {
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Perfilados(doc);
                            break;
                        }
                    case RequestId.Dutos:
                        {                           
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Dutos(doc);
                            break;
                        }
                    case RequestId.Tubos:
                        {                            
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Tubos(doc);                           
                            break;
                        }
                    case RequestId.Sistemas:
                        {
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Sistemas(doc);
                            break;
                        }
                    case RequestId.Septos:
                        {
                            LookupTableMap lookupTableMap = new LookupTableMap();
                            lookupTableMap.Septos(doc);
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
