using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_TurnRight : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public GH_TurnRight() : base("TurnRight", "+", "The L-system common operator +", "Einstein", "L-System") { }

        public override Guid ComponentGuid => new Guid("72bddadc-f67b-4e79-a843-9387a8aa3ccf");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item, "+");
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item, "The drawing turn right after this action");
            pManager.AddNumberParameter("Degree", "De", "the length of each step", GH_ParamAccess.item, 10);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("TurnRightAction", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "+", Description = "";
            double Degree = 50;

            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Degree", ref Degree);

            if (Description.Contains("GENERATEDES"))
            {
                Description = Description.Split('_')[0] + $"The drawing turn right {Degree} degree after this action";
            }

            var Right = new RotateActionRight(Name, Description, Degree);
            DA.SetData("TurnRightAction", Right);

        }
        protected override Bitmap Icon => IconLoader.TurnRight;
    }
}