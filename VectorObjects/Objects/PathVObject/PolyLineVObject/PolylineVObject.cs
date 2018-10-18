// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Polyline vector object.
    /// </summary>
    [System.Serializable]
    public class PolylineVObject : FreehandVObject
    {
        #region "Nested action class"

        public class MoveNodeAction : VObjectAction
        {
            internal MoveNodeAction(PolylineControlPointsProvider provider)
                : base(VObjectAction.MoveNode, "MoveNode")
            {
                if (provider == null)
                    throw new System.ArgumentNullException("provider");

                _provider = provider;
            }

            public override bool Enabled
            {
                get
                {
                    return _provider.ControlPointsEnabled;
                }
                set
                {
                    _provider.ControlPointsEnabled = value;
                }
            }

            public ControlPoint ControlPointPrototype
            {
                get
                {
                    return _provider.ControlPointPrototype;
                }
                set
                {
                    _provider.ControlPointPrototype = value;
                }
            }

            public System.Windows.Forms.Cursor ControlPointCursor
            {
                get
                {
                    return _provider.ControlPointCursor;
                }
                set
                {
                    _provider.ControlPointCursor = value;
                }
            }

            #region "Member variables"

            private PolylineControlPointsProvider _provider;

            #endregion "Member variables"
        }

        #endregion "Nested action class"

        public PolylineVObject(System.Drawing.PointF[] points, bool closePath, System.Drawing.Drawing2D.FillMode fillMode)
            : base("polyline", points, closePath, fillMode)
        {
            _polylineControlPointsProvider = new PolylineControlPointsProvider(this);
            base.JointControlPointsProvider.InsertProvider(0, _polylineControlPointsProvider);
        }

        protected PolylineVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _polylineControlPointsProvider = new PolylineControlPointsProvider(this);
            base.JointControlPointsProvider.InsertProvider(0, _polylineControlPointsProvider);
            _polylineControlPointsProvider.SetObjectData(info, context);
        }

        public override void Update()
        {
            base.UpdatePath();
            base.Update();
        }

        [System.ComponentModel.Browsable(false)]
        public new System.Drawing.PointF[] Points
        {
            get
            {
                return base.Points;
            }
        }

        #region "ISerializable interface implementation"

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);
            _polylineControlPointsProvider.GetObjectData(info, context);
        }

        #endregion "ISerializable interface implementation"

        #region "Members variables"

        private PolylineControlPointsProvider _polylineControlPointsProvider;

        #endregion "Members variables"
    }
}