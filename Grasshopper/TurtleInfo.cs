using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Tile.LSystem;

namespace Tile.Core.Grasshopper
{
    public class TurtleInfo : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public TurtleInfo() : base("TurtleInfo", "TInfo", "Get the information in this turtle", "Einstein", "L-System") { }
        public override Guid ComponentGuid => new Guid("b88880dd-fead-4089-b38f-ca86c17f8e7f");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle", "T", "The turtle object draws the geometries in Rhino viewpoints", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pointer", "P", "The pointer in the turtle object", GH_ParamAccess.item);
            pManager.AddTransformParameter("Action History", "ATs", "The action history", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Action Description", "ADs", "The action descriptions", GH_ParamAccess.tree);
            pManager.AddTextParameter("cmd", "cmd", "The execution message", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            TurtleGraphic Turtle = null;
            DA.GetData("Turtle", ref Turtle);

            var Pointer = Turtle.GetPointer;

            DA.SetData("Pointer", Pointer);
            var TSDataTree = new DataTree<Transform>();
            foreach (var TS in Pointer.ActionHistory)
            {
                TSDataTree.Add(TS.Value, new GH_Path(TS.Key));
            }
            var TADatatree = new DataTree<object>();
            foreach (var TD in Pointer.History)
            {
                TADatatree.AddRange(new List<object> { TD.Value.Item1, TD.Value.Item2 }, new GH_Path(TD.Key));
            }
            DA.SetDataTree(1, TSDataTree);
            DA.SetDataTree(2, TADatatree);
            DA.SetData("cmd", Turtle.cmd);
        }
    }
}