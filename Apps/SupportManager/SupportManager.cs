using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class SupportManager 
    {
        public Result Execute(Document doc, UIApplication uiapp)
        {            
            UIDocument uidoc = uiapp.ActiveUIDocument;            
            Element eleSupport = null;

            Converters cvn = new Converters();           

            //Variables            
            string myresult = null;
            double convertionFactor = 3.28084;                      
            string familyType = null;
            double midElevationDouble = default(double);
            double angleToY = 0;

            //Lists
            var xValuepoints = new List<double>();
            var yValuepoints = new List<double>();
            var levelsList = new List<Level>();
            var minimalElevations = new List<double>();

            //Get levels and adding levels to list
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels);
            ElementClassFilter filter = new ElementClassFilter(typeof(Level));
            levelCollector.WherePasses(filter);
            levelCollector.WhereElementIsNotElementType();       
                        
            foreach (Level item in levelCollector)
            {
                if (item.Name == "EXCLUIR")
                {
                    levelsList.Add(item);
                }
            }                  

            //Initializing
            try
            {
                //Creating new selection
                Selection sel = uiapp.ActiveUIDocument.Selection;

                //Create new PickObjects for cable tray elements
                IList<Reference> pickedObj = sel.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);  
                
                //Elements count
                double numberOfReferences = pickedObj.Count();               

                //Creating new PickObjects by face               
                Reference pickedRef = sel.PickObject(ObjectType.PointOnElement, "Selecione a face para inserir o suporte");
                Element e = doc.GetElement(pickedRef);

                //Getting face geometry and Z point to obtain elevation value                
                GeometryObject geomObj = e.GetGeometryObjectFromReference(pickedRef);
                Point p = geomObj as Point;                
                double elevationFace = pickedRef.GlobalPoint.Z / convertionFactor;

                //Get element by elementId                
                ElementId eleId = pickedObj[0].ElementId;
                Element element = doc.GetElement(eleId);

                if (element.Name.Contains("PERFILADO"))
                {                  
                    //Identifying elements previously selected and applying suitables commands
                    foreach (Reference eleRef in pickedObj)
                    {
                        eleId = eleRef.ElementId;
                        element = doc.GetElement(eleId);

                        //PERFILADOS                                 
                        //Select the suitable hanger by name of family
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, "V123");
                        FamilySymbol symbol = hangerElement as FamilySymbol;
                     
                        //Get point of support insertion in each cable tray
                        LocationCurve lc = element.Location as LocationCurve;
                        Line lineCurve = lc.Curve as Line;
                        var listPoints = new List<XYZ>();
                       
                        //Converting line length
                        double lineLengthMeter = cvn.FeettoMeter(lineCurve.ApproximateLength);

                        //SupportManagerViewModel supportManagerViewModel = new SupportManagerViewModel();
                        decimal lineLenght = decimal.Parse(lineLengthMeter.ToString());
                        decimal maximumDistance = SupportManagerMVVM.MainView.ViewModel.MaximumDistance;
                        decimal numberOfItens = Math.Round(lineLenght/ maximumDistance, 0, MidpointRounding.AwayFromZero);                                       
                        decimal incrementValue = 1 / numberOfItens;
                        List<decimal> myIList = new List<decimal>();

                        for (decimal i = 0; i <= 1; i += incrementValue)
                        {                            
                            listPoints.Add(lc.Curve.Evaluate((double)i, true));
                            myIList.Add(i);
                        }

                        //REMOVENDO DUPLICADOS
                        var listPointsNoDups = new HashSet<XYZ>(listPoints);

                        //Get Direction XYZ Points (Very Important!!)
                        XYZ directionPoint = lineCurve.Direction;


                        //DIRECTIONS STUDY
                        double XAxisDirection = directionPoint.X;
                        double YAxisDirection = directionPoint.Y;                       

                        if (XAxisDirection > 0 && YAxisDirection > 0)
                        {                          
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                        }
                        else if (XAxisDirection < 0 && YAxisDirection < 0)
                        {                          
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                        }
                        else
                        {                            
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                        }                                                               

                        //Distance between face and cable tray mid elevation
                        Parameter midElevationParam = element.LookupParameter("Elevação intermediária");
                        string midElevationValue = midElevationParam.AsValueString();
                        midElevationDouble = Math.Round(double.Parse(midElevationValue), 4);                   
                        double result = elevationFace - midElevationDouble;
                        myresult = result.ToString();

                        //Insert Hanger
                        using (var t = new Transaction(doc))
                        {
                            t.Start("Insert Hanger");
                            foreach (var point in listPointsNoDups)
                            {
                                //Set elevation value in support family
                                eleSupport = doc.Create.NewFamilyInstance(point, symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                Parameter param = eleSupport.LookupParameter("H5 Deslocamento");
                                Parameter param3 = eleSupport.LookupParameter("Tipo de serviço");
                                param.SetValueString(myresult);
                                param3.SetValueString("SUPORTES");

                                //Rotate element by vector in Y axis
                                LocationPoint location = eleSupport.Location as LocationPoint;
                                XYZ elementPoint = location.Point;
                                XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                                Line axis = Line.CreateBound(elementPoint, p2);
                                ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            }
                            t.Commit();
                        }
                    }                   
                }                 
                else
                {
                    //LISTA DE PONTOS
                    var listPoints = new List<XYZ>();
                    //REMOVENDO PONTOS DUPLICADOS
                    var listPointsNoDups = new HashSet<XYZ>(listPoints);                    

                    foreach (Reference eleRef in pickedObj)
                    {                        
                        eleId = eleRef.ElementId;
                        element = doc.GetElement(eleId);
                        Parameter param2 = element.LookupParameter("Elevação inferior");
                        string minimalElevation = param2.AsValueString();
                        double minimalElevationDouble = Math.Round(double.Parse(minimalElevation), 4);
                        minimalElevations.Add(minimalElevationDouble);

                       //Get point of support insertion in each cable tray
                        LocationCurve lc = element.Location as LocationCurve;
                        Line lineCurve = lc.Curve as Line;                       

                        //Converting line length
                        double lineLengthMeter = cvn.FeettoMeter(lineCurve.ApproximateLength);                        
                        decimal lineLenght = decimal.Parse(lineLengthMeter.ToString());
                        decimal maximumDistance = SupportManagerMVVM.MainView.ViewModel.MaximumDistance;
                        decimal numberOfItens = Math.Round(lineLenght / maximumDistance, 0, MidpointRounding.AwayFromZero);
                        decimal incrementValue = 1 / numberOfItens;

                        List<decimal> myIList = new List<decimal>();

                        for (decimal i = 0; i <= 1; i += incrementValue)
                        {
                            listPoints.Add(lc.Curve.Evaluate((double)i, true));
                            myIList.Add(i);
                        }                       

                        //xValuepoints.Add(lc.Curve.Evaluate(0.5, true).X);
                        //yValuepoints.Add(lc.Curve.Evaluate(0.5, true).Y);

                        //Get Direction XYZ Points (Very Important!!)
                        XYZ directionPoint = lineCurve.Direction;

                        //DIRECTIONS STUDY
                        double XAxisDirection = directionPoint.X;
                        double YAxisDirection = directionPoint.Y;

                        if (XAxisDirection > 0 && YAxisDirection > 0)
                        {
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                        }
                        else if (XAxisDirection < 0 && YAxisDirection < 0)
                        {
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                        }
                        else
                        {
                            //rotation angle 
                            angleToY = 0;
                            XYZ originPoint = lineCurve.Direction;
                            double xAbs = Math.Abs(originPoint.X);
                            double yAbs = Math.Abs(originPoint.Y);

                            if (originPoint.X > originPoint.Y || (originPoint.X == originPoint.Y))
                            {
                                angleToY = -Math.Acos(originPoint.Y);
                            }
                            else
                            {
                                angleToY = Math.Acos(originPoint.Y);
                            }
                        }
                    }

                    //Criando o ponto em X e Y para inserção da família  
                    double xMidPoint = xValuepoints.Average();
                    double yMidPoint = yValuepoints.Average();
                    XYZ inserctionPoint = new XYZ(xMidPoint, yMidPoint, minimalElevations.Min());

                    //Definindo a família com base nos elementos selecionados

                    //--------------------------------------------------------------A "CAMADA 1" ESTÁ PENDENTE PARA AJUSTE CONFORME OS PERFILADOS...A PARTIR DA DISTANCIA MAXIMA INSERIDA PELO USUÁRIO.


                    if (numberOfReferences == 1)
                    {
                        familyType = "1 CAMADA";

                        TaskDialog.Show("teste", "Cheguei");

                        //Selecionando a família de suporte com base no parametro "familyType" 
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, familyType);
                        FamilySymbol symbol = hangerElement as FamilySymbol;
                        var minimalElevationsListOrder = minimalElevations.OrderBy(i => i);

                        //Inserindo o suporte na família 
                        using (Transaction t = new Transaction(doc, "Suportes"))
                        {
                            t.Start();
                            foreach (var point in listPointsNoDups)
                            {
                                eleSupport = doc.Create.NewFamilyInstance(point, symbol, levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                //Inserindo valores nos parâmetros do suporte

                                double alturaTirante = Math.Round(elevationFace, 4) - Math.Round(minimalElevationsListOrder.ElementAt(0), 4);
                                Parameter tirante = eleSupport.LookupParameter("H5 Comprimento Tirante");
                                tirante.SetValueString(alturaTirante.ToString());

                                Parameter elevacaodoNivel = eleSupport.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                                elevacaodoNivel.SetValueString(Math.Round(minimalElevationsListOrder.ElementAt(0), 4).ToString());

                                //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                                LocationPoint location = eleSupport.Location as LocationPoint;
                                XYZ elementPoint = location.Point;
                                XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                                Line axis = Line.CreateBound(elementPoint, p2);
                                ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            }                            
                            t.Commit();
                        }
                    }

                    if (numberOfReferences == 2)
                    {
                        familyType = "2 CAMADAS";

                        //Selecionando a família de suporte com base no parametro "familyType" 
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, familyType);
                        FamilySymbol symbol = hangerElement as FamilySymbol;
                        var minimalElevationsListOrder = minimalElevations.OrderBy(i => i);

                        //Inserindo o suporte na família 
                        using (Transaction t = new Transaction(doc, "Suportes"))
                        {
                            t.Start();
                            eleSupport = doc.Create.NewFamilyInstance(inserctionPoint, symbol, levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            //Inserindo valores nos parâmetros do suporte

                            double alturaTirante = Math.Round(elevationFace,4) - Math.Round(minimalElevationsListOrder.ElementAt(0), 4); 
                            Parameter tirante = eleSupport.LookupParameter("H5 Comprimento Tirante");
                            tirante.SetValueString(alturaTirante.ToString());

                            Parameter altura02 = eleSupport.LookupParameter("H5 Altura 02");
                            double altura02Double = Math.Round(minimalElevationsListOrder.ElementAt(1) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura02.SetValueString(altura02Double.ToString());                       

                            Parameter elevacaodoNivel = eleSupport.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                            elevacaodoNivel.SetValueString(Math.Round(minimalElevationsListOrder.ElementAt(0), 4).ToString());

                            //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                            LocationPoint location = eleSupport.Location as LocationPoint;
                            XYZ elementPoint = location.Point;
                            XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                            Line axis = Line.CreateBound(elementPoint, p2);
                            ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            t.Commit();
                        }
                    }
                
                    if (numberOfReferences == 3)
                    {
                        familyType = "3 CAMADAS";

                        //Selecionando a família de suporte com base no parametro "familyType" 
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, familyType);
                        FamilySymbol symbol = hangerElement as FamilySymbol;

                        var minimalElevationsListOrder = minimalElevations.OrderBy(i => i);                    
                        //Inserindo o suporte na família 
                        using (Transaction t = new Transaction(doc, "Suportes"))
                        {
                            t.Start();                        
                            eleSupport = doc.Create.NewFamilyInstance(inserctionPoint, symbol, levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            //Inserindo valores nos parâmetros do suporte

                            double alturaTirante = Math.Round(elevationFace, 4) - Math.Round(minimalElevationsListOrder.ElementAt(0), 4);
                            Parameter tirante = eleSupport.LookupParameter("H5 Comprimento Tirante");
                            tirante.SetValueString(alturaTirante.ToString());

                            Parameter altura02 = eleSupport.LookupParameter("H5 Altura 02");
                            double altura02Double = Math.Round(minimalElevationsListOrder.ElementAt(1) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura02.SetValueString(altura02Double.ToString());

                            Parameter altura03 = eleSupport.LookupParameter("H5 Altura 03");
                            double altura03Double = Math.Round(minimalElevationsListOrder.ElementAt(2) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura03.SetValueString(altura03Double.ToString());                        

                            Parameter elevacaodoNivel = eleSupport.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                            elevacaodoNivel.SetValueString(Math.Round(minimalElevationsListOrder.ElementAt(0), 4).ToString());

                            //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                            LocationPoint location = eleSupport.Location as LocationPoint;
                            XYZ elementPoint = location.Point;
                            XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                            Line axis = Line.CreateBound(elementPoint, p2);
                            ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            t.Commit();
                        }
                    }
                
                    if (numberOfReferences == 4)
                    {
                        familyType = "4 CAMADAS";

                        //Selecionando a família de suporte com base no parametro "familyType" 
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, familyType);
                        FamilySymbol symbol = hangerElement as FamilySymbol;
                        var minimalElevationsListOrder = minimalElevations.OrderBy(i => i);

                        //Inserindo o suporte na família 
                        using (Transaction t = new Transaction(doc, "Suportes"))
                        {
                            t.Start();
                            eleSupport = doc.Create.NewFamilyInstance(inserctionPoint, symbol, levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            //Inserindo valores nos parâmetros do suporte

                            double alturaTirante = Math.Round(elevationFace, 4) - Math.Round(minimalElevationsListOrder.ElementAt(0), 4);
                            Parameter tirante = eleSupport.LookupParameter("H5 Comprimento Tirante");
                            tirante.SetValueString(alturaTirante.ToString());

                            Parameter altura02 = eleSupport.LookupParameter("H5 Altura 02");
                            double altura02Double = Math.Round(minimalElevationsListOrder.ElementAt(1) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura02.SetValueString(altura02Double.ToString());

                            Parameter altura03 = eleSupport.LookupParameter("H5 Altura 03");
                            double altura03Double = Math.Round(minimalElevationsListOrder.ElementAt(2) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura03.SetValueString(altura03Double.ToString());

                            Parameter altura04 = eleSupport.LookupParameter("H5 Altura 04");
                            double altura04Double = Math.Round(minimalElevationsListOrder.ElementAt(3) - minimalElevationsListOrder.ElementAt(0), 4);                    
                            altura04.SetValueString(altura04Double.ToString());

                            Parameter elevacaodoNivel = eleSupport.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                            elevacaodoNivel.SetValueString(Math.Round(minimalElevationsListOrder.ElementAt(0), 4).ToString());

                            //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                            LocationPoint location = eleSupport.Location as LocationPoint;
                            XYZ elementPoint = location.Point;
                            XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                            Line axis = Line.CreateBound(elementPoint, p2);
                            ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            t.Commit();
                        }
                    }
                
                    if (numberOfReferences == 5)
                    {
                        familyType = "5 CAMADAS";

                        //Selecionando a família de suporte com base no parametro "familyType" 
                        Collectors myCollector = new Collectors();
                        Element hangerElement = myCollector.HangerByFamilyName(doc, familyType);
                        FamilySymbol symbol = hangerElement as FamilySymbol;
                        var minimalElevationsListOrder = minimalElevations.OrderBy(i => i);

                        //Inserindo o suporte na família 
                        using (Transaction t = new Transaction(doc, "Suportes"))
                        {
                            t.Start();
                            eleSupport = doc.Create.NewFamilyInstance(inserctionPoint, symbol, levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            //Inserindo valores nos parâmetros do suporte

                            double alturaTirante = Math.Round(elevationFace, 4) - Math.Round(minimalElevationsListOrder.ElementAt(0), 4);
                            Parameter tirante = eleSupport.LookupParameter("H5 Comprimento Tirante");
                            tirante.SetValueString(alturaTirante.ToString());

                            Parameter altura02 = eleSupport.LookupParameter("H5 Altura 02");
                            double altura02Double = Math.Round(minimalElevationsListOrder.ElementAt(1) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura02.SetValueString(altura02Double.ToString());

                            Parameter altura03 = eleSupport.LookupParameter("H5 Altura 03");
                            double altura03Double = Math.Round(minimalElevationsListOrder.ElementAt(2) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura03.SetValueString(altura03Double.ToString());

                            Parameter altura04 = eleSupport.LookupParameter("H5 Altura 04");
                            double altura04Double = Math.Round(minimalElevationsListOrder.ElementAt(3) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura04.SetValueString(altura04Double.ToString());

                            Parameter altura05 = eleSupport.LookupParameter("H5 Altura 05");
                            double altura05Double = Math.Round(minimalElevationsListOrder.ElementAt(4) - minimalElevationsListOrder.ElementAt(0), 4);
                            altura05.SetValueString(altura05Double.ToString());

                            Parameter elevacaodoNivel = eleSupport.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                            elevacaodoNivel.SetValueString(Math.Round(minimalElevationsListOrder.ElementAt(0), 4).ToString());

                            //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                            LocationPoint location = eleSupport.Location as LocationPoint;
                            XYZ elementPoint = location.Point;
                            XYZ p2 = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                            Line axis = Line.CreateBound(elementPoint, p2);
                            ElementTransformUtils.RotateElement(doc, eleSupport.Id, axis, angleToY);
                            t.Commit();
                        }
                    }

                    //Definindo os pontos de inserção das fixações
                    foreach (Reference eleRef in pickedObj)
                    {
                        //Pegando as informações de altura de cada um dos elementos selecionados
                        eleId = eleRef.ElementId;
                        element = doc.GetElement(eleId);
                        Parameter param2 = element.LookupParameter("Elevação inferior");
                        string minimalElevation = param2.AsValueString();
                        double minimalElevationDouble = Math.Round(double.Parse(minimalElevation), 4);

                        //Criando uma linha central no eixo de cada eletrocalha/leito/eletroduto
                        LocationCurve lc = element.Location as LocationCurve;
                        Line lineCurve = lc.Curve as Line;                   

                        //Coletando os elementos dependentes dentro do conjunto de suporte
                        ElementClassFilter myFilter = new ElementClassFilter(typeof(FamilyInstance));
                        IList<ElementId> dependentIds = eleSupport.GetDependentElements(myFilter);
                                      
                        //Lista para armazenar os LocationPoints dos Tirantes
                        var locationPoints = new List<XYZ>();

                        //Pegando o perfilado dentro do conjunto de suportes 
                        foreach (ElementId elementsId in dependentIds)
                        {                       
                        
                            Element myElement = doc.GetElement(elementsId);
                            FamilyInstance fI = myElement as FamilyInstance;
                            FamilySymbol myElementSymbol = fI.Symbol;

                            if (myElementSymbol.FamilyName.Contains("TIRANTE"))
                            {
                                LocationPoint myLocation = myElement.Location as LocationPoint;
                                locationPoints.Add(myLocation.Point);                            
                            }
                        }

                        //Criando uma linha a partir dos Location Points dos tirantes que estão nos DependentElements
                        XYZ startPoint1 = locationPoints[0];
                        XYZ startPoint2 = locationPoints[1];
                        Line line1 = Line.CreateBound(startPoint1, startPoint2);                                   

                        //Definindo o ponto de inserção das fixações a partir da intersecção entre a linha da eletrocalha/ leito / eletroduto e a linha do suporte                              
                        //Criando um plano que passa pela linha central da eletrocalha/leito/eletroduto
                        //Criando três pontos que passam pela linha do eixo da eletrocalha/leito/eletroduto para criar um plano
                        XYZ startPoint = new XYZ(lineCurve.GetEndPoint(0).X, lineCurve.GetEndPoint(0).Y, lineCurve.GetEndPoint(0).Z);
                        XYZ endPoint = new XYZ(lineCurve.GetEndPoint(1).X, lineCurve.GetEndPoint(1).Y, lineCurve.GetEndPoint(1).Z + 1);
                        XYZ originPoint = new XYZ(lineCurve.Origin.X, lineCurve.Origin.Y, lineCurve.Origin.Z - 1);

                        Autodesk.Revit.DB.Plane plane = Autodesk.Revit.DB.Plane.CreateByThreePoints(startPoint, endPoint, originPoint);

                        //Definindo o ponto de interseção entre a linha que passa pelo eixo do perfilado e o plano que passa pelos elementos selecionados
                        XYZ locationPoint = LinePlaneIntersection(line1, plane, out double b);             

                        //Inserindo as fixações com base na altura e ponto de interseção
                        if (element.Name.Contains("ELETROCALHA"))
                        {
                            Collectors myCollector = new Collectors();
                            Element hangerElement = myCollector.HangerByFamilyName(doc, "V161");
                            FamilySymbol symbol = hangerElement as FamilySymbol;
                            Parameter param = element.LookupParameter("Largura");
                            string paramValue = param.AsValueString();

                            string larguraConvertida = (double.Parse(paramValue) / 1000).ToString();

                            using (Transaction t = new Transaction(doc, "Suportes"))
                            {
                                t.Start();
                                Element fixacao = doc.Create.NewFamilyInstance(locationPoint, symbol,
                                    levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                Parameter param1 = fixacao.LookupParameter("Largura");
                                param1.SetValueString(larguraConvertida);

                                Parameter elevacaodoNivel = fixacao.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                                elevacaodoNivel.SetValueString(Math.Round(minimalElevationDouble, 4).ToString());

                                //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                                LocationPoint location = fixacao.Location as LocationPoint;
                                XYZ elementPoint = location.Point;
                                XYZ pZ = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                                Line axis = Line.CreateBound(elementPoint, pZ);
                                ElementTransformUtils.RotateElement(doc, fixacao.Id, axis, angleToY);
                                t.Commit();
                            }
                        }

                        if (element.Name.Contains("LEITO"))
                        {
                            Collectors myCollector = new Collectors();
                            Element hangerElement = myCollector.HangerByFamilyName(doc, "V171");
                            FamilySymbol symbol = hangerElement as FamilySymbol;
                            Parameter param = element.LookupParameter("Largura");
                            string paramValue = param.AsValueString();

                            string larguraConvertida = (double.Parse(paramValue) / 1000).ToString();

                            using (Transaction t = new Transaction(doc, "Suportes"))
                            {
                                t.Start();
                                Element fixacao = doc.Create.NewFamilyInstance(locationPoint, symbol,
                                    levelsList.First(), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                Parameter param1 = fixacao.LookupParameter("Largura");
                                param1.SetValueString(larguraConvertida);

                                Parameter elevacaodoNivel = fixacao.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                                elevacaodoNivel.SetValueString(Math.Round(minimalElevationDouble, 4).ToString());


                                //Rotacionando o suporte para que esteja perpendicular as eletrocalhas
                                LocationPoint location = fixacao.Location as LocationPoint;
                                XYZ elementPoint = location.Point;
                                XYZ pZ = new XYZ(elementPoint.X, elementPoint.Y, elementPoint.Z + 10);
                                Line axis = Line.CreateBound(elementPoint, pZ);
                                ElementTransformUtils.RotateElement(doc, fixacao.Id, axis, angleToY);
                                t.Commit();
                            }
                        }                                        
                    }
                }
            }
            finally
            {

            }
            return Result.Succeeded;
        }

        public static XYZ LinePlaneIntersection( Line line,  Autodesk.Revit.DB.Plane plane, out double lineParameter)
        {
            XYZ planePoint = plane.Origin;
            XYZ planeNormal = plane.Normal;
            XYZ linePoint = line.GetEndPoint(0);
            XYZ lineDirection = (line.GetEndPoint(1)- linePoint).Normalize();
            // Is the line parallel to the plane, i.e.,
            // perpendicular to the plane normal?
            if ((planeNormal.DotProduct(lineDirection))==0)
            {
                lineParameter = double.NaN;
                return null;
            }
            lineParameter = (planeNormal.DotProduct(planePoint)-
                planeNormal.DotProduct(linePoint))/ planeNormal.DotProduct(lineDirection);
            return linePoint + lineParameter * lineDirection;
        }               

    }

}



