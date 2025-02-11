using Grasshopper.Kernel;
using System;
using Tile.Core.Patch;

namespace Tile.Core.Grasshopper
{
    public class PlanarSrfPatch : GH_Component
    {
        public PlanarSrfPatch() : base("PlanarSurfacePatch", "PSrf", "Create a rectangular planar surface patch", "Einstein", "Patch") { }

        public override Guid ComponentGuid => new Guid("a013b920-3128-47a4-b6af-b36adcafd8de");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("USize", "u", "the size at u direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("VSize", "v", "the size at v direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "h", "the level of the surface", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PlanarPatch", "E", "Planar Surface patch expression", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double u = 1, v = 1, h = 1;
            DA.GetData(0, ref u);
            DA.GetData(1, ref v);
            DA.GetData(2, ref h);
            u = u == 0 ? 1 : u;
            v = v == 0 ? 1 : v;

            var Function = new PlanarSurface(u, v, h);
            DA.SetData(0, Function);
        }
    }
}