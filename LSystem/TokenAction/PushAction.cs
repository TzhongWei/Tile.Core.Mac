using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class PushAction : ActionBase
    {
        public override Transform ActionTransform { get; protected set; }
        public PushAction() : base("[", "Pop the transformation") { }
        public override bool Execute(TokenPointer _pointer)
        {
            _pointer.Push();
            this.ActionTransform = _pointer.transform;
            return true;
        }
    }
}