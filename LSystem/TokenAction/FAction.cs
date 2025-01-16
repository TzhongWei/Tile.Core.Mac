using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class FAction : ActionBase, IPostTokenAction
    {
        public double InitialLength;
        public double ShortenScale;
        public FAction(string Name, string Description, double InitialLength, double ShortenScale) : base(Name, Description)
        {
            this.InitialLength = InitialLength;
            this.ShortenScale = ShortenScale;
            this.ActionTransform = Transform.Translation(0, InitialLength, 0);
        }
        public FAction() : base("F", "Go stright line")
        {
            InitialLength = 10;
            ShortenScale = 0.8;
            this.ActionTransform = Transform.Translation(0, InitialLength, 0);
        }
        public FAction(double InitialLength, double ShortenScale = 0.8) : this()
        {
            this.InitialLength = InitialLength;
            this.ShortenScale = ShortenScale;
            this.ActionTransform = Transform.Translation(0, InitialLength, 0);
        }
        public override bool Execute(TokenPointer _pointer)
        {
            Transform TS = Transform.Translation(0, InitialLength, 0);
            var Pt = new Point3d(0, 0, 0);
            Pt.Transform(_pointer.transform);
            _pointer.NextAction(TS, $"Go forward {InitialLength}");
            var Pt2 = new Point3d(0, 0, 0);
            Pt2.Transform(_pointer.transform);
            var LN = new LineCurve(Pt, Pt2);
            _pointer.AddDrawing(LN);
            return true;
        }
        private bool Shorten = false;

        public override Transform ActionTransform { get; protected set; }

        public bool IsActive(string Name)
        {
            if (Name == "[")
            {
                Shorten = true;
                return true;
            }
            else if (Name == "]")
            {
                Shorten = false;
                return true;
            }
            else
                return false;
        }
        public bool PostExecute(TokenPointer _pointer)
        {
            if (Shorten)
                this.InitialLength *= ShortenScale;
            else
                this.InitialLength /= ShortenScale;

            this.ActionTransform = Transform.Translation(0, InitialLength, 0);

            return true;
        }
    }
}