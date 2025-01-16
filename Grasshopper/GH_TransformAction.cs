using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_TransformAction : GH_Component
    {
        public GH_TransformAction() : base("TransformAction", "TS", "Customising a Transformation Action", "Einstein", "L-System") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("d178eb26-9d08-4f03-a15c-052dd5a2cc90");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddTransformParameter("Transform", "TS", "Transformation matrix of this Action", GH_ParamAccess.item);
            pManager.AddBooleanParameter("IsSketch", "Sk", "If true, this turtle will make a sketch line from the previous to the next transformation", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Transform", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";
            Transform TS = Transform.Identity;
            bool sketch = false;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Transform", ref TS);
            DA.GetData("IsSketch", ref sketch);

            if (Description.Contains("GENERATEDES"))
            {
                var Change = sketch ? "Need" : "No";
                Description = Description.Split('_')[0] + $"Transformation, {Change} Sketch";
            }


            DA.SetData("Rotate", new TransformAction(Name, Description, TS, sketch));
        }
    }
}