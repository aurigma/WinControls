// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Polyline vector object create designer
    /// </summary>
    public class PolyLineVObjectCreateDesigner : PathVObjectCreateDesigner
    {
        protected PolyLineVObjectCreateDesigner(bool closePath)
            : base()
        {
            _closePath = closePath;
            _fillMode = System.Drawing.Drawing2D.FillMode.Alternate;

            Reset();
        }

        public PolyLineVObjectCreateDesigner()
            : this(false)
        {
            base.BrushInternal = null;
        }

        protected new void Reset()
        {
            base.Reset();

            _points = new System.Collections.ArrayList();
            _workspaceMousePosition = System.Drawing.PointF.Empty;
            _viewportMousePosition = System.Drawing.Point.Empty;
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_points.Count > 0)
            {
                System.Drawing.Point[] points = GetViewportPoints(_points.Count + 1);
                points[points.Length - 1] = _viewportMousePosition;

                if (_closePath)
                {
                    if (base.Brush != null)
                    {
                        System.Drawing.Drawing2D.Matrix prevBrushMatrix = PathVObjectCreateDesigner.AdaptBrushToViewport(base.Brush, this.VObjectHost.HostViewer);
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
                    if (base.Pen != null)
                        g.DrawPolygon(base.CreateViewportPen(), points);
                }
                else
                {
                    if (base.Pen != null)
                        g.DrawLines(base.CreateViewportPen(), points);
                }
            }
        }

        #region IDesigner Members

        public override bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _points.Add(base.VObjectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point));
                base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                base.CreateObjectAndDetach();
            }

            return true;
        }

        public override bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            System.Drawing.Rectangle invalidationRect = GetInvalidationRectangle();

            _viewportMousePosition = new System.Drawing.Point(e.X, e.Y);
            _workspaceMousePosition = base.VObjectHost.HostViewer.ControlToWorkspace(_viewportMousePosition, Aurigma.GraphicsMill.Unit.Point);

            base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(System.Drawing.Rectangle.Union(invalidationRect, GetInvalidationRectangle())));
            return true;
        }

        #endregion IDesigner Members

        protected override System.Drawing.Rectangle GetInvalidationRectangle()
        {
            System.Drawing.Rectangle result = System.Drawing.Rectangle.Empty;
            if (_points.Count > 0)
            {
                System.Drawing.Point[] points = GetViewportPoints(_points.Count + 1);
                points[points.Length - 1] = _viewportMousePosition;
                result = VObjectsUtils.GetBoundingRectangle(points);

                int inflateValue = (int)System.Math.Ceiling(System.Math.Max(1.0f, 2 * base.Pen.Width * base.VObjectHost.HostViewer.Zoom));
                result.Inflate(inflateValue, inflateValue);
            }

            return result;
        }

        protected override IVObject CreateObject()
        {
            PolylineVObject result = null;
            if (_points.Count > 1)
            {
                System.Drawing.PointF[] points = (System.Drawing.PointF[])_points.ToArray(typeof(System.Drawing.PointF));
                System.Drawing.RectangleF pointsBounds = VObjectsUtils.GetBoundingRectangle(points);
                if (pointsBounds.Width > 1.0f && pointsBounds.Height > 1.0f)
                {
                    result = new PolylineVObject(points, _closePath, _fillMode);
                    result.Pen = base.Pen;
                    result.Brush = base.Brush;
                }
            }

            return result;
        }

        private System.Drawing.Point[] GetViewportPoints(int pointArrayLength)
        {
            System.Drawing.Point[] points = new System.Drawing.Point[pointArrayLength];
            for (int i = 0; i < points.Length && i < _points.Count; i++)
                points[i] = base.VObjectHost.HostViewer.WorkspaceToControl((System.Drawing.PointF)_points[i], Aurigma.GraphicsMill.Unit.Point);

            return points;
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
                    base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(GetInvalidationRectangle()));
                }
            }
        }

        #region "Member variables"

        private System.Collections.ArrayList _points;
        private System.Drawing.PointF _workspaceMousePosition;
        private System.Drawing.Point _viewportMousePosition;

        private System.Drawing.Drawing2D.FillMode _fillMode;
        private bool _closePath;

        #endregion "Member variables"
    }

    public class ClosedPolyLineVObjectCreateDesigner : PolyLineVObjectCreateDesigner
    {
        public ClosedPolyLineVObjectCreateDesigner()
            : base(true)
        {
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