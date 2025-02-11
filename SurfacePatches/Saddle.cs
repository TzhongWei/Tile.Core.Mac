using System;
using Rhino.Geometry.Intersect;

namespace Tile.Core.Patch
{
    public class SaddleA : PatchFunction
    {
        public override string Name => "SaddleA";
        public double a;
        public double m;

        public SaddleA(double a, double m)
        {
            this.a = a;
            this.m = m == 0 ? 1 : m;
        }

        public override string XExpression { get; protected set; }
        public override string YExpression { get; protected set; }
        public override string ZExpression { get; protected set; }

        public override double XFunction(double u, double v)
        {
            this.XExpression = "u";
            return u;
        }

        public override double YFunction(double u, double v)
        {
            this.XExpression = "v";
            return v;
        }

        public override double ZFunction(double u, double v)
        {
            this.ZExpression = $"1 - a / m * ((u-0.5)^2 - (v-0.5)^2), a = {a}, m = {m}";
            return 1 - a / m * (Math.Pow(u - 0.5, 2) - Math.Pow(v - 0.5, 2));
        }
    }
    public class SaddleB : PatchFunction
    {
        public override string Name => "SaddleB";
        public double a;
        public double m;

        public SaddleB(double a, double m)
        {
            this.a = a;
            this.m = m == 0 ? 1 : m;
        }

        public override string XExpression { get; protected set; }
        public override string YExpression { get; protected set; }
        public override string ZExpression { get; protected set; }

        public override double XFunction(double u, double v)
        {
            this.XExpression = "u";
            return u;
        }

        public override double YFunction(double u, double v)
        {
            this.XExpression = "v";
            return v;
        }

        public override double ZFunction(double u, double v)
        {
            this.ZExpression = $"1 - a / m * ((u-0.5)^2 + (v-0.5)^2), a = {a}, m = {m}";
            return 1 - a / m * (Math.Pow(u - 0.5, 2) + Math.Pow(v - 0.5, 2));
        }
    }
}