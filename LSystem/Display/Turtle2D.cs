using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Tile.LSystem.TokenAction;

namespace Tile.LSystem.Display
{
    public class Turtle2D
    {
        private Curve[] OriginalTurtle;
        public Curve[] Drawings { get; private set; }
        private double Factor = 1;
        public HashSet<Point3d> PassPoints { get; private set; }
        public List<Line> PassPath { get; private set; }
        public Turtle2D(double Factor = 1)
        {
            PassPoints = new HashSet<Point3d>();
            PassPath = new List<Line>();
            this.Factor = Factor < 0 ? 1 : Factor;
            // Write your logic here
            var body = new Ellipse(Plane.WorldXY, 11.5, 16);
            var Head_1 = Curve.CreateInterpolatedCurve(new Point3d[]{
            new Point3d(4.473364, 15.281747, 0),
            new Point3d(4.576597, 15.772521, 0),
            new Point3d(4.809613, 17.757585, 0),
            new Point3d(4.821539, 19.756076, 0),
            new Point3d(4.385132, 21.701381, 0),
            new Point3d(3.402325, 23.433461, 0),
            new Point3d(1.878409, 24.707915, 0),
            new Point3d(0.04071, 25.195554, 0),
            new Point3d(-1.878409, 24.707915, 0),
            new Point3d(-3.402325, 23.433461, 0),
            new Point3d(-4.385132, 21.701381, 0),
            new Point3d(-4.821539, 19.756076, 0),
            new Point3d(-4.809613, 17.757585, 0),
            new Point3d(-4.576597, 15.772521, 0),
            new Point3d(-4.473364, 15.281747, 0),
        }, 3);


            var Head_2 = Curve.CreateInterpolatedCurve(new Point3d[]{
                new Point3d(4.473364, 15.281747, 0),
                new Point3d(2.628831, 16.044315, 0),
                new Point3d(0.663816, 16.39654, 0),
                new Point3d(-0.663816, 16.39654, 0),
                new Point3d(-2.628831, 16.044315, 0),
                new Point3d(-4.473364, 15.281747, 0),
        }, 3);

            var Hand_1 = Curve.CreateInterpolatedCurve(
                new Point3d[]{
                new Point3d(-10.093849, 8.936043, 0),
                new Point3d(-11.9999, 9.263109, 0),
                new Point3d(-13.993941, 9.133979, 0),
                new Point3d(-15.936991, 8.673114, 0),
                new Point3d(-17.737851, 7.813452, 0),
                new Point3d(-19.282963, 6.551218, 0),
                new Point3d(-20.506633, 4.973679, 0),
                new Point3d(-21.449315, 3.211274, 0),
                new Point3d(-22.207073, 1.36162, 0),
                new Point3d(-22.445889, -0.597358, 0),
                new Point3d(-20.719373, -0.845703, 0),
                new Point3d(-18.830362, -0.189735, 0),
                new Point3d(-16.975451, 0.557742, 0),
                new Point3d(-15.152128, 1.379313, 0),
                new Point3d(-11.720618, 3.175731, 0)

                },
                3
            );

            var Hand_2 = Curve.CreateInterpolatedCurve(new Point3d[]{
                new Point3d(-11.720618, 3.175731, 0),
                new Point3d(-11.325898, 5.136055, 0),
                new Point3d(-10.794118, 7.063527, 0),
                new Point3d(-10.093849, 8.936043, 0)
        }, 3);

            var Leg_1 = Curve.CreateInterpolatedCurve(
                new Point3d[]{
                new Point3d(-10.400083, -8.297845, 0),
                new Point3d(-11.940026, -9.467708, 0),
                new Point3d(-13.2066, -11.013237, 0),
                new Point3d(-14.195916, -12.747912, 0),
                new Point3d(-14.796713, -14.650846, 0),
                new Point3d(-14.92873, -16.641619, 0),
                new Point3d(-14.61091, -18.612656, 0),
                new Point3d(-13.966199, -20.504499, 0),
                new Point3d(-13.131508, -22.320734, 0),
                new Point3d(-11.862731, -23.8323, 0),
                new Point3d(-10.502858, -22.739928, 0),
                new Point3d(-9.693021, -20.911589, 0),
                new Point3d(-8.973339, -19.045718, 0),
                new Point3d(-8.329371, -17.152364, 0),
                new Point3d(-7.300292, -13.418282, 0)

                },
                3
            );

            var Leg_2 = Curve.CreateInterpolatedCurve(new Point3d[]{
                new Point3d(-7.300292, -13.418282, 0),
                new Point3d(-8.463646, -11.79185, 0),
                new Point3d(-9.509445, -10.087665, 0),
                new Point3d(-10.400083, -8.297845, 0)
        }, 3);

            var Mir = Transform.Mirror(Point3d.Origin, Vector3d.XAxis);
            var Hand_3 = Hand_1.DuplicateCurve();
            var Hand_4 = Hand_2.DuplicateCurve();

            Hand_3.Transform(Mir);
            Hand_4.Transform(Mir);

            var Leg_3 = Leg_1.DuplicateCurve();
            var Leg_4 = Leg_2.DuplicateCurve();

            Leg_3.Transform(Mir);
            Leg_4.Transform(Mir);

            var Tail_1 = Curve.CreateInterpolatedCurve(new Point3d[]{
                new Point3d(-1.291319, -16.379506, 0),
                new Point3d(-1.141227, -17.367139, 0),
                new Point3d(-0.831127, -18.316556, 0),
                new Point3d(-0.362188, -19.198381, 0),
                new Point3d(0.245682, -19.991156, 0),
                 new Point3d(0.952045, -20.698309, 0),
                 new Point3d(1.837245, -21.088006, 0),
                 new Point3d(2.159144, -20.243631, 0),
                 new Point3d(2.127542, -19.245721, 0),
                 new Point3d(1.935726, -18.264877, 0),
                 new Point3d(1.658373, -17.304292, 0),
                 new Point3d(1.331634, -16.360076, 0),
        }, 3);

            var Tail_2 = Curve.CreateInterpolatedCurve(new Point3d[]{
                new Point3d(1.331634, -16.360076, 0),
                new Point3d(0.344101, -16.50934, 0),
                new Point3d(-0.654379, -16.486124, 0),
                new Point3d(-1.291319, -16.379506, 0),
        }, 3);

            var Lambs = Curve.JoinCurves(new Curve[]{Hand_1, Hand_2, Leg_1, Leg_2, Head_1,
        Head_2, Tail_1, Tail_2, Hand_3, Hand_4, Leg_3, Leg_4,});

            var Turtle = new List<Curve>();
            Turtle.AddRange(Lambs);
            Turtle.Add(body.ToNurbsCurve());

            this.Drawings = Turtle.ToArray();
            this.OriginalTurtle = this.Drawings.Select(x => x.DuplicateCurve()).ToArray();
        }
        public Turtle2D(Turtle2D _turtle, double Factor = 1) : this(Factor)
        {
            this.Drawings = _turtle.Drawings.Select(x => x.DuplicateCurve()).ToArray();
            this.PassPoints = _turtle.PassPoints;
            this.PassPath = _turtle.PassPath;
            //Resize
            if (_turtle.Factor != 1)
            {
                var TS = Transform.Scale(PassPoints.Last(), 1 / _turtle.Factor);
                this.Drawings = this.Drawings.Select(x => { x.Transform(TS); return x; }).ToArray();
            }
            if (this.Factor != 1)
            {
                var TS = Transform.Scale(PassPoints.Last(), this.Factor);
                this.Drawings = this.Drawings.Select(x => { x.Transform(TS); return x; }).ToArray();
            }
            
        }
        public void ResetTurtle() => this.OriginalTurtle = this.Drawings.Select(x => x.DuplicateCurve()).ToArray();
        public Turtle2D TurtleTransform(Transform TS)
        {

            var Pt = Point3d.Origin;
            var SC = Transform.Scale(Pt, Factor);
            Pt.Transform(TS);
            if (!PassPoints.Contains(Pt))
            {
                if (PassPoints.Count > 1)
                {
                    var Last = PassPoints.LastOrDefault();
                    this.PassPath.Add(new Line(Last, Pt));
                }
                PassPoints.Add(Pt);
            }

            this.Drawings = this.OriginalTurtle.Select(x => x.DuplicateCurve()).ToArray();

            this.Drawings = this.Drawings.Select(x => { x.Transform(SC); x.Transform(TS); return x; }).ToArray();
            return this;
        }
    }
}