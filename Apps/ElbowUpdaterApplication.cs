//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace H5Plugins.ElbowUpdater
//{
//    public class ElbowUpdaterApplication : Autodesk.Revit.UI.IExternalApplication
//    {
//        public Result OnStartup(Autodesk.Revit.UI.UIControlledApplication application)
//        {
//            // Register elbow updater with Revit
//            ElbowUpdater updater = new ElbowUpdater(application.ActiveAddInId);
//            UpdaterRegistry.RegisterUpdater(updater);

//            // Change Scope = any pipe fitting element
//            ElementCategoryFilter pipeFittingFilter = new ElementCategoryFilter(BuiltInCategory.OST_PipeFitting);

//            // Change type = element addition
//            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), pipeFittingFilter, Element.GetChangeTypeElementAddition());
//            return Result.Succeeded;
//        }

//        public Result OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
//        {
//            ElbowUpdater updater = new ElbowUpdater(application.ActiveAddInId);
//            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
//            return Result.Succeeded;
//        }
//    }

//    public class ElbowUpdater : IUpdater
//    {
//        static AddInId m_appId;
//        static UpdaterId m_updaterId;
//        WallType m_wallType = null;

//        // constructor takes the AddInId for the add-in associated with this updater
//        public ElbowUpdater(AddInId id)
//        {
//            m_appId = id;
//            m_updaterId = new UpdaterId(m_appId, new Guid("FBFBF6B2-4C06-42d4-97C1-D1B4EB593EFF"));
//        }

//        public void Execute(UpdaterData data)
//        {
//            Document doc = data.GetDocument();

//            // Cache the wall type
//            if (m_wallType == null)
//            {
//                FilteredElementCollector collector = new FilteredElementCollector(doc);
//                collector.OfClass(typeof(WallType));
//                var wallTypes = from element in collector
//                                where
//                                        element.Name == "Exterior - Brick on CMU"
//                                select element;
//                if (wallTypes.Count<Element>() > 0)
//                {
//                    m_wallType = wallTypes.Cast<WallType>().ElementAt<WallType>(0);
//                }
//            }

//            if (m_wallType != null)
//            {
//                // Change the wall to the cached wall type.
//                foreach (ElementId addedElemId in data.GetAddedElementIds())
//                {
//                    Wall wall = doc.GetElement(addedElemId) as Wall;
//                    if (wall != null)
//                    {
//                        wall.WallType = m_wallType;
//                    }
//                }
//            }
//        }

//        public string GetAdditionalInformation()
//        {
//            return "Wall type updater example: updates all newly created walls to a special wall";
//        }

//        public ChangePriority GetChangePriority()
//        {
//            return ChangePriority.FloorsRoofsStructuralWalls;
//        }

//        public UpdaterId GetUpdaterId()
//        {
//            return m_updaterId;
//        }

//        public string GetUpdaterName()
//        {
//            return "Wall Type Updater";
//        }
//    }






//}
