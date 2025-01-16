
using Grasshopper.GUI.SettingsControls;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Core.Util;

namespace Tile.Core.Grasshopper
{
    public class GH_TileInstance : GH_Param<BlockInstance>, IGH_PreviewObject
    {
        public GH_TileInstance() : base("HatTileInstance", "ET", "Represents a reference to a Einstein block instance",
          "Einstein", "Einstein", GH_ParamAccess.item)
        { }
        public override Guid ComponentGuid => new Guid("16A048BE-9077-4314-8855-7B424F54C261");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override BlockInstance PreferredCast(object data) => data is BlockInstance cast ? (BlockInstance)cast.DuplicateGeometry() : null;
        public bool Hidden { get; set; } = false;

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => Preview_ComputeClippingBox();


        protected override Bitmap Icon => IconLoader.Einstein_Outline;
        //protected override Bitmap Icon => base.Icon;
        public void DrawViewportWires(IGH_PreviewArgs args) => Preview_DrawMeshes(args);
        public void DrawViewportMeshes(IGH_PreviewArgs args) => Preview_DrawMeshes(args);
    }
}
