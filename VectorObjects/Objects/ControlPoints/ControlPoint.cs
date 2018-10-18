// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    public enum ControlPointType
    {
        Drag,
        Click,
    }

    public abstract class ControlPoint : System.Runtime.Serialization.ISerializable, System.ICloneable
    {
        #region "Member variables"

        private System.Drawing.PointF _point;
        private bool _enabled;

        #endregion "Member variables"

        #region "Construction / destruction"

        protected ControlPoint()
        {
            _enabled = true;
            _point = System.Drawing.PointF.Empty;
        }

        protected ControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _point = (System.Drawing.PointF)info.GetValue(SerializationNames.ControlPointLocation, typeof(System.Drawing.PointF));
            _enabled = info.GetBoolean(SerializationNames.ControlPointEnabled);
        }

        #endregion "Construction / destruction"

        #region "Public properties"

        public virtual bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public virtual System.Drawing.PointF Location
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
            }
        }

        public virtual float X
        {
            get
            {
                return _point.X;
            }
            set
            {
                _point.X = value;
            }
        }

        public virtual float Y
        {
            get
            {
                return _point.Y;
            }
            set
            {
                _point.Y = value;
            }
        }

        #endregion "Public properties"

        #region "Abstract methods"

        public abstract bool HitTest(System.Drawing.Point point, ICoordinateMapper coordinateMapper);

        public abstract void Draw(System.Drawing.Graphics g, ICoordinateMapper coordinateMapper);

        public abstract System.Drawing.Size GetSize();

        public abstract ControlPointType Type
        {
            get;
        }

        #endregion "Abstract methods"

        #region "Serialization support"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.ControlPointLocation, _point);
            info.AddValue(SerializationNames.ControlPointEnabled, _enabled);
        }

        #endregion "Serialization support"

        #region "ICloneable interface members"

        public abstract object Clone();

        #endregion "ICloneable interface members"
    }

    public abstract class PathControlPoint : ControlPoint
    {
        #region "Member variables"

        private System.Drawing.Drawing2D.GraphicsPath _path;
        private System.Drawing.Brush _brush;
        private System.Drawing.Pen _pen;
        private System.Drawing.Size _size;

        #endregion "Member variables"

        protected PathControlPoint()
        {
        }

        protected PathControlPoint(PathControlPoint obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");

            _path = obj._path;
            _brush = obj.Brush;
            _pen = obj.Pen;
            _size = obj.Size;
        }

        protected PathControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _path = BinarySerializer.DeserializePath((byte[])info.GetValue(SerializationNames.Path, typeof(byte[])));
            _pen = BinarySerializer.DeserializePen((byte[])info.GetValue(SerializationNames.Pen, typeof(byte[])));
            _brush = BinarySerializer.DeserializeBrush((byte[])info.GetValue(SerializationNames.Brush, typeof(byte[])));
            _size = (System.Drawing.Size)info.GetValue(SerializationNames.ControlPointSize, typeof(System.Drawing.Size));
        }

        #region ControlPoint Members

        public override bool HitTest(System.Drawing.Point point, ICoordinateMapper coordinateMapper)
        {
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            bool result = false;
            if (base.Enabled && _path != null)
            {
                System.Drawing.Point convertedPoint = coordinateMapper.WorkspaceToControl(base.Location, Aurigma.GraphicsMill.Unit.Point);

                point.X -= convertedPoint.X;
                point.Y -= convertedPoint.Y;

                result = _path.IsVisible(point);
                if (!result && this.Pen != null)
                    result = _path.IsOutlineVisible(point, this.Pen);
            }

            return result;
        }

        public override void Draw(System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            if (!base.Enabled || _path == null)
                return;

            System.Drawing.Point drawPnt = coordinateMapper.WorkspaceToControl(base.Location, Aurigma.GraphicsMill.Unit.Point);
            using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(1.0f, 0.0f, 0.0f, 1.0f, drawPnt.X, drawPnt.Y))
            {
                g.Transform = m;

                if (_brush != null)
                    g.FillPath(_brush, _path);
                if (this.Pen != null)
                    g.DrawPath(_pen, _path);

                g.Transform = new System.Drawing.Drawing2D.Matrix();
            }
        }

        public override System.Drawing.Size GetSize()
        {
            System.Drawing.Size result = _size;
            if (_pen != null)
            {
                result.Width += (int)(_pen.Width * 2);
                result.Height += (int)(_pen.Width * 2);
            }

            return result;
        }

        #endregion ControlPoint Members

        #region "Public methods"

        public System.Drawing.Brush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }

        public System.Drawing.Pen Pen
        {
            get
            {
                return _pen;
            }
            set
            {
                _pen = value;
            }
        }

        #endregion "Public methods"

        #region "Protected methods"

        protected System.Drawing.Drawing2D.GraphicsPath Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        protected System.Drawing.Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        #endregion "Protected methods"

        #region ISerializable Members

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.Path, BinarySerializer.Serialize(_path));
            info.AddValue(SerializationNames.Pen, BinarySerializer.Serialize(this.Pen));
            info.AddValue(SerializationNames.Brush, BinarySerializer.Serialize(this.Brush));
            info.AddValue(SerializationNames.ControlPointSize, _size);
        }

        #endregion ISerializable Members
    }

    #region "Different control points implementation"

    [System.Serializable]
    public class RectangleControlPoint : PathControlPoint
    {
        protected RectangleControlPoint()
        {
        }

        public RectangleControlPoint(System.Drawing.Size size, System.Drawing.Brush brush, System.Drawing.Pen pen)
        {
            if (size.Width < 1 || size.Height < 1)
                throw new System.ArgumentOutOfRangeException("size");

            base.Path = new System.Drawing.Drawing2D.GraphicsPath();
            base.Path.AddRectangle(new System.Drawing.Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height));

            base.Pen = pen;
            base.Brush = brush;
            base.Size = size;
        }

        public RectangleControlPoint(RectangleControlPoint obj)
            : base(obj)
        {
        }

        protected RectangleControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public override object Clone()
        {
            return new RectangleControlPoint(this);
        }

        public override ControlPointType Type
        {
            get
            {
                return ControlPointType.Drag;
            }
        }
    }

    [System.Serializable]
    public class EllipseControlPoint : RectangleControlPoint
    {
        public EllipseControlPoint(System.Drawing.Size size, System.Drawing.Brush brush, System.Drawing.Pen pen)
        {
            if (size.Width < 1 || size.Height < 1)
                throw new System.ArgumentOutOfRangeException("size");

            base.Path = new System.Drawing.Drawing2D.GraphicsPath();
            base.Path.AddEllipse(new System.Drawing.Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height));

            base.Pen = pen;
            base.Brush = brush;
            base.Size = size;
        }

        public EllipseControlPoint(EllipseControlPoint obj)
            : base(obj)
        {
        }

        protected EllipseControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        protected EllipseControlPoint()
        {
        }
    }

    [System.Serializable]
    public class DiamondControlPoint : PathControlPoint
    {
        public DiamondControlPoint(System.Drawing.Size size, System.Drawing.Brush brush, System.Drawing.Pen pen)
        {
            if (size.Width < 1 || size.Height < 1)
                throw new System.ArgumentOutOfRangeException("size");

            base.Path = new System.Drawing.Drawing2D.GraphicsPath();

            System.Drawing.Point[] points = new System.Drawing.Point[4];
            points[0] = new System.Drawing.Point(-size.Width / 2, 0);
            points[1] = new System.Drawing.Point(0, -size.Height / 2);
            points[2] = new System.Drawing.Point(size.Width / 2, 0);
            points[3] = new System.Drawing.Point(0, size.Height / 2);
            base.Path.AddPolygon(points);

            base.Pen = pen;
            base.Brush = brush;
            base.Size = size;
        }

        public DiamondControlPoint(DiamondControlPoint obj)
            : base(obj)
        {
        }

        protected DiamondControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public override object Clone()
        {
            return new DiamondControlPoint(this);
        }

        public override ControlPointType Type
        {
            get
            {
                return ControlPointType.Drag;
            }
        }
    }

    [System.Serializable]
    public class CrossedEllipseControlPoint : EllipseControlPoint
    {
        public CrossedEllipseControlPoint(System.Drawing.Size size, System.Drawing.Brush brush, System.Drawing.Pen pen)
        {
            base.Path = new System.Drawing.Drawing2D.GraphicsPath();
            base.Path.AddEllipse(-size.Width / 2, -size.Height / 2, size.Width, size.Height);

            base.Path.AddRectangle(new System.Drawing.Rectangle(-size.Width / 2, 0, size.Width, 1));
            base.Path.AddRectangle(new System.Drawing.Rectangle(0, -size.Height / 2, 1, size.Height));

            base.Pen = pen;
            base.Brush = brush;
            base.Size = size;
        }

        protected CrossedEllipseControlPoint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public CrossedEllipseControlPoint(CrossedEllipseControlPoint obj)
            : base(obj)
        {
        }

        public override object Clone()
        {
            return new CrossedEllipseControlPoint(this);
        }
    }

    #endregion "Different control points implementation"
}