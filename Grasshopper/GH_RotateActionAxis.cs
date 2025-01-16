using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_RotateActionAxis : GH_Component
    {
        public GH_RotateActionAxis() : base("RotateActionByAxis", "RotateAxis", "The rotation is defined an axis, point and angle", "Einstein", "L-System") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("02d1f222-1968-4782-972f-c4a96d02b95e");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddAngleParameter("Angle", "An", "Angle of this rotation", GH_ParamAccess.item, 0.5 * Math.PI);
            pManager.AddPointParameter("Point", "Pt", "Set a point for the rotation", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddVectorParameter("Axis", "Ax", "Set a Vector for the rotation", GH_ParamAccess.item, Vector3d.ZAxis);
            pManager.AddBooleanParameter("IsSketch", "Sk", "If true, this turtle will make a sketch line from the previous to the next transformation", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rotate", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";
            double Angle = 0.5;
            var Ax = Vector3d.ZAxis;
            var Pt = Point3d.Origin;
            bool sketch = false;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Angle", ref Angle);
            DA.GetData("Axis", ref Ax);
            DA.GetData("Point", ref Pt);
            DA.GetData("IsSketch", ref sketch);

            if (Description.Contains("GENERATEDES"))
            {
                var Change = sketch ? "Need" : "No";
                Description = Description.Split('_')[0] + $"Rotate Angle {Angle} by an axis {Ax} and a point {Pt}, {Change} Sketch";
            }

            DA.SetData("Rotate", new RotationAction(Name, Description, Angle, Pt, Ax, sketch));
        }
    }
}