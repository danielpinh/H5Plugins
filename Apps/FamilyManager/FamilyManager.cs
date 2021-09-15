using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H5Plugins
{
    class FamilyManager
    {
        public void Open(Document doc)
        {
            var allFamilyTypes = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .ToElements()
                .ToList();           


        }
    }
}
