
using Grasshopper.GUI.Widgets;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Core.Util;

namespace Tile.Core.Grasshopper
{
    public class HatGroup : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public HatGroup() : base("HatGroup", "HGroup", "This component is to sort out the input hat with desired labels and send as a group", "Einstein", "Einstein") { }
        public override Guid ComponentGuid => new Guid("{D3944476-5B1D-4B9A-9BB2-BCC21241E7E3}");
        protected override Bitmap Icon => IconLoader.group;
        //protected override Bitmap Icon => base.Icon;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "Hat_H", "H", "The desired H tile type", GH_ParamAccess.item);
            pManager.AddParameter(new GH_TileInstance(), "Hat_H1", "H1", "The desired H1 tile type", GH_ParamAccess.item);
            pManager.AddParameter(new GH_TileInstance(), "Hat_T", "T", "The desired T tile type", GH_ParamAccess.item);
            pManager.AddParameter(new GH_TileInstance(), "Hat_P", "P", "The desired P tile type", GH_ParamAccess.item);
            pManager.AddParameter(new GH_TileInstance(), "Hat_F", "F", "The desired F tile type", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "EinsteinHatInstances", "ETs", "The group of Einstein Hat instances", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            HatGroup<BlockInstance> hatGroup = new HatGroup<BlockInstance>();
            BlockInstance H = null, H1 = null, T = null, P = null, F = null;
            DA.GetData(0, ref H);
            DA.GetData(1, ref H1);
            DA.GetData(2, ref T);
            DA.GetData(3, ref P);
            DA.GetData(4, ref F);

            hatGroup[0] = H.BlockLabel != Label.H ?
                (BlockInstance)H.ChangeLabel(Label.H).DuplicateGeometry() : (BlockInstance)H.DuplicateGeometry();
            hatGroup[1] = H1.BlockLabel != Label.H1 ?
                (BlockInstance)H1.ChangeLabel(Label.H1).DuplicateGeometry() : (BlockInstance)H1.DuplicateGeometry();
            hatGroup[2] = T.BlockLabel != Label.T ?
                (BlockInstance)T.ChangeLabel(Label.T).DuplicateGeometry() : (BlockInstance)T.DuplicateGeometry();
            hatGroup[3] = P.BlockLabel != Label.P ?
                (BlockInstance)P.ChangeLabel(Label.P).DuplicateGeometry() : (BlockInstance)P.DuplicateGeometry();
            hatGroup[4] = F.BlockLabel != Label.F ?
                (BlockInstance)F.ChangeLabel(Label.F).DuplicateGeometry() : (BlockInstance)F.DuplicateGeometry();

            DA.SetDataList("EinsteinHatInstances", hatGroup.ToList());
        }
    }
}
