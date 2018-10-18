// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// The implementation of the IViewportImageRenderer interface renders vector object layers.
    /// No caching, no optimizations except clipping not visible objects.
    /// </summary>
    internal class LayerRenderer : IViewportImageRenderer
    {
        #region "------- Member variables ---------"

        private Layer _layer;
        private float _renderingResolution;

        #endregion "------- Member variables ---------"

        public LayerRenderer(Layer layer, float renderingResolution)
        {
            if (renderingResolution < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("renderingResolution");

            _renderingResolution = renderingResolution;
            _layer = layer;
        }

        public void Dispose()
        {
        }

        public void InvalidateLayerRegion(Layer layer, System.Drawing.RectangleF invalidationRectangle)
        {
        }

        public void Render(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            if (!_layer.Visible || _layer.VObjects.Count < 1)
                return;

            CoordinateMapper coordinateMapper = new CoordinateMapper();
            coordinateMapper.Viewport = viewport;
            coordinateMapper.Zoom = zoom;
            coordinateMapper.Resolution = _renderingResolution;

            renderingRegion.X -= viewport.X;
            renderingRegion.Y -= viewport.Y;

            System.IntPtr dc = System.IntPtr.Zero;
            System.IntPtr oldDc = System.IntPtr.Zero;
            System.Drawing.Graphics g = null;
            Aurigma.GraphicsMill.Drawing.Graphics gdiGraphics = null;

            try
            {
                // Read MSDN KB article "GDI & GDI+ interoperability". We should create System.Drawing.Graphics
                // from DC, not from Bitmap to avoid getting sentinel bitmap. Otherwise we will not be able to
                // blend images using Aurigma.GraphicsMill.Bitmap.Draw(System.Drawing.Graphics g, ...) method.
                //
                // The first branch is used when we render to the screen. In this case canvas has 24bppRgb format
                // and we just use its GDI graphics (most probably it has been already created by caching renderer) to
                // obtain HDC. Second branch is used when we render control content to 32bppArgb image - in such case
                // we cannot create GDI graphics and have to manually create DC and select canvas.Handle into it.
                if (canvas.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    gdiGraphics = canvas.GetGraphics();
                    g = System.Drawing.Graphics.FromHdc(gdiGraphics.GetDC());
                }
                else
                {
                    dc = NativeMethods.CreateCompatibleDC(System.IntPtr.Zero);

                    if (dc == System.IntPtr.Zero)
                        throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("Cannot create compatible DC."));

                    g = System.Drawing.Graphics.FromHdc(dc);
                }

                g.SetClip(renderingRegion);

                for (int i = 0; i < _layer.VObjects.Count; i++)
                {
                    System.Drawing.Rectangle bounds = coordinateMapper.WorkspaceToControl(_layer.VObjects[i].GetTransformedVObjectBounds(), Aurigma.GraphicsMill.Unit.Point);

                    if (bounds.IntersectsWith(renderingRegion))
                        _layer.VObjects[i].Draw(renderingRegion, g, coordinateMapper);
                }
            }
            finally
            {
                if (g != null)
                    g.Dispose();

                if (gdiGraphics != null)
                    gdiGraphics.Dispose();

                if (dc != System.IntPtr.Zero)
                {
                    NativeMethods.SelectObject(dc, oldDc);
                    NativeMethods.DeleteDC(dc);
                }
            }
        }
    }
}