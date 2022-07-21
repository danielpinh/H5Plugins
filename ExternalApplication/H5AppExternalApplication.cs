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
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        /// <summary>
        /// Custom tool to open new projects by newest templates by Head5 Engenharia.
        /// </summary>
        /// <param name="application">An object that is passed to the external application 
        /// which contains the controlled application.</param>
        void CreateCommandBinding(UIControlledApplication application)
        {
            RevitCommandId commandId = RevitCommandId.LookupCommandId("ID_FILE_NEW_CHOOSE_TEMPLATE");
            try
            {
                AddInCommandBinding importBinding = application.CreateAddInCommandBinding(commandId);
                importBinding.Executed += new EventHandler<Autodesk.Revit.UI.Events.ExecutedEventArgs>(ImportReplacement);
            }
            catch 
            { }
        }
        private void ImportReplacement(object sender, Autodesk.Revit.UI.Events.ExecutedEventArgs arg)
        {
            TemplateManagerAppMVVM templateManagerAppMVVM = new TemplateManagerAppMVVM();
            templateManagerAppMVVM.Show();
        }

        public Result OnStartup(UIControlledApplication app)
        {
            //CRIANDO O MENU RIBBON COM OS BOTÕES PRINCIPAIS NA INTERFACE DO USUÁRIO
            CreateH5AppMenuRibbon(app);

            CreateCommandBinding(app);

            //INSTANCIANDO ESSA APLICAÇÃO            
            h5App = this;
            return Result.Succeeded;
        }

        //------------------------------------------------------------------JANELAS EXTERNAS PARA OS PLUG-INS 
        //SCHEDULE EXPORT MECHANICAL APP        
        internal ExternalEvent scheduleExportMechanicalEvent = null;
        internal ScheduleExportMechanicalEEH scheduleExportMechanicalHandler = null;
        internal ScheduleExportMechanicalMVVM scheduleExportMechanicalMVVM = null;
        internal void ShowScheduleExportMechanicalUI()
        {
            if (scheduleExportMechanicalMVVM == null || scheduleExportMechanicalMVVM.IsLoaded == false)
            {            
                //CRIANDO O EXTERNAL EVENT               
                scheduleExportMechanicalHandler = new ScheduleExportMechanicalEEH();                
                scheduleExportMechanicalEvent = ExternalEvent.Create(scheduleExportMechanicalHandler);            
                
                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                scheduleExportMechanicalMVVM = new ScheduleExportMechanicalMVVM(scheduleExportMechanicalEvent);
                scheduleExportMechanicalMVVM.Show();
            }
        }

        //SCHEDULE EXPORT ELECTRICAL APP        
        internal ExternalEvent scheduleExportElectricalEvent = null;
        internal ScheduleExportElectricalEEH scheduleExportElectricalHandler = null;
        internal ScheduleExportElectricalMVVM scheduleExportElectricalMVVM = null;
        internal void ShowScheduleExportElectricalUI()
        {
            if (scheduleExportElectricalMVVM == null || scheduleExportElectricalMVVM.IsLoaded == false)
            {
                //CRIANDO O EXTERNAL EVENT               
                scheduleExportElectricalHandler = new ScheduleExportElectricalEEH();
                scheduleExportElectricalEvent = ExternalEvent.Create(scheduleExportElectricalHandler);

                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                scheduleExportElectricalMVVM = new ScheduleExportElectricalMVVM(scheduleExportElectricalEvent);
                scheduleExportElectricalMVVM.Show();
            }
        }

        //LINK PARAMETER APP       
        internal ExternalEvent linkParameterEvent = null;
        internal LinkParameterEEH linkParameterHandler = null;
        internal LinkParameterMVVM linkParameterMVVM = null;
        internal void ShowLinkParameterlUI()
        {
            if (linkParameterMVVM == null || linkParameterMVVM.IsLoaded == false)
            {
                //CRIANDO O EXTERNAL EVENT               
                linkParameterHandler = new LinkParameterEEH();
                linkParameterEvent = ExternalEvent.Create(linkParameterHandler);

                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                linkParameterMVVM = new LinkParameterMVVM(linkParameterEvent);
                linkParameterMVVM.Show();
            }
        }

        //LOOKUPTABLEMAP APP
        internal ExternalEvent lookupTableMapEvent = null;
        internal LookupTableMapEEH lookupTableMapHandler = null;
        internal LookupTableMapMVVM lookupTableMapMVVM = null;
        internal void ShowLookupTableMapUI()
        {
            if (lookupTableMapMVVM == null || lookupTableMapMVVM.IsLoaded == false)
            {
                //CRIANDO O EXTERNAL EVENT               
                lookupTableMapHandler = new LookupTableMapEEH();
                lookupTableMapEvent = ExternalEvent.Create(lookupTableMapHandler);


                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                lookupTableMapMVVM = new LookupTableMapMVVM(lookupTableMapEvent, lookupTableMapHandler);
                lookupTableMapMVVM.Show();
            }
        }


        //LOOKUPTABLEMAPPING APP
        internal LookupTableMappingMVVM lookupTableMappingMVVM = null;
        internal void ShowLookupTableMappingUI()
        {
            if (lookupTableMappingMVVM == null || lookupTableMappingMVVM.IsLoaded == false)
            {
                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                lookupTableMappingMVVM = new LookupTableMappingMVVM();
                lookupTableMappingMVVM.Show();
            }
        }

        //DETALHES TÍPICOS APP
        internal ExternalEvent detalhesTipicosEvent = null;
        internal DetalhesTipicosEEH detalhesTipicosHandler = null;
        internal DetalhesTipicosMVVM detalhesTipicosMVVM = null;        
        internal void ShowDetalhesTipicosUI()
        {
            if (detalhesTipicosMVVM == null || detalhesTipicosMVVM.IsLoaded == false)
            {
                //CRIANDO O EXTERNAL EVENT               
                detalhesTipicosHandler = new DetalhesTipicosEEH();
                detalhesTipicosEvent = ExternalEvent.Create(detalhesTipicosHandler);

                //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
                detalhesTipicosMVVM = new DetalhesTipicosMVVM();
                detalhesTipicosMVVM.Show();
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
        //
        //        //INICIALIZANDO A JANELA E PASSANDO O EXTERNAL EVENT
        //        scheduleExportMVVM = new ScheduleExportMVVM(scheduleExportEvent);
        //        scheduleExportMVVM.Show();
        //    }
        //}       
        //------------------------------------------------------------------BOTÕES NO MENU RIBBON 

        private void CreateH5AppMenuRibbon(UIControlledApplication app)
        {
            //Variables
            string addinName = "H5App";

            //New Menu Ribbon  
            app.CreateRibbonTab("H5App");
            string path = Assembly.GetExecutingAssembly().Location;

            //Help adress for buttons
            string helpAdress = @"https://www.head5.com.br/";

            //---------------------------------------------------------------CREATING MENU RIBBON PANELS            

            //Ribbon Menu Groups
            RibbonPanel panelMec = NewMenuRibbonGroup(app, addinName, "Mecânica");            
            RibbonPanel panelCiv = NewMenuRibbonGroup(app, addinName, "Civil");
            RibbonPanel panelGer = NewMenuRibbonGroup(app, addinName, "Geral");
            //RibbonPanel panelEle = NewMenuRibbonGroup(app, addinName, "Elétrica");
            RibbonPanel panelAbout = NewMenuRibbonGroup(app, addinName, "Sobre");
            //RibbonPanel panelSupport = NewMenuRibbonGroup(app, addinName, "Suportes");          

            //---------------------------------------------------------------ADDING BUTTONS TO MENU RIBBON PANELS            

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
                "H5Plugins.LookupTableMapEC",
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
                "H5Plugins.ScheduleExportElectricalEC",
                "Exporta as tabelas de quantitativo em formato .xls",                
                helpAdress);
            PushButton scheduleExportPushButtonEletrica  = scheduleExportSplitButton.AddPushButton(scheduleExportPushButtonDataEletrica);
            scheduleExportPushButtonEletrica.LargeImage = PngImageSource("H5Plugins.Resources.ListaDeMateriais.png");

            //PushButton - Lista de Materiais - Mecânica
            PushButtonData scheduleExportPushButtonDataMecanica = NewPushButtonData("Button13", String.Format("Lista de Materiais: Mecânica"),
                path,
                "H5Plugins.ScheduleExportMechanicalEC",
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

            //LINK PARAMETER          
            PushButton linkParameter = NewPushButton("Button15", String.Format("Vincular" + Environment.NewLine + "Parâmetros"),
                path,
                "H5Plugins.LinkParameterEC",
                "Atribui o valor do Parâmetro H5 Sistema às famílias aninhadas às famílias selecionadas",
                panelMec,
                "H5Plugins.Resources.LookupTableMapping.png",
                helpAdress);

            //Sobre a Head5   
            PushButton aboutButton = NewPushButton("Button16", String.Format("Head5"),
                path,
                "H5Plugins.About",
                "Mais que Projetos. Engenharia Aplicada à Inovação.",
                panelAbout,
                "H5Plugins.Resources.Head5.png",
                helpAdress);

            //Adding ToolTipImage to a about button
            aboutButton.ToolTipImage = PngImageSource("H5Plugins.Resources.Sobre.png");
        }

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

        public System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            return decoder.Frames[0];
        }


    }

}
