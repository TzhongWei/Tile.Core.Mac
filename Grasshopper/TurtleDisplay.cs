using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Tile.LSystem.Display;

namespace Tile.Core.Grasshopper
{
    public class TurtleDisplay : GH_Component
    {
        public TurtleDisplay() : base("TurtleDisplay", "TDis", "Display a turtle in L system, this component can be used to examine the execution process", "Einstein", "L-System") { }
        protected override Bitmap Icon => IconLoader.TurtleDisplay;
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public override Guid ComponentGuid => new Guid("b1375a51-c542-443b-810e-7057f63f33da");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Turtle2DInfo", "TurInfo", "The turtle drawing information", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Resize the turtle", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("DisplayPathLength", "DL", "The number of display curves", GH_ParamAccess.item, 10);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("TurtleShadow", "TS", "The turtle drawing", GH_ParamAccess.list);
        }
        private Curve[] DisplayPath = new Curve[5];
        private double[] DisplayFactor = new double[5];
        private Curve[] TurtleGeoms = new Curve[5];
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var _turtle2d = new Turtle2D();
            var DL = 10;
            var ScaleFac = 1.0;
            DA.GetData(0, ref _turtle2d);
            DA.GetData("Scale", ref ScaleFac);
            DA.GetData("DisplayPathLength", ref DL);

            _turtle2d = new Turtle2D(_turtle2d, ScaleFac);

            DisplayPath = new Curve[DL];
            DisplayFactor = new double[DL];
            for (int i = 0; i < DL; i++)
            {
                if (i < _turtle2d.PassPath.Count)
                {
                    var TempList = new List<Line>(_turtle2d.PassPath);
                    TempList.Reverse();
                    DisplayPath[i] = new LineCurve(TempList[i]);
                }
                DisplayFactor[i] = (i + 1);
            }

            // 1 - 5
            // 1 - 10
            var Max = DisplayFactor.Last() - 1;
            var Min = DisplayFactor.First();
            for (int i = 0; i < DL; i++)
            {
                DisplayFactor[i] = Min + 9 / Max * i;
            }
            this.TurtleGeoms = _turtle2d.Drawings;
            DA.SetDataList("TurtleShadow", _turtle2d.Drawings);
        }
        private Color InterpolateColor(Color start, Color end, double factor)
        {
            int a = (int)(start.A + (end.A - start.A) * factor);
            int r = (int)(start.R + (end.R - start.R) * factor);
            int g = (int)(start.G + (end.G - start.G) * factor);
            int b = (int)(start.B + (end.B - start.B) * factor);
            return Color.FromArgb(a, r, g, b);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            var TurtleColour = this.Attributes.Selected ? Color.Blue : Color.Red;

            foreach (var Curve in this.TurtleGeoms)
            {
                if (Curve == null) continue;
                args.Display.DrawCurve(Curve, TurtleColour);
            }

            DisplayFactor = DisplayFactor.Reverse().ToArray();
            if (this.Attributes.Selected)
            {
                for (int i = 0; i < DisplayFactor.Length; i++)
                {
                    if (this.DisplayPath[i] == null) continue;
                    var Colour = this.InterpolateColor(Color.Red, Color.Yellow, DisplayFactor[i] / 10);
                    args.Display.DrawCurve(this.DisplayPath[i], Colour, (int)DisplayFactor[i]);
                }
            }
            else
            {
                for (int i = 0; i < DisplayFactor.Length; i++)
                {
                    if (this.DisplayPath[i] == null) continue;
                    var Colour = this.InterpolateColor(Color.Red, Color.Yellow, DisplayFactor[i] / 10);
                    args.Display.DrawCurve(this.DisplayPath[i], Colour, (int)DisplayFactor[i]);
                }
            }
        }
    }
}