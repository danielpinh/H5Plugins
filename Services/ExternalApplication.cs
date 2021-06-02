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
            string addinName = "H5 Apps";

            //New Menu Ribbon  
            app.CreateRibbonTab("H5Apps");
            string path = Assembly.GetExecutingAssembly().Location;
            
            //Ribbon Menu Groups
            RibbonPanel panelMec = NewMenuRibbonGroup(app, addinName, "Mecânica");
            RibbonPanel panelGer = NewMenuRibbonGroup(app, addinName, "Geral");
            RibbonPanel panelEle = NewMenuRibbonGroup(app, addinName, "Elétrica");
            RibbonPanel panelAbout = NewMenuRibbonGroup(app, addinName, "Sobre");

            //Criando grupo no Menu Ribbon
            //RibbonPanel panelMec = application.CreateRibbonPanel(addinName, "Mecânica");
            //RibbonPanel panelGer = application.CreateRibbonPanel(addinName, "Geral");
            //RibbonPanel panelEle = application.CreateRibbonPanel(addinName, "Elétrica");
            //RibbonPanel panelAbout = application.CreateRibbonPanel(addinName, "Sobre");



            /////////////////////////////////////////////////DETALHES TÍPICOS  
            //Creating first button on Menu Ribbon
            string toolTip1 = "Cria Folhas com os Detalhes Típicos dos Equipamentos Inseridos no Projeto.";            
            PushButtonData button1 = new PushButtonData("Button1", String.Format("Detalhes" + Environment.NewLine + "Típicos"), path, "H5Plugins.ExternalCommand");
            string help = @"https://www.head5.com.br/";
            ContextualHelp contexHelp = new ContextualHelp(ContextualHelpType.Url, help);
            button1.SetContextualHelp(contexHelp);       
            button1.ToolTip = toolTip1;        
            // Adding icon to first button                        
            PushButton pushButton1 = panelMec.AddItem(button1) as PushButton;
            pushButton1.LargeImage = PngImageSource("H5Plugins.Resources.DetalhesTipicos.png");           

                
            /////////////////////////////////////////////////ORDENAR CORTES 
            //Creating second button on Menu Ribbon            
            string toolTip2 = "Ordena os Cortes do Projeto Conforme a Numeração das Folhas.";
            PushButtonData button2 = new PushButtonData("Button2", String.Format("Ordenar" + Environment.NewLine + "Cortes"), path, "H5Plugins.OrdenarCortes");            
            button2.ToolTip = toolTip2;
            button2.SetContextualHelp(contexHelp);
            // Adding icon to second button            
            PushButton pushButton2 = panelGer.AddItem(button2) as PushButton;
            pushButton2.LargeImage = PngImageSource("H5Plugins.Resources.OrdenarCortes.png");


            /////////////////////////////////////////////////GERENCIAR PARÂMETROS
            //Creating third button on Menu Ribbon           
            string toolTip3 = "Insere Parâmetros Compartilhados Nas Famílias.";
            PushButtonData button3 = new PushButtonData("Button3", String.Format("Gerenciar" + Environment.NewLine + "Parâmetros"), path, "H5Plugins.ParametrosCompartilhados");
            button3.ToolTip = toolTip3;
            button3.SetContextualHelp(contexHelp);
            // Adding icon to third button            
            PushButton pushButton3 = panelMec.AddItem(button3) as PushButton;
            pushButton3.LargeImage = PngImageSource("H5Plugins.Resources.GerenciarParametros.png");

            /////////////////////////////////////////////////ATRIBUIR CÓDIGOS        
            //Creating fourth button on Menu Ribbon           
            string toolTip4 = "Atribui os Códigos Internos e de Fabricantes às Famílias de Sistema";
            PushButtonData button4 = new PushButtonData("Button3", String.Format("Atribuir" + Environment.NewLine + "Códigos"), path, "H5Plugins.AtribuirCodigos");
            button4.ToolTip = toolTip4;
            button4.SetContextualHelp(contexHelp);
            // Adding icon to third button            
            PushButton pushButton4 = panelEle.AddItem(button4) as PushButton;
            pushButton4.LargeImage = PngImageSource("H5Plugins.Resources.AtribuirCodigos.png");


            /////////////////////////////////////////////////SOBRE A HEAD5
            //Creating about fifth on Menu Ribbon            
            string toolTipAbout = "Mais que Projetos. Engenharia Aplicada à Inovação.";
            string helpAbout = @"https://www.head5.com.br/";
            PushButtonData buttonAbout = new PushButtonData("Button1", String.Format("Head5"), path, "H5Plugins.About");          
            ContextualHelp contexHelpAbout = new ContextualHelp(ContextualHelpType.Url, helpAbout);
            buttonAbout.SetContextualHelp(contexHelpAbout);
            buttonAbout.ToolTip = toolTipAbout;
            //ToolTip Image path            
            buttonAbout.ToolTipImage = buttonAbout.LargeImage = PngImageSource("H5Plugins.Resources.Sobre.png");
            // Adding icon to about button         
            PushButton pushButtonAbout = panelAbout.AddItem(buttonAbout) as PushButton;
            pushButtonAbout.LargeImage = PngImageSource("H5Plugins.Resources.Head5.png"); ;


            mWnd = null; // the MainWindow (modeless dialog) will not be initialized here, but only by the ShowWindow mehod below (called by the ExternalCommand class, started by the app button)
            thisApp = this; // replacing the null value of the application with the application itself since its ribbon elements are now set

            return Result.Succeeded;
        }

        public RibbonPanel NewMenuRibbonGroup(UIControlledApplication application, string addinName, string groupName)
        {            
            
            RibbonPanel panelMec = application.CreateRibbonPanel(addinName, groupName);
            return panelMec;
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
