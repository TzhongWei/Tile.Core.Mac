
namespace Tile.Core.Patch
{
    public class PlanarSurface : PatchFunction
    {
        private double uSize;
        private double vSize;
        public override string Name => "PlanarSrf";
        private double H;
        private string _XExpression;
        private string _YExpression;
        private string _ZExpression;
        public override string XExpression { get => _XExpression; protected set { this._XExpression = value; } }
        public override string YExpression { get => _YExpression; protected set { this._YExpression = value; } }
        public override string ZExpression { get => _ZExpression; protected set { this._ZExpression = value; } }
        public PlanarSurface(double uSize = 1, double vSize = 1, double H = 0) : base()
        {
            this.uSize = uSize;
            this.vSize = vSize;
            this.H = H;
            this._XExpression = $"x(u,v) = {uSize} * u";
            this._YExpression = $"y(u,v) = {vSize} * v";
            this._ZExpression = $"z(u,v) = {H}";
        }
        public override double XFunction(double u, double v) => uSize * u;
        public override double YFunction(double u, double v) => vSize * v;
        public override double ZFunction(double u, double v) => H;
    }
}