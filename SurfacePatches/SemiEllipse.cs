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
        }

        public override string XExpression { get; protected set; }
        public override string YExpression { get; protected set; }
        public override string ZExpression { get; protected set; }

        public override double XFunction(double u, double v)
        {
            XExpression = $"a * (u / m) ^ (1/2) * cos(v), a = {a}, m = {m}";
            return a * Math.Pow(u / m, (1 / 2)) + Math.Cos(v);
        }

        public override double YFunction(double u, double v)
        {
            YExpression = $"a * (u / m) ^ (1/2) * sin(v), a = {a}, m = {m}";
            return a * Math.Pow(u / m, (1 / 2)) + Math.Sin(v);
        }

        public override double ZFunction(double u, double v)
        {
            ZExpression = "u";
            return u;
        }
    }
}