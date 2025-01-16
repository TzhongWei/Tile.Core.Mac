using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using Rhino.Render;

namespace Tile.LSystem.TokenAction
{
    public class TokenPointer
    {
        private int Pointer;
        public Dictionary<int, List<GeometryBase>> Drawing { get; private set; }
        public Transform transform { get; private set; }
        private Stack<Transform> PopAndPush;
        public Dictionary<int, (Transform, string)> History;
        public Dictionary<int, Transform> ActionHistory;
        public Dictionary<string, object> UserString;
        public List<GeometryBase> DisplayGeo
        {
            get
            {
                var _geoms = new List<GeometryBase>();
                foreach (var kvp in Drawing)
                    _geoms.AddRange(kvp.Value);
                return _geoms;
            }
        }
        public TokenPointer()
        {
            this.transform = Transform.Identity;
            this.History = new Dictionary<int, (Transform, string)>();
            this.PopAndPush = new Stack<Transform>();
            this.Drawing = new Dictionary<int, List<GeometryBase>>();
            ActionHistory = new Dictionary<int, Transform>();
            ActionHistory.Add(-1, this.transform);
            Pointer = 0;
        }
        public TokenPointer(Plane InitialLocation) : this()
        {
            this.transform = Transform.PlaneToPlane(Plane.WorldXY, InitialLocation);
            ActionHistory[-1] = this.transform;
        }
        public void NextAction(Transform nextTransform, string ActionDescription)
        {
            this.transform = this.transform * nextTransform;
            this.ActionHistory.Add(Pointer, this.transform);
            this.History.Add(Pointer, (nextTransform, ActionDescription));
            this.Pointer++;
        }
        public bool Pop()
        {
            if (PopAndPush.Count == 0) return false;
            this.transform = this.PopAndPush.Pop();
            this.ActionHistory.Add(Pointer, this.transform);
            this.History.Add(Pointer, (this.transform, "Pop a transformation matrix"));
            this.Pointer++;
            return true;
        }
        public bool Push()
        {
            this.PopAndPush.Push(this.transform);
            this.ActionHistory.Add(Pointer, this.transform);
            this.History.Add(Pointer, (this.transform, "Push a transformation matrix"));
            this.Pointer++;
            return true;
        }
        public void AddDrawing(IEnumerable<GeometryBase> geometryBases)
        {
            try
            {
                this.Drawing.Add(this.Pointer, new List<GeometryBase>(geometryBases));
            }
            catch
            {
                throw new Exception($"Pointer {Pointer} step hasn't update yet, please Execute NextAction");
            }
        }
        public void AddDrawing(GeometryBase geometryBase)
        {
            try
            {
                this.Drawing.Add(this.Pointer, new List<GeometryBase> { geometryBase });
            }
            catch
            {
                throw new Exception($"Pointer {Pointer} step hasn't update yet, please Execute NextAction");
            }
        }
        public void NoAction()
        {
            this.NextAction(Transform.Identity, "No Action");
        }
    }
}