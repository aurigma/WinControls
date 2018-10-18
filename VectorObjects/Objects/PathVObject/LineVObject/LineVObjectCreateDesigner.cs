// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Line vector object create designer.
    /// </summary>
    public class LineVObjectCreateDesigner : PathVObjectCreateDesigner
    {
        #region "Construction / destruction"

        public LineVObjectCreateDesigner()
        {
        }

        #endregion "Construction / destruction"

        #region "Functionality implementation"

        public override void NotifyConnect(IVObjectHost objectHost)
        {
            base.NotifyConnect(objectHost);
            _dragging = false;
        }

        public override bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!_dragging && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _dragging = true;
                _point0 = base.VObjectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point);
                _point1 = _point0;
            }

            return base.NotifyMouseDown(e);
        }

        public override bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (_dragging && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.CreateObjectAndDetach();
            }

            return base.NotifyMouseUp(e);
        }

        public override bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (_dragging)
            {
                System.Drawing.PointF[] points = new System.Drawing.PointF[3];
                points[0] = _point0;
                points[1] = _point1;

                _point1 = base.VObjectHost.HostViewer.ControlToWorkspace(new System.Drawing.Point(e.X, e.Y), Aurigma.GraphicsMill.Unit.Point);
                points[2] = _point1;

                System.Drawing.RectangleF workspaceChangedRect = VObjectsUtils.GetBoundingRectangle(points);
                if (base.Pen != null)
                    workspaceChangedRect.Inflate(base.Pen.Width * 2, base.Pen.Width * 2);

                System.Drawing.Rectangle invalidationRect = base.VObjectHost.HostViewer.WorkspaceToControl(workspaceChangedRect, Aurigma.GraphicsMill.Unit.Point);
                invalidationRect.Inflate(VObject.InvalidationMargin);
                base.VObjectHost.HostViewer.InvalidateViewer(new MultiLayerViewerInvalidationTarget(invalidationRect));
            }

            return base.NotifyMouseMove(e);
        }

        protected override System.Drawing.Rectangle GetInvalidationRectangle()
        {
            System.Drawing.Rectangle result = System.Drawing.Rectangle.Empty;
            if (_dragging)
            {
                System.Drawing.RectangleF rect = GetWorkspaceRectangle();
                if (base.Pen != null)
                    rect.Inflate(base.Pen.Width * 2, base.Pen.Width * 2);

                result = base.VObjectHost.HostViewer.WorkspaceToControl(rect, Aurigma.GraphicsMill.Unit.Point);
                result.Inflate(VObject.InvalidationMargin.Width, VObject.InvalidationMargin.Width);
            }

            return result;
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_dragging)
            {
                System.Drawing.Point controlPoint0 = base.VObjectHost.HostViewer.WorkspaceToControl(_point0, Aurigma.GraphicsMill.Unit.Point),
                                     controlPoint1 = base.VObjectHost.HostViewer.WorkspaceToControl(_point1, Aurigma.GraphicsMill.Unit.Point);

                if (base.Pen != null)
                    using (System.Drawing.Pen pen = CreateViewportPen())
                        g.DrawLine(pen, controlPoint0, controlPoint1);
            }
        }

        protected override IVObject CreateObject()
        {
            LineVObject result = null;

            if ((_point0.X - _point1.X) * (_point0.X - _point1.X) + (_point0.Y - _point1.Y) * (_point0.Y - _point1.Y) > 1)
            {
                result = new LineVObject(_point0, _point1);
                result.Brush = null;
                result.Pen = (System.Drawing.Pen)base.Pen.Clone();
            }

            return result;
        }

        protected System.Drawing.Rectangle GetViewportRectangle()
        {
            return base.VObjectHost.HostViewer.WorkspaceToControl(GetWorkspaceRectangle(), Aurigma.GraphicsMill.Unit.Point);
        }

        protected System.Drawing.RectangleF GetWorkspaceRectangle()
        {
            return VObjectsUtils.GetBoundingRectangle(_point0, _point1);
        }

        #endregion "Functionality implementation"

        #region "Protected methods for inheritors"

        protected System.Drawing.PointF WorkspacePoint0
        {
            get
            {
                return _point0;
            }
        }

        protected System.Drawing.PointF WorkspacePoint1
        {
            get
            {
                return _point1;
            }
        }

        protected bool Dragging
        {
            get
            {
                return _dragging;
            }
        }

        #endregion "Protected methods for inheritors"

        #region "Member variables"

        private System.Drawing.PointF _point0;
        private System.Drawing.PointF _point1;
        private bool _dragging;

        #endregion "Member variables"
    }
}