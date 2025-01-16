using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class TranslationAction : ActionBase
    {
        public override Transform ActionTransform { get => this.Translation; protected set => this.Translation = value; }
        /// <summary>
        /// Translation Action
        /// </summary>
        private Transform Translation;
        private Vector3d Motion;
        private bool IsSketch;
        public TranslationAction(string Name, string Description, Vector3d Translation, bool IsSketch = true) : base(Name, Description)
        {
            this.Translation = Transform.Translation(Translation);
            this.Motion = Translation;
            this.IsSketch = true;
        }
        public override bool Execute(TokenPointer _pointer)
        {
            Transform TS = this.Translation;

            var Pt = new Point3d(0, 0, 0);
            Pt.Transform(_pointer.transform);
            _pointer.NextAction(TS, $"Go forward {Motion.X}, {Motion.Y}, {Motion.Z}");
            var Pt2 = new Point3d(0, 0, 0);
            Pt2.Transform(_pointer.transform);
            var LN = new LineCurve(Pt, Pt2);
            if (IsSketch)
                _pointer.AddDrawing(LN);

            return true;
        }
    }
}