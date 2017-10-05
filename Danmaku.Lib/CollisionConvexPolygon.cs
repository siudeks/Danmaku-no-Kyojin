//using System;
//using System.Collections.Generic;

//namespace Danmaku
//{
//    public sealed class CollisionConvexPolygon : CollisionElement
//    {
//        #region Fields

//        public List<Vector2> Vertices
//        {
//            get { return _vertices; }
//            set { _vertices = value; }
//        }

//        public bool IsFilled { get; set; }

//        private List<Vector2> _axes;
//        private List<Vector2> _vertices;
//        private Vector2 _localPosition;
//        private Vector2 _center;
//        private Vector2 _size;
//        private float _healthPoint;

//        #endregion

//        #region Accessors

//        private List<Vector2> GetAxes()
//        {
//            return _axes;
//        }

//        #endregion

//        public CollisionConvexPolygon(object parent, Vector2 relativePosition, List<Vector2> vertices, float healthPoint = 100)
//            : base(parent, relativePosition)
//        {
//            Parent = parent;
//            Vertices = vertices;
//            _axes = new List<Vector2>();
//            _healthPoint = healthPoint;
//            _localPosition = Vector2.Zero;
//            _center = Vector2.Zero;

//            ComputeAxes();
//        }

//        public override bool Intersects(CollisionElement collisionElement)
//        {
//            var polygon = collisionElement as CollisionConvexPolygon;
//            if (polygon != null)
//                return Intersects(polygon);

//            var circle = collisionElement as CollisionCircle;
//            if (circle != null)
//                return Intersects(circle);

//            return collisionElement.Intersects(this);
//        }

//        public Vector2 GetSize()
//        {
//            if (_size == Vector2.Zero)
//                ComputeSize();

//            return _size;
//        }

//        private void ComputeSize()
//        {
//            var min = _vertices[0];
//            var max = _vertices[0];

//            for (int i = 1; i < _vertices.Count; i++)
//            {
//                if (_vertices[i].X < min.X ||
//                    _vertices[i].X.Equals(min.X) && _vertices[i].Y < min.Y)
//                {
//                    min = _vertices[i];
//                }

//                if (_vertices[i].X > max.X ||
//                    _vertices[i].X.Equals(max.X) && _vertices[i].Y > max.Y)
//                {
//                    max = _vertices[i];
//                }
//            }

//            _size = new Vector2(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y));
//        }

//        // Returns the top-left vertex
//        public Vector2 GetLocalPosition()
//        {
//            if (_localPosition == Vector2.Zero)
//                ComputeLocalPosition();

//            return _localPosition;
//        }

//        private void ComputeLocalPosition()
//        {
//            _localPosition = _vertices[0];

//            for (int i = 1; i < _vertices.Count; i++)
//            {
//                if (_vertices[i].X < _localPosition.X ||
//                    _vertices[i].X.Equals(_localPosition.X) && _vertices[i].Y < _localPosition.Y)
//                {
//                    _localPosition = _vertices[i];
//                }
//            }
//        }

//        public override Vector2 GetCenter()
//        {
//            if (_center == Vector2.Zero)
//                ComputeCenter();

//            return _center;
//        }

//        // Compute the center of the shape (it corresponds to what we call the "centroid")
//        private void ComputeCenter()
//        {
//            var center = Vector2.Zero;
//            var previousCenter = Vector2.Zero;
//            for (var i = 0; i < Vertices.Count; i++)
//            {
//                var currentCenter = (Vertices[i] + Vertices[(i + 1) % Vertices.Count]) / 2f;

//                if (previousCenter != Vector2.Zero)
//                    center = (previousCenter + currentCenter) / 2f;

//                previousCenter = currentCenter;
//            }

//            _center = (center + previousCenter) / 2f;
//        }

//        private bool Overlap(Vector2 p1, Vector2 p2)
//        {
//            // P = (X, Y) with X = min and Y = max
//            return (p1.Y > p2.X && p1.X < p2.Y) || (p2.Y > p1.X && p2.Y < p1.X);
//        }
//    }
//}
