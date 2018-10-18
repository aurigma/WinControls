// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Polyline-specific additional control points provider. Allows to drag node-points.
    /// </summary>
    internal class PolylineControlPointsProvider : IControlPointsProvider
    {
        #region "Construction / destruction"

        public PolylineControlPointsProvider(PolylineVObject obj)
        {
            _obj = obj;
            _obj.Changed += new System.EventHandler(VObjectChangedHandler);

            _controlPointPrototype = new EllipseControlPoint(new System.Drawing.Size(8, 8), new System.Drawing.SolidBrush(System.Drawing.Color.Tomato), new System.Drawing.Pen(System.Drawing.Color.WhiteSmoke));
            _controlPointCursor = System.Windows.Forms.Cursors.Cross;

            _controlPointsEnabled = true;
            _supportedActions = new VObjectActionCollection(new IVObjectAction[] { new PolylineVObject.MoveNodeAction(this) });

            RecreateControlPoints();
        }

        #endregion "Construction / destruction"

        #region "IControlPointsProvider functionality implementation"

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
            if (index < 0 || index >= _obj.Points.Length)
                throw new System.ArgumentOutOfRangeException("index");

            return _controlPointCursor;
        }

        public System.Drawing.RectangleF GetControlPointsBounds()
        {
            return _obj.GetTransformedVObjectBounds();
        }

        public void DragPoint(int index, System.Drawing.PointF newPosition)
        {
            if (index < 0 || index >= _obj.Points.Length)
                throw new System.ArgumentOutOfRangeException("index");

            System.Drawing.PointF dragVector = new System.Drawing.PointF(newPosition.X - _transformedPoints[index].X, newPosition.Y - _transformedPoints[index].Y);
            using (System.Drawing.Drawing2D.Matrix inverseMatrix = (System.Drawing.Drawing2D.Matrix)_obj.Transform.Clone())
            {
                inverseMatrix.Invert();
                VObjectsUtils.TransformVector(inverseMatrix, ref dragVector);

                _obj.Points[index].X += dragVector.X;
                _obj.Points[index].Y += dragVector.Y;
            }

            _obj.Update();
        }

        public void ClickPoint(int index)
        {
        }

        #region "Private part"

        private void RecreateControlPoints()
        {
            System.ICloneable prototype = (System.ICloneable)_controlPointPrototype;

            ControlPoint[] controlPoints = new ControlPoint[_obj.Points.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                controlPoints[i] = (ControlPoint)prototype.Clone();

            _transformedPoints = new ControlPointCollection(controlPoints);
            _updateProvider = true;

            UpdateControlPointsState();
        }

        private void UpdateControlPoints()
        {
            if (_updateProvider)
            {
                System.Drawing.PointF[] interim = VObjectsUtils.TransformPoints(_obj.Transform, _obj.Points);
                for (int i = 0; i < _transformedPoints.Count; i++)
                    _transformedPoints[i].Location = interim[i];

                _updateProvider = false;
            }
        }

        private void UpdateControlPointsState()
        {
            for (int i = 0; i < _transformedPoints.Count; i++)
                _transformedPoints[i].Enabled = _controlPointsEnabled;
        }

        private void VObjectChangedHandler(object sender, System.EventArgs e)
        {
            _updateProvider = true;
        }

        #endregion "Private part"

        #endregion "IControlPointsProvider functionality implementation"

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

        #region "Internal methods - for MoveNodeAction"

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
                    throw new System.ArgumentNullException("value");

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

        #endregion "Internal methods - for MoveNodeAction"

        #region "Member variables"

        private PolylineVObject _obj;
        private ControlPointCollection _transformedPoints;
        private bool _updateProvider;

        private bool _controlPointsEnabled;
        private VObjectActionCollection _supportedActions;

        private ControlPoint _controlPointPrototype;
        private System.Windows.Forms.Cursor _controlPointCursor;

        #endregion "Member variables"
    }
}