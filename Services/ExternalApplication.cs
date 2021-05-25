using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace H5Plugins
{
    class ExternalApplication : IExternalApplication
    {

        internal static ExternalApplication thisApp = null; // instantiating the class, initially null, since the app initialization user interface is not set yet
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

            /////////////////////////////////////////////////BUTTON #1            
            //Criando um Menu Ribbon  
            application.CreateRibbonTab("H5Apps");
            string path = Assembly.GetExecutingAssembly().Location;

            //Creating first button on Menu Ribbon
            RibbonPanel panelMec = application.CreateRibbonPanel("H5Apps", "Mecânica");
            string toolTip1 = "Cria Folhas com os Detalhes Típicos dos Equipamentos Inseridos no Projeto.";            
            PushButtonData button1 = new PushButtonData("Button1", String.Format("Detalhes" + Environment.NewLine + "Típicos"), path, "H5Plugins.ExternalCommand");
            string help = @"https://www.head5.com.br/";
            ContextualHelp contexHelp = new ContextualHelp(ContextualHelpType.Url, help);
            button1.SetContextualHelp(contexHelp);       
            button1.ToolTip = toolTip1;              

            // Adding icon to first button
            Uri imagePath1 = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\Detalhes Típicos.png");
            BitmapImage image1 = new BitmapImage(imagePath1);
            PushButton pushButton1 = panelMec.AddItem(button1) as PushButton;
            pushButton1.LargeImage = image1;

            /////////////////////////////////////////////////BUTTON #2   
            //Creating second button on Menu Ribbon
            RibbonPanel panelGer = application.CreateRibbonPanel("H5Apps", "Geral");
            string toolTip2 = "Ordena os Cortes do Projeto Conforme a Numeração das Folhas.";
            PushButtonData button2 = new PushButtonData("Button2", String.Format("Ordenar" + Environment.NewLine + "Cortes"), path, "H5Plugins.OrdenarCortes");            
            button2.ToolTip = toolTip2;
            button2.SetContextualHelp(contexHelp);

            // Adding icon to second button
            Uri imagePath2 = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\Cortes.png");
            BitmapImage image2 = new BitmapImage(imagePath2);
            PushButton pushButton2 = panelGer.AddItem(button2) as PushButton;
            pushButton2.LargeImage = image2;


            /////////////////////////////////////////////////BUTTON #3   
            //Creating third button on Menu Ribbon           
            string toolTip3 = "Insere Parâmetros Compartilhados Nas Famílias.";
            PushButtonData button3 = new PushButtonData("Button3", String.Format("Parâmetros" + Environment.NewLine + "Compartilhados"), path, "H5Plugins.ParametrosCompartilhados");
            button3.ToolTip = toolTip3;
            button3.SetContextualHelp(contexHelp);
            // Adding icon to third button
            Uri imagePath3 = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\Parametros.png");
            BitmapImage image3 = new BitmapImage(imagePath3);
            PushButton pushButton3 = panelMec.AddItem(button3) as PushButton;
            pushButton3.LargeImage = image3;



            /////////////////////////////////////////////////ABOUT BUTTON
            //Creating about button on Menu Ribbon
            RibbonPanel panelAbout = application.CreateRibbonPanel("H5Apps", "Sobre");
            string toolTipAbout = "Mais que Projetos. Engenharia Aplicada à Inovação.";
            string helpAbout = @"https://www.head5.com.br/";
            PushButtonData buttonAbout = new PushButtonData("Button1", String.Format("Head5"), path, "H5Plugins.About");          
            ContextualHelp contexHelpAbout = new ContextualHelp(ContextualHelpType.Url, helpAbout);
            buttonAbout.SetContextualHelp(contexHelpAbout);
            buttonAbout.ToolTip = toolTipAbout;
            //ToolTip Image path
            Uri toolTipimagePathAbout = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\AboutImage.png");
            BitmapImage tootlTipimageAbout = new BitmapImage(toolTipimagePathAbout);            
            buttonAbout.ToolTipImage = tootlTipimageAbout;
            // Adding icon to about button
            Uri imagePathAbout = new Uri(@"D:\Users\daniel.santos\OneDrive\1-HEAD5\3-DESENVOLVIMENTO BIM\7-PLUGINS\Imagens\h5.png");
            BitmapImage imageAbout = new BitmapImage(imagePathAbout);
            PushButton pushButtonAbout = panelAbout.AddItem(buttonAbout) as PushButton;
            pushButtonAbout.LargeImage = imageAbout;


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
