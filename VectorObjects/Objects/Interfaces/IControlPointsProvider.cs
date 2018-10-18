// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Control point collection interface.
    /// </summary>
    public interface IControlPointCollection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        ControlPoint this[int index]
        {
            get;
        }
    }

    public interface IVObjectActionCollection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        IVObjectAction this[int actionId]
        {
            get;
        }

        bool Contains(int actionId);
    }

    /// <summary>
    /// Allows to change the underlying object by dragging or clicking the exposed control points.
    /// </summary>
    public interface IControlPointsProvider
    {
        IControlPointCollection ControlPoints
        {
            get;
        }

        IVObjectActionCollection SupportedActions
        {
            get;
        }

        /// <summary>
        /// Returns max draw size of the control points. Used during calculation of the invalidation area.
        /// </summary>
        int MaxControlPointRadius
        {
            get;
        }

        System.Windows.Forms.Cursor GetPointCursor(int index);

        /// <summary>
        /// Returns bounding rectangle for all control points. Bounding rectangle is used to find invalidation area.
        /// The method allows to increase performance - internal implementation of the provider could cache
        /// the value and return cached rectangle instead of re-computing it on every call.
        /// </summary>
        /// <returns>Returns bounding rectangle for all control points.</returns>
        System.Drawing.RectangleF GetControlPointsBounds();

        void DragPoint(int index, System.Drawing.PointF newPosition);

        void ClickPoint(int index);
    }
}