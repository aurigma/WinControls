// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Freehand vector object create designer.
    /// </summary>
    public class FreehandVObjectCreateDesigner : PathVObjectCreateDesigner
    {
        #region "Construction / destruction"

        protected FreehandVObjectCreateDesigner(bool closePath)
            : this()
        {
            _closePath = closePath;
            _fillMode = System.Drawing.Drawing2D.FillMode.Alternate;
        }

        public FreehandVObjectCreateDesigner()
        {
            base.Pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;

            Reset();
        }

        protected new void Reset()
        {
            base.Reset();
            _points = new System.Collections.ArrayList(PointArrayInitialCapacity);
        }

        #endregion "Construction / destruction"

        #region IDesigner Members

        public override void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_points.Count > 1)
            {
                using (System.Drawing.Pen pen = CreateViewportPen())
                {
                    System.Drawing.Point[] points = (System.Drawing.Point[])_points.ToArray(typeof(System.Drawing.Point));

                    if (_closePath)
                    {
                        if (base.Brush != null)
                        {
                            System.Drawing.Drawing2D.Matrix prevBrushMatrix = PathVObjectCreateDesigner.AdaptBrushToViewport(base.Brush, base.VObjectHost.HostViewer);
                            try
                            {
                                g.FillPolygon(base.Brush, points, _fillMode);
                            }
                            finally
                            {
                                if (prevBrushMatrix != null)
                                    VObjectsUtils.SetBrushMatrix(base.Brush, prevBrushMatrix);
                            }
                        }
                        g.DrawPolygon(pen, points);
                    }
                    else
                    {
                        g.DrawLines(pen, points);
                    }
                }
            }
        }

        public override bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            CreateObjectAndDetach();
            return true;
        }

        public override bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            _points.Add(new System.Drawing.Point(e.X, e.Y));
            base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
            return true;
        }

        public override bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (_points.Count > 0)
            {
                System.Drawing.Point lastPoint = (System.Drawing.Point)_points[_points.Count - 1],
                                     newPoint = new System.Drawing.Point(e.X, e.Y);

                if (System.Math.Abs(lastPoint.X - newPoint.X) + System.Math.Abs(lastPoint.Y - newPoint.Y) > MinPointDistance)
                {
                    _points.Add(newPoint);
                    base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
                }
            }
            return true;
        }

        #endregion IDesigner Members

        protected override IVObject CreateObject()
        {
            System.Drawing.PointF[] convertedPoints = new System.Drawing.PointF[_points.Count];
            for (int i = 0; i < convertedPoints.Length; i++)
                convertedPoints[i] = base.VObjectHost.HostViewer.ControlToWorkspace((System.Drawing.Point)_points[i], Aurigma.GraphicsMill.Unit.Point);

            FreehandVObject obj = new FreehandVObject(convertedPoints, _closePath, _fillMode);

            if (base.Brush != null)
                obj.Brush = (System.Drawing.Brush)base.Brush.Clone();
            else
                obj.Brush = null;

            if (base.Pen != null)
                obj.Pen = (System.Drawing.Pen)base.Pen.Clone();
            else
                obj.Pen = null;

            return obj;
        }

        protected override System.Drawing.Rectangle GetInvalidationRectangle()
        {
            System.Drawing.Rectangle result = VObjectsUtils.GetBoundingRectangle((System.Drawing.Point[])_points.ToArray(typeof(System.Drawing.Point)));

            if (base.Pen != null)
            {
                int inflateValue = (int)System.Math.Ceiling(base.Pen.Width * base.VObjectHost.HostViewer.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));
                result.Inflate(inflateValue, inflateValue);
            }

            return result;
        }

        protected System.Drawing.Drawing2D.FillMode FillMode
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

                    if (base.VObjectHost != null)
                        base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
                }
            }
        }

        #region "Member constants & variables"

        private const int MinPointDistance = 10;
        private const int PointArrayInitialCapacity = 1000;

        private System.Collections.ArrayList _points;
        private bool _closePath;

        private System.Drawing.Drawing2D.FillMode _fillMode;

        #endregion "Member constants & variables"
    }

    public class ClosedFreehandVObjectCreateDesigner : FreehandVObjectCreateDesigner
    {
        public ClosedFreehandVObjectCreateDesigner()
            : base(true)
        {
            base.BrushInternal = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 255, 255));
        }

        public new System.Drawing.Brush Brush
        {
            get
            {
                return base.Brush;
            }
            set
            {
                base.Brush = value;
            }
        }

        public new System.Drawing.Drawing2D.FillMode FillMode
        {
            get
            {
                return base.FillMode;
            }
            set
            {
                base.FillMode = value;
            }
        }
    }
}