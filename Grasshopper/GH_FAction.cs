using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_FAction : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public GH_FAction() : base("LineDrawing", "LineAC",
        "Turtle draw a straight line after receiving this component. The default token is F",
        "Einstein", "L-System")
        { }
        public override Guid ComponentGuid => new Guid("960e83c5-e16c-4a5c-b936-890d12c2e440");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item, "F");
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item, "Go stright line");
            pManager.AddNumberParameter("Length", "L", "the length of each step", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Rescale", "S", "the rescale for each pop and push action", GH_ParamAccess.item, 0.5);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("FAction", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";
            double Length = 1, Rescale = 0.5;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Length", ref Length);
            DA.GetData("Rescale", ref Rescale);

            if (Description.Contains("GENERATEDES"))
            {
                Description = Description.Split('_')[0] + $"The a straight line {Length} unit(s) after this action";
            }

            var FAC = new FAction(Name, Description, Length, Rescale);
            DA.SetData("FAction", FAC);
        }
        protected override Bitmap Icon => IconLoader.FAction;
    }
}