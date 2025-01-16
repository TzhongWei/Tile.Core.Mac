using System;

namespace Tile.LSystem.TokenAction
{
    public interface ITokenAction
    {
        bool Execute(TokenPointer _pointer);
    }
}