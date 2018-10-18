// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Implementation of the simplest coordinates mapper class.
    /// </summary>
    internal class CoordinateMapper : ICoordinateMapper
    {
        #region "Static part"

        internal static float ConvertPixelsToUnits(float resolution, float value, Aurigma.GraphicsMill.Unit unit)
        {
            if (resolution < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("resolution");

            if (unit == Aurigma.GraphicsMill.Unit.Pixel)
                return value;

            float inches = value / resolution;
            return Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(resolution, inches, Aurigma.GraphicsMill.Unit.Inch, unit);
        }

        public static System.Drawing.PointF ControlToWorkspace(System.Drawing.Point controlPoint, float zoom, System.Drawing.Point viewportOrigin, Aurigma.GraphicsMill.Unit workspaceUnit, float dpi)
        {
            float x, y;
            x = (controlPoint.X + viewportOrigin.X) / zoom;
            y = (controlPoint.Y + viewportOrigin.Y) / zoom;

            if (workspaceUnit != Aurigma.GraphicsMill.Unit.Pixel)
            {
                x = ConvertPixelsToUnits(dpi, x, workspaceUnit);
                y = ConvertPixelsToUnits(dpi, y, workspaceUnit);
            }

            return new System.Drawing.PointF(x, y);
        }

        public static System.Drawing.Point WorkspaceToControl(System.Drawing.PointF workspacePoint, float zoom, System.Drawing.Point viewportOrigin, Aurigma.GraphicsMill.Unit workspaceUnit, float dpi)
        {
            if (workspaceUnit != Aurigma.GraphicsMill.Unit.Pixel)
            {
                workspacePoint.X = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(dpi, workspacePoint.X * zoom, workspaceUnit);
                workspacePoint.Y = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToPixels(dpi, workspacePoint.Y * zoom, workspaceUnit);
            }

            int x, y;
            x = (int)(workspacePoint.X - viewportOrigin.X);
            y = (int)(workspacePoint.Y - viewportOrigin.Y);

            return new System.Drawing.Point(x, y);
        }

        public static System.Drawing.RectangleF ControlToWorkspace(System.Drawing.Rectangle controlRect, float zoom, System.Drawing.Point viewportOrigin, Aurigma.GraphicsMill.Unit workspaceUnit, float dpi)
        {
            System.Drawing.PointF lt, rb;
            lt = ControlToWorkspace(controlRect.Location, zoom, viewportOrigin, workspaceUnit, dpi);
            rb = ControlToWorkspace(new System.Drawing.Point(controlRect.Right, controlRect.Bottom), zoom, viewportOrigin, workspaceUnit, dpi);

            return System.Drawing.RectangleF.FromLTRB(lt.X, lt.Y, rb.X, rb.Y);
        }

        public static System.Drawing.Rectangle WorkspaceToControl(System.Drawing.RectangleF workspaceRect, float zoom, System.Drawing.Point viewportOrigin, Aurigma.GraphicsMill.Unit workspaceUnit, float dpi)
        {
            System.Drawing.Point lt, rb;
            lt = WorkspaceToControl(workspaceRect.Location, zoom, viewportOrigin, workspaceUnit, dpi);
            rb = WorkspaceToControl(new System.Drawing.PointF(workspaceRect.Right, workspaceRect.Bottom), zoom, viewportOrigin, workspaceUnit, dpi);

            return System.Drawing.Rectangle.FromLTRB(lt.X, lt.Y, rb.X, rb.Y);
        }

        #endregion "Static part"

        public CoordinateMapper()
        {
            _resolution = 96;
            _zoom = 1.0f;
            _viewport = System.Drawing.Rectangle.Empty;
        }

        #region ICoordinateMapper Members

        public System.Drawing.RectangleF ControlToWorkspace(System.Drawing.Rectangle controlRectangle, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            return CoordinateMapper.ControlToWorkspace(controlRectangle, _zoom, _viewport.Location, workspaceUnit, _resolution);
        }

        public System.Drawing.Rectangle WorkspaceToControl(System.Drawing.RectangleF workspaceRect, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            return CoordinateMapper.WorkspaceToControl(workspaceRect, _zoom, _viewport.Location, workspaceUnit, _resolution);
        }

        public System.Drawing.PointF ControlToWorkspace(System.Drawing.Point controlPoint, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            return CoordinateMapper.ControlToWorkspace(controlPoint, _zoom, _viewport.Location, workspaceUnit, _resolution);
        }

        public System.Drawing.Point WorkspaceToControl(System.Drawing.PointF workspacePoint, Aurigma.GraphicsMill.Unit workspaceUnit)
        {
            return CoordinateMapper.WorkspaceToControl(workspacePoint, _zoom, _viewport.Location, workspaceUnit, _resolution);
        }

        public float GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit unit)
        {
            float result;
            if (unit == Aurigma.GraphicsMill.Unit.Pixel)
                result = _zoom;
            else
            {
                float inches = Aurigma.GraphicsMill.UnitConverter.ConvertUnitsToUnits(_resolution, 1.0f, unit, Aurigma.GraphicsMill.Unit.Inch);
                result = inches * _resolution * _zoom;
            }

            return result;
        }

        public float GetControlPixelsPerUnitY(Aurigma.GraphicsMill.Unit unit)
        {
            return GetControlPixelsPerUnitX(unit);
        }

        #endregion ICoordinateMapper Members

        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (value < VObject.Eps)
                    throw new System.ArgumentOutOfRangeException("value", StringResources.GetString("ExStrValueIsTooSmall"));
                _zoom = value;
            }
        }

        public float Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                if (value < VObject.Eps)
                    throw new System.ArgumentOutOfRangeException("value");

                _resolution = value;
            }
        }

        public System.Drawing.Rectangle Viewport
        {
            get
            {
                return _viewport;
            }
            set
            {
                _viewport = value;
            }
        }

        #region "------- Member variables ---------"

        private float _zoom;
        private System.Drawing.Rectangle _viewport;
        private float _resolution;

        #endregion "------- Member variables ---------"
    }
}