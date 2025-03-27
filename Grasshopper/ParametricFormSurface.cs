using Grasshopper;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Geometry;
using Tile.Core.Patch;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Tile.Core.Grasshopper
{
    public class ParametricFormSurface : GH_Component
    {
        private Surface _previewSurface = null;
        public ParametricFormSurface() : base("ParametricFormSurface", "ParamSrf",
        "Using parametric form to define a surface", "Einstein", "Patch")
        { }

        public override Guid ComponentGuid => new Guid("ab44ff02-4aea-4b5f-a085-73b0d8b60e7c");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("uvDomain", "uv", "UVDomain", GH_ParamAccess.item, new Interval(-1, 1));
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
            var InV = new Interval(0, 1);
            DA.GetData("uvDomain", ref InV);
            PatchFunction Patch = null;
            DA.GetData("Expression", ref Patch);
            int acc = 100;
            DA.GetData("Accuracy", ref acc);
            acc = acc <= 0 ? 100 : acc;
            Transform X = Transform.Identity;
            DA.GetData("Transformation", ref X);
            var Srf = new SurfacePatch(InV, Patch, acc);
            
            
            if (!Srf.Run())
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Patch failed");
                this._previewSurface = null;
                return;
            }
            Srf.Transform(X, out this._previewSurface, out var Pts);
            

            DA.SetData(0, this._previewSurface);
            DA.SetDataTree(1, Pts);
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