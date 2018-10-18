// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Caching renderer of the multiple vector layers.
    /// </summary>
    internal class MultipleLayerRenderer : CachingRendererImpl
    {
        #region "------- Member variables ---------"

        private static int bgGridCellSize = 15;

        private float _renderingResolution;
        private bool _renderBackground;

        private WorkspaceBackgroundStyle _workspaceBackgroundStyle;
        private System.Drawing.Color _backColor;
        private System.Drawing.Color _workspaceBackColor1;
        private System.Drawing.Color _workspaceBackColor2;

        private IViewportImageRenderer[] _childRenderers;
        private System.Drawing.RectangleF _invalidatedRectangle;
        private Aurigma.GraphicsMill.Bitmap _bgGridTemplate;

        #endregion "------- Member variables ---------"

        public MultipleLayerRenderer(bool renderBackground, float renderingResolution)
            : base(renderingResolution)
        {
            if (renderingResolution < 1)
                throw new System.ArgumentOutOfRangeException("renderResolution");

            _renderBackground = renderBackground;
            _renderingResolution = renderingResolution;

            _childRenderers = new IViewportImageRenderer[0];
            _invalidatedRectangle = System.Drawing.RectangleF.Empty;

            _workspaceBackgroundStyle = Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle.Grid;
            _workspaceBackColor1 = System.Drawing.Color.LightGray;
            _workspaceBackColor2 = System.Drawing.Color.DarkGray;
            _backColor = System.Drawing.Color.LightGray;
        }

        private void DisposeLayerRenders()
        {
            for (int i = 0; i < _childRenderers.Length; i++)
            {
                _childRenderers[i].Dispose();
                _childRenderers[i] = null;
            }

            _childRenderers = new IViewportImageRenderer[0];
        }

        public void SetLayers(LayerCollection layers)
        {
            if (layers == null)
                throw new System.ArgumentNullException("layers");

            DisposeLayerRenders();

            _childRenderers = new IViewportImageRenderer[layers.Count];
            for (int i = 0; i < _childRenderers.Length; i++)
                _childRenderers[i] = new LayerRenderer(layers[i], _renderingResolution);
        }

        protected override void DrawNoncachedArea(Aurigma.GraphicsMill.Bitmap canvas, float zoom, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            if (_renderBackground)
                DrawViewportBackground(canvas, viewport, renderingRegion);

            for (int i = 0; i < _childRenderers.Length; i++)
                _childRenderers[i].Render(canvas, zoom, viewport, renderingRegion);
        }

        public override void InvalidateLayerRegion(Layer layer, System.Drawing.RectangleF invalidationRectangle)
        {
            for (int i = 0; i < _childRenderers.Length; i++)
                _childRenderers[i].InvalidateLayerRegion(layer, invalidationRectangle);

            if (this.InvalidatedRegion.IsEmpty)
                this.InvalidatedRegion = invalidationRectangle;
            else
                this.InvalidatedRegion = System.Drawing.RectangleF.Union(this.InvalidatedRegion, invalidationRectangle);
        }

        protected override System.Drawing.RectangleF InvalidatedRegion
        {
            get
            {
                return _invalidatedRectangle;
            }
            set
            {
                _invalidatedRectangle = value;
            }
        }

        #region "Viewport background rendering"

        private void CreateBackgroundGridTemplate(int width)
        {
            if (width < 1)
                throw new System.ArgumentOutOfRangeException("width", StringResources.GetString("ExStrValueShouldBeAboveZero"));
            if (_bgGridTemplate != null && _bgGridTemplate.Width >= width)
                return;

            if (_bgGridTemplate != null)
                _bgGridTemplate.Dispose();

            _bgGridTemplate = new Aurigma.GraphicsMill.Bitmap(width, 2 * bgGridCellSize, Aurigma.GraphicsMill.PixelFormat.Format24bppRgb);

            using (Aurigma.GraphicsMill.Drawing.Graphics g = _bgGridTemplate.GetGraphics())
            {
                Aurigma.GraphicsMill.Drawing.SolidBrush brush0 = new Aurigma.GraphicsMill.Drawing.SolidBrush(_workspaceBackColor1),
                                                        brush1 = new Aurigma.GraphicsMill.Drawing.SolidBrush(_workspaceBackColor2);

                System.Drawing.Rectangle cellRect = new System.Drawing.Rectangle(0, 0, bgGridCellSize, bgGridCellSize);

                int n = (int)System.Math.Ceiling((float)width / (2.0f * bgGridCellSize));
                for (int i = 0; i < n; i++)
                {
                    g.FillRectangle(brush0, cellRect);
                    cellRect.Offset(bgGridCellSize, 0);
                    g.FillRectangle(brush1, cellRect);
                    cellRect.Offset(bgGridCellSize, 0);
                }

                cellRect.Location = new System.Drawing.Point(0, bgGridCellSize);
                for (int i = 0; i < n; i++)
                {
                    g.FillRectangle(brush1, cellRect);
                    cellRect.Offset(bgGridCellSize, 0);
                    g.FillRectangle(brush0, cellRect);
                    cellRect.Offset(bgGridCellSize, 0);
                }
            }
        }

        private void DrawViewportBackground(Aurigma.GraphicsMill.Bitmap canvas, System.Drawing.Rectangle viewport, System.Drawing.Rectangle renderingRegion)
        {
            using (Aurigma.GraphicsMill.Drawing.Graphics g = canvas.GetGraphics())
            {
                System.Drawing.Rectangle screenRect = renderingRegion;
                screenRect.X -= viewport.X;
                screenRect.Y -= viewport.Y;

                if (_workspaceBackgroundStyle == Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle.Grid)
                {
                    int gridPatternSize = 2 * bgGridCellSize;
                    int patternOffsetX = renderingRegion.X % gridPatternSize,
                        patternOffsetY = renderingRegion.Y % gridPatternSize;

                    CreateBackgroundGridTemplate(renderingRegion.Width + gridPatternSize);

                    System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(0, 0, _bgGridTemplate.Width, _bgGridTemplate.Height),
                                             dstRect = new System.Drawing.Rectangle(renderingRegion.Location, srcRect.Size);
                    dstRect.Offset(-viewport.X, -viewport.Y);
                    dstRect.Offset(-patternOffsetX, -patternOffsetY);

                    g.SetClip(new System.Drawing.Rectangle(renderingRegion.X - viewport.X, renderingRegion.Y - viewport.Y, renderingRegion.Width, renderingRegion.Height));
                    try
                    {
                        int templateRepeats = (int)System.Math.Ceiling((float)(renderingRegion.Height + patternOffsetY) / _bgGridTemplate.Height);
                        for (int j = 0; j < templateRepeats; j++)
                        {
                            g.DrawImage(_bgGridTemplate, dstRect, /*srcRect,*/ Aurigma.GraphicsMill.Transforms.CombineMode.Copy, 1.0f, Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode.NearestNeighbour);
                            dstRect.Offset(0, _bgGridTemplate.Height);
                        }
                    }
                    finally
                    {
                        g.ResetClip();
                    }
                }
                else if (_workspaceBackgroundStyle == Aurigma.GraphicsMill.WinControls.WorkspaceBackgroundStyle.Solid)
                {
                    g.FillRectangle(new Aurigma.GraphicsMill.Drawing.SolidBrush(_workspaceBackColor1), screenRect);
                }
                else
                {
                    g.FillRectangle(new Aurigma.GraphicsMill.Drawing.SolidBrush(_backColor), screenRect);
                }
            }
        }

        public WorkspaceBackgroundStyle WorkspaceBackgroundStyle
        {
            get
            {
                return _workspaceBackgroundStyle;
            }
            set
            {
                _workspaceBackgroundStyle = value;
            }
        }

        public System.Drawing.Color WorkspaceBackColor1
        {
            get
            {
                return _workspaceBackColor1;
            }
            set
            {
                _workspaceBackColor1 = value;
            }
        }

        public System.Drawing.Color WorkspaceBackColor2
        {
            get
            {
                return _workspaceBackColor2;
            }
            set
            {
                _workspaceBackColor2 = value;
            }
        }

        public System.Drawing.Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }

        #endregion "Viewport background rendering"

        #region IDisposable Members

        public override void Dispose()
        {
            try
            {
                if (_bgGridTemplate != null)
                {
                    _bgGridTemplate.Dispose();
                    _bgGridTemplate = null;
                }

                DisposeLayerRenders();
            }
            finally
            {
                base.Dispose();
            }
        }

        #endregion IDisposable Members
    }
}