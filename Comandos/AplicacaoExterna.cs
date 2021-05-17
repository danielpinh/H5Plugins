using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace H5Plugins
{
    class AplicacaoExterna : IExternalApplication
    {

        internal static AplicacaoExterna thisApp = null; // instantiating the class, initially null, since the app initialization user interface is not set yet
        private MainWindow mWnd; // instantiating the MainWindow (modeless dialog)
        public Result OnShutdown(UIControlledApplication application)
        {
            {
                /* Very important: always use the Dispose method in the shutdown to close the window and any other control/instance initialized by the application. */

                if (mWnd != null && mWnd.IsVisible)
                {
                    mWnd.Dispose();
                }
                return Result.Succeeded;
            }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Criando um Menu Ribbon  
            application.CreateRibbonTab("H5Apps");
            string path = Assembly.GetExecutingAssembly().Location;


            //Creating first button on Menu Ribbon
            PushButtonData button = new PushButtonData("Button1", "Detalhes Típicos", path, "H5Plugins.ExternalCommand");
            RibbonPanel panel = application.CreateRibbonPanel("H5Apps", "Mecânica");
            // Adding icon to first button
            Uri imagePath = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\Head5.png");

            BitmapImage image = new BitmapImage(imagePath);
            PushButton pushButton = panel.AddItem(button) as PushButton;
            pushButton.LargeImage = image;


            mWnd = null; // the MainWindow (modeless dialog) will not be initialized here, but only by the ShowWindow mehod below (called by the ExternalCommand class, started by the app button)
            thisApp = this; // replacing the null value of the application with the application itself since its ribbon elements are now set

            return Result.Succeeded;

        }

        public void ShowWindow(UIApplication uiApp)
        {
            /* This method will be called by the ExternalCommand (activated by the click on the ribbon button of the app). */

            //----Showing the window
            if (mWnd == null || mWnd.IsLoaded == false)
            {
                //----Creating the handler to manage the requests done by the commands in the MainWindow (modeless dialog)
                RequestHandler handler = new RequestHandler();

                //----Creating the external event to be raised as a request identified by the handler
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                //----Initializing the MainWindow with the above instances as arguments (see the code behind of the Main Window and the MakeRequest method)       
                mWnd = new MainWindow(exEvent, handler);
                mWnd.Show();
            }
        }

        public void WakeWindowUp()
        {
            /* This method is used in the RequestHandler Execute method to keep the modeless dialog active after a request. */

            if (mWnd != null)
            {
                mWnd.WakeUp();
                mWnd.Activate();
            }
        }


    }
}
