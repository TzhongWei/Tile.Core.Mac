namespace Tile.LSystem.TokenAction
{
    public interface IPostTokenAction
    {
        bool IsActive(string Name);
        bool PostExecute(TokenPointer _pointer);
    }
}