// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// The class provides a basic implementation of the IControlPointsProvider interface.
    /// The implementation supports: scale/rotate/skew operations.
    /// </summary>
    internal class GenericControlPointsProvider : IControlPointsProvider
    {
        #region "Private constants"

        private const float MinRotationAndle = 0.1f;

        #endregion "Private constants"

        #region "Internal ObjectDragControlPoint class"

        internal class ObjectDragControlPoint : ControlPoint
        {
            public ObjectDragControlPoint(GenericControlPointsProvider gcpp)
            {
                if (gcpp == null)
                    throw new System.ArgumentNullException("gcpp");

                _gcpp = gcpp;
            }

            #region ControlPoint overloaded members

            public override bool HitTest(System.Drawing.Point point, ICoordinateMapper coordinateMapper)
            {
                if (coordinateMapper == null)
                    throw new System.ArgumentNullException("coordinateMapper");

                bool result = false;
                if (this.Enabled)
                {
                    System.Drawing.PointF convertedPoint = coordinateMapper.ControlToWorkspace(point, Aurigma.GraphicsMill.Unit.Point);
                    result = _gcpp._obj.HitTest(convertedPoint, VObject.SelectionPrecisionDelta / coordinateMapper.GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit.Point));
                    if (result)
                        this.Location = convertedPoint;
                }

                return result;
            }

            public override void Draw(System.Drawing.Graphics g, ICoordinateMapper coordinateMapper)
            {
            }

            public override System.Drawing.Size GetSize()
            {
                return System.Drawing.Size.Empty;
            }

            public override Aurigma.GraphicsMill.WinControls.ControlPointType Type
            {
                get
                {
                    return Aurigma.GraphicsMill.WinControls.ControlPointType.Drag;
                }
            }

            public override object Clone()
            {
                return new ObjectDragControlPoint(_gcpp);
            }

            #endregion ControlPoint overloaded members

            #region "Member variables"

            private GenericControlPointsProvider _gcpp;

            #endregion "Member variables"
        }

        #endregion "Internal ObjectDragControlPoint class"

        #region "Construction / destruction"

        public GenericControlPointsProvider(IVObject obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");

            _obj = obj;
            _obj.Changed += new System.EventHandler(VObjectChangedHandler);

            _resizeEnabled = true;
            _skewEnabled = true;
            _rotateEnabled = true;
            _dragEnabled = true;
            _resizeMode = ResizeMode.Arbitrary;

            _majorResizePointPrototype = new RectangleControlPoint(new System.Drawing.Size(9, 9), new System.Drawing.SolidBrush(System.Drawing.Color.White), new System.Drawing.Pen(System.Drawing.Color.IndianRed));
            _minorResizePointPrototype = new RectangleControlPoint(new System.Drawing.Size(7, 7), new System.Drawing.SolidBrush(System.Drawing.Color.White), new System.Drawing.Pen(System.Drawing.Color.IndianRed));
            _skewPointPrototype = new RectangleControlPoint(new System.Drawing.Size(20, 20), new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(64, System.Drawing.Color.Gold)), new System.Drawing.Pen(System.Drawing.Color.FromArgb(64, System.Drawing.Color.Black)));
            _rotatePointPrototype = new EllipseControlPoint(new System.Drawing.Size(24, 24), new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(64, System.Drawing.Color.LightSkyBlue)), new System.Drawing.Pen(System.Drawing.Color.FromArgb(64, System.Drawing.Color.Black)));
            _rotateCenterPointPrototype = new CrossedEllipseControlPoint(new System.Drawing.Size(11, 11), null, new System.Drawing.Pen(System.Drawing.Color.Black, 1));

            RecreateControlPoints();

            _resizeNESWCursor = System.Windows.Forms.Cursors.SizeNESW;
            _resizeNSCursor = System.Windows.Forms.Cursors.SizeNS;
            _resizeNWSECursor = System.Windows.Forms.Cursors.SizeNWSE;
            _resizeWECursor = System.Windows.Forms.Cursors.SizeWE;
            _rotateCenterCursor = System.Windows.Forms.Cursors.Cross;
            _rotatePointCursor = System.Windows.Forms.Cursors.Arrow;
            _skewCursor = System.Windows.Forms.Cursors.SizeAll;
            _dragCursor = System.Windows.Forms.Cursors.Default;

            IVObjectAction[] actions = new IVObjectAction[4];
            actions[0] = new ResizeVObjectAction(this);
            actions[1] = new RotateVObjectAction(this);
            actions[2] = new SkewVObjectAction(this);
            actions[3] = new DragVObjectAction(this);
            _supportedActions = new VObjectActionCollection(actions);
        }

        private void RecreateControlPoints()
        {
            ControlPoint[] points = new ControlPoint[18];

            // Rotation center point
            System.ICloneable prototype = (System.ICloneable)_rotateCenterPointPrototype;
            points[0] = (ControlPoint)prototype.Clone();

            // Major resize points
            prototype = (System.ICloneable)_majorResizePointPrototype;
            points[1] = (ControlPoint)prototype.Clone();
            points[2] = (ControlPoint)prototype.Clone();
            points[3] = (ControlPoint)prototype.Clone();
            points[4] = (ControlPoint)prototype.Clone();

            // Minor resize points
            prototype = (System.ICloneable)_minorResizePointPrototype;
            points[5] = (ControlPoint)prototype.Clone();
            points[6] = (ControlPoint)prototype.Clone();
            points[7] = (ControlPoint)prototype.Clone();
            points[8] = (ControlPoint)prototype.Clone();

            // Rotation points
            prototype = (System.ICloneable)_rotatePointPrototype;
            points[9] = (ControlPoint)prototype.Clone();
            points[10] = (ControlPoint)prototype.Clone();
            points[11] = (ControlPoint)prototype.Clone();
            points[12] = (ControlPoint)prototype.Clone();

            // Skew points
            prototype = (System.ICloneable)_skewPointPrototype;
            points[13] = (ControlPoint)prototype.Clone();
            points[14] = (ControlPoint)prototype.Clone();
            points[15] = (ControlPoint)prototype.Clone();
            points[16] = (ControlPoint)prototype.Clone();

            // Whole object drag "virtual" point
            points[17] = new ObjectDragControlPoint(this);

            _transformedPoints = new ControlPointCollection(points);
            _updateProvider = true;

            UpdateMaxControlPointRadius();
        }

        private void UpdateMaxControlPointRadius()
        {
            _maxControlPointRadius = 0;
            for (int i = 0; i < _transformedPoints.Count; i++)
            {
                System.Drawing.Size controlPntSize = _transformedPoints[i].GetSize();

                if (controlPntSize.Width > _maxControlPointRadius)
                    _maxControlPointRadius = controlPntSize.Width;
                if (controlPntSize.Height > _maxControlPointRadius)
                    _maxControlPointRadius = controlPntSize.Height;
            }

            _maxControlPointRadius = (_maxControlPointRadius + 1) / 2;
        }

        #endregion "Construction / destruction"

        #region IControlPointsProvider interface implementation

        // Provider supplies 18 control points. Ñontrol points order is:
        //
        //    9      13      10
        //      1 --- 5 --- 2
        //	    |           |       0 - rotation center point
        //      |           |       1-4 - major size points
        //   16 8     0     6 14    5-8 - minor middle-side size points
        //	    |           |       9-12 - rotation grip points
        //      |           |       13-16 - skew grip points
        //      4 --- 7 --- 3       17 - virtual object drag point
        //   12      15     11
        public virtual IControlPointCollection ControlPoints
        {
            get
            {
                UpdateControlPoints();
                return _transformedPoints;
            }
        }

        public virtual IVObjectActionCollection SupportedActions
        {
            get
            {
                return _supportedActions;
            }
        }

        public int MaxControlPointRadius
        {
            get
            {
                return _maxControlPointRadius;
            }
        }

        public virtual void ClickPoint(int index)
        {
        }

        public System.Drawing.RectangleF GetControlPointsBounds()
        {
            float l = float.MaxValue, t = float.MaxValue, r = float.MinValue, b = float.MinValue;
            for (int i = 0; i < 5; i++)
            {
                if (_transformedPoints[i].X < l)
                    l = _transformedPoints[i].X;
                if (_transformedPoints[i].X > r)
                    r = _transformedPoints[i].X;
                if (_transformedPoints[i].Y < t)
                    t = _transformedPoints[i].Y;
                if (_transformedPoints[i].Y > b)
                    b = _transformedPoints[i].Y;
            }

            return System.Drawing.RectangleF.FromLTRB(l, t, r, b);
        }

        #region "Dragging of point implementation"

        public virtual void DragPoint(int index, System.Drawing.PointF newPosition)
        {
            if (index < 0 || index >= _ctrlPoints.Length)
                throw new System.ArgumentOutOfRangeException("index");
            if (_transformedPoints[index].Type != ControlPointType.Drag)
                throw new System.ArgumentException(StringResources.GetString("ExStrCannotDragControlPoint"), "index");
            if (!_transformedPoints[index].Enabled)
                throw new System.ArgumentException(StringResources.GetString("ExStrCannotProcessDisabledControlPoint"));

            if (index == 0)
                DragRotationCenterPoint(newPosition);
            else if (index < 5)
                DragCornerSizePoint(index, newPosition);
            else if (index < 9)
                DragMiddleSideSizePoint(index, newPosition);
            else if (index < 13)
                DragRotationPoint(index, newPosition);
            else if (index < 17)
                DragSkewPoint(index, newPosition);
            else
                DragWholeObject(newPosition);
        }

        protected virtual void DragCornerSizePoint(int index, System.Drawing.PointF newPosition)
        {
            if (!_resizeEnabled || _resizeMode == ResizeMode.None)
                return;

            System.Drawing.PointF scaleRefPoint = GetScaleRefPoint(index);

            bool flipX, flipY;
            System.Drawing.RectangleF newBounds = GetChangedBounds(index, newPosition, out flipX, out flipY),
                                      oldBounds = _obj.GetVObjectBounds();

            float scaleX = (flipX ? -1 : 1) * newBounds.Width / oldBounds.Width;
            float scaleY = (flipY ? -1 : 1) * newBounds.Height / oldBounds.Height;

            if (_resizeMode == ResizeMode.Proportional)
            {
                scaleX = System.Math.Max(scaleX, scaleY);
                scaleY = scaleX;
            }

            if (scaleX == float.NaN || scaleY == float.NaN)
                return;

            System.Drawing.Drawing2D.Matrix newMatrix = (System.Drawing.Drawing2D.Matrix)_obj.Transform.Clone();
            newMatrix.Translate(scaleRefPoint.X, scaleRefPoint.Y);
            newMatrix.Scale(scaleX, scaleY);
            newMatrix.Translate(-scaleRefPoint.X, -scaleRefPoint.Y);

            if (IsActionAcceptable(_obj, newMatrix))
            {
                _obj.Transform = newMatrix;
                UpdateControlPoints();
            }
            else
                newMatrix.Dispose();
        }

        protected virtual void DragMiddleSideSizePoint(int index, System.Drawing.PointF newPosition)
        {
            if (!_resizeEnabled || _resizeMode == ResizeMode.None)
                return;

            System.Drawing.PointF scaleRefPoint = GetScaleRefPoint(index),
                                  workSpcDragV = GetProjectionOfDragVector(index, newPosition, false);

            System.Drawing.RectangleF oldBounds = _obj.GetVObjectBounds();

            float scaleX, scaleY;
            scaleX = scaleY = 1;
            switch (index)
            {
                case 5:
                    scaleY = (oldBounds.Height - workSpcDragV.Y) / oldBounds.Height;
                    break;

                case 7:
                    scaleY = (oldBounds.Height + workSpcDragV.Y) / oldBounds.Height;
                    break;

                case 6:
                    scaleX = (oldBounds.Width + workSpcDragV.X) / oldBounds.Width;
                    break;

                case 8:
                    scaleX = (oldBounds.Width - workSpcDragV.X) / oldBounds.Width;
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException("index");
            }

            if (scaleX == float.NaN || scaleY == float.NaN)
                return;

            if (_resizeMode == ResizeMode.Proportional)
            {
                if (scaleX == 1.0f)
                    scaleX = scaleY;
                else
                    scaleY = scaleX;
            }

            System.Drawing.Drawing2D.Matrix newMatrix = (System.Drawing.Drawing2D.Matrix)_obj.Transform.Clone();
            newMatrix.Translate(scaleRefPoint.X, scaleRefPoint.Y);
            newMatrix.Scale(scaleX, scaleY);
            newMatrix.Translate(-scaleRefPoint.X, -scaleRefPoint.Y);

            if (IsActionAcceptable(_obj, newMatrix))
            {
                _obj.Transform = newMatrix;
                UpdateControlPoints();
            }
            else
                newMatrix.Dispose();
        }

        protected virtual void DragRotationPoint(int index, System.Drawing.PointF newPosition)
        {
            if (!_rotateEnabled)
                return;

            System.Drawing.PointF prevPosition = _transformedPoints[index].Location,
                                  rotationCenter = _transformedPoints[0].Location;

            System.Drawing.PointF prevV = new System.Drawing.PointF(prevPosition.X - rotationCenter.X, prevPosition.Y - rotationCenter.Y),
                                  newV = new System.Drawing.PointF(newPosition.X - rotationCenter.X, newPosition.Y - rotationCenter.Y);

            float prevL = (float)System.Math.Sqrt(prevV.X * prevV.X + prevV.Y * prevV.Y),
                  newL = (float)System.Math.Sqrt(newV.X * newV.X + newV.Y * newV.Y);

            int sign = prevV.X * newV.Y - newV.X * prevV.Y > 0 ? 1 : -1;
            float cosA = (prevV.X * newV.X + prevV.Y * newV.Y) / (prevL * newL);
            float angle = (float)System.Math.Acos(cosA) * sign;
            float degreeAngle = (float)(angle * 180.0f / System.Math.PI);

            if (System.Math.Abs(degreeAngle) > MinRotationAndle)
            {
                _obj.Transform.RotateAt(degreeAngle, rotationCenter, System.Drawing.Drawing2D.MatrixOrder.Append);
                UpdateControlPoints();
            }
        }

        protected virtual void DragRotationCenterPoint(System.Drawing.PointF newPosition)
        {
            if (!_rotateEnabled)
                return;

            System.Drawing.PointF savedValue = newPosition;
            VObjectsUtils.TransformPoint(_inverseMatrix, ref newPosition);

            if (newPosition != _ctrlPoints[0])
            {
                _ctrlPoints[0] = newPosition;
                _transformedPoints[0].Location = savedValue;
            }
        }

        protected virtual void DragWholeObject(System.Drawing.PointF newPosition)
        {
            if (!_dragEnabled)
                return;

            System.Drawing.PointF translationVector = new System.Drawing.PointF(newPosition.X - _transformedPoints[17].X, newPosition.Y - _transformedPoints[17].Y);

            if (System.Math.Abs(translationVector.X) < VObject.Eps && System.Math.Abs(translationVector.Y) < VObject.Eps)
                return;

            if (System.Math.Abs(translationVector.X) > 10000 || System.Math.Abs(translationVector.Y) > 10000)
            {
                return;
            }

            VObjectsUtils.TransformVector(_inverseMatrix, ref translationVector);
            _obj.Transform.Translate(translationVector.X, translationVector.Y);

            _transformedPoints[17].Location = newPosition;
            UpdateControlPoints();
        }

        protected virtual void DragSkewPoint(int index, System.Drawing.PointF newPosition)
        {
            if (!_skewEnabled)
                return;

            System.Drawing.PointF workspaceDragV = GetProjectionOfDragVector(index, newPosition, true);

            System.Drawing.RectangleF oldBounds = _obj.GetVObjectBounds();
            float shearX = 0, shearY = 0;
            switch (index)
            {
                case 13:
                    shearX = -workspaceDragV.X / oldBounds.Height;
                    break;

                case 15:
                    shearX = workspaceDragV.X / oldBounds.Height;
                    break;

                case 14:
                    shearY = workspaceDragV.Y / oldBounds.Width;
                    break;

                case 16:
                    shearY = -workspaceDragV.Y / oldBounds.Width;
                    break;
            }

            float shiftX = oldBounds.X + oldBounds.Width / 2.0f,
                  shiftY = oldBounds.Y + oldBounds.Height / 2.0f;

            if (System.Math.Abs(shearX) > 0.001 || System.Math.Abs(shearY) > 0.001)
            {
                System.Drawing.Drawing2D.Matrix newMatrix = (System.Drawing.Drawing2D.Matrix)_obj.Transform.Clone();
                newMatrix.Translate(shiftX, shiftY);
                newMatrix.Shear(shearX, shearY);
                newMatrix.Translate(-shiftX, -shiftY);

                if (IsActionAcceptable(_obj, newMatrix))
                {
                    _obj.Transform = newMatrix;
                    UpdateControlPoints();
                }
                else
                    newMatrix.Dispose();
            }
        }

        protected System.Drawing.PointF GetProjectionOfDragVector(int pointIndex, System.Drawing.PointF newPosition, bool projectOnHostSide)
        {
            System.Drawing.PointF dragV = new System.Drawing.PointF(newPosition.X - _transformedPoints[pointIndex].X, newPosition.Y - _transformedPoints[pointIndex].Y);
            System.Drawing.PointF sideV = System.Drawing.PointF.Empty;
            switch (pointIndex)
            {
                case 13:
                case 15:
                case 5:
                case 7:
                    if (projectOnHostSide)
                        sideV = new System.Drawing.PointF(_transformedPoints[2].X - _transformedPoints[1].X, _transformedPoints[2].Y - _transformedPoints[1].Y);
                    else
                        sideV = new System.Drawing.PointF(_transformedPoints[2].X - _transformedPoints[3].X, _transformedPoints[2].Y - _transformedPoints[3].Y);
                    break;

                case 14:
                case 16:
                case 6:
                case 8:
                    if (projectOnHostSide)
                        sideV = new System.Drawing.PointF(_transformedPoints[2].X - _transformedPoints[3].X, _transformedPoints[2].Y - _transformedPoints[3].Y);
                    else
                        sideV = new System.Drawing.PointF(_transformedPoints[2].X - _transformedPoints[1].X, _transformedPoints[2].Y - _transformedPoints[1].Y);
                    break;
            }

            float sideL = (float)System.Math.Sqrt(sideV.X * sideV.X + sideV.Y * sideV.Y);
            float scalarProduct = dragV.X * sideV.X + dragV.Y * sideV.Y;
            float projectionL = scalarProduct / sideL;

            sideV.X /= sideL;
            sideV.Y /= sideL;
            sideV.X *= projectionL;
            sideV.Y *= projectionL;
            newPosition = _transformedPoints[pointIndex].Location;

            System.Drawing.PointF workspaceV = sideV;
            VObjectsUtils.TransformVector(_inverseMatrix, ref workspaceV);

            return workspaceV;
        }

        private bool IsActionAcceptable(IVObject obj, System.Drawing.Drawing2D.Matrix newMatrix)
        {
            if (!newMatrix.IsInvertible)
                return false;

            System.Drawing.RectangleF newBounds = VObjectsUtils.GetBoundingRectangle(obj.GetVObjectBounds(), newMatrix);
            return newBounds.Width < VObject.MaxSize && newBounds.Height < VObject.MaxSize && newBounds.Width > VObject.Eps && newBounds.Height > VObject.Eps;
        }

        protected System.Drawing.PointF GetScaleRefPoint(int changingPointIndex)
        {
            switch (changingPointIndex)
            {
                case 1:
                    return _ctrlPoints[3];

                case 2:
                    return _ctrlPoints[4];

                case 3:
                    return _ctrlPoints[1];

                case 4:
                    return _ctrlPoints[2];

                case 5:
                    return _ctrlPoints[7];

                case 6:
                    return _ctrlPoints[8];

                case 7:
                    return _ctrlPoints[5];

                case 8:
                    return _ctrlPoints[6];

                default:
                    throw new System.ArgumentOutOfRangeException("changingPointIndex");
            }
        }

        protected System.Drawing.RectangleF GetChangedBounds(int changingPointIndex, System.Drawing.PointF newPosition, out bool flipX, out bool flipY)
        {
            flipX = false;
            flipY = false;

            VObjectsUtils.TransformPoint(_inverseMatrix, ref newPosition);

            float left, top, right, bottom;
            left = _ctrlPoints[1].X;
            top = _ctrlPoints[1].Y;
            right = _ctrlPoints[3].X;
            bottom = _ctrlPoints[3].Y;

            switch (changingPointIndex)
            {
                case 1:
                case 4:
                case 8:
                    left = newPosition.X;
                    break;

                case 2:
                case 3:
                case 6:
                    right = newPosition.X;
                    break;
            }

            switch (changingPointIndex)
            {
                case 1:
                case 2:
                case 5:
                    top = newPosition.Y;
                    break;

                case 4:
                case 3:
                case 7:
                    bottom = newPosition.Y;
                    break;
            }

            if (left > right)
            {
                VObjectsUtils.Swap(ref left, ref right);
                flipX = true;
            }
            if (top > bottom)
            {
                VObjectsUtils.Swap(ref top, ref bottom);
                flipY = true;
            }

            return System.Drawing.RectangleF.FromLTRB(left, top, right, bottom);
        }

        #endregion "Dragging of point implementation"

        #region "Updating control points positions"

        private void VObjectChangedHandler(object sender, System.EventArgs e)
        {
            _updateProvider = true;
        }

        protected void UpdateControlPoints()
        {
            if (!_updateProvider)
                return;

            System.Drawing.RectangleF bounds = _obj.GetVObjectBounds();

            bool firstTime = false;
            if (_ctrlPoints == null)
            {
                _ctrlPoints = new System.Drawing.PointF[18];
                firstTime = true;
            }

            // Corner size points
            _ctrlPoints[1] = new System.Drawing.PointF(bounds.Left, bounds.Top);
            _ctrlPoints[2] = new System.Drawing.PointF(bounds.Right, bounds.Top);
            _ctrlPoints[3] = new System.Drawing.PointF(bounds.Right, bounds.Bottom);
            _ctrlPoints[4] = new System.Drawing.PointF(bounds.Left, bounds.Bottom);

            // Rotation grip points
            _ctrlPoints[9] = _ctrlPoints[1];
            _ctrlPoints[10] = _ctrlPoints[2];
            _ctrlPoints[11] = _ctrlPoints[3];
            _ctrlPoints[12] = _ctrlPoints[4];

            // Middle-side size points
            float midWidth = bounds.Left + bounds.Width / 2;
            float midHeight = bounds.Top + bounds.Height / 2;
            _ctrlPoints[5] = new System.Drawing.PointF(midWidth, bounds.Top);
            _ctrlPoints[6] = new System.Drawing.PointF(bounds.Right, midHeight);
            _ctrlPoints[7] = new System.Drawing.PointF(midWidth, bounds.Bottom);
            _ctrlPoints[8] = new System.Drawing.PointF(bounds.Left, midHeight);

            // Skew points
            _ctrlPoints[13] = _ctrlPoints[5];
            _ctrlPoints[14] = _ctrlPoints[6];
            _ctrlPoints[15] = _ctrlPoints[7];
            _ctrlPoints[16] = _ctrlPoints[8];

            // Virtual drag point
            _ctrlPoints[17] = System.Drawing.PointF.Empty;

            // Rotation center point
            if (firstTime)
            {
                _ctrlPoints[0] = new System.Drawing.PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
            }

            TransformControlPoints();
            UpdateInverseMatrix();

            _updateProvider = false;
        }

        private void TransformControlPoints()
        {
            if (_transformedPoints == null)
                throw new Aurigma.GraphicsMill.UnexpectedException();

            System.Drawing.PointF[] interimPnts = VObjectsUtils.TransformPoints(_obj.Transform, _ctrlPoints);
            for (int i = 0; i < _ctrlPoints.Length - 1; i++)
                _transformedPoints[i].Location = interimPnts[i];
        }

        private void UpdateInverseMatrix()
        {
            if (_obj.Transform.IsInvertible)
            {
                _inverseMatrix = _obj.Transform.Clone();
                _inverseMatrix.Invert();
            }
        }

        #endregion "Updating control points positions"

        #region "Cursor handling"

        public System.Windows.Forms.Cursor GetPointCursor(int index)
        {
            if (index < 0 || index >= _transformedPoints.Count)
                throw new System.ArgumentOutOfRangeException("index");

            if (index == 0)
                return _rotateCenterCursor;

            if (index >= 9 && index <= 12)
                return _rotatePointCursor;

            if (index >= 13 && index <= 16)
                return _skewCursor;

            if (index >= 5 && index <= 8)
                return GetSideMiddleResizePointCursor(index);

            if (index >= 1 && index <= 4)
                return GetCornerResizePointCursor(index);

            if (index == 17)
                return _dragCursor;

            return System.Windows.Forms.Cursors.Arrow;
        }

        private System.Windows.Forms.Cursor GetSideMiddleResizePointCursor(int index)
        {
            System.Drawing.PointF directionVector;
            switch (index)
            {
                case 5:
                case 7:
                    directionVector = new System.Drawing.PointF(_transformedPoints[1].X - _transformedPoints[4].X, _transformedPoints[1].Y - _transformedPoints[4].Y);
                    break;

                case 6:
                case 8:
                    directionVector = new System.Drawing.PointF(_transformedPoints[1].X - _transformedPoints[2].X, _transformedPoints[1].Y - _transformedPoints[2].Y);
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException("index");
            }

            return GetResizeCursorForDirection(directionVector);
        }

        private System.Windows.Forms.Cursor GetCornerResizePointCursor(int index)
        {
            System.Drawing.PointF centerPoint = System.Drawing.PointF.Empty;
            for (int i = 1; i < 5; i++)
            {
                centerPoint.X += _transformedPoints[i].X;
                centerPoint.Y += _transformedPoints[i].Y;
            }
            centerPoint.X /= 4;
            centerPoint.Y /= 4;

            System.Drawing.PointF directionVector = new System.Drawing.PointF(_transformedPoints[index].X - centerPoint.X, _transformedPoints[index].Y - centerPoint.Y);
            return GetResizeCursorForDirection(directionVector);
        }

        // Returns a resize-arrow cursor whose direction is the closest to a given vector.
        private System.Windows.Forms.Cursor GetResizeCursorForDirection(System.Drawing.PointF directionVector)
        {
            const float Cos90div4 = 0.92387953251f;         // cos(90 / 4)      | border values which separates vertical/horizontal directions
            const float Cos3x90div4 = 0.38268343236f;       // cos(3 * 90 / 4)  | from diagonal ones.

            float directionL = (float)System.Math.Sqrt(directionVector.X * directionVector.X + directionVector.Y * directionVector.Y);
            float cosA = System.Math.Abs(directionVector.X / directionL);

            if (cosA > Cos90div4)
                return _resizeWECursor;
            if (cosA < Cos3x90div4)
                return _resizeNSCursor;
            if (directionVector.X * directionVector.Y > 0)
                return _resizeNWSECursor;

            return _resizeNESWCursor;
        }

        #endregion "Cursor handling"

        #endregion IControlPointsProvider interface implementation

        #region "Internal methods for VObjectAction objects"

        internal ControlPoint MajorResizePointPrototype
        {
            get
            {
                return _majorResizePointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _majorResizePointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal ControlPoint MinorResizePointPrototype
        {
            get
            {
                return _minorResizePointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _minorResizePointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal ControlPoint SkewPointPrototype
        {
            get
            {
                return _skewPointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _skewPointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal ControlPoint RotatePointPrototype
        {
            get
            {
                return _rotatePointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _rotatePointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal ControlPoint RotateCenterPointPrototype
        {
            get
            {
                return _rotateCenterPointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _rotateCenterPointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal ResizeMode ResizeMode
        {
            get
            {
                return _resizeMode;
            }
            set
            {
                _resizeMode = value;
            }
        }

        internal bool ResizeEnabled
        {
            get
            {
                return _resizeEnabled;
            }
            set
            {
                if (_resizeEnabled != value)
                {
                    _resizeEnabled = value;
                    UpdateControlPointsState();
                }
            }
        }

        internal bool SkewEnabled
        {
            get
            {
                return _skewEnabled;
            }
            set
            {
                _skewEnabled = value;
                UpdateControlPointsState();
            }
        }

        internal bool RotateEnabled
        {
            get
            {
                return _rotateEnabled;
            }
            set
            {
                _rotateEnabled = value;
                UpdateControlPointsState();
            }
        }

        internal bool DragEnabled
        {
            get
            {
                return _dragEnabled;
            }
            set
            {
                _dragEnabled = value;
                UpdateControlPointsState();
            }
        }

        private void UpdateControlPointsState()
        {
            int i;

            // Resize points
            for (i = 0; i < 9; i++)
                _transformedPoints[i].Enabled = _resizeEnabled;

            // Skew points
            for (i = 13; i < 17; i++)
                _transformedPoints[i].Enabled = _skewEnabled;

            // Rotation points
            _transformedPoints[0].Enabled = _rotateEnabled;
            for (i = 9; i < 13; i++)
                _transformedPoints[i].Enabled = _rotateEnabled;

            // Drag whole object point
            _transformedPoints[17].Enabled = _dragEnabled;

            _obj.Update();
        }

        internal System.Windows.Forms.Cursor ResizeWECursor
        {
            get
            {
                return _resizeWECursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _resizeWECursor = value;
            }
        }

        internal System.Windows.Forms.Cursor ResizeNSCursor
        {
            get
            {
                return _resizeNSCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _resizeNSCursor = value;
            }
        }

        internal System.Windows.Forms.Cursor ResizeNWSECursor
        {
            get
            {
                return _resizeNWSECursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _resizeNWSECursor = value;
            }
        }

        internal System.Windows.Forms.Cursor ResizeNESWCursor
        {
            get
            {
                return _resizeNESWCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _resizeNESWCursor = value;
            }
        }

        internal System.Windows.Forms.Cursor RotateCenterCursor
        {
            get
            {
                return _rotateCenterCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _rotateCenterCursor = value;
            }
        }

        internal System.Windows.Forms.Cursor RotatePointCursor
        {
            get
            {
                return _rotatePointCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _rotatePointCursor = value;
            }
        }

        internal System.Windows.Forms.Cursor SkewCursor
        {
            get
            {
                return _skewCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _skewCursor = value;
            }
        }

        internal System.Windows.Forms.Cursor DragCursor
        {
            get
            {
                return _dragCursor;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _dragCursor = value;
            }
        }

        #endregion "Internal methods for VObjectAction objects"

        #region "Serialization support methods"

        internal void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            // Actions options
            info.AddValue(SerializationNames.GcppResizeMode, (int)_resizeMode);
            info.AddValue(SerializationNames.GcppResizeEnabled, _resizeEnabled);
            info.AddValue(SerializationNames.GcppRotateEnabled, _rotateEnabled);
            info.AddValue(SerializationNames.GcppSkewEnabled, _skewEnabled);
            info.AddValue(SerializationNames.GcppDragEnabled, _dragEnabled);

            // Cursors
            info.AddValue(SerializationNames.GcppResizeWECursor, _resizeWECursor);
            info.AddValue(SerializationNames.GcppResizeNSCursor, _resizeNSCursor);
            info.AddValue(SerializationNames.GcppResizeNWSECursor, _resizeNWSECursor);
            info.AddValue(SerializationNames.GcppResizeNESWCursor, _resizeNESWCursor);
            info.AddValue(SerializationNames.GcppRotateCenterCursor, _rotateCenterCursor);
            info.AddValue(SerializationNames.GcppRotatePointCursor, _rotatePointCursor);
            info.AddValue(SerializationNames.GcppSkewCursor, _skewCursor);
            info.AddValue(SerializationNames.GcppDragCursor, _dragCursor);

            // Control point types
            SaveControlPointPrototype(info, SerializationNames.GcppMajorResizePointPrototype, _majorResizePointPrototype);
            SaveControlPointPrototype(info, SerializationNames.GcppMinorResizePointPrototype, _minorResizePointPrototype);
            SaveControlPointPrototype(info, SerializationNames.GcppRotateCenterPointPrototype, _rotateCenterPointPrototype);
            SaveControlPointPrototype(info, SerializationNames.GcppRotatePointPrototype, _rotatePointPrototype);
            SaveControlPointPrototype(info, SerializationNames.GcppSkewPointPrototype, _skewPointPrototype);
        }

        internal void SetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            // Action options
            _resizeMode = (Aurigma.GraphicsMill.WinControls.ResizeMode)info.GetInt32(SerializationNames.GcppResizeMode);
            _resizeEnabled = info.GetBoolean(SerializationNames.GcppResizeEnabled);
            _rotateEnabled = info.GetBoolean(SerializationNames.GcppRotateEnabled);
            _skewEnabled = info.GetBoolean(SerializationNames.GcppSkewEnabled);
            _dragEnabled = info.GetBoolean(SerializationNames.GcppDragEnabled);

            // Cursors
            _resizeWECursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppResizeWECursor, typeof(System.Windows.Forms.Cursor));
            _resizeNSCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppResizeNSCursor, typeof(System.Windows.Forms.Cursor));
            _resizeNWSECursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppResizeNWSECursor, typeof(System.Windows.Forms.Cursor));
            _resizeNESWCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppResizeNESWCursor, typeof(System.Windows.Forms.Cursor));
            _rotateCenterCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppRotateCenterCursor, typeof(System.Windows.Forms.Cursor));
            _rotatePointCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppRotatePointCursor, typeof(System.Windows.Forms.Cursor));
            _skewCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppSkewCursor, typeof(System.Windows.Forms.Cursor));
            _dragCursor = (System.Windows.Forms.Cursor)info.GetValue(SerializationNames.GcppDragCursor, typeof(System.Windows.Forms.Cursor));

            // Control point types
            LoadControlPointPrototype(info, SerializationNames.GcppMajorResizePointPrototype, ref _majorResizePointPrototype);
            LoadControlPointPrototype(info, SerializationNames.GcppMinorResizePointPrototype, ref _minorResizePointPrototype);
            LoadControlPointPrototype(info, SerializationNames.GcppRotatePointPrototype, ref _rotatePointPrototype);
            LoadControlPointPrototype(info, SerializationNames.GcppRotateCenterPointPrototype, ref _rotateCenterPointPrototype);
            LoadControlPointPrototype(info, SerializationNames.GcppSkewPointPrototype, ref _skewPointPrototype);
            RecreateControlPoints();
        }

        private void SaveControlPointPrototype(System.Runtime.Serialization.SerializationInfo info, string key, ControlPoint prototype)
        {
            if (prototype.GetType().IsSerializable)
                info.AddValue(key, prototype);
        }

        private void LoadControlPointPrototype(System.Runtime.Serialization.SerializationInfo info, string key, ref ControlPoint prototype)
        {
            try
            {
                prototype = (ControlPoint)info.GetValue(key, typeof(ControlPoint));
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                // Prototype hadn't been saved during serialization.
            }
        }

        #endregion "Serialization support methods"

        #region "Member variables"

        private IVObject _obj;
        private bool _updateProvider;

        private System.Drawing.Drawing2D.Matrix _inverseMatrix;

        private ControlPointCollection _transformedPoints;
        private System.Drawing.PointF[] _ctrlPoints;

        private ResizeMode _resizeMode;
        private bool _resizeEnabled;
        private bool _skewEnabled;
        private bool _rotateEnabled;
        private bool _dragEnabled;

        private System.Windows.Forms.Cursor _resizeWECursor;
        private System.Windows.Forms.Cursor _resizeNSCursor;
        private System.Windows.Forms.Cursor _resizeNWSECursor;
        private System.Windows.Forms.Cursor _resizeNESWCursor;
        private System.Windows.Forms.Cursor _rotateCenterCursor;
        private System.Windows.Forms.Cursor _rotatePointCursor;
        private System.Windows.Forms.Cursor _skewCursor;
        private System.Windows.Forms.Cursor _dragCursor;

        private ControlPoint _majorResizePointPrototype;
        private ControlPoint _minorResizePointPrototype;
        private ControlPoint _rotatePointPrototype;
        private ControlPoint _rotateCenterPointPrototype;
        private ControlPoint _skewPointPrototype;

        private int _maxControlPointRadius;

        private VObjectActionCollection _supportedActions;

        #endregion "Member variables"
    }

    #region "Classes of the operations supported by GenericControlPointsProvider"

    public class ResizeVObjectAction : VObjectAction
    {
        internal ResizeVObjectAction(GenericControlPointsProvider provider)
            : base(VObjectAction.Resize, "Resize")
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _provider = provider;
        }

        public override bool Enabled
        {
            get
            {
                return _provider.ResizeEnabled;
            }
            set
            {
                _provider.ResizeEnabled = value;
            }
        }

        public ResizeMode ResizeMode
        {
            get
            {
                return _provider.ResizeMode;
            }
            set
            {
                _provider.ResizeMode = value;
            }
        }

        public ControlPoint MajorResizePointPrototype
        {
            get
            {
                return _provider.MajorResizePointPrototype;
            }
            set
            {
                _provider.MajorResizePointPrototype = value;
            }
        }

        public ControlPoint MinorResizePointPrototype
        {
            get
            {
                return _provider.MinorResizePointPrototype;
            }
            set
            {
                _provider.MinorResizePointPrototype = value;
            }
        }

        public System.Windows.Forms.Cursor ResizeNSCursor
        {
            get
            {
                return _provider.ResizeNSCursor;
            }
            set
            {
                _provider.ResizeNSCursor = value;
            }
        }

        public System.Windows.Forms.Cursor ResizeWECursor
        {
            get
            {
                return _provider.ResizeWECursor;
            }
            set
            {
                _provider.ResizeWECursor = value;
            }
        }

        public System.Windows.Forms.Cursor ResizeNESWCursor
        {
            get
            {
                return _provider.ResizeNESWCursor;
            }
            set
            {
                _provider.ResizeNESWCursor = value;
            }
        }

        public System.Windows.Forms.Cursor ResizeNWSECursor
        {
            get
            {
                return _provider.ResizeNWSECursor;
            }
            set
            {
                _provider.ResizeNWSECursor = value;
            }
        }

        #region "Member variables"

        private GenericControlPointsProvider _provider;

        #endregion "Member variables"
    }

    public class RotateVObjectAction : VObjectAction
    {
        internal RotateVObjectAction(GenericControlPointsProvider provider)
            : base(VObjectAction.Rotate, "Rotate")
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _provider = provider;
        }

        public override bool Enabled
        {
            get
            {
                return _provider.RotateEnabled;
            }
            set
            {
                _provider.RotateEnabled = value;
            }
        }

        public ControlPoint RotatePointPrototype
        {
            get
            {
                return _provider.RotatePointPrototype;
            }
            set
            {
                _provider.RotatePointPrototype = value;
            }
        }

        public ControlPoint RotateCenterPointPrototype
        {
            get
            {
                return _provider.RotateCenterPointPrototype;
            }
            set
            {
                _provider.RotateCenterPointPrototype = value;
            }
        }

        public System.Windows.Forms.Cursor RotatePointCursor
        {
            get
            {
                return _provider.RotatePointCursor;
            }
            set
            {
                _provider.RotatePointCursor = value;
            }
        }

        public System.Windows.Forms.Cursor RotateCenterCursor
        {
            get
            {
                return _provider.RotateCenterCursor;
            }
            set
            {
                _provider.RotateCenterCursor = value;
            }
        }

        #region "Member variables"

        private GenericControlPointsProvider _provider;

        #endregion "Member variables"
    }

    public class SkewVObjectAction : VObjectAction
    {
        internal SkewVObjectAction(GenericControlPointsProvider provider)
            : base(VObjectAction.Skew, "Skew")
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _provider = provider;
        }

        public override bool Enabled
        {
            get
            {
                return _provider.SkewEnabled;
            }
            set
            {
                _provider.SkewEnabled = value;
            }
        }

        public ControlPoint ControlPointPrototype
        {
            get
            {
                return _provider.SkewPointPrototype;
            }
            set
            {
                _provider.SkewPointPrototype = value;
            }
        }

        public System.Windows.Forms.Cursor ControlPointCursor
        {
            get
            {
                return _provider.SkewCursor;
            }
            set
            {
                _provider.SkewCursor = value;
            }
        }

        #region "Member variables"

        private GenericControlPointsProvider _provider;

        #endregion "Member variables"
    }

    public class DragVObjectAction : VObjectAction
    {
        internal DragVObjectAction(GenericControlPointsProvider provider)
            : base(VObjectAction.Drag, "Drag")
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _provider = provider;
        }

        public override bool Enabled
        {
            get
            {
                return _provider.DragEnabled;
            }
            set
            {
                _provider.DragEnabled = value;
            }
        }

        public System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return _provider.DragCursor;
            }
            set
            {
                _provider.DragCursor = value;
            }
        }

        #region "Member variables"

        private GenericControlPointsProvider _provider;

        #endregion "Member variables"
    }

    #endregion "Classes of the operations supported by GenericControlPointsProvider"
}