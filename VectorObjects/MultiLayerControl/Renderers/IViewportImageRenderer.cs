// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Inheritors of this interface are responsible for rendering specified part of
    /// the control workspace to the canvas with specified zoom value.
    /// </summary>
    internal interface IViewportImageRenderer : System.IDisposable
    {
        /// <summary>
        /// Render part of the workspace with specified zoom.
        /// </summary>
        /// <param name="canvas">Destination bitmap. Its dimensions should be equal to the viewport ones.</param>
        /// <param name="zoom">Rendering zoom.</param>
        /// <param name="viewport">Part of the workspace to render in absolute viewport coordinates.</param>
        /// <param name="renderingRect">Part of the viewport that should be rendered. In absolute viewport coordinates.</param>
        void Render(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRect);

        /// <summary>
        /// Invalidates specified layer. If layer == null - all layers should be invalidated.
        /// </summary>
        /// <param name="layer">Invalidated layer.</param>
        /// <param name="invalidationRect">Invalidation area in workspace coordinates.</param>
        void InvalidateLayerRegion(Layer layer, System.Drawing.RectangleF invalidationRect);
    }
}