using System;
using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class RotateActionLeft : ActionBase
    {
        public override Transform ActionTransform { get; protected set; }
        private double Degree;
        public RotateActionLeft(string Name, string Description, double Degree) : base(Name, Description)
        {
            this.Degree = Degree;
            this.ActionTransform = Transform.Rotation(-Degree / 360 * Math.PI, Point3d.Origin);
        }
        public RotateActionLeft() : base("-", "The drawing turn right after this action")
        {
            Degree = 60;
            this.ActionTransform = Transform.Rotation(-Degree / 360 * Math.PI, Point3d.Origin);
        }
        public RotateActionLeft(double Degree) : this()
        {
            this.Degree = Degree;
            this.ActionTransform = Transform.Rotation(-Degree / 360 * Math.PI, Point3d.Origin);
        }
        public override bool Execute(TokenPointer _pointer)
        {
            _pointer.NextAction(Transform.Rotation(-Degree / 360 * Math.PI, Point3d.Origin), $"Turn right {Degree} Degree");
            return false;
        }
    }
}