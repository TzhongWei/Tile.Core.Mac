using Grasshopper;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Geometry;
using Tile.Core.Patch;
using Grasshopper.Kernel;
using System;
using GH_IO.Types;

namespace Tile.Core.Grasshopper
{
    public class ParametricFormSurfaceUV : GH_Component
    {
        public ParametricFormSurfaceUV() : base("ParametricFormSurfaceUV", "ParamSrfUV",
        "Using different UVdomains for parametric form to define a surface", "Einstein", "Patch")
        { }

        public override Guid ComponentGuid => new Guid("4b88c2b0-4a15-4933-a68a-7844f5ef1626");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddInterval2DParameter("uvDomain2D", "UVDM2D", "uv domain in two dimension", GH_ParamAccess.item);
            pManager.AddGenericParameter("Expression", "E", "the expression r(u,v)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Accuracy", "acc", "the accuracy of the functional patch", GH_ParamAccess.item, 100);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "The surface output", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "The points generate from the expression", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var InUV = new GH_Interval2D();
            DA.GetData("uvDomain2D", ref InUV);

            PatchFunction Patch = null;
            DA.GetData("Expression", ref Patch);
            int acc = 100;
            DA.GetData("Accuracy", ref acc);
            acc = acc <= 0 ? 100 : acc;
            var InU = new Interval(InUV.u.a, InUV.u.b);
            var InV = new Interval(InUV.v.a, InUV.v.b);

            var Srf = new SurfacePatch(InU, InV, Patch, acc);

            if (!Srf.Run())
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Patch failed");
                return;
            }
            DA.SetData(0, Srf.ResultSurface);
            DA.SetData(1, Srf.ResultPts);
        }
    }
}