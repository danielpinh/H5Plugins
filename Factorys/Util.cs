using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WinForms = System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using System.IO;
using System.Xml.Linq;

namespace H5Plugins
{
    class Util
    {        
        /// <summary>
        /// Return the given element's connector manager, 
        /// using either the family instance MEPModel or 
        /// directly from the MEPCurve connector manager
        /// for ducts and pipes.
        /// </summary>
        static ConnectorManager GetConnectorManager(
          Element e)
        {
            MEPCurve mc = e as MEPCurve;
            FamilyInstance fi = e as FamilyInstance;

            if (null == mc && null == fi)
            {
                throw new ArgumentException(
                  "Element is neither an MEP curve nor a fitting.");
            }

            return null == mc
              ? fi.MEPModel.ConnectorManager
              : mc.ConnectorManager;
        }

        /// <summary>
        /// Return the element's connector at the given
        /// location, and its other connector as well, 
        /// in case there are exactly two of them.
        /// </summary>
        /// <param name="e">An element, e.g. duct, pipe or family instance</param>
        /// <param name="location">The location of one of its connectors</param>
        /// <param name="otherConnector">The other connector, in case there are just two of them</param>
        /// <returns>The connector at the given location</returns>
        static Connector GetConnectorAt(
          Element e,
          XYZ location,
          out Connector otherConnector)
        {
            otherConnector = null;

            Connector targetConnector = null;

            ConnectorManager cm = GetConnectorManager(e);

            bool hasTwoConnectors = 2 == cm.Connectors.Size;

            foreach (Connector c in cm.Connectors)
            {
                if (c.Origin.IsAlmostEqualTo(location))
                {
                    targetConnector = c;

                    if (!hasTwoConnectors)
                    {
                        break;
                    }
                }
                else if (hasTwoConnectors)
                {
                    otherConnector = c;
                }
            }
            return targetConnector;
        }

        /// <summary>
        /// Return the connector set element
        /// closest to the given point.
        /// </summary>
        static Connector GetConnectorClosestTo(
          ConnectorSet connectors,
          XYZ p)
        {
            Connector targetConnector = null;
            double minDist = double.MaxValue;

            foreach (Connector c in connectors)
            {
                double d = c.Origin.DistanceTo(p);

                if (d < minDist)
                {
                    targetConnector = c;
                    minDist = d;
                }
            }
            return targetConnector;
        }

        /// <summary>
        /// Return the connector on the element 
        /// closest to the given point.
        /// </summary>
        public static Connector GetConnectorClosestTo(
          Element e,
          XYZ p)
        {
            ConnectorManager cm = GetConnectorManager(e);

            return null == cm
              ? null
              : GetConnectorClosestTo(cm.Connectors, p);
        }

        /// <summary>
        /// Connect two MEP elements at a given point p.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if
        /// one of the given elements lacks connectors.
        /// </exception>
        public static void Connect(
          XYZ p,
          Element a,
          Element b)
        {
            ConnectorManager cm = GetConnectorManager(a);

            if (null == cm)
            {
                throw new ArgumentException(
                  "Element a has no connectors.");
            }

            Connector ca = GetConnectorClosestTo(
              cm.Connectors, p);

            cm = GetConnectorManager(b);

            if (null == cm)
            {
                throw new ArgumentException(
                  "Element b has no connectors.");
            }

            Connector cb = GetConnectorClosestTo(
              cm.Connectors, p);

            ca.ConnectTo(cb);
            //cb.ConnectTo( ca );
        }
    }
}