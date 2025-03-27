using System;
using System.Collections.Generic;
using System.Threading;
using Eto.Forms;
using Eto.IO;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.FileIO;
using Rhino.Geometry;

namespace Tile.Core.Patch
{
    public class AsymptoticCurvatureGrid
    {
        private SurfacePatch PatchFun = null;
        private Surface _surface;
        public Surface GetSurface() => _surface;
        public DataTree<Vector3d> GetNormalVector(double Offset)
        {
            var Result = new DataTree<Vector3d>();
            for (int i = 0; i < PointOnSurface.BranchCount; i++)
            {
                for (int j = 0; j < PointOnSurface.Branch(PointOnSurface.Path(i)).Count; j++)
                {
                    _surface.ClosestPoint(PointOnSurface.Branch(i)[j], out var u, out var v);
                    Result.Add(_surface.NormalAt(u, v) * Offset, PointOnSurface.Path(i));
                }
            }
            return Result;
        }
        public DataTree<Curve> AsymptoticCurvatureCurve
        {
            get
            {
                var Tree = new DataTree<Curve>();
                for (int i = 0; i < this.PointOnSurface.BranchCount; i++)
                {
                    var Curve = NurbsCurve.Create(false, 2, this.PointOnSurface.Branch(this.PointOnSurface.Path(i)));
                    Tree.Add(Curve, this.PointOnSurface.Path(i));
                }
                return Tree;
            }
        }
        public DataTree<Point3d> PointOnSurface { get; private set; }
        private AsymptoticCurvatureGrid()
        {
            PointOnSurface = new DataTree<Point3d>();
        }
        public static DataTree<Point3d> DiagonalData(DataTree<Point3d> Data)
        {
            // Write your logic here
            int n = Data.BranchCount; // Assuming an n x n structured DataTree
            var diagonalGroups = new Dictionary<int, List<Point3d>>();

            // Traverse the DataTree and group by sum of indices (i + j)
            for (int i = 0; i < n; i++)
            {
                List<Point3d> row = Data.Branch(i);
                for (int j = 0; j < row.Count; j++)
                {
                    int key = i + j;
                    if (!diagonalGroups.ContainsKey(key))
                    {
                        diagonalGroups[key] = new List<Point3d>();
                    }
                    diagonalGroups[key].Add(row[j]);
                }
            }

            // Convert dictionary values to a new DataTree format
            var result = new DataTree<Point3d>();
            foreach (var key in diagonalGroups.Keys)
            {
                result.AddRange(diagonalGroups[key], new GH_Path(key));
            }

            // Output the transformed DataTree
            return result;
        }
        public static AsymptoticCurvatureGrid GridFromParametricFunction(SurfacePatch surfacePatch, List<(double, double)> uv = null)
        {
            var Self = new AsymptoticCurvatureGrid();
            Self.PatchFun = surfacePatch;
            if (uv == null)
            {
                if (Self.PatchFun.Type == PatchType.PrincipalPatch && Self.PatchFun.MinimalSurface)
                {
                    Self._surface = Self.PatchFun.ResultSurface;
                    
                    var ResultTree = new DataTree<Point3d>(Self.PatchFun.ResultPts);
                    var NewTree = new DataTree<Point3d>();
                    for (int i = 0; i < ResultTree.BranchCount; i++)
                    {
                        var GH_Path = ResultTree.Path(i);
                        var ListObj = ResultTree.Branch(GH_Path);
                        ListObj.Reverse();
                        NewTree.AddRange(ListObj, GH_Path);
                    }
                    ResultTree = DiagonalData(ResultTree);
                    NewTree = DiagonalData(NewTree);

                    for(int i = 0; i < ResultTree.BranchCount; i++)
                    {
                        Self.PointOnSurface.AddRange(ResultTree.Branch(i), new GH_Path(0, i));
                    }
                    for(int i = 0; i < NewTree.BranchCount; i++)
                    {
                        Self.PointOnSurface.AddRange(NewTree.Branch(i), new GH_Path(1, i));
                    }
                }
                else
                {
                    Self = AsymptoticCurvatureGrid.GridFromAnticlasticSurface(Self._surface, uv);
                }
            }
            else
            {
                Self = AsymptoticCurvatureGrid.GridFromAnticlasticSurface(Self._surface, uv);
            }

            return Self;
        }

        public static AsymptoticCurvatureGrid GridFromAnticlasticSurface(Surface AnitSurf, List<(double, double)> uv = null, double Step = 0.1)
        {
            if (uv == null)
            {
                uv = new List<(double, double)> { (0.5, 0.5) };
            }
            var Self = new AsymptoticCurvatureGrid();
            Self._surface = AnitSurf;

            int FirstBranch = 0;
            Self.PointOnSurface = new DataTree<Point3d>();
            for (int i = 0; i < uv.Count; i++)
            {
                Self.FindAsymptoticCurvature(Self._surface, uv[i], out var UPts, out var VPts);
                Self.PointOnSurface.AddRange(UPts, new GH_Path(new int[] { FirstBranch, 0 }));
                Self.PointOnSurface.AddRange(VPts, new GH_Path(new int[] { FirstBranch, 1 }));
                FirstBranch++;
            }

            return Self;
        }

        private void FindAsymptoticCurvature(Surface Srf, (double, double) uv, out List<Point3d> DirUPts, out List<Point3d> DirVPts, double Step = 0.1)
        {
            var U = uv.Item1;
            var V = uv.Item2;
            var SrfDU = Srf.Domain(0);
            var SrfDV = Srf.Domain(1);

            if (U > 1 || U < 0 || V > 1 || V < 0)
            {
                throw new Exception("uv setting must be in [0,1]");
            }

            var _SrfDU = SrfDU.Min + SrfDU.Length * U;
            var _SrfDV = SrfDV.Min + SrfDV.Length * V;

            var PtOnSrf = Srf.PointAt(_SrfDU, _SrfDV);
            var CrvK = Srf.CurvatureAt(_SrfDU, _SrfDV);
            var K1Vec = CrvK.Direction(0);
            var K2Vec = CrvK.Direction(1);
            var K1 = CrvK.Kappa(0);
            var K2 = CrvK.Kappa(1);

            if (K1 * K2 > 0)
            {
                DirUPts = new List<Point3d>();
                DirVPts = new List<Point3d>();
                return;
            }
            var VecNormal01 = (K2Vec * K1 - K1Vec * K2) / (K1 - K2); //Interpolate
            var VecNormal02 = (K2Vec * K1 + K1Vec * K2) / (K1 - K2);

            DirUPts = FindAsymptoticCurve(PtOnSrf, VecNormal01, Srf, Step, out var _);
            DirVPts = FindAsymptoticCurve(PtOnSrf, VecNormal02, Srf, Step, out var _);

            return;
        }
        private List<Point3d> FindAsymptoticCurve(Point3d InitLocation,
    Vector3d InitDir, Surface Srf,
    double Step, out List<(double, double)> Test)
        {
            var Reverse = new Vector3d(InitDir);
            Reverse.Reverse();
            var Bag01 = FindAsymptoticCurveSingle(InitLocation, InitDir, Srf, Step, out var Exam1);
            var Bag02 = FindAsymptoticCurveSingle(InitLocation, Reverse, Srf, Step, out var Exam2);
            Bag01.Reverse();
            Bag01.RemoveAt(Bag01.Count - 1);
            Bag01.AddRange(Bag02);
            Test = Exam1;
            Test.AddRange(Exam2);
            return Bag01;

        }

        private List<Point3d> FindAsymptoticCurveSingle(Point3d InitLocation,
        Vector3d InitDir, Surface Srf,
        double Step, out List<(double, double)> Exam)
        {
            Exam = new List<(double, double)>();
            bool HitBoundary = false;
            int Limit = 0;
            var CurrentVector = new Vector3d(InitDir);
            var CurrentLocation = new Point3d(InitLocation);

            var PtBag = new List<Point3d> { InitLocation };
            var UDM = Srf.Domain(0);
            var VDM = Srf.Domain(1);

            while (!HitBoundary && Limit < 5000)
            {
                var NextVector = CurrentVector * Step;

                var NextPt = CurrentLocation + NextVector;

                Srf.ClosestPoint(NextPt, out var u, out var v);

                if (u >= UDM.Max || v >= VDM.Max || u <= UDM.Min || v <= VDM.Min)
                {
                    HitBoundary = true;
                    continue;
                }
                Exam.Add((u, v));
                PtBag.Add(NextPt);
                var CrvK = Srf.CurvatureAt(u, v);

                var K1Vec = CrvK.Direction(0);
                var K2Vec = CrvK.Direction(1);
                var K1 = CrvK.Kappa(0);
                var K2 = CrvK.Kappa(1);

                var VecNormal01 = (K2Vec * K1 - K1Vec * K2) / (K1 - K2); //Interpolate
                var VecNormal02 = (K2Vec * K1 + K1Vec * K2) / (K1 - K2);
                var VecNormal11 = new Vector3d(VecNormal01);
                var VecNormal12 = new Vector3d(VecNormal02);
                VecNormal11.Reverse();
                VecNormal12.Reverse();

                var VecList = new List<Vector3d> { VecNormal01, VecNormal02, VecNormal11, VecNormal12 };

                var Angle = 2 * Math.PI;
                var VectorOut = VecNormal01;
                for (int i = 0; i < 4; i++)
                {
                    var TestAngle = Vector3d.VectorAngle(CurrentVector, VecList[i]);
                    if (TestAngle < Angle)
                    {
                        Angle = TestAngle;
                        VectorOut = VecList[i];
                    }
                }
                CurrentVector = VectorOut;
                CurrentLocation = NextPt;

                Limit++;
            }
            return PtBag;
        }

    }
}