using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H5Plugins
{
    class OverrideGraphicsInView
    {
        public void Clear(View view, ElementId elementId)
        {
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            view.SetElementOverrides(elementId, ogs);
        }
    }
}
