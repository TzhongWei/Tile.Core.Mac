using Grasshopper;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Rhino;
using Rhino.Geometry;
using Tile.Core.Patch;
using Grasshopper.Kernel;
using System;
using GH_IO.Types;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace Tile.Core.Grasshopper
{
    public class ParametricFormSurfaceUV : GH_Component
    {
        private Surface _previewSurface = null;
        public ParametricFormSurfaceUV() : base("ParametricFormSurfaceUV", "ParamSrfUV",
        "Using different UVdomains for parametric form to define a surface", "Einstein", "Patch")
        { }

        public override Guid ComponentGuid => new Guid("4b88c2b0-4a15-4933-a68a-7844f5ef1626");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddInterval2DParameter("uvDomain2D", "uv2D", "uv domain in two dimension", GH_ParamAccess.item);
            pManager.AddGenericParameter("Expression", "E", "the expression r(u,v)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Accuracy", "acc", "the accuracy of the functional patch", GH_ParamAccess.item, 100);
              pManager.AddTransformParameter("Transformation", "X", "Transform the surface", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "The surface output", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "The points generate from the expression", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //var InUV = new GH_Interval2D();
            var InUV = new UVInterval();
            DA.GetData("uvDomain2D", ref InUV);

            PatchFunction Patch = null;
            DA.GetData("Expression", ref Patch);
            int acc = 100;
            DA.GetData("Accuracy", ref acc);
            acc = acc <= 0 ? 100 : acc;
            Transform X = Transform.Identity;
            DA.GetData("Transformation", ref X);

            var Srf = new SurfacePatch(InUV.U, InUV.V, Patch, acc);

            if (!Srf.Run())
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Patch failed");
                this._previewSurface = null;
                return;
            }
            Srf.Transform(X, out this._previewSurface, out var Pts);

            this._previewSurface = Srf.ResultSurface;
            DA.SetData(0, Srf.ResultSurface);
            DA.SetDataTree(1, Srf.ResultPts);
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if(this._previewSurface == null) return;

            var Colour = this.Attributes.Selected ? Color.FromArgb(150, 0, 255, 0)  : Color.FromArgb(150, 255, 0, 0);  
            args.Display.DrawSurface(_previewSurface, Colour, 1);
            var Meshised = Mesh.CreateFromSurface(this._previewSurface);
            if(Meshised != null) args.Display.DrawMeshShaded(Meshised, new Rhino.Display.DisplayMaterial(Colour));
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if(this._previewSurface == null) return;

            var Colour = this.Attributes.Selected ? Color.FromArgb(150, 0, 255, 0)  : Color.FromArgb(150, 255, 0, 0);  

            args.Display.DrawSurface(_previewSurface, Colour, 1);
        }
    }
}