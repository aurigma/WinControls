// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Ellipse vector object create designer.
    /// </summary>
    public class EllipseVObjectCreateDesigner : RectangleVObjectCreateDesigner
    {
        public override void Draw(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");

            if (base.Dragging)
            {
                System.Drawing.Rectangle rectangle = GetViewportRectangle();

                if (base.Brush != null)
                {
                    System.Drawing.Drawing2D.Matrix prevBrushMatrix = PathVObjectCreateDesigner.AdaptBrushToViewport(base.Brush, this.VObjectHost.HostViewer);
                    try
                    {
                        g.FillEllipse(base.Brush, rectangle);
                    }
                    finally
                    {
                        if (prevBrushMatrix != null)
                            VObjectsUtils.SetBrushMatrix(base.Brush, prevBrushMatrix);
                    }
                }

                if (base.Pen != null)
                    using (System.Drawing.Pen pen = CreateViewportPen())
                        g.DrawEllipse(pen, rectangle);
            }
        }

        protected override IVObject CreateObject()
        {
            EllipseVObject result = null;

            System.Drawing.RectangleF rectangle = GetWorkspaceRectangle();
            if (rectangle.Width > 1 && rectangle.Height > 1)
            {
                result = new EllipseVObject(rectangle);

                if (base.Brush != null)
                    result.Brush = (System.Drawing.Brush)base.Brush.Clone();
                else
                    result.Brush = null;

                if (base.Pen != null)
                    result.Pen = (System.Drawing.Pen)base.Pen.Clone();
                else
                    result.Pen = null;
            }

            return result;
        }
    }
}