// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Line vector object.
    /// </summary>
    [System.Serializable]
    public class LineVObject : PathVObject
    {
        public LineVObject(float x0, float y0, float x1, float y1)
            : base("line")
        {
            CreatePath(x0, y0, x1, y1);
        }

        public LineVObject(System.Drawing.PointF pnt0, System.Drawing.PointF pnt1)
            : base("line")
        {
            CreatePath(pnt0.X, pnt0.Y, pnt1.X, pnt1.Y);
        }

        protected LineVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _path = BinarySerializer.DeserializePath((byte[])info.GetValue(SerializationNames.Path, typeof(byte[])));
        }

        private void CreatePath(float x0, float y0, float x1, float y1)
        {
            if (System.Math.Abs(x0 - x1) < VObject.Eps)
                x0 += 0.05f;

            if (System.Math.Abs(y0 - y1) < VObject.Eps)
                y0 += 0.05f;

            _path = new System.Drawing.Drawing2D.GraphicsPath();
            _path.AddLine(x0, y0, x1, y1);
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