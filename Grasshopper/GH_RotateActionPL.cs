using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_RotateActionPL : GH_Component
    {
        public GH_RotateActionPL() : base("RotateActionByPlane", "RotatePL", "The rotation is defined a plane and angle", "Einstein", "L-System") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("7cdd27ec-5927-4cde-89aa-d3679642d478");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddAngleParameter("Angle", "A", "Angle of this rotation", GH_ParamAccess.item, 0.5 * Math.PI);
            pManager.AddPlaneParameter("Plane", "P", "Set a plane for the rotation", GH_ParamAccess.item, Plane.WorldYZ);
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
            var PL = Plane.WorldXY;
            var IsSketch = false;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Angle", ref Angle);
            DA.GetData("Plane", ref PL);
            DA.GetData("IsSketch", ref IsSketch);

            if (Description.Contains("GENERATEDES"))
            {
                var Change = IsSketch ? "Need" : "No";
                Description = Description.Split('_')[0] + $"Rotate Angle {Angle} by a plane {PL}, {Change} Sketch";
            }

            DA.SetData("Rotate", new RotationAction(Name, Description, Angle, PL.Origin, PL.ZAxis, IsSketch));
        }
    }
}