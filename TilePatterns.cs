
using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Tile.Core
{
    /// <summary>
    /// This class focuses on the geometry aspects
    /// </summary>
    public class TilePatterns
    {
        //Test if the patterns has a frame instance in the Patterns List
        public bool HasFrame;
        public Label label;
        public List<GeometryBase> Patterns;
        //The patterns needs a geometry frame from the setting
        public bool Frame;
        public List<ObjectAttributes> PatternAtts;
        public bool ColourFromObject;
        //Geometry Guids in Rhino
        public List<System.Guid> Guids;
        public TilePatterns() { }
    }
}
