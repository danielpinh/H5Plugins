using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;

namespace H5Plugins
{
    public class ClearPaintedFace
    {
        public void Clear(Face face, Document doc)
        {            
            ElementId eleId = face.Reference.ElementId;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Descolorir Face");
                try
                {
                    doc.RemovePaint(eleId, face);
                }
                catch 
                {                    
                }
                tx.Commit();
            }
        }
    }                          
}