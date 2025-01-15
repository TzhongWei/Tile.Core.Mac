using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Tile.Core.Util;
using System.Threading;


namespace Tile.Core.Grasshopper
{
    public class EinsteinPermuteComponent : GH_Component
    {
        public EinsteinPermuteComponent() : base("EinsteinPatternPatch",
            "EinPatch", "This component is to match the pattern based on einstein core",
            "Einstein", "Einstein")
        { }
        public override Guid ComponentGuid => new Guid("A5B4441C-C292-425E-BBC9-3E1E88748350");
        protected override Bitmap Icon => IconLoader.Pattern_patch_2;
        //protected override Bitmap Icon => base.Icon;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Einstein", "EinCore", "The einstein class provides the positions and data for pattern arrangement", GH_ParamAccess.item);
            pManager.AddPointParameter("OriginPoint", "Pt", "The origin point of the pattern", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddBooleanParameter("PlaceBlock", "Run", "Set the hats into blocks in Rhino Environment", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "The Scale setting of tiles", GH_ParamAccess.item, 1);
            pManager.AddParameter(new GH_TileInstance(), "HatTileInstance", "ET", "The Einstein Hat tile instance block", GH_ParamAccess.list);
            pManager[4].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("HatTileInstances", "ETs", "The tile figures", GH_ParamAccess.list);
            pManager.AddTransformParameter("Transformation", "TS", "The permutation transformation of the hats", GH_ParamAccess.list);
        }
        private List<BlockInstance> EinTiles = new List<BlockInstance>();
        private List<Transform> History = new List<Transform>();
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double _Scale = double.NaN;
            Point3d _OriPt = Point3d.Origin;
            Einstein Ein = new Einstein();
            var Run = false;
            var PatternList = new List<BlockInstance>();

            DA.GetData("Einstein", ref Ein);
            DA.GetData("OriginPoint", ref _OriPt);
            DA.GetData("PlaceBlock", ref Run);
            DA.GetData("Scale", ref _Scale);
            DA.GetDataList("HatTileInstance", PatternList);

            var ResizeEin = new Einstein_Resize(_Scale, _OriPt);
            ResizeEin.SetTile = Ein;

            ResizeEin.SetPermutedBlock(PatternList);

            if (Run)
            {
                ResizeEin.PlaceBlock(Ein, out EinTiles, out History);
            }
           

            DA.SetDataList("HatTileInstances", EinTiles);
            DA.SetDataList("Transformation", History);
        }
    }
}
