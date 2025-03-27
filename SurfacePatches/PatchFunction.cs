using System;
using System.Collections.Generic;

namespace Tile.Core.Patch
{
    abstract public class PatchFunction
    {
        public abstract string Name { get; }
                protected string _XExpression;
        protected string _YExpression;
        protected string _ZExpression;
        public abstract string XExpression { get; protected set; }
        public abstract string YExpression { get; protected set; }
        public abstract string ZExpression { get; protected set; }
        public abstract double XFunction(double u, double v);
        public abstract double YFunction(double u, double v);
        public abstract double ZFunction(double u, double v);
        public override string ToString()
        {
            return this.XExpression + "\n" + this.YExpression + "\n" + this.ZExpression;
        }
    }
}