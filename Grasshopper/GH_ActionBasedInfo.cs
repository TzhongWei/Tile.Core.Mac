using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_ActionBasedInfo : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public GH_ActionBasedInfo() : base("ActionBased Information", "ABInfo", "The information of the input action", "Einstein", "L-System") { }
        protected override Bitmap Icon => IconLoader.ActionInfo;
        public override Guid ComponentGuid => new Guid("ca92e160-a135-4497-9000-d97ce5e809e9");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ActionBase", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddTransformParameter("Transformation", "TS", "Transformation action", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ActionBase AB = null;
            DA.GetData("ActionBase", ref AB);

            if (AB == null)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"The input ActionBases '{AB}' is not a valid ActionBase object. Ensure all inputs implement ActionBase.");

            DA.SetData("TokenName", AB.Name);
            DA.SetData("Description", AB.Description);
            DA.SetData("Transformation", AB.ActionTransform);

        }
    }
}