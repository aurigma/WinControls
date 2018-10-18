// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Interim object which simplifies work with IControlPointsProvider interface of a vector object.
    /// It provides coordinate mapping functionality for hit test operations and draws control points and
    /// of an object and border around it.
    /// </summary>
    internal sealed class GripsProvider : System.IDisposable
    {
        #region "Constants"

        public const int InvalidPointHandle = -1;

        #endregion "Constants"

        #region "Construction / destruction"

        public GripsProvider(IVObject obj, ViewerBase hostControl)
        {
            if (hostControl == null)
                throw new System.ArgumentNullException("hostControl");

            if (obj == null)
                throw new System.ArgumentNullException("obj");

            _hostControl = hostControl;

            _obj = obj;
            _controlPointsProvider = obj as IControlPointsProvider;

            _objectBorderPen = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 1.0f);
            _objectBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        public void Dispose()
        {
            try
            {
                if (_objectBorderPen != null)
                {
                    try
                    {
                        _objectBorderPen.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                    }
                    finally
                    {
                        _objectBorderPen = null;
                    }
                }
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        #endregion "Construction / destruction"

        #region "Grips click & drag processing"

        public bool HitTest(System.Drawing.Point pnt)
        {
            System.Drawing.PointF convertedPoint = _hostControl.ControlToWorkspace(pnt, Aurigma.GraphicsMill.Unit.Point);
            return _obj.HitTest(convertedPoint, VObject.SelectionPrecisionDelta / _hostControl.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));
        }

        public int TestPoint(System.Drawing.Point pnt)
        {
            if (_controlPointsProvider != null)
            {
                int i = 0;
                for (; i < _controlPointsProvider.ControlPoints.Count; i++)
                    if (_controlPointsProvider.ControlPoints[i].HitTest(pnt, _hostControl))
                        break;

                if (i < _controlPointsProvider.ControlPoints.Count)
                    return i;
            }

            return InvalidPointHandle;
        }

        public void DragPoint(int point, System.Drawing.Point pnt)
        {
            System.Drawing.PointF convertedPoint = _hostControl.ControlToWorkspace(pnt, Aurigma.GraphicsMill.Unit.Point);

            if (_controlPointsProvider != null && point < _controlPointsProvider.ControlPoints.Count)
                _controlPointsProvider.DragPoint(point, convertedPoint);
            else
                throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectPointHandle"), "hPoint");
        }

        public void ClickPoint(int point)
        {
            _controlPointsProvider.ClickPoint(point);
        }

        public System.Windows.Forms.Cursor GetCursor(int point)
        {
            if (point == GripsProvider.InvalidPointHandle)
                throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectPointHandle"), "hPoint");

            if (_controlPointsProvider != null && point > -1 && point < _controlPointsProvider.ControlPoints.Count)
                return _controlPointsProvider.GetPointCursor(point);
            else
                throw new System.ArgumentException(StringResources.GetString("ExStrIncorrectPointHandle"), "hPoint");
        }

        #endregion "Grips click & drag processing"

        #region "Drawing & invalidating grips"

        public void DrawGrips(System.Drawing.Graphics g)
        {
            DrawVObjectBounds(g);

            if (_controlPointsProvider != null)
            {
                for (int i = _controlPointsProvider.ControlPoints.Count - 1; i >= 0; i--)
                    _controlPointsProvider.ControlPoints[i].Draw(g, _hostControl);
            }
        }

        private void DrawVObjectBounds(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (_objectBorderPen != null)
            {
                System.Drawing.RectangleF baseRect = _obj.GetVObjectBounds();
                System.Drawing.PointF[] points = new System.Drawing.PointF[5];
                points[0] = baseRect.Location;
                points[1] = new System.Drawing.PointF(baseRect.Right, baseRect.Top);
                points[2] = new System.Drawing.PointF(baseRect.Right, baseRect.Bottom);
                points[3] = new System.Drawing.PointF(baseRect.Left, baseRect.Bottom);
                points[4] = System.Drawing.PointF.Empty;

                VObjectsUtils.TransformPointsInplace(_obj.Transform, points);
                for (int i = 0; i < points.Length; i++)
                    points[i] = _hostControl.WorkspaceToControl(points[i], Aurigma.GraphicsMill.Unit.Point);

                points[points.Length - 1] = points[0];
                g.DrawLines(_objectBorderPen, points);
            }
        }

        public System.Drawing.Rectangle GetInvalidationRectangle()
        {
            System.Drawing.Rectangle result = System.Drawing.Rectangle.Empty;

            if (_controlPointsProvider != null)
            {
                result = _hostControl.WorkspaceToControl(_controlPointsProvider.GetControlPointsBounds(), Aurigma.GraphicsMill.Unit.Point);
                result.Inflate(_controlPointsProvider.MaxControlPointRadius, _controlPointsProvider.MaxControlPointRadius);
            }

            return result;
        }

        public System.Drawing.Pen VObjectBorderPen
        {
            get
            {
                return _objectBorderPen;
            }
            set
            {
                _objectBorderPen = value;
            }
        }

        #endregion "Drawing & invalidating grips"

        #region "Member variables"

        private IVObject _obj;
        private IControlPointsProvider _controlPointsProvider;
        private ViewerBase _hostControl;
        private System.Drawing.Pen _objectBorderPen;

        #endregion "Member variables"
    }
}