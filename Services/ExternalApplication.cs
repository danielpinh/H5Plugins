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
        public Result OnStartup(UIControlledApplication app)
        {
            //Variables
            string addinName = "H5App";

            //New Menu Ribbon  
            app.CreateRibbonTab("H5App");
            string path = Assembly.GetExecutingAssembly().Location;

            //Help adress for buttons
            string helpAdress = @"https://www.head5.com.br/";

            //Ribbon Menu Groups
            //RibbonPanel panelMec = NewMenuRibbonGroup(app, addinName, "Mecânica");
            RibbonPanel panelCiv = NewMenuRibbonGroup(app, addinName, "Civil");
            RibbonPanel panelGer = NewMenuRibbonGroup(app, addinName, "Geral");
            //RibbonPanel panelEle = NewMenuRibbonGroup(app, addinName, "Elétrica");
            RibbonPanel panelAbout = NewMenuRibbonGroup(app, addinName, "Sobre");
            RibbonPanel panelSupport = NewMenuRibbonGroup(app, addinName, "Support" + Environment.NewLine + "Manager");

            //Adding buttons
            //Detalhes Típicos            
            PushButton detalhesTipicos = NewMenuRibbonButton("Button1", String.Format("Detalhes" + Environment.NewLine + "Típicos"),
                path,
                "H5Plugins.DTExternalCommand",
                "Cria Folhas com os Detalhes Típicos dos Equipamentos Inseridos no Projeto.",
                panelGer,
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

            //Lookup Table Mapping    
            PushButton lookupTableMapping = NewMenuRibbonButton("Button3", String.Format("Lookup Table" + Environment.NewLine + "Mapping"),
                path,
                "H5Plugins.LookupExternalCommand",
                "Atribui os Códigos Internos e de Fabricantes às Famílias de Sistema",
                panelGer,
                "H5Plugins.Resources.LookupTableMapping.png",
                helpAdress);

            //Support Manager   
            PushButton supportManager = NewMenuRibbonButton("Button4", String.Format("Support" + Environment.NewLine + "Manager"),
                path,
                "H5Plugins.SupportManager",
                "Insere suportes em famílias de eletrodutos, eletrocalhas, perfilados, leitos e tubos",
                panelSupport,
                "H5Plugins.Resources.SupportManager.png",
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

            //Adicionar Parâmetros    
            PushButton adicionarParametros = NewMenuRibbonButton("Button6", String.Format("Adicionar" + Environment.NewLine + "Parâmetros"),
                path,
                "H5Plugins.GerenciarParametros",
                "Adiciona Parâmetros em Lote na Família.",
                panelGer,
                "H5Plugins.Resources.GerenciarParametros.png",
                helpAdress);

            //Connect Conduit           
            PushButton connectConduit = NewMenuRibbonButton("Button7", String.Format("Connect" + Environment.NewLine + "Conduit"),
                path,
                "H5Plugins.ConnectConduit",
                "Insere eletrodutos entre conectores de famílias selecionadas",
                panelGer,
                "H5Plugins.Resources.ConnectConduit.png",
                helpAdress);

            //VedaJunta            
            PushButton vedaJunta = NewMenuRibbonButton("Button8", String.Format("Veda" + Environment.NewLine + "Junta"),
                path,
                "H5Plugins.VedaJunta",
                "Insere os comprimentos equivalentes por trecho de veda junta.",
                panelCiv,
                "H5Plugins.Resources.VedaJunta.png",
                helpAdress);

            return Result.Succeeded;
        }

        public RibbonPanel NewMenuRibbonGroup(UIControlledApplication application, string addinName, string groupName)
        {

            RibbonPanel newPanel = application.CreateRibbonPanel(addinName, groupName);
            return newPanel;
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

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            return decoder.Frames[0];
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }
    }

}
