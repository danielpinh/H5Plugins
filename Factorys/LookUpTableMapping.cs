using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H5Plugins
{
    
    public class LookUpTableMapping
    {
        public string LookupByHeader(string keyHeader, string keyValue, string headerToSearchValue) 
        {

            string rowValue = "";

            //Key Header List
            var keyHeaderListColumnValues = CsvColumnListValuesByHeader(keyHeader);          

            //Head5 Code List
            var listToSearchValue = CsvColumnListValuesByHeader(headerToSearchValue);

            //Search Parameter Value
            for (int i = 0; i < keyHeaderListColumnValues.Count; i++)
            {
                if (keyHeaderListColumnValues[i] == keyValue)
                {
                    rowValue += i.ToString();
                }
            }

            // Return final value

            string parameterValue = ValueByCsvColumnListAndRowIndex(listToSearchValue, rowValue);      
            return parameterValue; 
           
        }
        public List<string> CsvColumnListValuesByHeader(string header)
        {
            //necessary default variables
            var csvList = new List<string[]>();
            var heightColumn = new List<string>();
            string keyColumnValue = "";

            //Path to acess the .csv file
            string[] csvLines = System.IO.File.ReadAllLines(@"V:\Projetos\2108-BIM\Desenvolvimento-BIM\04-COMPONENTES 3D\02-ELÉTRICA\1004.3.2 - Leitos_Para_Cabos\Leito_Cabos.csv");

            //Split each row into column data and create an array of strings with each row into a new List
            for (int i = 0; i < csvLines.Length; i++)
            {
                csvList.Add(csvLines[i].Split(';'));
            }
            //Creating a column lis based on header value
            for (int j = 0; j < csvList.Count; j++)
            {
                for (int z = 0; z < csvList[j].Length; z++)
                {
                    if (csvList[j][z] == header)
                    {
                        keyColumnValue += z.ToString();
                    }
                }
                heightColumn.Add(csvList[j][int.Parse(keyColumnValue)]);
            }
            return heightColumn;
        }
        public string ValueByCsvColumnListAndRowIndex(List<string> List, string rowValue)
        {
            string myValue = List[int.Parse(rowValue)];
            return myValue;
        }
    }


}