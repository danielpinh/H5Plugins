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
        public string LookupByTwoHeaders(string keyHeader1, string keyHeader2, string keyValue1, string keyValue2, string headerToSearchValue, string csvPath)
        {
            string rowValue = "";

            //Key Header1 List
            var keyHeader1ListColumnValues = CsvColumnListValuesByHeader(keyHeader1, csvPath);

            //Key Header2 List
            var keyHeader2ListColumnValues = CsvColumnListValuesByHeader(keyHeader2, csvPath);

            //Head5 Code List
            var listToSearchValue = CsvColumnListValuesByHeader(headerToSearchValue, csvPath);

            //Search Parameter Value
            for (int i = 0; i < keyHeader1ListColumnValues.Count; i++)
            {
                if (keyHeader1ListColumnValues[i] == keyValue1 && keyHeader2ListColumnValues[i] == keyValue2)
                {
                    rowValue += i.ToString();
                }
            }
            // Return final value
            string parameterValue = ValueByCsvColumnListAndRowIndex(listToSearchValue, rowValue);
            return parameterValue;
        }

        public string LookupByOneHeader(string keyHeader, string keyValue, string headerToSearchValue, string csvPath) 
        {

            string rowValue = "";

            //Key Header List
            var keyHeaderListColumnValues = CsvColumnListValuesByHeader(keyHeader, csvPath);          

            //Head5 Code List
            var listToSearchValue = CsvColumnListValuesByHeader(headerToSearchValue, csvPath);

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
        public List<string> CsvColumnListValuesByHeader(string header, string csvPath)
        {
            //necessary default variables
            var csvList = new List<string[]>();
            var columnValues = new List<string>();
            string keyColumnValue = "";

            //Path to acess the .csv file
            string[] csvLines = System.IO.File.ReadAllLines(csvPath);

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
                columnValues.Add(csvList[j][int.Parse(keyColumnValue)]);
            }
            return columnValues;
        }
        public string ValueByCsvColumnListAndRowIndex(List<string> List, string rowValue)
        {
            string myValue = List[int.Parse(rowValue)];
            return myValue;
        }
    }


}