
namespace Tile.Core.Patch{
    public class PlanarSurface : PatchFunction
    {
        public PlanarSurface() : base("PlanarSrf") { }
        public override double XFunction(double u, double v) => u;
        public override double YFunction(double u, double v) => v;
        public override double ZFunction(double u, double v) => 1;
    }
}