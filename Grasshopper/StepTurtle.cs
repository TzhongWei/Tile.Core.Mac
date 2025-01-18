using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tile.LSystem;
using Tile.LSystem.Display;
using Tile.LSystem.TokenAction;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Reflection;

namespace Tile.Core.Grasshopper
{
    public class StepTurtle : GH_Component
    {
        public StepTurtle() : base("StepTurtle", "SpTurtle",
        "Display a turtle which draws your L-system with transformation matrix information", "Einstein", "L-System")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public override Guid ComponentGuid => new Guid("873383ee-844a-4ec4-b130-31998d0c55a1");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Steps", "S", "The step that turtle moves", GH_ParamAccess.item, 0);
            pManager.AddGenericParameter("L-System", "LS", "The Lsystem", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Location", "P", "A plane to place the drawings", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddGenericParameter("ActionBases", "ABs", "These inputs define the turtle's actions", GH_ParamAccess.list);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle2DInfo", "TurInfo", "The turtle drawing information", GH_ParamAccess.item);
            pManager.AddTextParameter("ExecutedToken", "Token", "The current token that is executed", GH_ParamAccess.item);
            pManager.AddGenericParameter("Turtle", "T", "The turtle object draws the geometries in Rhino viewpoints", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Drawing", "Geoms", "The geometry outputs", GH_ParamAccess.tree);
        }
        RuleExecuter _lSystem = RuleExecuter.Unset;
        Plane _PL = Plane.WorldXY;
        TurtleGraphic _graphic = null;
        //Turtle2D _turtle2d = new Turtle2D();
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Step = 0;
            RuleExecuter lSystem = RuleExecuter.Unset;
            var ABs = new List<ActionBase>();
            var PL = Rhino.Geometry.Plane.WorldXY;
            DataTree<GeometryBase> DisplayGeometry = new DataTree<GeometryBase>();
            DA.GetData("Steps", ref Step);
            DA.GetData("L-System", ref lSystem);
            DA.GetData("Location", ref PL);
            DA.GetDataList("ActionBases", ABs);
            
            Step--;

            if (lSystem == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The L-System input is required but was not provided.");
                return;
            }
            foreach (var AB in ABs)
            {
                if (AB != null) continue;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"The input ActionBases '{AB}' is not a valid ActionBase object. Ensure all inputs implement ActionBase.");
                return;
            }
            
            if(this._graphic == null || !this._lSystem.Equals(lSystem) || PL != _PL)
            {
                this._graphic = new TurtleGraphic(lSystem, PL);
                this._lSystem = lSystem.Clone();
                //_turtle2d = new Turtle2D();
                _PL = PL;
                if (!ABs.Contains(new PopAction())) ABs.Add(new PopAction());
                if (!ABs.Contains(new PushAction())) ABs.Add(new PushAction());

                if (ABs.Count < lSystem.ComputeToken().Count)
                    return;

                _graphic.AddAction(ABs);

                if (!_graphic.Run())
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, _graphic.cmd);
                    return;
                }
            }
            
            var Pointer = _graphic.GetPointer;
            var TS = Transform.PlaneToPlane(Plane.WorldXY, PL);
            string Token = "Non";
            var _turtle2d = new Turtle2D();
            _turtle2d.TurtleTransform(TS);
            if(Step < 0) 
            {
                goto Conclusion;
            }
            
            if (Step > _graphic.ExecuteTokens.Count)
            {
                this.AddRuntimeMessage( GH_RuntimeMessageLevel.Warning, "The Step need to smaller than the number of token");
                for(int i = 0; i < _graphic.ExecuteTokens.Count; i ++)
            {
                DisplayGeometry.AddRange(Pointer.Drawing[i], new GH_Path(i));
            }
                goto Conclusion;
            }
            
            Token = _graphic.ExecuteTokens[Step];
            
            for(int i = 0; i <= Step; i ++)
            {
                if(Pointer.Drawing.ContainsKey(i))
                    DisplayGeometry.AddRange(Pointer.Drawing[i], new GH_Path(i));
                
                TS = Pointer.ActionHistory[i];
                _turtle2d.TurtleTransform(TS);
            }
            

            Conclusion:
            DA.SetData("Turtle", this._graphic);
            DA.SetData("ExecutedToken", Token);
            DA.SetDataTree(3, DisplayGeometry);
            DA.SetData("Turtle2DInfo", _turtle2d);         
        }
         private BoundingBox _clip;
         /// <summary>
  /// Return a BoundingBox that contains all the geometry you are about to draw.
  /// </summary>
  public override BoundingBox ClippingBox
  {
    get { return _clip; }
  }
    }


    //This Component has error
    /*
    public class TurtleDisplay : GH_Component
    {
        public TurtleDisplay() : base("VirtualTurtle", "VirTurtle",
        "Display a turtle which draws your L-system with transformation matrix information", "Einstein", "L-System")
        { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public override Guid ComponentGuid => new Guid("873383ee-844a-4ec4-b130-31998d0c55a1");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Zombie turtle start running", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "Zombie turtle resets", GH_ParamAccess.item);
            pManager.AddGenericParameter("L-System", "LS", "The Lsystem", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Location", "P", "A plane to place the drawings", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Factor", "F", "The scale factor of a turtle", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("ActionBases", "ABs", "These inputs define the turtle's actions", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle", "T", "The turtle object draws the geometries in Rhino viewpoints", GH_ParamAccess.item);
            pManager.AddCurveParameter("Turtle2D", "Tu", "A turtle in Rhinoviewpoint", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Drawing", "Geoms", "The geometry outputs", GH_ParamAccess.list);
        }
        private int IterationCount = 0;
        private TurtleGraphic _TurtleGraphic = null;
        private List<GeometryBase> GeomBags = new List<GeometryBase>();
        private bool JustReset = false;
        private bool StartRunning = false;
        private Turtle2D turtle2D = null;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Run = false, Reset = false;
            double Factor = 0.5;
            DA.GetData("Run", ref Run);
            DA.GetData("Reset", ref Reset);

            DA.GetData("Factor", ref Factor);
            var LSystem = RuleExecuter.Unset;
            DA.GetData(2, ref LSystem);
            var PL = Rhino.Geometry.Plane.WorldXY;
            var ABs = new List<ActionBase>();
            DA.GetData("Location", ref PL);
            DA.GetDataList("ActionBases", ABs);

            Factor = Factor < 1 ? 1 : Factor;


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
                turtle2D = new Turtle2D(_TurtleGraphic, Factor);
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
                    this.turtle2D.Action();
                    IterationCount++;
                }
            }

            JustReset = false;

            if (!Reset && IterationCount < _TurtleGraphic.ExecuteTokens.Count)
            {
                ExpireSolution(true);
            }


        Conclusion:
            DA.SetDataList("Turtle2D", this.turtle2D.Drawings);
            DA.SetDataList("Drawing", _TurtleGraphic.GetGeometries);
            DA.SetData("Turtle", _TurtleGraphic);
        }

        //Display
        private BoundingBox _clip;
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<int> _widths = new List<int>();

        /// <summary>
        /// This method will be called once every solution, before any calls to RunScript.
        /// </summary>
        public override void ExpirePreview(bool redraw)
        {
            List<Color> _colors = new List<Color>();
            List<int> _widths = new List<int>();
            base.ExpirePreview(redraw);
        }

        /// <summary>
        /// Return a BoundingBox that contains all the geometry you are about to draw.
        /// </summary>
        public override BoundingBox ClippingBox
        {
            get { return _clip; }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            

            if (this.turtle2D != null)
            {
                

                for(int i = 0; i < this.turtle2D.Drawings.Length; i++)
                
                    args.Display.DrawCurve(this.turtle2D.Drawings[i], Color.Red);
                }
                if (this.turtle2D.PassPath.Count > 0)
                {
                    var Path = this.turtle2D.PassPath;
                    this._colors.Clear();
                    this._widths.Clear();

                    var ScaleFactor = new List<double> { 1.0, 0.9, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.1 };

                    //Resize Factor
                    var TempFactor = new List<double>();
                    for (int i = 0; i < Path.Count; i++)
                    {
                        if (i > 9)
                            TempFactor.Add(0.1);
                        else
                            TempFactor.Add(ScaleFactor[i]);
                    }
                    ScaleFactor = TempFactor;
                    //Reset Color and Width

                    var a = 119-226;
                    var b = 119-227;
                    var c = 119-50;

                    for (int i = 0; i < ScaleFactor.Count; i++)
                    {
                        double SCFac = ScaleFactor[i];
                        _widths.Add(((int)(SCFac * 10)));
                        _colors.Add(
                            Color.FromArgb(255,
                        (int)(119 - a * SCFac),
                        (int)(119 - b * SCFac),
                        (int)(119 - c * SCFac))
                        );
                    }

                    //226, 227, 50
                    //...
                    //119,119,119
                    _colors.Reverse();
                    _widths.Reverse();

                    for (int i = 0; i < Path.Count; i++)
                        args.Display.DrawCurve(new LineCurve(Path[i]), _colors[i], _widths[i]);

                }
            }

        }
        */
}
