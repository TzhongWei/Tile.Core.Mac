using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class TransformAction : ActionBase
    {
        /// <summary>
        /// Transform Action
        /// </summary>
        private Transform TransformTS;
        private bool IsSketch;
        public TransformAction(string Name, string Description, Transform TS, bool IsSketch = true) : base(Name, Description)
        {
            this.TransformTS = new Transform(TS);
            this.IsSketch = true;
        }

        public override Transform ActionTransform { get => this.TransformTS; protected set => this.TransformTS = value; }

        public override bool Execute(TokenPointer _pointer)
        {
            Transform TS = this.TransformTS;

            var Pt = new Point3d(0, 0, 0);
            Pt.Transform(_pointer.transform);
            
            var Pt2 = new Point3d(0, 0, 0);
            Pt2.Transform(_pointer.transform * TS);
            var LN = new LineCurve(Pt, Pt2);
            if (IsSketch)
                _pointer.AddDrawing(LN);
            _pointer.NextAction(TS, $"Custom Transformation");
            return true;
        }
    }
}