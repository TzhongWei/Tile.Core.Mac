using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Tile.LSystem;
using Tile.LSystem.TokenAction;

namespace Tile.Core.Grasshopper
{
    public class Turtle : GH_Component
    {
        public Turtle() : base("Turtle Graphic", "Turtle",
        "This component functions as turtle graphic can draw an image based on the L-system setting", "Einstein", "L-System")
        { }
        public override Guid ComponentGuid => new Guid("d8824492-c8a8-42ec-8ce5-207d79f7a9cb");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("L-System", "LS", "The Lsystem", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Location", "P", "A plane to place the drawings", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("ActionBases", "ABs", "These inputs define the turtle's actions", GH_ParamAccess.list);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle", "T", "The turtle object draws the geometries in Rhino viewpoints", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Drawing", "Geoms", "The geometry outputs", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var LSystem = RuleExecuter.Unset;
            DA.GetData("L-System", ref LSystem);
            var PL = Rhino.Geometry.Plane.WorldXY;
            var ABs = new List<ActionBase>();
            DA.GetData("Location", ref PL);
            DA.GetDataList("ActionBases", ABs);

            if (LSystem == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The L-System input is required but was not provided.");
                return;
            }

            var _TurtleGraphic = new TurtleGraphic(LSystem, PL);
            foreach (var AB in ABs)
            {
                if (AB != null) continue;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"The input ActionBases '{AB}' is not a valid ActionBase object. Ensure all inputs implement ActionBase.");
                return;
            }

            if (!ABs.Contains(new PopAction())) ABs.Add(new PopAction());
            if (!ABs.Contains(new PushAction())) ABs.Add(new PushAction());

            if (ABs.Count < LSystem.ComputeToken().Count)
                return;

            _TurtleGraphic.AddAction(ABs);

            if (!_TurtleGraphic.Run())
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, _TurtleGraphic.cmd);
                return;
            }
            DA.SetData("Turtle", _TurtleGraphic);
            DA.SetDataList("Drawing", _TurtleGraphic.GetGeometries);
        }
    }
}