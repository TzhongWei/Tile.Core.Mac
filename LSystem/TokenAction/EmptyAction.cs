using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class EmptyAction : ActionBase
    {
        public override Transform ActionTransform { get; protected set; }
        public EmptyAction(string Name, string Description) : base(Name, Description) { this.ActionTransform = Transform.Identity; }
        public EmptyAction() : base("X", "No Action Is Executed") { }
        public override bool Execute(TokenPointer _pointer)
        {
            _pointer.NoAction();
            return true;
        }
    }
}