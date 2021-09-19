using Autodesk.Revit.UI;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace H5Plugins
{
    public class H5AppExternalApplication : IExternalApplication
    {
        internal static H5AppExternalApplication h5App = new H5AppExternalApplication();
        internal UIApplication uiApp = null;

        //LISTA DE MATERIAIS APP        
        internal ExternalEvent scheduleExportEvent = null;        
        internal ScheduleExportEEH scheduleExportHandler = null;
        internal ScheduleExportMVVM scheduleExportMVVM = null;        
        ////ÁREA DE FORMAS APP
        //internal ExternalEvent areaDeFormasEvent = null;
        //internal AreaDeFormasEEH areaDeFormas     

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication app)
        {
            //CRIANDO O MENU RIBBON COM OS BOTÕES PRINCIPAIS NA INTERFACE DO USUÁRIO
            CreateH5AppMenuRibbon(app);

            //INSTANCIANDO ESSA APLICAÇÃO            
            h5App = this;            
            return Result.Succeeded;
        }

        //------------------------------------------------------------------JANELAS EXTERNAS PARA OS PLUG-INS 

        //EXPORTAR LISTAS DE MATERIAIS
        internal void ShowScheduleExportUI()
        {
            if (scheduleExportMVVM == null || scheduleExportMVVM.IsLoaded == false)
            {            
                //CRIANDO O EXTERNAL EVENT               
                scheduleExportHandler = new ScheduleExportEEH();                
                scheduleExportEvent = ExternalEvent.Create(scheduleExportHandler);              
                

                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                scheduleExportMVVM = new ScheduleExportMVVM(scheduleExportEvent);
                scheduleExportMVVM.Show();
            }
        }

        //ÁREA DE FORMAS

        //internal void ShowAreaDeFormasUI()
        //{
        //    if (AreaDeFormasMVVM == null || scheduleExportMVVM.IsLoaded == false)
        //    {
        //        //CRIANDO O EXTERNAL EVENT               
        //        scheduleExportHandler = new ScheduleExportEEH();
        //        scheduleExportEvent = ExternalEvent.Create(scheduleExportHandler);


        //        //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
        //        scheduleExportMVVM = new ScheduleExportMVVM(scheduleExportEvent);
        //        scheduleExportMVVM.Show();
        //    }
        //}

        //------------------------------------------------------------------MÉTODOS PARA CRIAR E PREENCHER O MENU RIBBON

        public RibbonPanel NewMenuRibbonGroup(UIControlledApplication application, string addinName, string groupName)
        {
            RibbonPanel newPanel = application.CreateRibbonPanel(addinName, groupName);
            return newPanel;
        }
        public PushButton NewPushButton(string genericName, string buttonName, string buttonPath, string buttonCommand, string toolTip, RibbonPanel panelName, string imageIconSource, string helpAdress)
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

        public PushButtonData NewPushButtonData(string genericName, string buttonName, string buttonPath, string buttonCommand, string toolTip, string helpAdress)
        {
            //Help button adress            
            ContextualHelp contexHelp = new ContextualHelp(ContextualHelpType.Url, helpAdress);

            //Create a button
            PushButtonData newPushButtonData = new PushButtonData(genericName, buttonName, buttonPath, buttonCommand);
            newPushButtonData.SetContextualHelp(contexHelp);
            newPushButtonData.ToolTip = toolTip;            

            return newPushButtonData;
        }
        public SplitButton NewSplitButton(RibbonPanel panel, string toolTip, string splitButtonGenericName, string splitButtonName, string imageIconSource, string helpAdress)
        {
            //Help button adress            
            ContextualHelp contexHelp = new ContextualHelp(ContextualHelpType.Url, helpAdress);

            //Create a Split Button
            SplitButtonData splitButtonData = new SplitButtonData(splitButtonGenericName, splitButtonName);
            splitButtonData.Name = splitButtonName;
            splitButtonData.Text = splitButtonName;            
            splitButtonData.LargeImage = PngImageSource(imageIconSource);
            splitButtonData.ToolTip = toolTip;
            splitButtonData.SetContextualHelp(contexHelp);

            //Add icon to button
            SplitButton newsplitButton = panel.AddItem(splitButtonData) as SplitButton;          

            return newsplitButton;
        }

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            return decoder.Frames[0];
        }           

        private void CreateH5AppMenuRibbon(UIControlledApplication app)
        {
            //Variables
            string addinName = "H5App";

            //New Menu Ribbon  
            app.CreateRibbonTab("H5App");
            string path = Assembly.GetExecutingAssembly().Location;

            //Help adress for buttons
            string helpAdress = @"https://www.head5.com.br/";

            //Ribbon Menu Groups
            RibbonPanel panelMec = NewMenuRibbonGroup(app, addinName, "Mecânica");
            RibbonPanel panelCiv = NewMenuRibbonGroup(app, addinName, "Civil");
            RibbonPanel panelGer = NewMenuRibbonGroup(app, addinName, "Geral");
            //RibbonPanel panelEle = NewMenuRibbonGroup(app, addinName, "Elétrica");
            RibbonPanel panelAbout = NewMenuRibbonGroup(app, addinName, "Sobre");
            //RibbonPanel panelSupport = NewMenuRibbonGroup(app, addinName, "Suportes");

            //Adding buttons
            //
            //Detalhes Típicos            
            PushButton detalhesTipicos = NewPushButton("Button1", String.Format("Detalhes" + Environment.NewLine + "Típicos"),
                path,
                "H5Plugins.DetalhesTipicosEC",
                "Cria Folhas com os Detalhes Típicos dos Equipamentos Inseridos no Projeto.",
                panelGer,
                "H5Plugins.Resources.DetalhesTipicos.png",
                helpAdress);

            //Gerar Isometricos
            PushButton gerarIsometrico = NewPushButton("Button2", String.Format("Criar" + Environment.NewLine + "Isométrico"),
                path,
                "H5Plugins.IsometricCreator",
                "Selecione elementos para compor uma vista isométrica de tubulação.",
                panelMec,
                "H5Plugins.Resources.Isometrico.png",
                helpAdress);

            //Gerar Tabela de Vistas Isométricas
            PushButton tabelaIsometrico = NewPushButton("Button3", String.Format("Tabela de" + Environment.NewLine + "Isométricos"),
                path,
                "H5Plugins.ElementsInIsometricSheets",
                "Correlaciona os elementos aos seus isométricos de referência, inserindo o valor no parâmetro 'H5 Isométrico'",
                panelMec,
                "H5Plugins.Resources.TabelaIsometrico.png",
                helpAdress);

            //Ordenar Cortes    
            PushButton ordenarCortes = NewPushButton("Button4", String.Format("Ordenar" + Environment.NewLine + "Cortes"),
                path,
                "H5Plugins.OrdenarCortes",
                "Ordena os Cortes do Projeto Conforme a Numeração das Folhas.",
                panelGer,
                "H5Plugins.Resources.OrdenarCortes.png",
                helpAdress);

            //Lookup Table Mapping    
            PushButton lookupTableMapping = NewPushButton("Button5", String.Format("Mapear" + Environment.NewLine + "Lookup Tables"),
                path,
                "H5Plugins.LookupExternalCommand",
                "Atribui os Códigos Internos e de Fabricantes às Famílias de Sistema",
                panelGer,
                "H5Plugins.Resources.LookupTableMapping.png",
                helpAdress);

            //Support Manager   
            PushButton supportManager = NewPushButton("Button6", String.Format("Inserir" + Environment.NewLine + "Suportes"),
                path,
                "H5Plugins.SupportManagerEC",
                "Insere suportes em famílias de eletrodutos, eletrocalhas, perfilados, leitos e tubos",
                panelGer,
                "H5Plugins.Resources.SupportManager.png",
                helpAdress);

            //Adicionar Parâmetros    
            PushButton adicionarParametros = NewPushButton("Button7", String.Format("Adicionar" + Environment.NewLine + "Parâmetros"),
                path,
                "H5Plugins.GerenciarParametros",
                "Adiciona Parâmetros em Lote na Família.",
                panelGer,
                "H5Plugins.Resources.GerenciarParametros.png",
                helpAdress);

            ////Connect Conduit           
            //PushButton connectConduit = NewMenuRibbonButton("Button9", String.Format("Connect" + Environment.NewLine + "Conduit"),
            //    path,
            //    "H5Plugins.ConnectConduit",
            //    "Insere eletrodutos entre conectores de famílias selecionadas",
            //    panelGer,
            //    "H5Plugins.Resources.ConnectConduit.png",
            //    helpAdress);

            //VedaJunta            
            PushButton vedaJunta = NewPushButton("Button8", String.Format("Veda" + Environment.NewLine + "Junta"),
                path,
                "H5Plugins.VedaJunta",
                "Insere os comprimentos equivalentes por trecho de veda junta.",
                panelCiv,
                "H5Plugins.Resources.VedaJunta.png",
                helpAdress);

            //Atualizar Cotas Isometrico                    
            PushButton atualizarCotasIsometrico = NewPushButton("Button9", String.Format("Atualizar" + Environment.NewLine + "Cotas"),
                path,
                "H5Plugins.IsometricRefreshDimensions",
                "Atualiza cotas numa vista de Isométrico.",
                panelMec,
                "H5Plugins.Resources.AtualizarCotasIsometrico.png",
                helpAdress);

            //Atualizar Cotas Isométrico Por Seleção                  
            PushButton atualizarCotasIsometricoSelecao = NewPushButton("Button10", String.Format("Atualizar Cotas" + Environment.NewLine + "Seleção"),
                path,
                "H5Plugins.IsometricRefreshDimensionsBySelection",
                "Atualiza cotas numa vista de Isométrico dos Elementos selecionados e seus dependentes.",
                panelMec,
                "H5Plugins.Resources.AtualizarCotasIsometrico.png",
                helpAdress);

            //QUANTITATIVOS PARA CURVA GOMADA                    
            PushButton curvaGomada = NewPushButton("Button11", String.Format("Curva Gomada" + Environment.NewLine + "Quantitativos"),
                path,
                "H5Plugins.CurvaGomada",
                "Atribui comprimentos das curvas gomadas aos tubos para quantitativos.",
                panelMec,
                "H5Plugins.Resources.LookupTableMapping.png",
                helpAdress);

            //EXPORTAR LISTA DE MATERIAIS
            SplitButton scheduleExportSplitButton = NewSplitButton(panelGer,
                "Exporta as tabelas de materiais para .xls",
                 "SplitButton1",
                 "Exportar" + Environment.NewLine + "Listas de Materiais",
                 "H5Plugins.Resources.ListaDeMateriais.png",
                helpAdress);

            scheduleExportSplitButton.ItemText = "Exportar" + Environment.NewLine + "Listas de Materiais";            

            //PushButton - Lista de Materiais - Elétrica
            PushButtonData scheduleExportPushButtonDataEletrica = NewPushButtonData("Button12", String.Format("Lista de Materiais: Elétrica"),
                path,
                "H5Plugins.ScheduleExportEletricaEC",
                "Exporta as tabelas de quantitativo em formato .xls",                
                helpAdress);
            PushButton scheduleExportPushButtonEletrica  = scheduleExportSplitButton.AddPushButton(scheduleExportPushButtonDataEletrica);
            scheduleExportPushButtonEletrica.LargeImage = PngImageSource("H5Plugins.Resources.ListaDeMateriais.png");

            //PushButton - Lista de Materiais - Mecânica
            PushButtonData scheduleExportPushButtonDataMecanica = NewPushButtonData("Button13", String.Format("Lista de Materiais: Mecânica"),
                path,
                "H5Plugins.ScheduleExportMecanicaEC",
                "Exporta as tabelas de quantitativo em formato .xls",                
                helpAdress);
            PushButton scheduleExportPushButtonMecanica = scheduleExportSplitButton.AddPushButton(scheduleExportPushButtonDataMecanica);
            scheduleExportPushButtonMecanica.LargeImage = PngImageSource("H5Plugins.Resources.ListaDeMateriais.png");

            //Definindo a opção de PushButton vigente:
            scheduleExportSplitButton.CurrentButton = scheduleExportPushButtonMecanica;

            //ÁREA DE FORMAS           
            PushButton areaDeFormas = NewPushButton("Button14", String.Format("Área de" + Environment.NewLine + "Formas"),
                path,
                "H5Plugins.AreaDeFormasEC",
                "Calcula a área de formas das estruturas de concreto selecionadas",
                panelCiv,
                "H5Plugins.Resources.AreaDeFormas.png",
                helpAdress);

            //Sobre a Head5   
            PushButton aboutButton = NewPushButton("Button15", String.Format("Head5"),
                path,
                "H5Plugins.About",
                "Mais que Projetos. Engenharia Aplicada à Inovação.",
                panelAbout,
                "H5Plugins.Resources.Head5.png",
                helpAdress);

            //Adding ToolTipImage to a about button
            aboutButton.ToolTipImage = PngImageSource("H5Plugins.Resources.Sobre.png");
        }


    }

}
