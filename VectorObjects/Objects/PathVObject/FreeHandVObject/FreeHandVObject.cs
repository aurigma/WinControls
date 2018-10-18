// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Free-hand vector object.
    /// </summary>
    [System.Serializable]
    public class FreehandVObject : PathVObject
    {
        #region "Construction / destruction"

        protected FreehandVObject(string objectName, System.Drawing.PointF[] points, bool closePath, System.Drawing.Drawing2D.FillMode fillMode)
            : base(objectName)
        {
            if (points == null)
                throw new System.ArgumentNullException("points");
            if (points.Length == 0)
                throw new System.ArgumentException(StringResources.GetString("ExStrArrayZeroLengthError"), "points");

            _points = points;
            _closePath = closePath;
            _fillMode = fillMode;

            UpdatePath();
        }

        public FreehandVObject(System.Drawing.PointF[] points, bool closePath, System.Drawing.Drawing2D.FillMode fillMode)
            : this("freeHand", points, closePath, fillMode)
        {
        }

        protected FreehandVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _points = BinarySerializer.DeserializePointFArray((byte[])info.GetValue(SerializationNames.FreehandPoints, typeof(byte[])));
            _closePath = info.GetBoolean(SerializationNames.FreehandClosePath);
            _fillMode = (System.Drawing.Drawing2D.FillMode)info.GetInt32(SerializationNames.FreehandFillMode);

            UpdatePath();
        }

        #endregion "Construction / destruction"

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _path != null)
                _path.Dispose();
        }

        public override void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            // FillPath doesn't support specifying FillMode, it always uses FillMode.Alternate,
            // so we have to use Graphics.FillPolygon method in other cases.
            if (!_closePath || base.Brush == null || _fillMode == System.Drawing.Drawing2D.FillMode.Alternate)
            {
                base.Draw(renderingRect, g, coordinateMapper);
            }
            else
            {
                System.Drawing.PointF[] transformedPoints = VObjectsUtils.TransformPoints(base.Transform, _points);
                for (int i = 0; i < transformedPoints.Length; i++)
                    transformedPoints[i] = coordinateMapper.WorkspaceToControl(transformedPoints[i], Aurigma.GraphicsMill.Unit.Point);

                System.Drawing.Drawing2D.SmoothingMode oldSmoothingMode = g.SmoothingMode;
                System.Drawing.Pen pen = base.CreateViewportPen(coordinateMapper);
                try
                {
                    switch (base.DrawMode)
                    {
                        case VObjectDrawMode.Draft:
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                            break;

                        case VObjectDrawMode.Normal:
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            break;

                        default:
                            throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUnexpectedDrawMode"));
                    }

                    if (base.Brush != null)
                    {
                        AdaptBrushToViewport(coordinateMapper);
                        try
                        {
                            g.FillPolygon(base.Brush, transformedPoints, _fillMode);
                        }
                        finally
                        {
                            RestoreBrush();
                        }
                    }
                    if (pen != null)
                        g.DrawPolygon(pen, transformedPoints);
                }
                finally
                {
                    pen.Dispose();
                    g.SmoothingMode = oldSmoothingMode;
                }
            }
        }

        protected void UpdatePath()
        {
            // After deserialization _points array will be null.
            if (_points == null)
                return;

            if (_path != null)
                _path.Dispose();

            System.Drawing.RectangleF bounds = VObjectsUtils.GetBoundingRectangle(_points);
            if (bounds.Width < VObject.Eps)
                _points[0].X += 0.05f;
            if (bounds.Height < VObject.Eps)
                _points[0].Y += 0.05f;

            _path = new System.Drawing.Drawing2D.GraphicsPath();
            _path.AddLines(_points);
            if (_closePath)
                _path.CloseFigure();
        }

        protected override System.Drawing.Drawing2D.GraphicsPath Path
        {
            get
            {
                return _path;
            }
        }

        protected System.Drawing.PointF[] Points
        {
            get
            {
                return _points;
            }
        }

        public System.Drawing.Drawing2D.FillMode FillMode
        {
            get
            {
                return _fillMode;
            }
            set
            {
                if (_fillMode != value)
                {
                    _fillMode = value;
                    OnChanged(System.EventArgs.Empty);
                }
            }
        }

        #region "ISerializable interface implementation"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);

            info.AddValue(SerializationNames.FreehandPoints, BinarySerializer.Serialize(_points));
            info.AddValue(SerializationNames.FreehandClosePath, _closePath);
            info.AddValue(SerializationNames.FreehandFillMode, (int)_fillMode);
        }

        #endregion "ISerializable interface implementation"

        #region "Member variables"

        private System.Drawing.Drawing2D.FillMode _fillMode;
        private System.Drawing.Drawing2D.GraphicsPath _path;
        private System.Drawing.PointF[] _points;
        private bool _closePath;

        #endregion "Member variables"
    }
}