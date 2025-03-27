using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Linq;
using System;
using Rhino.Geometry;

namespace Tile.Core.Patch
{
    public class SurfacePatch
    {
        private Surface _ResultSurface;
        public Surface ResultSurface { get { return (Surface) _ResultSurface.Duplicate() ; } }
        private Interval UDomain = new Interval(0, 1);
        private Interval VDomain = new Interval(0, 1);
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
            this.Accuracy = Accuracy >= 0 ? Accuracy : 100;
        }
        public SurfacePatch(Interval UDomain, Interval VDomain, PatchFunction Function, int Accuracy = 100) : this()
        {
            this.UDomain = UDomain;
            this.VDomain = VDomain;
            this.Function = Function;
            this.Accuracy = Accuracy >= 0 ? Accuracy : 100;
        }
        public DataTree<Point3d> ResultPts { get; private set; } = new DataTree<Point3d>();
        public bool Run()
        {
            DataTree<Point3d> PtBags = new DataTree<Point3d>();
            int PathCount = 0;
            double u_value = UDomain.Min;
            double v_value = VDomain.Min;
            for (double i = 0; i < Accuracy; i++)
            {
                v_value = VDomain.Min;
                for (double j = 0; j < Accuracy; j++)
                {

                    var Xv = Function.XFunction(u_value, v_value);
                    var Yv = Function.YFunction(u_value, v_value);
                    var Zv = Function.ZFunction(u_value, v_value);
                    var Pt = new Rhino.Geometry.Point3d(
                        Xv, Yv, Zv
                    );

                    v_value += VDomain.Length / (Accuracy - 1);

                    PtBags.Add(Pt, new GH_Path(PathCount));
                }
                u_value += UDomain.Length / (Accuracy - 1);
                PathCount++;
            }
            this._ResultSurface = NurbsSurface.CreateFromPoints(PtBags.AllData(), Accuracy, Accuracy, udegree, vdegree);
            this.ResultPts = PtBags;
            this.SetType();
            return true;
        }
        private void SetType()
        {
            Random Rand = new Random();
            bool IsPrinciple = true;
            MinimalSurface = true;
            bool IsAsymptotic = true;
            for (int i = 0; i < 10; i++)
            {
                var IntR = Rand.Next(0, this.ResultPts.BranchCount);
                var TestC = Curve.CreateInterpolatedCurve(this.ResultPts.Branch(IntR), 2);

                var douT = Rand.NextDouble();

                var T = TestC.Domain.Min + TestC.Domain.Length * douT;

                var Ct = TestC.TangentAt(T);
                var Pt = TestC.PointAt(T);

                var SrfUV = this._ResultSurface.ClosestPoint(Pt, out var u, out var v);

                var StK = this._ResultSurface.CurvatureAt(u, v);

                if (Math.Abs(StK.Direction(0) * Ct) > 1e-6)
                {
                    IsPrinciple = false;
                }
                if (Math.Abs(StK.Mean) > 1e-6 || Math.Abs(StK.Direction(0) * Ct) > 1e-6) 
                {
                    IsAsymptotic = false;
                }
                if(Math.Abs(StK.Mean) > 1e-6)
                   this.MinimalSurface = false;
            }

            var FlipTree = FlipMatrix(ResultPts);

            for (int i = 0; i < 10; i++)
            {
                var IntR = Rand.Next(0, FlipTree.BranchCount);
                var TestC = Curve.CreateInterpolatedCurve(FlipTree.Branch(IntR), 2);

                var douT = Rand.NextDouble();

                var T = TestC.Domain.Min + TestC.Domain.Length * douT;

                var Ct = TestC.TangentAt(T);
                var Pt = TestC.PointAt(T);

                var SrfUV = this._ResultSurface.ClosestPoint(Pt, out var u, out var v);

                var StK = this._ResultSurface.CurvatureAt(u, v);

                if (Math.Abs(StK.Direction(0) * Ct) > 1e-6)
                {
                    IsPrinciple = false;
                }
                if (Math.Abs(StK.Mean) > 1e-6 || Math.Abs(StK.Direction(0) * Ct) > 1e-6) 
                {
                    IsAsymptotic = false;
                }
                if(Math.Abs(StK.Mean) > 1e-6)
                   this.MinimalSurface = false;
            }

            if (IsAsymptotic)
                this.Type = PatchType.AsymptoticPatch;
            else if (IsPrinciple)
                this.Type = PatchType.PrincipalPatch;
            else
                this.Type = PatchType.OtherPatch;

            DataTree<Point3d> FlipMatrix(DataTree<Point3d> dataTree)
            {
                var NewDataTree = new DataTree<Point3d>();
                for (int i = 0; i < dataTree.BranchCount; i++)
                {
                    for (int j = 0; j < dataTree.Branch(i).Count; j++)
                    {
                        // Correct way to transpose: Assigning j as new branch index
                        NewDataTree.Add(dataTree.Branch(i)[j], new GH_Path(j));
                    }
                }
                return NewDataTree;
            }
        }
        public PatchType Type { get; private set; }
        public bool MinimalSurface {get; private set;}
        public void Transform(Transform X, out Surface oSurface, out DataTree<Point3d> PtTrees)
        {
            oSurface = this.ResultSurface;
            oSurface.Transform(X);

            PtTrees = new DataTree<Point3d>();

            for(int i = 0; i < this.ResultPts.BranchCount; i++)
            {
                for(int j = 0; j < this.ResultPts.Branch(i).Count; j++)
                {
                    var Pt = this.ResultPts.Branch(i)[j];
                    Pt.Transform(X);
                    PtTrees.Add(Pt, this.ResultPts.Paths[i]);
                }
            }

        }
    }

    public enum PatchType
    {
        PrincipalPatch,
        AsymptoticPatch,
        OtherPatch
    }
}
