using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace H5Plugins
{

    public partial class AreaDeFormasMVVM : Window
    {
        readonly ExternalEvent externalEvent = ExternalEvent.Create(new AreaDeFormasEEH());
        readonly ExternalEvent paintfaceexternalEvent = ExternalEvent.Create(new PaintFaceOverrideExternalEventHandler());
        readonly ExternalEvent setparameterCurveFaceExternalEvent = ExternalEvent.Create(new SetCurveFaceParametersExternalEventHandler());
        readonly ExternalEvent setparameterPlanarFaceExternalEvent = ExternalEvent.Create(new SetPlanarFaceParametersExternalEventHandler());
        readonly ExternalEvent selectcurveFaceExternalEvent = ExternalEvent.Create(new SelectCurveFaceExternalEventHandler());
        readonly ExternalEvent selectplanarFaceExternalEvent = ExternalEvent.Create(new SelectPlanarFaceExternalEventHandler());
        readonly ExternalEvent paintcurvefacesbyselectionExternalEvent = ExternalEvent.Create(new PaintFinalCurveFacesOverrideExternalEventHandler());
        readonly ExternalEvent paintplanarfacesbyselectionExternalEvent = ExternalEvent.Create(new PaintFinalPlanarFacesOverrideExternalEventHandler());
        readonly ExternalEvent clearpaintedfaces = ExternalEvent.Create(new ClearFacesEEH());

        public static List<string> CurveFaceListFinalString = new List<string>();
        public static List<string> PlanarFaceListFinalString = new List<string>();      
        public static AreaDeFormasMVVM MainView { get; set; } = new AreaDeFormasMVVM();

        public AreaDeFormasMVVM()
        {
            InitializeComponent();
            InitializeCommands();

            TotalArea.Click += TotalArea_Click;
        }      

        private void SelectElements_Click(object sender, RoutedEventArgs e)
        {         
            try
            {                              
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
                //LIMPA OS VALORES DENTRO DE TODAS AS LISTBOX              
                FinalsPlanarFaceListBox.Items.Clear();
                FinalsCurveFaceListBox.Items.Clear();
                AllPlanarFacesListBox.Items.Clear();
                AllCurveFacesListBox.Items.Clear();                
            }
            catch 
            {                
            }            
            //Reativa o evento externo para seleção dos elementos na UI    
            externalEvent.Raise();             
        }
        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }

        //////////////////////////////////////////////////////////////////////////BOTÕES PARA ENVIO DE FACE VIA ADICIONAR        
        //FACES CURVAS          
        private void FaceCurveAddF1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedItem = AllCurveFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsCurveFaceListBox.Items.Add(selectedItem + " - F1");
                    AllCurveFacesListBox.Items.Remove(selectedItem);
                }                
            }
            catch (Exception)
            {

            }
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }       

        private void FaceCurveAddF2_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                string selectedItem = AllCurveFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsCurveFaceListBox.Items.Add(selectedItem + " - F2");
                    AllCurveFacesListBox.Items.Remove(selectedItem);
                }
            }
            catch (Exception)
            {

            }
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void FaceCurveAddF3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedItem = AllCurveFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsCurveFaceListBox.Items.Add(selectedItem + " - F3");
                    AllCurveFacesListBox.Items.Remove(selectedItem);
                }
            }
            catch (Exception)
            {

            }
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void AllCurveFacesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Pintar o elemento selecionado na listbox
            string selectedItem = AllCurveFacesListBox.SelectedItem.ToString();

            foreach (Face face in ADFSelectElements.CurveFacesElementList)
            {
                string hash = face.GetHashCode().ToString();

                if (selectedItem.Contains(hash))
                {                    
                    ADFSelectElements.FaceToPaint = face;                   
                }
                else
                {
                    continue;
                }
            }
            paintfaceexternalEvent.Raise(); ;

        }
        private void FinalsCurveFaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ////Pintar o elemento selecionado na listbox
            //string selectedItem = FinalsCurveFaceListBox.SelectedItem.ToString();

            //foreach (Face face in ADFSelectElements.CurveFacesElementList)
            //{
            //    string hash = face.GetHashCode().ToString();

            //    if (selectedItem.Contains(hash))
            //    {
            //        ADFSelectElements.FaceToPaint = face;
            //        break;
            //    }
            //}
            //paintfaceexternalEvent.Raise();
        }
        private void FaceCurveRemoveButton_Click(object sender, RoutedEventArgs e)
        {       
            //try
            //{
            //    string selectedItem = FinalsCurveFaceListBox.SelectedItem.ToString();
            //    int index = 0;
            //    string myValue = "";

            //    if (selectedItem != null)
            //    {
            //        try
            //        {
            //            foreach (string item in ADFSelectElements.CurveFacesHashCodeString)
            //            {
            //                if (selectedItem.Contains(item))
            //                {
            //                    index = ADFSelectElements.CurveFacesHashCodeString.IndexOf(item);
            //                    myValue = ADFSelectElements.CurveFacesAreaAndHashCodeString.ElementAt(index);

            //                    if (!AllCurveFacesListBox.Items.Contains(myValue))
            //                    {
            //                        AllCurveFacesListBox.Items.Add(myValue);
            //                        FinalsCurveFaceListBox.Items.Remove(selectedItem);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception)
            //        {
            //        }
            //    }
            //}
            //catch
            //{
            //}                     
        }


        //FACES PLANAS
        private void PlanarFaceAddF1_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = null;
            try
            {
                selectedItem = AllPlanarFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsPlanarFaceListBox.Items.Add(selectedItem + " - F1");
                    AllPlanarFacesListBox.Items.Remove(selectedItem);
                }
            }
            catch (Exception)
            {
            }            
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {                
            }            
        }

        private void PlanarFaceAddF2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedItem = AllPlanarFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsPlanarFaceListBox.Items.Add(selectedItem + " - F2");
                    AreaDeFormasMVVM.MainView.AllPlanarFacesListBox.Items.Remove(selectedItem);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void PlanarFaceAddF3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedItem = AllPlanarFacesListBox.SelectedItem.ToString();
                if (selectedItem != null)
                {
                    FinalsPlanarFaceListBox.Items.Add(selectedItem + " - F3");
                    AreaDeFormasMVVM.MainView.AllPlanarFacesListBox.Items.Remove(selectedItem);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void AllPlanarFacesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Pintar o elemento selecionado na listbox
            string selectedItem = AllPlanarFacesListBox.SelectedItem.ToString();

            foreach (Face face in ADFSelectElements.PlanarFacesElementList)
            {
                string hash = face.GetHashCode().ToString();

                if (selectedItem.Contains(hash))
                {
                    ADFSelectElements.FaceToPaint = face;
                    break;
                }
            }
            paintfaceexternalEvent.Raise();
        }

        private void PlanarFaceRemoveButton_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                string selectedItem = FinalsPlanarFaceListBox.SelectedItem.ToString();
                int index = 0;
                string myValue = "";

                if (selectedItem != null)
                {
                    try
                    {
                        foreach (string item in ADFSelectElements.PlanarFacesHashCodeString)
                        {
                            if (selectedItem.Contains(item))
                            {
                                index = ADFSelectElements.PlanarFacesHashCodeString.IndexOf(item);
                                myValue = ADFSelectElements.PlanarFacesAreaAndHashCodeString.ElementAt(index);

                                if (!AllPlanarFacesListBox.Items.Contains(myValue))
                                {
                                    AllPlanarFacesListBox.Items.Add(myValue);
                                    FinalsPlanarFaceListBox.Items.Remove(selectedItem);                                    
                                }
                            }
                        }                        
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch 
            {                
            }            
        }
        private void FinalsPlanarFaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////Pintar o elemento selecionado na listbox
            //string selectedItem = FinalsPlanarFaceListBox.SelectedItem.ToString();

            ////foreach (Face face in ADFSelectElements.PlanarFacesElementList)
            ////{
            ////    string hash = face.GetHashCode().ToString();

            ////    if (selectedItem.Contains(hash))
            ////    {
            ////        ADFSelectElements.FaceToPaint = face;
            ////        break;
            ////    }
            ////}
            ////paintfaceexternalEvent.Raise();
        }
        
        //////////////////////////////////////////////////////////////////////////BOTÕES PARA SELEÇÃO DE FACE NA UI
        //FACES CURVAS
        private void FaceCurveAddF1_Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch 
            {                
            }
            
            //LIMPANDO A LISTA DE FACES SELECIONADAS
            SelectFaces.MySelectedFaces.Clear();
            SelectCurveFaceExternalEventHandler.typeofFace = "F1";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA            
            selectcurveFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }       

        private void FaceCurveAddF2_Select_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch
            {
            }

            SelectFaces.MySelectedFaces.Clear();
            SelectCurveFaceExternalEventHandler.typeofFace = "F2";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA            
            selectcurveFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void FaceCurveAddF3_Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch
            {
            }

            SelectFaces.MySelectedFaces.Clear();
            SelectCurveFaceExternalEventHandler.typeofFace = "F3";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA
            selectcurveFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        //FACES PLANAS
        private void PlanarFaceAddF1_Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch
            {
            }
            SelectFaces.MySelectedFaces.Clear();
            SelectPlanarFaceExternalEventHandler.typeofFace = "F1";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA
            selectplanarFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void PlanarFaceAddF2_Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch
            {
            }

            SelectFaces.MySelectedFaces.Clear();
            SelectPlanarFaceExternalEventHandler.typeofFace = "F2";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA
            selectplanarFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        private void PlanarFaceAddF3_Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
            }
            catch
            {
            }

            SelectFaces.MySelectedFaces.Clear();
            SelectPlanarFaceExternalEventHandler.typeofFace = "F3";
            //PINTANDO A FACE APÓS A SELEÇÃO CONFORME A COR DO TIPO DE FORMA
            selectplanarFaceExternalEvent.Raise();
            try
            {
                paintplanarfacesbyselectionExternalEvent.Raise();
                paintcurvefacesbyselectionExternalEvent.Raise();
            }
            catch (Exception)
            {
            }
        }

        //LIMPAR TODAS AS FACES DAS PEÇAS SELECIONADAS
        private void ClearFaces_Click(object sender, RoutedEventArgs e)
        {
            try
            {             
                //RETIRA A SELEÇÃO DE TODOS OS ITEMS DENTRO DE TODAS AS LISTBOX     
                FinalsCurveFaceListBox.UnselectAll();
                FinalsPlanarFaceListBox.UnselectAll();
                AllPlanarFacesListBox.UnselectAll();
                AllCurveFacesListBox.UnselectAll();
                //LIMPA OS VALORES DENTRO DE TODAS AS LISTBOX              
                FinalsPlanarFaceListBox.Items.Clear();
                FinalsCurveFaceListBox.Items.Clear();
                AllPlanarFacesListBox.Items.Clear();
                AllCurveFacesListBox.Items.Clear();
                clearpaintedfaces.Raise();
            }
            catch 
            {                      
            }            
        }

        //CÁLCULAR ÁREA TOTAL
        private void TotalArea_Click(object sender, RoutedEventArgs e)
        {
            CurveFaceListFinalString.Clear();
            PlanarFaceListFinalString.Clear();

            try
            {
                foreach (var item in FinalsCurveFaceListBox.Items)
                {
                    CurveFaceListFinalString.Add(item.ToString());
                }
                foreach (var item in FinalsPlanarFaceListBox.Items)
                {
                    PlanarFaceListFinalString.Add(item.ToString());
                }
            }
            catch 
            {                
            }           
            try
            {
                setparameterCurveFaceExternalEvent.Raise();
            }
            catch 
            {               
            }
            try
            {
                setparameterPlanarFaceExternalEvent.Raise();
            }
            catch 
            {
            }           
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }
       
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string countAllCurveFaces;

        public string CountAllCurveFaces
        {
            get { return countAllCurveFaces; }
            set
            {
                countAllCurveFaces = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}




