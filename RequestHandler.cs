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
        /*  */

        public string GetName()
        {
            /* This method is needed to identify this event handler. */
            return "Request Handler";
        }

        public Request Request { get; } = new Request(); // instantiating the Request class, which will take the commands by the user by indentifying its RequestId 

        public void Execute(UIApplication app)
        {
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
                    case RequestId.Command01:
                        {
                            DetalhesTipicos.NewMethod(app);
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
                AplicacaoExterna.thisApp.WakeWindowUp(); // keeping the dialog active after a request
            }
            return;
        }
    }
     
}
