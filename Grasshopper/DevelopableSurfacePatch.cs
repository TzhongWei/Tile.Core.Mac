using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Tile.Core.Patch;

namespace Tile.Core.Grasshopper
{
    public class DevelopableSurfacePatch : GH_Component
    {
        public DevelopableSurfacePatch() : base("DevelopableSurfacePatch", "DSrf",
         "Deveopable surface example function", "Einstein", "Patch")
        { }
        public override Guid ComponentGuid => new Guid("0a241b90-b466-485a-94df-e21b24e839fa");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Parameter_a", "a", "a float number constant a", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Parameter_b", "b", "a float number constant b", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Parameter_m", "m", "a float number and non-zero constant m", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DevelopableSurfacePatch", "E", "Developable Surface patch expression", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a = 1.0, b = 1.0, m = 1.0;
            DA.GetData("Parameter_a", ref a);
            DA.GetData("Parameter_b", ref b);
            DA.GetData("Parameter_m", ref m);

            if (m == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "m cannot be 0");
                return;
            }

            var DSrf = new DevelopableSurface(a, b, m);

            DA.SetData("DevelopableSurfacePatch", DSrf);
        }
    }
}