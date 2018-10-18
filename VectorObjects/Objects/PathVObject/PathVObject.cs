// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base class for all System.Drawing.Drawing2D.GraphicsPath based objects. Supplies IRectangle interface
    /// implementation, hit-testing, determining vector object bounds, drawing (with draft/normal modes), and
    /// some trivial properties (pen width, pen/brush colors).
    /// Implementors should implement abstract Path property.
    /// </summary>
    [System.Serializable]
    public abstract class PathVObject : VObject
    {
        #region "Construction / destruction"

        protected PathVObject(string name)
        {
            base.Name = name;
            _matrix = new System.Drawing.Drawing2D.Matrix();
            _identityMatrix = new System.Drawing.Drawing2D.Matrix();

            _pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1.0f);
            _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
            _brush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            _brushMatrices = new System.Collections.Stack();
        }

        protected PathVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _brushMatrices = new System.Collections.Stack();

            _matrix = BinarySerializer.DeserializeMatrix((byte[])info.GetValue(SerializationNames.Matrix, typeof(byte[])));
            _pen = BinarySerializer.DeserializePen((byte[])info.GetValue(SerializationNames.Pen, typeof(byte[])));
            _brush = BinarySerializer.DeserializeBrush((byte[])info.GetValue(SerializationNames.Brush, typeof(byte[])));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_matrix != null)
                {
                    _matrix.Dispose();
                    _matrix = null;
                }
                if (_identityMatrix != null)
                {
                    _identityMatrix.Dispose();
                    _identityMatrix = null;
                }
                if (_pen != null)
                {
                    try
                    {
                        _pen.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                    }
                    finally
                    {
                        _pen = null;
                    }
                }
                if (_brush != null)
                {
                    try
                    {
                        _brush.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                    }
                    finally
                    {
                        _brush = null;
                    }
                }
            }
        }

        #endregion "Construction / destruction"

        #region "IBaseRectangle members"

        public override System.Drawing.RectangleF GetVObjectBounds()
        {
            return this.Path.GetBounds();
        }

        public override System.Drawing.Drawing2D.Matrix Transform
        {
            get
            {
                return _matrix;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _matrix = value;
            }
        }

        #endregion "IBaseRectangle members"

        #region IVObject Members

        public override void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
        {
            if (g == null)
                throw new System.ArgumentNullException("g");
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            System.Drawing.Drawing2D.GraphicsPath drawPath = CreateViewportPath(coordinateMapper);
            System.Drawing.Pen pen = CreateViewportPen(coordinateMapper);

            System.Drawing.Drawing2D.SmoothingMode oldSmoothingMode = g.SmoothingMode;
            try
            {
                switch (base.DrawMode)
                {
                    case VObjectDrawMode.Draft:
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        break;

                    case VObjectDrawMode.Normal:
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        break;

                    default:
                        throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrUnexpectedDrawMode"));
                }

                if (_brush != null)
                {
                    AdaptBrushToViewport(coordinateMapper);
                    try
                    {
                        g.FillPath(_brush, drawPath);
                    }
                    finally
                    {
                        RestoreBrush();
                    }
                }
                if (pen != null)
                    g.DrawPath(pen, drawPath);
            }
            finally
            {
                if (pen != null)
                    pen.Dispose();
                drawPath.Dispose();
                g.SmoothingMode = oldSmoothingMode;
            }
        }

        public override bool HitTest(System.Drawing.PointF point, float precisionDelta)
        {
            bool result = false;

            if (_pen != null || _brush != null)
            {
                using (System.Drawing.Drawing2D.GraphicsPath actualPath = (System.Drawing.Drawing2D.GraphicsPath)this.Path.Clone())
                {
                    actualPath.Transform(_matrix);

                    if (_pen != null)
                        using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, _pen.Width + precisionDelta))
                            result = actualPath.IsOutlineVisible(point, pen);

                    if (!result && _brush != null)
                        result = actualPath.IsVisible(point);
                }
            }

            return result;
        }

        public override System.Drawing.RectangleF GetTransformedVObjectBounds()
        {
            //
            // We cannot use evident way to get bounds of the Path, something like this:
            //
            // return this.Path.GetBounds(_matrix, _pen);
            //
            // because this implementation returns wrong values. It seems that the GraphicsPath algorithm
            // first finds bounds and after that applies matrix! Also it returns wrong (too large)
            // rectangle for curves (e.g. for ellipse). To avoid first issue we have to clone path
            // and manually transform it. To avoid second problem we have to use Flatten() method.
            //

            using (System.Drawing.Drawing2D.GraphicsPath c = (System.Drawing.Drawing2D.GraphicsPath)this.Path.Clone())
            {
                c.Flatten(_matrix, 1.0f);
                return c.GetBounds(_identityMatrix, _pen);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override IDesigner Designer
        {
            get
            {
                if (_editDesigner == null)
                    _editDesigner = new GenericVObjectEditDesigner(this);

                return _editDesigner;
            }
        }

        #endregion IVObject Members

        #region "Draw-related methods"

        #region "Brush transform updating"

        protected void AdaptBrushToViewport(ICoordinateMapper coordinateMapper)
        {
            if (coordinateMapper == null)
                throw new System.ArgumentNullException("coordinateMapper");

            if (_brush != null && _brush.GetType() != typeof(System.Drawing.SolidBrush) && _brush.GetType() != typeof(System.Drawing.Drawing2D.HatchBrush))
            {
                System.Drawing.Drawing2D.Matrix originalMatrix = VObjectsUtils.GetBrushMatrix(_brush);
                _brushMatrices.Push(originalMatrix);

                System.Drawing.Point viewportTranslation = coordinateMapper.WorkspaceToControl(System.Drawing.PointF.Empty, Aurigma.GraphicsMill.Unit.Pixel);
                float scale = coordinateMapper.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point);

                System.Drawing.Drawing2D.Matrix brushMatrix = (System.Drawing.Drawing2D.Matrix)_matrix.Clone();
                brushMatrix.Scale(scale, scale, System.Drawing.Drawing2D.MatrixOrder.Append);
                brushMatrix.Translate(viewportTranslation.X, viewportTranslation.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
                brushMatrix.Multiply(originalMatrix, System.Drawing.Drawing2D.MatrixOrder.Prepend);

                VObjectsUtils.SetBrushMatrix(_brush, brushMatrix);
            }
        }

        protected void RestoreBrush()
        {
            if (_brush != null && _brush.GetType() != typeof(System.Drawing.SolidBrush) && _brush.GetType() != typeof(System.Drawing.Drawing2D.HatchBrush))
            {
                if (_brushMatrices.Count < 1)
                    throw new Aurigma.GraphicsMill.UnexpectedException(StringResources.GetString("ExStrModifyBrushTransformOrderCorrupted"));

                VObjectsUtils.SetBrushMatrix(_brush, (System.Drawing.Drawing2D.Matrix)_brushMatrices.Peek());
            }
        }

        #endregion "Brush transform updating"

        protected System.Drawing.Pen CreateViewportPen(ICoordinateMapper coordinateMapper)
        {
            if (_pen == null)
                return null;

            System.Drawing.Pen result = (System.Drawing.Pen)_pen.Clone();
            result.Width = _pen.Width * coordinateMapper.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point);
            return result;
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateViewportPath(ICoordinateMapper coordinateMapper)
        {
            System.Drawing.Drawing2D.GraphicsPath result = (System.Drawing.Drawing2D.GraphicsPath)this.Path.Clone();

            result.Transform(_matrix);
            System.Drawing.RectangleF pathBounds = this.Path.GetBounds(_matrix);
            System.Drawing.Rectangle mappedBounds = coordinateMapper.WorkspaceToControl(pathBounds, Aurigma.GraphicsMill.Unit.Point);

            if (pathBounds.Width > VObject.Eps && pathBounds.Height > VObject.Eps)
            {
                float scaleX = (float)mappedBounds.Width / pathBounds.Width,
                      scaleY = (float)mappedBounds.Height / pathBounds.Height;

                using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
                {
                    m.Translate(mappedBounds.X, mappedBounds.Y);
                    m.Scale(scaleX, scaleY);
                    m.Translate(-pathBounds.X, -pathBounds.Y);
                    result.Transform(m);
                }
            }

            return result;
        }

        #endregion "Draw-related methods"

        #region "ISerialization interface"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);

            info.AddValue(SerializationNames.Matrix, BinarySerializer.Serialize(this.Transform));
            info.AddValue(SerializationNames.Pen, BinarySerializer.Serialize(this.Pen));
            info.AddValue(SerializationNames.Brush, BinarySerializer.Serialize(this.Brush));
        }

        #endregion "ISerialization interface"

        #region "Trivial properties"

        [System.ComponentModel.TypeConverter(typeof(FilteringExpandableObjectConverter))]
        public System.Drawing.Pen Pen
        {
            get
            {
                return _pen;
            }
            set
            {
                if (_pen != value)
                {
                    _pen = value;
                    OnChanged(System.EventArgs.Empty);
                }
            }
        }

        protected System.Drawing.Pen PenInternal
        {
            get
            {
                return _pen;
            }
            set
            {
                _pen = value;
            }
        }

        [System.ComponentModel.TypeConverter(typeof(FilteringExpandableObjectConverter))]
        public System.Drawing.Brush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    OnChanged(System.EventArgs.Empty);
                }
            }
        }

        protected System.Drawing.Brush BrushInternal
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }

        protected abstract System.Drawing.Drawing2D.GraphicsPath Path
        {
            get;
        }

        #endregion "Trivial properties"

        #region "Member variables"

        private System.Drawing.Drawing2D.Matrix _identityMatrix;
        private System.Drawing.Drawing2D.Matrix _matrix;
        private System.Drawing.Pen _pen;
        private System.Drawing.Brush _brush;
        private System.Collections.Stack _brushMatrices;

        private IDesigner _editDesigner;

        #endregion "Member variables"
    }
}