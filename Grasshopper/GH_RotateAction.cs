using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_RotateAction : GH_Component
    {
        public GH_RotateAction() : base("RotateActionByPoint", "RotatePt", "The rotation is defined a central point and angle", "Einstein", "L-System") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("da1fcd62-9ebb-49e8-bbd7-a887f4efcb55");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddAngleParameter("Angle", "A", "Angle of this rotation", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "Pt", "Set a point for the rotation", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddBooleanParameter("IsSketch", "Sk", "If true, this turtle will make a sketch line from the previous to the next transformation", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rotate", "AB", "this is an action setting", GH_ParamAccess.item);
        }
        protected override Bitmap Icon => IconLoader.Rotate_1;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";
            double Angle = 0.5;
            Point3d Pt = Point3d.Origin;
            bool sketch = false;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Angle", ref Angle);
            DA.GetData("Point", ref Pt);
            DA.GetData("IsSketch", ref sketch);

            if (Description.Contains("GENERATEDES"))
            {
                var Change = sketch ? "Need" : "No";
                Description = Description.Split('_')[0] + $"Rotate Angle {Angle} by a point {Pt}, {Change} Sketch";
            }


            DA.SetData("Rotate", new RotationAction(Name, Description, Angle, Pt, sketch));
        }
    }
}