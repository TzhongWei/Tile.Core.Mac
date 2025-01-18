using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class RotationAction : ActionBase
    {
        public override Transform ActionTransform { get { return Rotation; } protected set { Rotation = value; } }
        /// <summary>
        /// Rotation Action
        /// </summary>
        private Transform Rotation;
        private bool IsSketch;
        public RotationAction(string Name, string Description, double angle, Point3d PointCentre, bool IsSketch = true) : base(Name, Description)
        {
            this.Rotation = Transform.Rotation(angle, PointCentre);
            this.IsSketch = true;
        }
        public RotationAction(string Name, string Description, double angle, Point3d PointCentre, Vector3d Axis, bool IsSketch = true) : base(Name, Description)
        {
            this.Rotation = Transform.Rotation(angle, Axis, PointCentre);
            this.IsSketch = true;
        }
        public override bool Execute(TokenPointer _pointer)
        {
            Transform TS = this.Rotation;

            var Pt = new Point3d(0, 0, 0);
            Pt.Transform(_pointer.transform);
            
            var Pt2 = new Point3d(0, 0, 0);
            Pt2.Transform(_pointer.transform * TS);
            var LN = new LineCurve(Pt, Pt2);
            if (IsSketch)
                _pointer.AddDrawing(LN);
            _pointer.NextAction(TS, $"Rotation Customisation");
            return true;
        }
    }
}