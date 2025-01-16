using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public class PopAction : ActionBase
    {
        public override Transform ActionTransform { get; protected set; }
        public PopAction() : base("]", "Pop the transformation") { }
        public override bool Execute(TokenPointer _pointer)
        {
            _pointer.Pop();
            this.ActionTransform = _pointer.transform;
            return true;
        }
    }
}