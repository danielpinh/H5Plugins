using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;


namespace H5Plugins
{
    /// <summary>
    /// This is the code behind of the MainWindow control, 
    /// which presents the application main user interface and we want to work as a modeless dialog.
    /// Here we set methods that should be called by other classes to control the window activity.
    /// Here we also set the interactions between the user input through the window controls 
    /// and the classes that build the commands that Revit should receive as requests.
    /// </summary>
    public partial class MainWindow : Window, IDisposable
{
    private RequestHandler m_Handler;
    private ExternalEvent m_ExEvent;
    /* These two variable declared above are necessary since the MainWindow needs to raise an ExternalEvent
     for each command handled by the RequestHandler through the MakeRequest auxiliary method. */

    public MainWindow(ExternalEvent exEvent, RequestHandler handler)
    {
        /* The initialization of the MainWindow will be called by the ShowWindow method in the ExternalApplication class, executed by the ExternalCommand class,
         and will take the RequestHandler and the ExternalEvent already instantiated by the application as arguments. */

        InitializeComponent();

            this.Topmost = true;
            this.ShowInTaskbar = true;            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
                  

        m_Handler = handler;
        /* Here we assign the instance from the ExternalApllication class to the RequestHandler variable.
         This class will handle each command started through the MainWindow controls as Requests (see auxiliary method: MakeRequest). */

        m_ExEvent = exEvent;
        /* Here we assign the instance from the ExternalApllication class to the ExternalEvent variable.
         This class will raise the command requested as an external event for Revit (see auxiliary method: MakeRequest). */
    }


    //----MAKING REQUESTS--------------------------------------------------

    /* These are the methods for button click events that will feed the MakeRequest auxiliary method with Requests 
     (though the RequestId enum in the Request class) for specific commands as arguments. 
     The methods for each of these commands are coded in the RequestHandler class and are executed via Delegate */

    private void comando01(object sender, RoutedEventArgs e)
    {
        MakeRequest(RequestId.Command01);
    }           

    //----AUXILIARY METHODS--------------------------------------------------
    private void MakeRequest(RequestId request)
    {
        /* As seen above this method is used to handle and raise the commands requested by the user as external events for Revit. */

        m_Handler.Request.Make(request); // uses the Make method of the Request class instantiated in the RequestHandler class to identify the command started by the user
        m_ExEvent.Raise(); // raises the command requested as an external event for Revit and the Execute method in the handler can finally be done
    }

    public void WakeUp()
    {
        /* Use this method to enable all controls in the window if and when needed. */
        this.IsEnabled = true;
    }

    public void DozeOff()
    {
        /* Use this method to disable all controls in the window if and when needed. */
        this.IsEnabled = false;
    }


    //----DISPOSING--------------------------------------------------
    public void Dispose()
    {
        /* This method must be called in the OnShutdown method of the External Application class 
         to correctly close the application and the instances of the RequestHandler and the ExternalEvent */
        m_ExEvent.Dispose();
        m_ExEvent = null;
        m_Handler = null;
        this.Close();
    }

    }
}
