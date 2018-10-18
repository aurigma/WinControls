// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Vector object draw mode. Draft mode is used during
    /// dragging/rotating/scaling object to increase performance.
    /// </summary>
    public enum VObjectDrawMode
    {
        Draft,
        Normal
    }

    /// <summary>
    /// Base interface of the vector objects.
    /// </summary>
    public interface IVObject : IControlPointsProvider
    {
        VObjectDrawMode DrawMode
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        object Tag
        {
            get;
            set;
        }

        bool Locked
        {
            get;
            set;
        }

        IDesigner Designer
        {
            get;
        }

        System.Drawing.Drawing2D.Matrix Transform
        {
            get;
            set;
        }

        System.Drawing.RectangleF GetVObjectBounds();

        /// <summary>
        /// Returns object's current bounds in workspace coordinates.
        /// </summary>
        /// <returns></returns>
        System.Drawing.RectangleF GetTransformedVObjectBounds();

        bool HitTest(System.Drawing.PointF point, float precisionDelta);

        /// <summary>
        /// Draws the object on the specified Graphics.
        /// </summary>
        /// <param name="renderingRect">Invalidated area in relative viewport coordinates.</param>
        /// <param name="g">Destination graphics object.</param>
        /// <param name="coordinateMapper">Output coordinates mapper object.</param>
        void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper);

        void Update();

        event System.EventHandler Changed;
    }
}