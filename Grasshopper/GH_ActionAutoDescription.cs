using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class AutoDescription : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public AutoDescription() : base("AutoDescription", "AuDe",
        "This Component links can generate the default description for the actionbased automatically", "Einstein", "L-System")
        { }
        protected override Bitmap Icon => IconLoader.AutoDescription;
        public override Guid ComponentGuid => new Guid("3db14976-ec2c-4473-a3be-5b9df1bbeff0");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Prefix", "Pre", "Any prefix notations", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Description", "De", "Generate a default Description to an actionbase. Please link to the input of desciption", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Prefix = "";
            DA.GetData(0, ref Prefix);
            DA.SetData(0, $"{Prefix}_GENERATEDES");
        }
    }
}
