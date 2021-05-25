using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H5Plugins
{
    [Transaction(TransactionMode.Manual)]
    public class About
    {
        public void Abt ()

        {

            try
            {
                System.Diagnostics.Process.Start("https://www.head5.com.br/sobre");                
            }
            finally
            {

            }


        }

    }
}




