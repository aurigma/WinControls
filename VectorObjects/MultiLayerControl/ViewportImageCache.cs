// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Viewport image cache abstraction. Encapsulates validation of the current cache state,
    /// calculation of the actual part of the cache and drawing cached image.
    /// </summary>
    internal class ViewportImageCache : System.IDisposable
    {
        #region "Member variables"

        private float _zoom;
        private System.Drawing.Rectangle _viewport;
        private Aurigma.GraphicsMill.Bitmap _image;

        #endregion "Member variables"

        #region "Construction / destruction"

        public ViewportImageCache()
        {
            _viewport = System.Drawing.Rectangle.Empty;
            _zoom = -1.0f;
        }

        #endregion "Construction / destruction"

        /// <summary>
        /// Returns true if cached data is actual for specified zoom and cached region intersects witch specified rectangle.
        /// </summary>
        public bool HasActualData(float zoom, System.Drawing.Rectangle rect)
        {
            if (zoom != _zoom || _image == null)
                return false;

            return _viewport.IntersectsWith(rect);
        }

        /// <summary>
        /// Returns true if cache is actual for specified zoom and cached region contains entire specified rectangle.
        /// </summary>
        public bool IsEntirelyInCache(float zoom, System.Drawing.Rectangle rect)
        {
            if (zoom != _zoom || _image == null)
                return false;

            return _viewport.Contains(rect);
        }

        /// <summary>
        /// Returns part of the specified region which is actual for specified zoom and can be restored from the cache.
        /// </summary>
        /// <param name="zoom">Zoom.</param>
        /// <param name="rect">Region (in global viewportOrigin coordinates)</param>
        public System.Drawing.Rectangle GetActualRegion(float zoom, System.Drawing.Rectangle rect)
        {
            if (zoom != _zoom)
                return System.Drawing.Rectangle.Empty;

            return System.Drawing.Rectangle.Intersect(rect, _viewport);
        }

        /// <summary>
        /// A part of the cached image lying inside the specified rectangle is draw on the given bitmap.
        /// No drawing is performed unless the cache is actual for the specified zoom.
        /// </summary>
        /// <returns>Return part of the rect that had been drawn from the cache (in viewportOrigin coordinates).</returns>
        public System.Drawing.Rectangle DrawCached(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            if (!HasActualData(zoom, renderingRegion))
                return System.Drawing.Rectangle.Empty;

            System.Drawing.Rectangle intersection = System.Drawing.Rectangle.Intersect(renderingRegion, _viewport),
                                     dstRect = intersection,
                                     srcRect = intersection;

            srcRect.Offset(-_viewport.X, -_viewport.Y);
            dstRect.Offset(-viewport.X, -viewport.Y);

            using (var ct = new Aurigma.GraphicsMill.Transforms.Crop(srcRect))
            using (var cropResult = ct.Apply(_image))
            {
                canvas.Draw(cropResult, dstRect.Left, dstRect.Top, Aurigma.GraphicsMill.Transforms.CombineMode.Copy);
            }

            return intersection;
        }

        public System.Drawing.Rectangle DrawCached(Aurigma.GraphicsMill.Drawing.Graphics g, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            if (!HasActualData(zoom, renderingRegion))
                return System.Drawing.Rectangle.Empty;

            System.Drawing.Rectangle intersection = System.Drawing.Rectangle.Intersect(renderingRegion, _viewport),
                dstRect = intersection,
                srcRect = intersection;

            srcRect.Offset(-_viewport.X, -_viewport.Y);
            dstRect.Offset(-viewport.X, -viewport.Y);

            g.DrawImage(_image, dstRect, srcRect, Aurigma.GraphicsMill.Transforms.CombineMode.Copy, 1.0f, Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.Low);
            return intersection;
        }

        private void Clear()
        {
            _zoom = -1.0f;

            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
        }

        /// <summary>
        /// Saves a copy of the specified image in the cache.
        /// </summary>
        /// <param name="image">New actual image of the viewport.</param>
        /// <param name="zoom">Current zoom value.</param>
        /// <param name="viewport">Viewport position and coordinates.</param>
        public void UpdateCache(Aurigma.GraphicsMill.Bitmap image, float zoom, System.Drawing.Rectangle viewport)
        {
            if (image == null)
                throw new System.ArgumentNullException("image");
            if (viewport.Width != image.Width || viewport.Height != image.Height)
                throw new System.ArgumentException(StringResources.GetString("ExStrImageAndViewportDimsShouldBeEqual"), "image");

            _zoom = zoom;
            _viewport = viewport;

            _image = new Aurigma.GraphicsMill.Bitmap(image);
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Clear();
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable Members
    }
}