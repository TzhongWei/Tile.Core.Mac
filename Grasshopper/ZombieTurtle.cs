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
    public class ZombieTurtle : GH_Component
    {
        public ZombieTurtle() : base("ZombieTurtle", "ZTurtle", "The turtle makes the drawing step by step",
         "Einstein", "L-System")
        { }

        public override Guid ComponentGuid => new Guid("8e531bed-5a3d-43a6-91ea-5987e7611f5a");
        int IterationCount = 0;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Zombie turtle start running", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "Zombie turtle resets", GH_ParamAccess.item);
            pManager.AddGenericParameter("L-System", "LS", "The Lsystem", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Location", "P", "A plane to place the drawings", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddGenericParameter("ActionBases", "ABs", "These inputs define the turtle's actions", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle", "T", "The turtle object draws the geometries in Rhino viewpoints", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Drawing", "Geoms", "The geometry outputs", GH_ParamAccess.list);
        }
        private TurtleGraphic _TurtleGraphic = null;
        private List<GeometryBase> GeomBags = new List<GeometryBase>();
        private bool JustReset = false;
        private bool StartRunning = false;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Run = false, Reset = false;
            DA.GetData("Run", ref Run);
            DA.GetData("Reset", ref Reset);

            var LSystem = RuleExecuter.Unset;
            DA.GetData(2, ref LSystem);
            var PL = Rhino.Geometry.Plane.WorldXY;
            var ABs = new List<ActionBase>();
            DA.GetData("Location", ref PL);
            DA.GetDataList("ActionBases", ABs);

            if (LSystem == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The L-System input is required but was not provided.");
                return;
            }

            if (Run)
            {
                this.StartRunning = true;
            }

            if (Reset || _TurtleGraphic == null)
            {
                _TurtleGraphic = new TurtleGraphic(LSystem, PL);
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
                this.IterationCount = 0;
                GeomBags = new List<GeometryBase>();
                this.JustReset = true;
                this.StartRunning = false;
                goto Conclusion;
            }

            if (!JustReset && StartRunning)
            {
                if (_TurtleGraphic.RunStepByStep(ref IterationCount))
                {
                    IterationCount++;
                }
            }

            JustReset = false;

            if (!Reset && IterationCount < _TurtleGraphic.ExecuteTokens.Count)
            {
                ExpireSolution(true);
            }


        Conclusion:

            DA.SetDataList("Drawing", _TurtleGraphic.GetGeometries);
            DA.SetData("Turtle", _TurtleGraphic);
        }
    }
}