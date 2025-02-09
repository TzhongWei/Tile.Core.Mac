using System.Collections.Generic;
using System.Linq;
using System;
using Rhino.Geometry;
using Eto.Forms;

namespace Tile.Core.Patch
{
    public class SurfacePatch
    {
        private Surface _ResultSurface;
        public Surface ResultSurface {get { return _ResultSurface;} }
        private Interval UDomain = new Interval(0, 1);
        private Interval VDomain = new Interval(0,1);
        private int Accuracy = 100;
        public int udegree;
        public int vdegree;
        private PatchFunction Function;
        public SurfacePatch()
        {
            this.udegree = 2;
            this.vdegree = 2;
        }
        public SurfacePatch(Interval UVDomain, PatchFunction Function, int Accuracy = 100) : this()
        {
            this.Function = Function;
            this.UDomain = UVDomain;
            this.VDomain = UVDomain;
            this.Accuracy = Accuracy;
        }
        public List<Point3d> ResultPts = new List<Point3d>();
        public bool Run()
        {
            List<Point3d> PtBags = new List<Point3d>();
            for(double i = UDomain.Min ; i <= UDomain.Max; i += UDomain.Length / (Accuracy - 1))
            {
                for(double j = VDomain.Min ; j <= VDomain.Max; j += VDomain.Length / (Accuracy - 1))
                {
                    var Xv = Function.XFunction(i,j);
                    var Yv = Function.YFunction(i,j);
                    var Zv = Function.ZFunction(i,j);
                    var Pt = new Rhino.Geometry.Point3d(
                        Xv, Yv, Zv
                    );
                    PtBags.Add(Pt);
                }
            }
            this._ResultSurface = NurbsSurface.CreateFromPoints(PtBags, Accuracy, Accuracy, udegree, vdegree);
            this.ResultPts = PtBags;
            return true;
        }
    }
}
