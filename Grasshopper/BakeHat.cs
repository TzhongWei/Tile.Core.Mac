
using Grasshopper.Kernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Core.Util;

namespace Tile.Core.Grasshopper
{
    public class BakeHat : GH_Component
    {
        public override Guid ComponentGuid => new Guid("{02442E70-18F5-4E7E-A1D0-D50041489D13}");
        //protected override Bitmap Icon => base.Icon;
        protected override Bitmap Icon => IconLoader.Pattern_patch_5;
        public BakeHat() : base("BakeHat", "bake", "Bake the hat instance into rhino environment", "Einstein", "Einstein") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Bake", "B", "Bake the hats", GH_ParamAccess.item);
            pManager.AddParameter(new GH_TileInstance(), "EinsteinInstanceTiles", "ETs", "The hats need to be bakes", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GUID", "GUID", "The Guid of the instance reference in rhino", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var ListBI = new List<BlockInstance>();
            var run = false;
            DA.GetData("Bake", ref run);
            DA.GetDataList("EinsteinInstanceTiles", ListBI);
            var Guids = new List<Guid>();
            if (run)
                Guids = ListBI.Select(x => x.Bake()).ToList();
            DA.SetDataList("GUID", Guids);
        }
    }
}
