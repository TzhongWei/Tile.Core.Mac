using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Tile.Core
{
    public class AddPatternOption
    {
        private List<GeometryBase> pattern = new List<GeometryBase>();
        private string Label = string.Empty;
        private bool Frame = false;
        private double ScaleFactor = 1;
        private bool ColourFromObject = false;
        private List<System.Guid> PatternsGuids = new List<System.Guid>();
        private List<ObjectAttributes> PatternsAtts = new List<ObjectAttributes>();
        public string Name { get; set; } = string.Empty;
        public readonly bool IsEmpty = true;
        public static Curve PreviewShape(double Scale = 1)
        {
            if (Scale < 0) Scale = 1;
            Curve outline = (new Einstein.HatTile("Outline")).PreviewShape;
            outline.Transform(Transform.Scale(Point3d.Origin, Scale));
            return outline;
        }
        public static Curve PreviewShape(Plane PL, double Scale = 1)
        {
            if (Scale < 0) Scale = 1;
            Curve outline = (new Einstein.HatTile("Outline")).PreviewShape;
            outline.Transform(Transform.Scale(Point3d.Origin, Scale));
            outline.Transform(Transform.PlaneToPlane(Plane.WorldXY, PL));
            return outline;
        }
        /// <summary>
        /// Add a pattern with the same attributes in Rhino.
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="PatternsGuid"></param>
        /// <param name="Frame"></param>
        /// <param name="ScaleSet"></param>
        /// <param name="ColourFromObject"></param>
        public AddPatternOption(string Label, List<System.Guid> PatternsGuid, bool Frame = false, double ScaleSet = 1,
            bool ColourFromObject = false)
        {
            IsEmpty = false;
            this.ScaleFactor = (ScaleSet < 0) ? 1 : 1 / ScaleSet;
            this.Label = Label;
            this.Frame = Frame;
            this.PatternsGuids = PatternsGuid;
            this.ColourFromObject = ColourFromObject;
            var Doc = RhinoDoc.ActiveDoc;
            for (int i = 0; i < PatternsGuid.Count; i++)
            {
                var GeoRefer = Doc.Objects.FindId(PatternsGuid[i]);
                var Geom = GeoRefer.Geometry;
                Geom.Transform(Transform.Scale(Point3d.Origin, this.ScaleFactor));
                this.pattern.Add(Geom);
                this.PatternsAtts.Add(GeoRefer.Attributes);
            }
        }
        /// <summary>
        /// Add a pattern with colour setting from layers
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="pattern"></param>
        /// <param name="Frame"></param>
        /// <param name="ScaleSet"></param>
        public AddPatternOption(string Label, List<GeometryBase> pattern, bool Frame = false, double ScaleSet = 1)
        {
            if (ScaleSet < 0) this.ScaleFactor = 1;
            else
                this.ScaleFactor = 1 / ScaleSet;

            this.Label = Label;
            this.pattern = pattern;
            this.Frame = Frame;
            this.ColourFromObject = false;

            for (int i = 0; i < this.pattern.Count; i++)
            {
                this.pattern[i].Transform(Transform.Scale(Point3d.Origin, this.ScaleFactor));
                ObjectAttributes Att = new ObjectAttributes();
                Att.ColorSource = ObjectColorSource.ColorFromLayer;
                this.PatternsAtts.Add(Att);
            }
        }
        /// <summary>
        /// Add a pattern from a differen place
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="pattern"></param>
        /// <param name="ReferencePL"></param>
        /// <param name="Frame"></param>
        /// <param name="ScaleSet"></param>
        public AddPatternOption(string Label, List<GeometryBase> pattern, Plane ReferencePL, bool Frame = false, double ScaleSet = 1)
        {
            if (ScaleSet < 0) this.ScaleFactor = 1;
            else
                this.ScaleFactor = 1 / ScaleSet;

            this.Label = Label;
            this.pattern = pattern;
            this.Frame = Frame;
            this.ColourFromObject = false;

            for (int i = 0; i < this.pattern.Count; i++)
            {
                this.pattern[i].Transform(Transform.Scale(Point3d.Origin, this.ScaleFactor));
                this.pattern[i].Transform(Transform.PlaneToPlane(ReferencePL, Plane.WorldXY));
                ObjectAttributes Att = new ObjectAttributes();
                Att.ColorSource = ObjectColorSource.ColorFromLayer;
                this.PatternsAtts.Add(Att);
            }
        }
        /// <summary>
        /// Add a pattern from a differen place
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="pattern"></param>
        /// <param name="ReferencePL"></param>
        /// <param name="Frame"></param>
        /// <param name="ScaleSet"></param>
        public AddPatternOption(string Label, List<System.Guid> PatternsGuid, Plane ReferencePL, bool Frame = false, double ScaleSet = 1,
            bool ColourFromObject = false)
        {
            IsEmpty = false;
            this.ScaleFactor = (ScaleSet < 0) ? 1 : 1 / ScaleSet;
            this.Label = Label;
            this.Frame = Frame;
            this.PatternsGuids = PatternsGuid;
            this.ColourFromObject = ColourFromObject;
            var Doc = RhinoDoc.ActiveDoc;
            for (int i = 0; i < PatternsGuid.Count; i++)
            {
                var GeoRefer = Doc.Objects.FindId(PatternsGuid[i]);
                var Geom = GeoRefer.Geometry;
                Geom.Transform(Transform.Scale(Point3d.Origin, this.ScaleFactor));
                Geom.Transform(Transform.PlaneToPlane(ReferencePL, Plane.WorldXY));
                this.pattern.Add(Geom);
                this.PatternsAtts.Add(GeoRefer.Attributes);
            }
        }
        /// <summary>
        /// Get geometry settings
        /// </summary>
        /// <param name="Label"></param> Label for setting, all, h, h1, t, p, f
        /// <param name="pattern"></param> The List of Geometries
        /// <param name="Frame"></param> If the frame need to be set up
        /// <param name="objectAttributes"></param> The list of objectAttributes
        /// <param name="ColourFromObject"></param> The colour source setting from layer or object, which is a boolean
        /// <param name="guids"></param> List of guid labelling the geometries in rhino
        public void Get(out string Label, out List<GeometryBase> pattern, out bool Frame, out List<ObjectAttributes> objectAttributes,
            out bool ColourFromObject, out List<System.Guid> guids)
        {
            this.Get(out Label, out pattern, out Frame);
            objectAttributes = this.PatternsAtts;
            guids = this.GetGuid(out ColourFromObject);
        }
        public void Get(out string Label, out List<GeometryBase> pattern, out bool Frame)
        {
            Label = this.Label;
            pattern = this.pattern;
            Frame = this.Frame;
        }
        public List<System.Guid> GetGuid(out bool ColourFromObject)
        {
            ColourFromObject = this.ColourFromObject;
            return this.PatternsGuids;
        }
        public override string ToString()
        => $"TilePatterns.{this.Label.ToString()}";
    }
}
