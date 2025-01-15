using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.UI;
using Tile.Core.Util;

namespace Tile.Core
{
    public class Einstein_Resize: Einstein
    {
        private double Hatsize = 1;
        public int[] BlocksId = new int[5];
        private Transform Translation = new Transform();
        private HatGroup<BlockInstance> _HatID = new HatGroup<BlockInstance>
            (BlockInstance.Unset, 
            BlockInstance.Unset,
            BlockInstance.Unset, 
            BlockInstance.Unset,
            BlockInstance.Unset);
        public List<GeometryBase> HPatterns = new List<GeometryBase>();
        //private TilePatterns[] PatternsManager = new TilePatterns[5];

        public Einstein SetTile { private get; set; } = new Einstein();
        public Einstein_Resize(double size, Point3d StartPt)
        {
            if(size < 0) this.Hatsize = 1;
            else this.Hatsize = size;
            this.Translation = Transform.Translation(new Vector3d(StartPt.X, StartPt.Y, StartPt.Z));
            Label[] LabelTags = { Label.H, Label.H1, Label.T, Label.P, Label.F };
        }
        public bool SetPermutedBlock(IEnumerable<BlockInstance> blocks)
        {
            foreach(var block in blocks) 
            {
                switch (block.BlockLabel)
                {
                    case (Label.H):
                        _HatID[0] = block;
                        break;
                    case (Label.H1):
                        _HatID[1] = block;
                        break;
                    case (Label.T):
                        _HatID[2] = block;
                        break;
                    case Label.P:
                        _HatID[3] = block;
                        break;
                    case Label.F:
                        _HatID[4] = block;
                        break;
                }
            }

            if(_HatID.ToList().Select(x => x != null).Aggregate((Re1, Re2) => Re1 | Re2))
            {
                Label[] MatchLabel = new Label[]{Label.H, Label.H1, Label.T, Label.F, Label.P}; 
                for(int i = 0; i < _HatID.Length; i++)
                {
                    if(_HatID[i] == null)
                    {
                        _HatID[i] = HatTileDoc.BlockInstances.FirstOrDefault(x => x.BlockLabel == MatchLabel[i]);
                    }
                }
            }

            return !_HatID.ToList().Select(x => x != null).Aggregate((Re1, Re2) => Re1 | Re2);
        }
        public List<Curve> PreviewShape()
        {
            if (SetTile.Hat_Transform.Count <= 0) return new List<Curve>();
            object LockObj = new object();
            ConcurrentBag<Curve> HatCrvs = new ConcurrentBag<Curve>();
            ConcurrentBag<int> Seq = new ConcurrentBag<int>();
            var TS = this.SetTile.Hat_Transform;
            var Scale = Transform.Scale(Point3d.Origin, Hatsize);
            Parallel.For(0, TS.Count, i =>
            {
                var HatShape = new Einstein.HatTile("Outline").PreviewShape;
                var Final = Translation * Scale * TS[i];
                Seq.Add(i);
                HatShape.Transform(Final);
                HatCrvs.Add(HatShape);
            });
            List<Curve> sortedCurve = new List<Curve>();
            lock (LockObj)
            {
                var indexes = Seq.ToList();
                indexes.Sort();
                sortedCurve = indexes.Select(i => HatCrvs.ElementAt(i)).ToList();
            }
            return sortedCurve;
        }
        public bool PlaceBlock(Einstein MonoTile, out List<BlockInstance> Tiles, out List<Transform> History)
        {
            History = new List<Transform>();
            Tiles = new List<BlockInstance>();
            if (this.Hatsize < 0 || MonoTile.Hat_Labels.Count != MonoTile.Hat_Transform.Count ||
                    MonoTile.Hat_Labels.Count < 0)
                return false;
            var Doc = RhinoDoc.ActiveDoc;
            string[] LayerName = { "Hat_H", "Hat_H1", "Hat_T", "Hat_P", "Hat_F" };
            
            if (!_HatID.ToList().Select(x => x != null).Aggregate((Re1, Re2) => Re1 | Re2))
                throw new Exception("Objects hasn't been defined as blocks");
            
            var labels = MonoTile.Hat_Labels;
            var Transforms = MonoTile.Hat_Transform;
            var Scale = Transform.Scale(Point3d.Origin, Hatsize);
            for (int i = 0; i < Transforms.Count; i++)
            {
                var Final = Translation * Scale * Transforms[i];
                ObjectAttributes Att = new ObjectAttributes();
                switch (labels[i])
                {
                    case "H":
                        Att.LayerIndex = Doc.Layers.FindName(LayerName[0]).Index;
                        Tiles.Add((BlockInstance)_HatID[0].DuplicateGeometry().Transform(Final));
                        break;
                    case "H1":
                        Att.LayerIndex = Doc.Layers.FindName(LayerName[1]).Index;
                        Tiles.Add((BlockInstance)_HatID[1].DuplicateGeometry().Transform(Final));
                        break;
                    case "T":
                        Att.LayerIndex = Doc.Layers.FindName(LayerName[2]).Index;
                        Tiles.Add((BlockInstance)_HatID[2].DuplicateGeometry().Transform(Final));
                        break;
                    case "P":
                        Att.LayerIndex = Doc.Layers.FindName(LayerName[3]).Index;
                        Tiles.Add((BlockInstance)_HatID[3].DuplicateGeometry().Transform(Final));
                        break;
                    case "F":
                        Att.LayerIndex = Doc.Layers.FindName(LayerName[4]).Index;
                        Tiles.Add((BlockInstance)_HatID[4].DuplicateGeometry().Transform(Final));
                        break;
                    default:
                        return false;
                }
                History.Add(Final);
            }
            return true;
        }
    }
}
