// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Base implementation of the IVObject interface. It provides some properties (Tag/DrawMode/Name/EditDesigner/etc),
    /// resize/rotate/skew control points and methods to extend control points set.
    /// </summary>
    [System.Serializable]
    public abstract class VObject : IVObject,
        System.Runtime.Serialization.ISerializable,
        System.IDisposable
    {
        #region "Constants"

        internal const float Eps = 0.00001f;
        internal const int MaxSize = 20000;
        internal const float SelectionPrecisionDelta = 12.0f;
        static internal readonly System.Drawing.Size InvalidationMargin = new System.Drawing.Size(10, 10);

        #endregion "Constants"

        #region "Construction / destruction"

        protected VObject()
        {
            _drawMode = VObjectDrawMode.Normal;
            _name = "VObject";

            _genericControlPointsProvider = new GenericControlPointsProvider(this);
            _jointControlPointsProvider = new JointControlPointsProvider(_genericControlPointsProvider);
        }

        protected VObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : this()
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _name = info.GetString(SerializationNames.Name);
            _locked = info.GetBoolean(SerializationNames.Locked);

            _genericControlPointsProvider.SetObjectData(info, context);
        }

        ~VObject()
        {
            Dispose(false);
        }

        #endregion "Construction / destruction"

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                OnChanged(System.EventArgs.Empty);
            }
            finally
            {
                System.GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        #endregion IDisposable Members

        #region "IBaseRectangle interface members"

        [System.ComponentModel.Browsable(false)]
        public abstract System.Drawing.Drawing2D.Matrix Transform
        {
            get;
            set;
        }

        public abstract System.Drawing.RectangleF GetVObjectBounds();

        #endregion "IBaseRectangle interface members"

        #region "IVObject interface members"

        [System.ComponentModel.Browsable(false)]
        public virtual Aurigma.GraphicsMill.WinControls.VObjectDrawMode DrawMode
        {
            get
            {
                return _drawMode;
            }
            set
            {
                _drawMode = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnChanged(System.EventArgs.Empty);
                }
            }
        }

        public virtual void Update()
        {
            OnChanged(System.EventArgs.Empty);
        }

        [System.ComponentModel.Browsable(false)]
        public abstract IDesigner Designer
        {
            get;
        }

        public abstract System.Drawing.RectangleF GetTransformedVObjectBounds();

        public abstract bool HitTest(System.Drawing.PointF point, float precisionDelta);

        public abstract void Draw(System.Drawing.Rectangle renderingRect, System.Drawing.Graphics g, ICoordinateMapper coordinateMapper);

        public event System.EventHandler Changed;

        #endregion "IVObject interface members"

        #region "Changed event methods"

        protected virtual void OnChanged(System.EventArgs e)
        {
            if (this.Changed != null)
                this.Changed(this, e);
        }

        #endregion "Changed event methods"

        #region "IControlPointsProvider interface implementing"

        [System.ComponentModel.Browsable(false)]
        public IControlPointCollection ControlPoints
        {
            get
            {
                return _jointControlPointsProvider.ControlPoints;
            }
        }

        public IVObjectActionCollection SupportedActions
        {
            get
            {
                return _jointControlPointsProvider.SupportedActions;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int MaxControlPointRadius
        {
            get
            {
                return _jointControlPointsProvider.MaxControlPointRadius;
            }
        }

        public System.Windows.Forms.Cursor GetPointCursor(int index)
        {
            return _jointControlPointsProvider.GetPointCursor(index);
        }

        public System.Drawing.RectangleF GetControlPointsBounds()
        {
            return _jointControlPointsProvider.GetControlPointsBounds();
        }

        public void DragPoint(int index, System.Drawing.PointF newPosition)
        {
            _jointControlPointsProvider.DragPoint(index, newPosition);
            OnChanged(System.EventArgs.Empty);
        }

        public void ClickPoint(int index)
        {
            _jointControlPointsProvider.ClickPoint(index);
            OnChanged(System.EventArgs.Empty);
        }

        #endregion "IControlPointsProvider interface implementing"

        #region ISerializable Members

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(SerializationNames.Name, this.Name);
            info.AddValue(SerializationNames.Locked, this.Locked);

            _genericControlPointsProvider.GetObjectData(info, context);
        }

        #endregion ISerializable Members

        #region "Protected properties for inheritors"

        protected JointControlPointsProvider JointControlPointsProvider
        {
            get
            {
                return _jointControlPointsProvider;
            }
        }

        protected IControlPointsProvider GenericControlPointsProvider
        {
            get
            {
                return _genericControlPointsProvider;
            }
        }

        #endregion "Protected properties for inheritors"

        #region "Member variables"

        private string _name;
        private VObjectDrawMode _drawMode;
        private object _tag;
        private bool _locked;
        private bool _isDisposed;

        private JointControlPointsProvider _jointControlPointsProvider;
        private GenericControlPointsProvider _genericControlPointsProvider;

        #endregion "Member variables"
    }
}