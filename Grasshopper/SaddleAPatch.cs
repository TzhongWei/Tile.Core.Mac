using System;
using Grasshopper.Kernel;
using Tile.Core.Patch;

namespace Tile.Core.Grasshopper
{
    public class SaddleAPatch : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public SaddleAPatch() : base("SaddleAPatch", "Saddle", "Saddle surface in anticlastic function",
         "Einstein", "Patch")
        { }
        public override Guid ComponentGuid => new Guid("5f8782f3-bd10-4133-9ca7-74f7bf868dea");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Parameter_a", "a", "a float number constant a", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Parameter_m", "m", "a float number constant m", GH_ParamAccess.item, 1);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SaddleAPatch", "E", "An anticlastic saddle Surface patch expression", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a = 1.0, m = 1.0;
            DA.GetData("Parameter_a", ref a);
            DA.GetData("Parameter_m", ref m);

            if (m == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "m cannot be 0");
                return;
            }
            var SadSrf = new SaddleA(a, m);

            DA.SetData(0, SadSrf);
        }
    }
}