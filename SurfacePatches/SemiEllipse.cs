using System;
using Rhino.Geometry;

namespace Tile.Core.Patch
{
    public class SemiEllipse : PatchFunction
    {
        public override string Name => "SemiEllipse";
        public double a;
        public double b;
        public double m;
        public SemiEllipse(double a, double b, double m) : base()
        {
            this.a = a;
            this.b = b;
            this.m = m;
            XExpression = $"a * (u / m) ^ (1/2) * cos(v), a = {a}, m = {m}";
            YExpression = $"b * (u / m) ^ (1/2) * sin(v), b = {b}, m = {m}";
            ZExpression = "u";
        }

        public override string XExpression { get; protected set; }
        public override string YExpression { get; protected set; }
        public override string ZExpression { get; protected set; }

        public override double XFunction(double u, double v)
        {
            
            return a * Math.Pow(u / m, 0.5) * Math.Cos(v);
        }

        public override double YFunction(double u, double v)
        {
            var Test = u / m;
            return b * Math.Pow(Test, 0.5) * Math.Sin(v);
        }

        public override double ZFunction(double u, double v)
        {
            
            return u;
        }
    }
}