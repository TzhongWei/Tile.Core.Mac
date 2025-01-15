
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
    public class InstanceManager : GH_Component
    {
        public override Guid ComponentGuid => new Guid("{1E8AA468-DE1A-4268-B354-4C4587C7DAC1}");
        public InstanceManager():base("HatTileManager", "Manager", "This component can list all hat tile defined in rhino environment. " +
            "And it can also be used to delete the tiles", "Einstein", "Einstein") { }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("RemoveTiles", "RTiles", "Input the tiles' name which will be removed in both rhino and this manager database", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }
        protected override Bitmap Icon => IconLoader.EinsteinInfo;
        //protected override Bitmap Icon => base.Icon;
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(),
                "EinsteinTiles", "ETs", "All the Einstein tile instance", GH_ParamAccess.list);
        }
        List<string> RemoveList = new List<string>();
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetDataList("RemoveTiles", RemoveList);


            DA.SetDataList("EinsteinTiles", Tile.Core.Util.HatTileDoc.BlockInstances);
        }

        //Button
        public override void CreateAttributes()
        {
            m_attributes = new CustomUI.ButtonUIAttributes(this, "Update", FunctionToRunOnClick, "Update Block Information");
        }
        public void FunctionToRunOnClick()
        {
            if(RemoveList.Count > 0)
            {
                var DoList = RemoveList.Where(x =>
                HatTileDoc.BlockInstances.Contains(x)
                ).ToList();
                for (int i = 0; i < DoList.Count; i++)
                    HatTileDoc.BlockInstances.Remove(DoList[i]);
                RemoveList.Clear();
            }
            this.ExpireSolution(true);
        }
    }
}
