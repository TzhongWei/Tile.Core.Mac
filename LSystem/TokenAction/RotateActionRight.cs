using System;
using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class RotateActionRight : ActionBase
    {
        private double Degree;

        public override Transform ActionTransform { get; protected set; }

        public RotateActionRight(string Name, string Description, double Degree) : base(Name, Description)
        {
            this.Degree = Degree;
        }
        public RotateActionRight() : base("+", "The drawing turn right after this action")
        {
            Degree = 60;
            this.ActionTransform = Transform.Rotation(Degree / 360 * Math.PI, Point3d.Origin);
        }
        public RotateActionRight(double Degree) : this()
        {
            this.Degree = Degree;
            this.ActionTransform = Transform.Rotation(Degree / 360 * Math.PI, Point3d.Origin);
        }
        public override bool Execute(TokenPointer _pointer)
        {
            _pointer.NextAction(Transform.Rotation(Degree / 360 * Math.PI, Point3d.Origin), $"Turn right {Degree} Degree");
            return false;
        }
    }
}