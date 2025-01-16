using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_EmptyAction : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public GH_EmptyAction() : base("EmptyAction", "Empty", "Set an empty action when execute", "Einstein", "L-System")
        {
        }

        public override Guid ComponentGuid => new Guid("59a17ea2-a5b9-4e7e-bbbf-8f3ad1699751");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item, "X");
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item, "Do nothing");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EmptyAction", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";

            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.SetData("EmptyAction", new EmptyAction(Name, Description));
        }
    }
}