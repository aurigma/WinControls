// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base class for all caching renderers. Inheritors should implement DrawNonCachedArea() & InvalidateLayerRegion() methods.
    /// </summary>
    internal abstract class CachingRendererImpl : IViewportImageRenderer, System.IDisposable
    {
        #region "------- Member variables ---------"

        private ViewportImageCache _viewportCache;
        private float _renderingResolution;

        #endregion "------- Member variables ---------"

        protected CachingRendererImpl(float renderingResolution)
        {
            if (renderingResolution < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("renderingResolution");

            _renderingResolution = renderingResolution;
            _viewportCache = new ViewportImageCache();
        }

        public void Render(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            if (viewport.Width < 1 || viewport.Height < 1 || renderingRegion.Width < 1 || renderingRegion.Height < 1)
                return;

            if (canvas == null)
                throw new System.ArgumentNullException("canvas");
            if (canvas.IsEmpty)
                throw new System.ArgumentException(StringResources.GetString("ExStrBitmapCannotBeEmpty"), "canvas");
            if (!viewport.Contains(renderingRegion))
                throw new System.ArgumentException(StringResources.GetString("ExStrRenderingRegionShouldBeInsideViewport"), "renderingRegion");

            System.Drawing.Rectangle viewportInvalidatedRect = CoordinateMapper.WorkspaceToControl(this.InvalidatedRegion, zoom, System.Drawing.Point.Empty, Aurigma.GraphicsMill.Unit.Point, _renderingResolution);
            if (renderingRegion.IntersectsWith(viewportInvalidatedRect) || !_viewportCache.IsEntirelyInCache(zoom, renderingRegion))
            {
                BuildUpViewportImage(canvas, zoom, viewport, viewportInvalidatedRect);
                this.InvalidatedRegion = System.Drawing.RectangleF.Empty;
                _viewportCache.UpdateCache(canvas, zoom, viewport);
            }
            else
            {
                System.Diagnostics.Debug.Assert(_viewportCache.IsEntirelyInCache(zoom, renderingRegion), "At this point we should already have actual image in cache.");
                _viewportCache.DrawCached(canvas, zoom, viewport, renderingRegion);
            }
        }

        /// <summary>
        /// Method generates actual image for specified viewport.
        /// </summary>
        private void BuildUpViewportImage(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle invalidatedRect)
        {
            if (canvas.Width != viewport.Width || canvas.Height != viewport.Height)
                throw new System.ArgumentException(StringResources.GetString("ExStrCanvasAndViewportDimsShouldBeEqual"));

            // We should take into account 2 issues here
            // 1) Shift or resize of the viewport.
            // 2) Invalidated areas.
            //
            // Note: Current implementation may draw twice invalidatedRect in some case. Taking this case
            // into account would cause code complication. I think that it is a rather rare case, because invalidatedRect
            // is usually empty during scrolling. On the other hand, it is not empty during
            // object editing, but in such case viewportOrigin is not changed. In both of these most regular cases
            // invalidatedRect will be drawn once.

            // Drawing actual cached region of the viewportOrigin.
            System.Drawing.Rectangle cachedRegion = _viewportCache.GetActualRegion(zoom, viewport);
            System.Drawing.Rectangle[] cachedActualRegions = VObjectsUtils.SubstractRectangle(cachedRegion, invalidatedRect);

            for (int i = 0; i < cachedActualRegions.Length; i++)
            {
                if (cachedActualRegions[i].Width < 1 || cachedActualRegions[i].Height < 1)
                    continue;

                using (var canvasGdiGraphics = canvas.GetGraphics())
                {
                    _viewportCache.DrawCached(canvasGdiGraphics, zoom, viewport, cachedActualRegions[i]);
                }
            }

            // Drawing non-cached parts of the viewportOrigin image.
            invalidatedRect.Intersect(viewport);
            if (invalidatedRect.Width > 0 && invalidatedRect.Height > 0)
                DrawNoncachedArea(canvas, zoom, viewport, invalidatedRect);

            System.Drawing.Rectangle[] newRegions = VObjectsUtils.SubstractRectangle(viewport, cachedRegion);
            for (int i = 0; i < newRegions.Length; i++)
            {
                newRegions[i].Intersect(viewport);

                if (newRegions[i].Width < 1 || newRegions[i].Height < 1)
                    continue;

                DrawNoncachedArea(canvas, zoom, viewport, newRegions[i]);
            }
        }

        protected abstract void DrawNoncachedArea(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle rect);

        public abstract void InvalidateLayerRegion(Layer layer, System.Drawing.RectangleF invalidationRectangle);

        /// <summary>
        /// Inheritors should implement this property and update it in implementation of the InvalidateLayerRegion.
        /// </summary>
        protected abstract System.Drawing.RectangleF InvalidatedRegion
        {
            get;
            set;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            try
            {
                _viewportCache.Dispose();
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable Members
    }
}