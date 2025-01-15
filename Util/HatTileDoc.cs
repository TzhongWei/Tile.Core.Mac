using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Tile.Core.Util
{
    /// <summary>
    /// This is a static class managing the hat_tile instance file
    /// </summary>
    public static class HatTileDoc
    {
        private static readonly BlockInstanceManager _blockInstances;

        private static readonly object _lock = new object();

        public static BlockInstanceManager BlockInstances
        {
            get
            {
                lock (_lock)
                {
                    return _blockInstances;
                }
            }
        }

        static HatTileDoc() 
        {
            _blockInstances = InitialBlock();
        }
        /// <summary>
        /// Provide the block name list
        /// </summary>
        /// <returns></returns>
        public static List<string> HatBlock_NameList()
            => _blockInstances.Select(x => x.BlockName).ToList();
        public static List<string> HatBlock_NameList(Label label)
            => _blockInstances.Where(x => x.BlockLabel == label).Select(x=>x.BlockName).ToList();

        /// <summary>
        /// If the initialblock() cannot work, you have to use this function to renew the list
        /// use it when it's enmergency
        /// </summary>
        public static void ForceUpdate()
        {
            _blockInstances.Clear();
            var RHDoc = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions;
            foreach (var instance in RHDoc)
            {
                if (RhinoDoc.ActiveDoc.InstanceDefinitions.Find(instance.Name) == null) continue;
                if (instance.GetUserString("Hat") != "HatDoc") continue;

                try
                {
                    _blockInstances.Add(instance);
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine($"Failed to add instance: {instance.Name}. Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Initialise the block system from rhino
        /// </summary>
        /// <returns></returns>
        private static BlockInstanceManager InitialBlock()
        {
            var Manager = new BlockInstanceManager();
            var RHDoc = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions;

            foreach (var instance in RHDoc)
            {
                if (RhinoDoc.ActiveDoc.InstanceDefinitions.Find(instance.Name) == null) continue;
                if (instance.GetUserString("Hat") != "HatDoc") continue;

                try
                {
                    Manager.Add(instance);
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine($"Failed to add instance: {instance.Name}. Error: {ex.Message}");
                }
            }
            return Manager;
        }
        /// <summary>
        /// Add new hat instance in both rhino and this program. The reference point is set to origin point.
        /// </summary>
        /// <param name="Name">Name of this instance</param>
        /// <param name="label"></param>
        /// <param name="tilePatterns">the tile patterns</param>
        /// <param name="ID"></param>
        /// <returns></returns>
        internal static bool AddNewHatInstance(string Name, TilePatterns tilePatterns, out int ID)
        {
            ID = -1;
            //if (string.IsNullOrWhiteSpace(Name))
            //{
            //    RhinoApp.WriteLine("Invalid block name.");
            //    return false;
            //}

            var Ins = RhinoDoc.ActiveDoc.InstanceDefinitions;
            if(Ins.Find(Name) is null)
            {
                ID = Ins.Add(Name, 
                    "This is Einstein Hat Tile Program blocks",
                    Point3d.Origin,
                    tilePatterns.Patterns,
                    tilePatterns.PatternAtts
                    );
                if (ID < 0)
                {
                    throw new Exception($"Instance Define errpr");
                }
                //SetUserString
                var InsObj = Ins[ID];
                InsObj.SetUserString("Hat", "HatDoc");
                InsObj.SetUserString("BlockName", Name);
                InsObj.SetUserString("Label", tilePatterns.label.ToString());
                InsObj.SetUserString("ID", ID.ToString());
                InsObj.SetUserString("Frame", tilePatterns.Frame.ToString());
                InsObj.SetUserString("ColourFromObject", tilePatterns.ColourFromObject.ToString());

                _blockInstances.Add( InsObj );

                return true;
            }
            else
            {
                throw new Exception($"Block with name '{Name}' already exists.");
            }
        }
        internal static bool RemoveHatInstance(string Name)
        {
            if(!_blockInstances.Contains(Name)) return false;
            else
            {
                _blockInstances.Remove(Name);
                return true;
            }
        }
        
    }
}
