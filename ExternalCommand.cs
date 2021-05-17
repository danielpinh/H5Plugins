using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using H5Plugins;

namespace H5Plugins
{
    /// <summary>
    /// This class implements the IExternalCommand interface and contains the command to execute the application, 
    /// which will be called by the button created in the application class.
    /// </summary>
    
    [Transaction(TransactionMode.Manual)] // setting transactions to manual in order to associate them with our add-in commands, if needed.
[Regeneration(RegenerationOption.Manual)] //enumeration of the Revit API regeneration options. Not really an option currently. The automatic option was supressed.
public class ExternalCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        /* Through this method the user will launch the Add-In and open its MainWindow 
         by clicking on the Ribbon panel button created in the ExternalApplication class. */
        try
        {
                /* Calling the application to show the MainWindow with its controls (each containing a different command). */
                AplicacaoExterna.thisApp.ShowWindow(commandData.Application);

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            /* The message users should see in case of any error when trying to start the Add-In. */
            message = ex.Message;
            TaskDialog.Show("Error!", message);
            return Result.Failed;
        }
    }
}
}
