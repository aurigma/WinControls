// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Rectangle vector object.
    /// </summary>
    [System.Serializable]
    public class RectangleVObject : PathVObject
    {
        internal RectangleVObject(string name, System.Drawing.RectangleF rect)
            : base(name)
        {
            _path = new System.Drawing.Drawing2D.GraphicsPath();
            _path.AddRectangle(rect);
        }

        public RectangleVObject(System.Drawing.RectangleF rectangle)
            : base("rectangle")
        {
            _path = new System.Drawing.Drawing2D.GraphicsPath();
            _path.AddRectangle(rectangle);
        }

        public RectangleVObject(float x, float y, float width, float height)
            : base("rectangle")
        {
            _path = new System.Drawing.Drawing2D.GraphicsPath();
            _path.AddRectangle(new System.Drawing.RectangleF(x, y, width, height));
        }

        protected RectangleVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _path = BinarySerializer.DeserializePath((byte[])info.GetValue(SerializationNames.Path, typeof(byte[])));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _path != null)
                _path.Dispose();
        }

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue(SerializationNames.Path, BinarySerializer.Serialize(_path));
        }

        protected override System.Drawing.Drawing2D.GraphicsPath Path
        {
            get
            {
                return _path;
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath _path;
    }
}