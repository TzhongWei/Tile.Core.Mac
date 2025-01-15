using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Grasshopper.Rhinoceros.Model;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Text.RegularExpressions;
using Rhino.UI;

namespace Tile.Core.Util
{
    /// <summary>
    /// This class refer to the Rhino Instance reference, the definition is placed in the HatTileDoc
    /// </summary>
    public class BlockInstance : GH_InstanceReference
    {
        public Label BlockLabel { get; private set; }
        public string BlockName { get; private set; }
        public int BlockIndex { get; private set; } = -1;
        public bool IsBlock { get; private set; } = false;
        public TilePatterns tilePatterns { get; private set; }
        public BlockInstance(Label blockLabel, string blockName) : base(Rhino.Geometry.Transform.Identity)
        {
            BlockLabel = blockLabel;
            BlockName = blockName;
            var RhinoBlock = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions.Find(blockName);
            BlockIndex = RhinoBlock?.Index ?? -1;
            IsBlock = BlockIndex != -1;
            var ColourFromObject = RhinoBlock.GetUserString("ColourFromObject") == "True" ? true : false;
            var Frame = RhinoBlock.GetUserString("Frame") == "true" ? true : false;

            this.InstanceDefinition = new ModelInstanceDefinition(RhinoBlock);
            tilePatterns = new TilePatterns()
            {
                label = blockLabel,
                Frame = Frame,
                ColourFromObject = ColourFromObject,
                Patterns = RhinoBlock.GetObjects().Select(x => x.Geometry).ToList(),
                PatternAtts = RhinoBlock.GetObjects().Select(x => x.Attributes).ToList(),
                Guids = RhinoBlock.GetObjects().Select(x => x.Id).ToList()
            };
        }
        public override string TypeName => $"TilePatterns.{tilePatterns.label.ToString()}.{this.BlockName}";
        public static BlockInstance Unset => null;
        public override string TypeDescription => "This is a Einstein hat tile";

        public override bool Equals(object obj)
        => (obj is BlockInstance) && ((BlockInstance)obj).BlockIndex == BlockIndex;
        public override int GetHashCode()
        {
            string Code = this.BlockIndex.ToString() + ((int)this.BlockLabel).ToString() + this.BlockName;
            return Code.GetHashCode();
        }
        public override string ToString()
        {
            return $"BlockInstance.{BlockLabel}.{BlockName}";
        }
        public string ToJson()
        {
            var Instance = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions.Find(BlockName);
            var Infor = new Dictionary<string, string>
            {
                {"Hat", Instance.GetUserString("Hat") },
                {"Label", Instance.GetUserString("Label") },
                {"BlockName", BlockName },
                {"ID", this.BlockIndex.ToString()},
                {"Frame", Instance.GetUserString("Frame") },
                {"ColourFromObject", tilePatterns.ColourFromObject.ToString() }
            };
            return JsonConvert.SerializeObject(Infor, Formatting.Indented);
        }
        public BlockInstance ChangeLabel(Label label)
        { 
            this.BlockLabel = label;
            
            var Ref = RhinoDoc.ActiveDoc.InstanceDefinitions.Find(BlockName);
            Ref.SetUserString("Label", label.ToString());
            this.InstanceDefinition = Ref;

            return this;
        }
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            var duplicate = new BlockInstance(this.BlockLabel, this.BlockName)
            {
                tilePatterns = this.tilePatterns, // Copy additional properties
                IsBlock = this.IsBlock,
                BlockIndex = this.BlockIndex
            };
            return duplicate;
        }
        public override bool Write(GH_IWriter writer)
        {

            writer.SetString("BlockName", BlockName);
            writer.SetInt32("BlockIndex", BlockIndex);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            BlockName = reader.GetString("BlockName");
            BlockIndex = reader.GetInt32("BlockIndex");


            var RhinoBlock = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions.Find(BlockName);
            BlockIndex =
                RhinoBlock == null ? -1 : RhinoBlock.Index;
            if (BlockIndex == -1)
                IsBlock = false;
            var ColourFromObject = RhinoBlock.GetUserString("ColourFromObject") == "true" ? true : false;
            var Frame = RhinoBlock.GetUserString("Frame") == "true" ? true : false;

            Label LB = Label.H;
            switch (RhinoBlock.GetUserString("Label"))
            {
                case ("H"):
                    LB = Label.H;
                    break;
                case ("H1"):
                    LB = Label.H1;
                    break;
                case ("T"):
                    LB = Label.T;
                    break;
                case ("F"):
                    LB = Label.F;
                    break;
                case ("P"):
                    LB = Label.P;
                    break;
            }

            tilePatterns = new TilePatterns()
            {
                label = LB,
                Frame = Frame,
                ColourFromObject = ColourFromObject,
                Patterns = RhinoBlock.GetObjects().Select(x => x.Geometry).ToList(),
                PatternAtts = RhinoBlock.GetObjects().Select(x => x.Attributes).ToList(),
                Guids = RhinoBlock.GetObjects().Select(x => x.Id).ToList()
            };
            return base.Read(reader);
        }
        public static implicit operator BlockInstance(string Name)
        {
            if (Name.Contains("."))
            {
                string pattern = Name.Split('.').Last();


               return HatTileDoc.BlockInstances.Find(pattern)?.DuplicateGeometry() as BlockInstance;

            }

            return HatTileDoc.BlockInstances.Find(Name)?.DuplicateGeometry() as BlockInstance;
        }

        public static implicit operator string(BlockInstance instance)
            => instance.BlockName;
        public Guid Bake()
        {
            var m_Guid = Guid.NewGuid();
            string[] LayerName = { "Hat_H", "Hat_H1", "Hat_T", "Hat_P", "Hat_F" };
            var Doc = Rhino.RhinoDoc.ActiveDoc;
            ObjectAttributes Att = new ObjectAttributes {
                LayerIndex = Doc.Layers.FindName(LayerName[0]).Index
            };
            this.BakeGeometry(Doc, Att, ref m_Guid);
            return m_Guid;
        }
        public override bool CastFrom(object source)
        {
            switch(source)
            {
                case BlockInstance system:
                    Value = ((BlockInstance)source).Value;
                    return true;
                default: 
                    return false;
            }
        }
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q).IsAssignableFrom(typeof(BlockInstance)))
            {
                target = (Q)(object)Value;
                return true;
            }

            return false;
        }
    }
}
