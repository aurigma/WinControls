// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class TextControlPointsProvider : IControlPointsProvider
    {
        //
        // The class provides control points which could be used to
        // change text bounds of the underlying object. Order of points:
        //
        // 0         1
        //   -------
        //  |		|
        //  |		|
        //	 -------
        // 3         2
        //

        #region "Construction / destruction"

        public TextControlPointsProvider(TextVObject obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("obj");

            _obj = obj;
            _obj.Changed += new EventHandler(VObjectChangedHandler);

            _points = new System.Drawing.PointF[4];
            _controlPointsEnabled = true;

            _controlPointPrototype = new DiamondControlPoint(new System.Drawing.Size(12, 12), new System.Drawing.SolidBrush(System.Drawing.Color.OrangeRed), new System.Drawing.Pen(System.Drawing.Color.DarkRed));
            _controlPointCursor = System.Windows.Forms.Cursors.Cross;
            _supportedActions = new VObjectActionCollection(new IVObjectAction[] { new TextVObject.ChangeTextAreaAction(this) });

            RecreateControlPoints();
        }

        #endregion "Construction / destruction"

        #region "IControlPointsProvider implementation"

        public IControlPointCollection ControlPoints
        {
            get
            {
                UpdateControlPoints();
                return _transformedPoints;
            }
        }

        public IVObjectActionCollection SupportedActions
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
                System.Drawing.Size size = _transformedPoints[0].GetSize();
                return System.Math.Max(size.Width, size.Height) + 1 / 2;
            }
        }

        public System.Windows.Forms.Cursor GetPointCursor(int index)
        {
            return _controlPointCursor;
        }

        public System.Drawing.RectangleF GetControlPointsBounds()
        {
            return _obj.GetTransformedVObjectBounds();
        }

        public void ClickPoint(int index)
        {
        }

        public void DragPoint(int index, System.Drawing.PointF newPosition)
        {
            if (index < 0 || index > 3)
                throw new System.ArgumentOutOfRangeException("index");

            UpdateInverseMatrix();
            System.Drawing.PointF newBasePoint = newPosition;
            VObjectsUtils.TransformPoint(_objInverseMatrix, ref newBasePoint);

            System.Drawing.PointF scaleRefPoint = GetScaleRefPoint(index);
            float dx = Math.Sign(_points[index].X - scaleRefPoint.X) * (newBasePoint.X - _points[index].X);
            float dy = Math.Sign(_points[index].Y - scaleRefPoint.Y) * (newBasePoint.Y - _points[index].Y);

            System.Drawing.RectangleF newBounds = VObjectsUtils.GetBoundingRectangle(newBasePoint, scaleRefPoint);
            if (newBounds.Width < 1 || newBounds.Height < 1)
                return;

            if (_obj.TextArea.Height + dy < 0)
                _flipY = !_flipY;
            if (_obj.TextArea.Width + dx < 0)
                _flipX = !_flipX;

            _obj.TextArea = newBounds;
        }

        private void UpdateInverseMatrix()
        {
            if (_objInverseMatrix == null || _updateInverseMatrix)
            {
                if (_objInverseMatrix != null)
                    _objInverseMatrix.Dispose();

                _objInverseMatrix = _obj.Transform.Clone();
                _objInverseMatrix.Invert();

                _updateInverseMatrix = false;
            }
        }

        private System.Drawing.PointF GetScaleRefPoint(int index)
        {
            switch (index)
            {
                case 0:
                    return _points[2];

                case 1:
                    return _points[3];

                case 2:
                    return _points[0];

                case 3:
                    return _points[1];

                default:
                    throw new System.ArgumentOutOfRangeException("index");
            }
        }

        private void UpdateControlPoints()
        {
            if (_updateControlPoints)
            {
                UpdateBaseRectanglePoints();

                System.Drawing.PointF[] interim = VObjectsUtils.TransformPoints(_obj.Transform, _points);
                for (int i = 0; i < _transformedPoints.Count; i++)
                    _transformedPoints[i].Location = interim[i];

                _updateControlPoints = false;
            }
        }

        private void UpdateBaseRectanglePoints()
        {
            System.Drawing.RectangleF baseRect = _obj.GetVObjectBounds();
            _points[0] = new System.Drawing.PointF(baseRect.Left, baseRect.Top);
            _points[1] = new System.Drawing.PointF(baseRect.Right, baseRect.Top);
            _points[2] = new System.Drawing.PointF(baseRect.Right, baseRect.Bottom);
            _points[3] = new System.Drawing.PointF(baseRect.Left, baseRect.Bottom);

            System.Drawing.PointF tmp;
            if (_flipX)
            {
                tmp = _points[0];
                _points[0] = _points[1];
                _points[1] = tmp;

                tmp = _points[3];
                _points[3] = _points[2];
                _points[2] = tmp;
            }

            if (_flipY)
            {
                tmp = _points[0];
                _points[0] = _points[3];
                _points[3] = tmp;

                tmp = _points[1];
                _points[1] = _points[2];
                _points[2] = tmp;
            }
        }

        #endregion "IControlPointsProvider implementation"

        #region "Internal methods (for ChangeTextAreaAction)"

        internal ControlPoint ControlPointPrototype
        {
            get
            {
                return _controlPointPrototype;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");
                if (value.GetType().GetInterface("System.ICloneable", false) == null)
                    throw new System.ArgumentException(StringResources.GetString("ExStrTypeShouldBeCloneable"), "value");

                _controlPointPrototype = value;
                RecreateControlPoints();
                _obj.Update();
            }
        }

        internal System.Windows.Forms.Cursor ControlPointCursor
        {
            get
            {
                return _controlPointCursor;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _controlPointCursor = value;
            }
        }

        internal bool ControlPointsEnabled
        {
            get
            {
                return _controlPointsEnabled;
            }
            set
            {
                if (_controlPointsEnabled != value)
                {
                    _controlPointsEnabled = value;
                    UpdateControlPointsState();
                    _obj.Update();
                }
            }
        }

        #endregion "Internal methods (for ChangeTextAreaAction)"

        #region "Serialization support"

        internal void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.TextAreaPointsEnabled, _controlPointsEnabled);
            if (_controlPointPrototype.GetType().IsSerializable)
                info.AddValue(SerializationNames.TextAreaPointPrototype, _controlPointPrototype);
        }

        internal void SetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _controlPointsEnabled = info.GetBoolean(SerializationNames.TextAreaPointsEnabled);

            try
            {
                _controlPointPrototype = (ControlPoint)info.GetValue(SerializationNames.TextAreaPointPrototype, typeof(ControlPoint));
                RecreateControlPoints();
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                // Prototype wasn't saved during serialization.
            }
        }

        #endregion "Serialization support"

        #region "Other private methods"

        private void VObjectChangedHandler(object sender, EventArgs e)
        {
            _updateControlPoints = true;
            _updateInverseMatrix = true;
        }

        private void RecreateControlPoints()
        {
            System.ICloneable prototype = (System.ICloneable)_controlPointPrototype;

            ControlPoint[] controlPoints = new ControlPoint[4];
            for (int i = 0; i < controlPoints.Length; i++)
                controlPoints[i] = (ControlPoint)prototype.Clone();

            _transformedPoints = new ControlPointCollection(controlPoints);
            _updateControlPoints = true;

            UpdateControlPointsState();
        }

        private void UpdateControlPointsState()
        {
            foreach (ControlPoint point in _transformedPoints)
                point.Enabled = _controlPointsEnabled;
        }

        #endregion "Other private methods"

        #region "Member variables"

        private TextVObject _obj;
        private System.Drawing.Drawing2D.Matrix _objInverseMatrix;

        private bool _flipX;
        private bool _flipY;
        private System.Drawing.PointF[] _points;

        private bool _updateControlPoints;
        private bool _updateInverseMatrix;
        private bool _controlPointsEnabled;

        private ControlPointCollection _transformedPoints;
        private VObjectActionCollection _supportedActions;

        private ControlPoint _controlPointPrototype;
        private System.Windows.Forms.Cursor _controlPointCursor;

        #endregion "Member variables"
    }
}