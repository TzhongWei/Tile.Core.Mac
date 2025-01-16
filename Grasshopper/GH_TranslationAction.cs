using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class GH_TranslationAction : GH_Component
    {
        public GH_TranslationAction() : base("TranslationByVector", "MoveAC", "The translation is defined a vector", "Einstein", "L-System") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("4461426d-0f8e-490b-b613-0713513da81e");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TokenName", "T", "the token name for this action.", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "D", "the description of this action", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "V", "The Vector of this translation", GH_ParamAccess.item, Vector3d.ZAxis);
            pManager.AddBooleanParameter("IsSketch", "Sk", "If true, this turtle will make a sketch line from the previous to the next transformation", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rotate", "AB", "this is an action setting", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Name = "", Description = "";
            Vector3d Vc = Vector3d.ZAxis;
            bool sketch = false;
            DA.GetData("TokenName", ref Name);
            DA.GetData("Description", ref Description);
            DA.GetData("Vector", ref Vc);
            DA.GetData("IsSketch", ref sketch);

            if (Description.Contains("GENERATEDES"))
            {
                var Change = sketch ? "Need" : "No";
                Description = Description.Split('_')[0] + $"Translation {Vc.X},{Vc.Y},{Vc.Z} , {Change} Sketch";
            }

            DA.SetData("Rotate", new TranslationAction(Name, Description, Vc, sketch));
        }
    }
}