using Autodesk.Revit.UI;
using System;
using System.Drawing;
using System.IO;
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

        public Result OnStartup(UIControlledApplication app)
        {
            //Variables
            string addinName = "H5Apps";          

            //New Menu Ribbon  
            app.CreateRibbonTab("H5Apps");
            string path = Assembly.GetExecutingAssembly().Location;

            //Help adress for buttons
            string helpAdress = @"https://www.head5.com.br/";            

            //Ribbon Menu Groups
            RibbonPanel panelMec = NewMenuRibbonGroup(app, addinName, "Mecânica");
            RibbonPanel panelGer = NewMenuRibbonGroup(app, addinName, "Geral");
            RibbonPanel panelEle = NewMenuRibbonGroup(app, addinName, "Elétrica");
            RibbonPanel panelAbout = NewMenuRibbonGroup(app, addinName, "Sobre");

            //Adding buttons
            //Detalhes Típicos            
            PushButton detalhesTipicos = NewMenuRibbonButton("Button1", String.Format("Detalhes" + Environment.NewLine + "Típicos"),
                path, 
                "H5Plugins.ExternalCommand", 
                "Cria Folhas com os Detalhes Típicos dos Equipamentos Inseridos no Projeto.", 
                panelMec, 
                "H5Plugins.Resources.DetalhesTipicos.png", 
                helpAdress);

            //Ordenar Cortes    
            PushButton ordenarCortes = NewMenuRibbonButton("Button2", String.Format("Ordenar" + Environment.NewLine + "Cortes"),
                path, 
                "H5Plugins.OrdenarCortes", 
                "Ordena os Cortes do Projeto Conforme a Numeração das Folhas.", 
                panelGer, 
                "H5Plugins.Resources.OrdenarCortes.png", 
                helpAdress);

            //Gerenciar Parâmetros    
            PushButton gerenciarParametros = NewMenuRibbonButton("Button3", String.Format("Gerenciar" + Environment.NewLine + "Parâmetros"),
                path,
                "H5Plugins.ParametrosCompartilhados",
                "Insere Parâmetros em Lotes em Projetos e Famílias.",
                panelGer,
                "H5Plugins.Resources.GerenciarParametros.png", 
                helpAdress);

            //Atribuir Códigos    
            PushButton atribuirCodigos = NewMenuRibbonButton("Button4", String.Format("Atribuir" + Environment.NewLine + "Códigos"),
                path,
                "H5Plugins.AtribuirCodigos",
                "Atribui os Códigos Internos e de Fabricantes às Famílias de Sistema",
                panelEle,
                "H5Plugins.Resources.AtribuirCodigos.png",
                helpAdress);

            //Sobre a Head5   
            PushButton aboutButton = NewMenuRibbonButton("Button5", String.Format("Head5"),
                path,
                "H5Plugins.About",
                "Mais que Projetos. Engenharia Aplicada à Inovação.",
                panelAbout,
                "H5Plugins.Resources.Head5.png",
                helpAdress);

            //Adding ToolTipImage to a about button
            aboutButton.ToolTipImage = PngImageSource("H5Plugins.Resources.Sobre.png");
            

            mWnd = null; // the MainWindow (modeless dialog) will not be initialized here, but only by the ShowWindow mehod below (called by the ExternalCommand class, started by the app button)
            thisApp = this; // replacing the null value of the application with the application itself since its ribbon elements are now set

            return Result.Succeeded;
        }

        public RibbonPanel NewMenuRibbonGroup(UIControlledApplication application, string addinName, string groupName)
        {            
            
            RibbonPanel panelMec = application.CreateRibbonPanel(addinName, groupName);
            return panelMec;
        }

        public PushButton NewMenuRibbonButton(string genericName, string buttonName, string buttonPath, string buttonCommand, string toolTip, RibbonPanel panelName, string imageIconSource, string helpAdress)
        {
            //Help button adress            
            ContextualHelp contexHelp = new ContextualHelp(ContextualHelpType.Url, helpAdress);

            //Create a button
            PushButtonData newPushButtonData = new PushButtonData(genericName, buttonName, buttonPath, buttonCommand);
            newPushButtonData.SetContextualHelp(contexHelp);
            newPushButtonData.ToolTip = toolTip;

            //Adding icon to button                        
            PushButton newpushButton = panelName.AddItem(newPushButtonData) as PushButton;
            newpushButton.LargeImage = PngImageSource(imageIconSource);         
            
            return newpushButton;
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
        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            return decoder.Frames[0];
        }
    }

}  
