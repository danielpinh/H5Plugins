using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class ScheduleExportMechanical
    {        
        public static int numberofTables {get; set;}        

        public void ScheduleExport(Document doc)
        {
            CheckExcellProcesses();

            ScheduleExportMechanicalMVVM scheduleExportmainView = H5AppExternalApplication.h5App.scheduleExportMechanicalMVVM;

            //COLETANDO A REVISÃO INSERIDA PELO USUÁRIO        
            string myreviewValue = scheduleExportmainView.ViewModel.MyReviewValue;

            //COLENTANDO O NÚMERO DO DOCUMENTO
            string myDocumentValue = scheduleExportmainView.ViewModel.MyDocumentValue;

            //COLETANDO TODOS OS DADOS DE INFORMAÇÃO DO PROJETO
            //NOME DO DOCUMENTO
            string documentTitle = doc.Title.ToString();

            //CENTRO DE CUSTO 
            string centroDeCusto = documentTitle.Substring(0, 4);

            //INFORMAÇÕES DE PROJETO
            ProjectInfo projectInfo = doc.ProjectInformation;
            //NOME DO PROJETO
            Autodesk.Revit.DB.Parameter nomeDoProjeto = projectInfo.get_Parameter(BuiltInParameter.PROJECT_BUILDING_NAME);
            string nomeDoProjetoString = nomeDoProjeto.AsString();

            //LISTAS DE ELEMENTOS E STRINGS DAS TABELAS
            List<string> SelectedSchedulesListString = new List<string>();
            List<ViewSchedule> SelectedScheduleList = new List<ViewSchedule>();

            //COLETANDO OS VALORES SELECIONADOS NA CHECKLISTBOX
            IEnumerable<ScheduleExportMechanicalViewModel> selecteds = scheduleExportmainView.MainViewModel.Where(ps => ps.IsCheckedScheduleName == true);
            foreach (var item in selecteds)
            {
                SelectedSchedulesListString.Add(item.ScheduleName.ToString());
            }

            //COLETANDO TODAS AS TABELAS DO PROJETO
            FilteredElementCollector scheduleCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule));

            foreach (ViewSchedule item in scheduleCollector)
            {
                if (SelectedSchedulesListString.Contains(item.Name))
                {
                    SelectedScheduleList.Add(item);
                }
            }

            numberofTables = SelectedScheduleList.Count();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file(*.xls)|*.xls";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Pasta de destino das Tabelas de Quantitativo do Revit";
            saveFileDialog.ShowDialog();

            //DEFININDO O CAMINHO PARA SALVAR AS TABELAS COMO ARQUIVOS DE PLANILHAS
            string fileName = saveFileDialog.FileName.ToString();
            FileInfo fileInfo = new FileInfo(fileName);
            string directoryName = fileInfo.DirectoryName.ToString();

            //DEFININDO O NOME DO ARQUIVO QUE SERÁ SALVA A PLANILHA FINAL
            string pathString = System.IO.Path.Combine(directoryName, documentTitle + "-" + myreviewValue);

            ////SE A PASTA ESTIVER ABERTA, FECHE-A!
            //foreach (Process p in Process.GetProcessesByName("explorer"))
            //{
            //    if (p.MainWindowTitle.ToLower().Contains(documentTitle))
            //    {
            //        p.Kill();
            //    }
            //}

            //SE A PASTA EXISTIR, PARE O PROCESSO E ALERTE O USUÁRIO
            if (Directory.Exists(pathString))
            {
                scheduleExportmainView.Close();
                TaskDialogResult tskresult = TaskDialogResult.Close;
                TaskDialogCommonButtons tskbuttons = TaskDialogCommonButtons.Close;
                TaskDialog.Show("ERRO!", "ERRO! A revisão que você está tentando criar já existe. Crie uma nova revisão ou exclua a revisão criada anteriormente.", tskbuttons, tskresult);
            }
            else
            {
                //Coletando o arquivo de excel que contém o modelo de capa dentro do diretório da pasta e inserindo no workbook criado
                string coverfilePath = @"H:\Projetos\2108-BIM\Desenvolvimento-BIM\00-TEMPLATES\07-LISTA DE MATERIAIS\XX-CAPAS\" + centroDeCusto + "-CAPA.XLS";                       
                FileInfo coverFileInfor = new FileInfo(coverfilePath);

                //VERIFICANDO SE O ARQUIVO DE CAPA EXISTE NO CAMINHO ESPECIFICADO NA REDE
                if (coverFileInfor.Exists)
                {
                    //Create a directory to salve schedules
                    DirectoryInfo myDirectory = Directory.CreateDirectory(pathString);

                    //Final directory to save the schedules
                    string directoryFinalName = myDirectory.FullName;

                    //Options 
                    ViewScheduleExportOptions opt = new ViewScheduleExportOptions();

                    //CRIANDO UM ARQUIVO DE PLANILHA DE EXCEL PARA CADA TABELA DE QUANTITATIVO DO REVIT
                    try
                    {
                        foreach (ViewSchedule vs in SelectedScheduleList)
                        {
                            if (!vs.IsTemplate)
                            {
                                string scheduleName = vs.Name.ToString();
                                vs.Export(directoryFinalName, scheduleName + ".xls", opt);
                            }
                        }
                    }
                    catch
                    {
                    }

                    //Criando um novo arquivo de excel 
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

                    //DESABILITANDO ALERTAS DURANTE A EXECUÇÃO DO CÓDIGO
                    app.DisplayAlerts = false;

                    //Criando um workbook em branco no arquivo de excel criado
                    //Os Workbooks são uma coleção de uma ou mais tabelas - também chamadas de worksheets -, reunidas em um único arquivo
                    app.Workbooks.Add("");

                    //Lendo todos os arquivos que estão no caminho fornecido pelo diretório
                    string path = directoryFinalName;
                    DirectoryInfo dir = new DirectoryInfo(path);

                    //Para cada arquivo dentro do diretório fornecido, adicione ao workbook criado anteriormente, em seguida, deleta o arquivo.
                    foreach (FileInfo myfileInfo in dir.GetFiles())
                    {
                        app.Workbooks.Add(myfileInfo.FullName);
                        myfileInfo.Delete();
                    }

                    app.Workbooks.Add(coverFileInfor.FullName);
                    //Adicionando todos as tabelas (worksheets) dentro dos arquivos de excel (workbooks) dentro da pasta no caminho do diretório para dentro do workbook em branco que foi criado anteriormente.
                    //Pegando o primeiro workbook
                    Workbook myWorkbook = app.Workbooks[1];

                    ////Criando uma worksheet fictícia dentro do workbook
                    Worksheet dummyWorksheet = myWorkbook.Worksheets[1];

                    for (int i = 2; i <= app.Workbooks.Count; i++)
                    {
                        int count = app.Workbooks[i].Worksheets.Count;
                        for (int j = 1; j <= count; j++)
                        {
                            Worksheet ws = (Worksheet)app.Workbooks[i].Worksheets[j];
                            ws.Columns.AutoFit();
                            ws.Copy(app.Workbooks[1].Worksheets[1]);
                        }
                    }

                    ////Deletando a planilha em branco criada inicialmente            
                    dummyWorksheet.Delete();

                    //DEFININDO O NOME FINAL DO ARQUIVO NOVO
                    string finalFileName = documentTitle + "-" + myreviewValue + ".xls";
                    string pathString2 = System.IO.Path.Combine(directoryFinalName, finalFileName);

                    //Criando uma nova pasta com o workbook criado com todas as worksheets unidas dentro dele
                    app.Workbooks[1].SaveCopyAs(pathString2);

                    //COLETANDO O ARQUIVO CRIADO      
                    Microsoft.Office.Interop.Excel.Application finalApp = new Microsoft.Office.Interop.Excel.Application();
                    Workbook workbookFinalApp = finalApp.Workbooks.Open(pathString2);

                    //ORGANIZANDO A VISUALIZAÇÃO DOS DADOS NA PLANILHA PARA CADA WORKSHEET NO WORKBOOK, EXCETO O PRIMEIRO, QUE É A CAPA
                    int numberofWorksheets = workbookFinalApp.Worksheets.Count;
                    //int initialValue = 1;
                    List<Worksheet> worksheetsList = new List<Worksheet>();

                    foreach (Worksheet activeWorksheet in workbookFinalApp.Worksheets)
                    {
                        worksheetsList.Add(activeWorksheet);
                    }

                    //ADICIONANDO BARRA DE PROGRESSO AO PROCESSO DE CRIAÇÃO DAS PLANILHAS
                    using (var progressBar = new ProgressBar("Preparando as Listas de Materiais..."))
                    {
                        progressBar.RunSheets(worksheetsList, (Worksheet activeWorksheet) =>
                        {
                            if (activeWorksheet.Name != "Capa" && activeWorksheet.Name != "DADOS")
                            {
                                Range usedRange = activeWorksheet.UsedRange;
                                Range rows = usedRange.Rows;
                                Range columns = usedRange.Columns;
                                int numberOfRows = rows.Count;
                                int numberOfColumns = columns.Count;

                                //RANGE
                                
                                Range firstCellInFirstColumnRange = activeWorksheet.Cells[1, 1];                             
                                Range secondCellInFirstColumnRange = activeWorksheet.Cells[2, 1];
                                Range lastCellInLastColumnRange = activeWorksheet.Cells[numberOfRows, numberOfColumns];
                                Range cellAfterlastCellInLastColumnRange = activeWorksheet.Cells[numberOfRows + 50, numberOfColumns];
                                Range lastCellAfterLastCellInFirtsColumnRange = activeWorksheet.Cells[numberOfRows + 1, 1];
                                Range lastCellinLastColumninFirtsRow = activeWorksheet.Cells[1, numberOfColumns];
                                Range lastCellinLastColumninSecondRow = activeWorksheet.Cells[2, numberOfColumns];
                                Range CellAfterLastCellinFirtsRow = activeWorksheet.Cells[1, numberOfColumns + 1];
                                Range cellAfterlastCellinFirtsRow = activeWorksheet.Cells[1, numberOfColumns + 1];
                                Range latestCell = activeWorksheet.Cells[numberOfRows + 50, numberOfColumns + 50];
                                Range lastCellInFirstColumn = activeWorksheet.Cells[numberOfRows, 1];
                                Range firstCellinSecondRow = activeWorksheet.Cells[2, 1];
                                Range allCellswithText = activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange);

                                //ORGANIZANDO O TÍTULO DA PLANILHA
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Merge();
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Font.FontStyle = "Bold";
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Cells.Font.Name = "Arial";
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Cells.Font.Size = "12";
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Borders.LineStyle = XlLineStyle.xlContinuous;
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellinLastColumninFirtsRow).Borders.Weight = XlBorderWeight.xlThin;

                                //ORGANIZANDO OS CABEÇALHOS
                                activeWorksheet.get_Range(secondCellInFirstColumnRange, lastCellinLastColumninSecondRow).Font.FontStyle = "Bold";
                                activeWorksheet.get_Range(secondCellInFirstColumnRange, lastCellinLastColumninSecondRow).Borders.LineStyle = XlLineStyle.xlContinuous;
                                activeWorksheet.get_Range(secondCellInFirstColumnRange, lastCellinLastColumninSecondRow).Borders.Weight = XlBorderWeight.xlThin;

                                //ADICIONANDO BORDAS NA TABELA, AJUSTANDO A FONTE E TAMANHO DO TEXTO, ALINHANDO O TEXTO 
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange).Borders.LineStyle = XlLineStyle.xlContinuous;
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange).Borders.Weight = XlBorderWeight.xlThin;
                                //activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange).BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, (XlColorIndex)5);
                                activeWorksheet.get_Range(secondCellInFirstColumnRange, lastCellInLastColumnRange).Cells.Font.Name = "Arial";
                                activeWorksheet.get_Range(secondCellInFirstColumnRange, lastCellInLastColumnRange).Cells.Font.Size = "10";
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange).Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                                activeWorksheet.get_Range(firstCellInFirstColumnRange, lastCellInLastColumnRange).Style.VerticalAlignment = XlHAlign.xlHAlignCenter;

                                //PINTANDO TODAS AS DEMAIS CÉLULAS DE CINZA            
                                //activeWorksheet.get_Range(CellAfterLastCellinFirtsRow, latestCell).Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                //activeWorksheet.get_Range(lastCellAfterLastCellInFirtsColumnRange, cellAfterlastCellInLastColumnRange).Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.Gray);

                                //AUTO AJUSTANDO ALTURA E LARGURA DAS CÉLULAS
                                activeWorksheet.Columns.AutoFit();
                                activeWorksheet.Rows.AutoFit();
                                activeWorksheet.Cells.RowHeight = 25.00;                               

                                //VISUALIZANDO A QUEBRA DE PÁGINAS PARA IMPRESSÃO
                                activeWorksheet.Activate();
                                activeWorksheet.Application.ActiveWindow.View = XlWindowView.xlPageBreakPreview;

                                //FIXANDO A PRIMEIRA E A SEGUNDA LINHA COMO CABEÇALHO DE TODAS AS FOLHAS DE IMPRESSÃO
                                activeWorksheet.PageSetup.PrintTitleRows = "$1:$1";
                                activeWorksheet.PageSetup.PrintTitleRows = "$2:$1";

                                //INSERINDO CABEÇALHO COM O NÚMERO DE PÁGINAS
                                activeWorksheet.PageSetup.CenterFooter = "Folha &P de &N";
                                activeWorksheet.PageSetup.LeftHeader = "Nº Revisão: " + myreviewValue;
                                activeWorksheet.PageSetup.RightHeader = "Nº Documento: " + myDocumentValue;

                                //DEFININDO A ÁREA DE IMPRESSÃO NA FOLHA 

                                string firstcolumnNumberToLetter = ColumnIndexToColumnLetter(firstCellInFirstColumnRange.Column);
                                string firstrowNumber = firstCellInFirstColumnRange.Row.ToString();

                                string lastcolumnNumberToLetter = ColumnIndexToColumnLetter(lastCellInLastColumnRange.Column);
                                string lastrowNumber = lastCellInLastColumnRange.Row.ToString();


                                activeWorksheet.PageSetup.PrintArea = "$" + firstcolumnNumberToLetter + "$"
                                    + firstrowNumber + ":$" + lastcolumnNumberToLetter + "$" + lastrowNumber;                            
                                                                                                             
                                //AJUSTANDO O VPAGEBREAKS
                                activeWorksheet.VPageBreaks[1].DragOff(XlDirection.xlToRight, 1);                              

                            }
                            else if (activeWorksheet.Name == "Capa")
                            {
                                //VISUALIZANDO A QUEBRA DE PÁGINAS PARA IMPRESSÃO
                                activeWorksheet.Activate();
                                activeWorksheet.Application.ActiveWindow.View = XlWindowView.xlPageBreakPreview;

                                //DATA DO DIA 
                                DateTime today = DateTime.Today;

                                //REPLACE NOS NOMES DAS CÉLULAS    
                                //activeWorksheet.Cells.Replace("[ELAB]", nomeDoProjetoString);
                                //activeWorksheet.Cells.Replace("[DATA_ELAB]", DateTime.Now.ToString("M/d/yyyy"));
                                //activeWorksheet.Cells.Replace("CODIGO_CLIENTE", );
                            }
                            //initialValue++;            
                        });
                    }

                    //DEIXANDO A CAPA COMO A ÚLTIMA VISTA ATIVA NA PLANILHA
                    foreach (Worksheet activeWorksheet in workbookFinalApp.Worksheets)
                    {
                        if (activeWorksheet.Name == "Capa")
                        {
                            activeWorksheet.Activate();
                        }
                    }

                    //RESTAURANDO ALERTAS DO ARQUIVO
                    app.DisplayAlerts = true;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    //FECHANDO E SALVANDO O ARQUIVO
                    workbookFinalApp.Save();
                    workbookFinalApp.Close();
                    finalApp.Quit();
                    Marshal.FinalReleaseComObject(workbookFinalApp);
                    Marshal.FinalReleaseComObject(finalApp);

                    KillExcel();
                }
                else
                {
                    scheduleExportmainView.Close();
                    TaskDialogResult tskresult = TaskDialogResult.Close;
                    TaskDialogCommonButtons tskbuttons = TaskDialogCommonButtons.Close;
                    TaskDialog.Show("ERRO!", "ERRO! O arquivo de Capa não foi encontrado. Certifique-se de que ele está no caminho correto e que o centro de custo é adequado." + Environment.NewLine + Environment.NewLine +
                        @"O caminho para o arquivo na pasta é:" + Environment.NewLine + Environment.NewLine + @"H:\Projetos\2108 - BIM\Desenvolvimento - BIM\00 - TEMPLATES\07 - LISTA DE MATERIAIS\XX - CAPAS\", tskbuttons, tskresult);
                }
            }
        }

        private void CheckExcellProcesses()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");
            Hashtable myHashtable = new Hashtable();
            int iCount = 0;

            foreach (Process ExcelProcess in AllProcesses)
            {
                myHashtable.Add(ExcelProcess.Id, iCount);
                iCount = iCount + 1;
            }
        }
        private void KillExcel()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");
            Hashtable myHashtable = new Hashtable();

            // check to kill the right process
            foreach (Process ExcelProcess in AllProcesses)
            {
                if (myHashtable.ContainsKey(ExcelProcess.Id) == false)
                    ExcelProcess.Kill();
            }
            AllProcesses = null;
        }

        static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }
    }   
}