using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H5Plugins
{
    /// <summary>
    /// Database acess
    /// </summary>
    public class DataAcess
    {
        /// <summary>
        /// Returns the file path of the lookup table based on it's name from GED-BIM Database.
        /// </summary>
        /// <param name="lookupName"></param>
        public string GetLookupTable(string lookupName)
        {
            string filePath = "null"; 

            //Open connection with SQL DataBase
            using (SqlConnection connection = new SqlConnection(@"Data Source=192.168.10.7\SQLExpress;Initial Catalog=GED_BIM;User ID=sa;Password=adm*0450"))
            {
                connection.Open();

                //this will return one row, even if there are more than one with the same max first max value!
                SqlCommand command = new SqlCommand($"SELECT TOP 1 str_PATH FROM View_Anexos_Documento WHERE (str_TITULO_1 = '{lookupName}' AND str_CATEGORIA = 'LOOKUP') ORDER BY str_REV DESC", connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return filePath = reader.GetString(0).ToString();
                    }
                }

                connection.Close();
            }

            return filePath;
        }
    }
}
