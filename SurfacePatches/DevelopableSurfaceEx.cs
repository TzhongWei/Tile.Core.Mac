using System;
using System.Collections.Generic;

namespace Tile.Core.Patch
{
    public class DevelopableSurface : PatchFunction
    {
        public override string Name => "DevelopableSurfacePatchA";
        public double a = 1;
        public double b = 1;
        public double m = 1;
        public override string XExpression { get; protected set; }
        public override string YExpression { get; protected set; }
        public override string ZExpression { get; protected set; }

        public DevelopableSurface(double a, double b, double m) : base()
        {
            this.a = a;
            this.b = b;
            this.m = m == 0 ? 1 : m;
        }

        public override double XFunction(double u, double v)
        {
            this.XExpression = $"x(u,v) = a * cos(v * pi) - a * u * sin(v * pi / m, a = {a}, n = {m})";
            return a * Math.Cos(v * Math.PI) - a * u * Math.Sin(v * Math.PI / m);
        }

        public override double YFunction(double u, double v)
        {
            this.XExpression = $"x(u,v) = a * sin(v * pi) - a * u * cos(v * pi / m), a = {a}, m = {m}";
            return a * Math.Sin(v * Math.PI) - a * u * Math.Cos(v * Math.PI / m);
        }

        public override double ZFunction(double u, double v)
        {
            this.ZExpression = $"b * v + b * u / m, b = {b}, m = {m}";
            return b * v + b * u / m;
        }
    }
}