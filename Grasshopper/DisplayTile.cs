using Grasshopper.Kernel;
using System;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tile.Core.Util;
using System.Numerics;
using Rhino;
using Rhino.UI.Theme;


namespace Tile.Core.Grasshopper
{
    public class DisplayTile : GH_Component
    {
        public DisplayTile():base("DisplayEinsteinTile", "DisEin",
            "Display the einstein block from the block definiton", 
            "Einstein", "Einstein")
        { }
        public override Guid ComponentGuid => new Guid("FB10CCF6-BEB5-4673-9609-7C90EBAD9D24");
        protected override Bitmap Icon => IconLoader.Einstein_core_4;
        //protected override Bitmap Icon => base.Icon;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TileName", "N", "The name of the hat tile, the RhinoInstanceObject Name", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "The location to place the block", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "TileInstance", "ET", "The tile instance block", GH_ParamAccess.item);
            pManager.AddGenericParameter("TileLabel", "Label", "The Label of the tile", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Patterns", "P", "The pattern setting of the tile", GH_ParamAccess.list);
            pManager.AddColourParameter("PatternsColour", "PC", "The Colour setting of the tile", GH_ParamAccess.list);
            pManager.AddTextParameter("Information", "Info", "the information about the tile", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Name = string.Empty;
            var PL = Rhino.Geometry.Plane.WorldXY;
            DA.GetData("TileName", ref Name);
            DA.GetData("Plane", ref PL);
            var TS = Transform.PlaneToPlane(Rhino.Geometry.Plane.WorldXY, PL);
            BlockInstance Tile = BlockInstance.Unset; 

            Tile = (BlockInstance) Name;
                
            if (Tile == null) throw new Exception($"The block {Name} isn't defined in this block instances.");
            var TileCopy = (BlockInstance)Tile.DuplicateGeometry();
            TileCopy.Transform(TS);

            var Colours = TileCopy.tilePatterns.ColourFromObject ? 
                TileCopy.tilePatterns.PatternAtts.Select(x => x.ObjectColor) : 
                TileCopy.tilePatterns.PatternAtts.Select(x => RhinoDoc.ActiveDoc.Layers.
                FindIndex(x.LayerIndex).Color);

            DA.SetData("TileLabel", TileCopy.BlockLabel);
            DA.SetData("TileInstance", TileCopy);
            DA.SetDataList("Patterns", TileCopy.tilePatterns.Patterns);
            DA.SetDataList("PatternsColour", Colours);
            DA.SetData("Information", TileCopy.ToJson());
        }
    }
}
