using System;
using System.Collections.Generic;

namespace Tile.Core.Patch
{
    abstract public class PatchFunction
    {
        public string Name { get; }
        public abstract double XFunction(double u, double v);
        public abstract double YFunction(double u, double v);
        public abstract double ZFunction(double u, double v);
        public PatchFunction(string name)
        {
            this.Name = name;
        }
    }

}