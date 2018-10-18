// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Text vector object. Uses GDI+ for text rendering. Allows to recalculate vector object bounds
    /// depending on entered text dimensions - see Update(bool) method.
    /// </summary>
    [System.Serializable]
    public class TextVObject : PathVObject
    {
        #region "Nested action class"

        public class ChangeTextAreaAction : VObjectAction
        {
            internal ChangeTextAreaAction(TextControlPointsProvider provider)
                : base(VObjectAction.ChangeTextArea, "ChangeTextArea")
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

            private TextControlPointsProvider _provider;

            #endregion "Member variables"
        }

        #endregion "Nested action class"

        #region "Construction / destruction"

        public TextVObject()
            : base("text")
        {
            base.BrushInternal = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            base.PenInternal = null;

            _font = new System.Drawing.Font("Arial", 16);
            _format = (System.Drawing.StringFormat)System.Drawing.StringFormat.GenericDefault.Clone();
            _text = string.Empty;
            _textArea = new System.Drawing.RectangleF(0, 0, 300, 200);

            _textControlPointsProvider = new TextControlPointsProvider(this);
            _textControlPointsProvider.ControlPointsEnabled = false;
            base.JointControlPointsProvider.InsertProvider(0, _textControlPointsProvider);
        }

        public TextVObject(string text, string fontName, float fontSize, System.Drawing.RectangleF bounds)
            : base("text")
        {
            if (text == null)
                throw new System.ArgumentNullException("text");

            if (fontName == null)
                throw new System.ArgumentNullException("fontName");

            if (fontSize < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("fontSize");

            if (bounds.Width < VObject.Eps || bounds.Height < VObject.Eps)
                throw new System.ArgumentOutOfRangeException("bounds");

            base.BrushInternal = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            base.PenInternal = null;

            _font = new System.Drawing.Font(fontName, fontSize);
            _format = (System.Drawing.StringFormat)System.Drawing.StringFormat.GenericDefault.Clone();
            _text = text;
            _textArea = bounds;

            Update(false);

            _textControlPointsProvider = new TextControlPointsProvider(this);
            _textControlPointsProvider.ControlPointsEnabled = false;
            base.JointControlPointsProvider.InsertProvider(0, _textControlPointsProvider);
        }

        protected TextVObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            _text = info.GetString(SerializationNames.TextText);
            _font = (System.Drawing.Font)info.GetValue(SerializationNames.TextFont, typeof(System.Drawing.Font));
            _format = BinarySerializer.DeserializeStringFormat((byte[])info.GetValue(SerializationNames.TextFormat, typeof(byte[])));
            _textArea = (System.Drawing.RectangleF)info.GetValue(SerializationNames.TextBounds, typeof(System.Drawing.RectangleF));

            Update(false);

            _textControlPointsProvider = new TextControlPointsProvider(this);
            base.JointControlPointsProvider.InsertProvider(0, _textControlPointsProvider);
            _textControlPointsProvider.SetObjectData(info, context);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_path != null)
                {
                    _path.Dispose();
                    _path = null;
                }
                if (_boundsPath != null)
                {
                    _boundsPath.Dispose();
                    _boundsPath = null;
                }
                if (_format != null)
                {
                    _format.Dispose();
                    _format = null;
                }
                if (_font != null)
                {
                    _font.Dispose();
                    _font = null;
                }
            }
        }

        #endregion "Construction / destruction"

        #region "Properties"

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _text = value;
                Update(false);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.RectangleF TextArea
        {
            get
            {
                return _textArea;
            }
            set
            {
                _textArea = value;
                Update(false);
            }
        }

        public System.Drawing.Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _font = value;
                Update(false);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public System.Drawing.StringFormat Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException("value");

                _format = value;
                Update(false);
            }
        }

        protected override System.Drawing.Drawing2D.GraphicsPath Path
        {
            get
            {
                return _path;
            }
        }

        #endregion "Properties"

        #region "Implementation of the functionality"

        public override System.Drawing.RectangleF GetTransformedVObjectBounds()
        {
            System.Drawing.RectangleF result = _boundsPath.GetBounds(base.Transform, base.Pen);
            return result;
        }

        public override System.Drawing.RectangleF GetVObjectBounds()
        {
            return _boundsPath.GetBounds();
        }

        public override bool HitTest(System.Drawing.PointF point, float precisionDelta)
        {
            bool result = false;

            using (System.Drawing.Drawing2D.GraphicsPath boundsPath = (System.Drawing.Drawing2D.GraphicsPath)_boundsPath.Clone())
            {
                boundsPath.Transform(base.Transform);
                result = boundsPath.IsVisible(point);
            }

            return result;
        }

        public override void Update()
        {
            Update(false);
        }

        public void Update(bool recalculateTextBounds)
        {
            if (_path != null)
                _path.Dispose();

            if (_boundsPath != null)
                _boundsPath.Dispose();

            _format.FormatFlags |= System.Drawing.StringFormatFlags.LineLimit;
            _path = new System.Drawing.Drawing2D.GraphicsPath();

            if (recalculateTextBounds)
            {
                System.Drawing.PointF origin = System.Drawing.PointF.Empty;
                switch (_format.Alignment)
                {
                    case System.Drawing.StringAlignment.Far:
                        origin.X = _textArea.Right;
                        break;

                    case System.Drawing.StringAlignment.Center:
                        origin.X = _textArea.Left + _textArea.Width / 2;
                        break;

                    case System.Drawing.StringAlignment.Near:
                        origin.X = _textArea.Left;
                        break;

                    default:
                        throw new Aurigma.GraphicsMill.UnexpectedException();
                }

                switch (_format.LineAlignment)
                {
                    case System.Drawing.StringAlignment.Far:
                        origin.Y = TextArea.Bottom;
                        break;

                    case System.Drawing.StringAlignment.Center:
                        origin.Y = _textArea.Top + _textArea.Height / 2;
                        break;

                    case System.Drawing.StringAlignment.Near:
                        origin.Y = _textArea.Top;
                        break;

                    default:
                        throw new Aurigma.GraphicsMill.UnexpectedException();
                }

                _path.AddString(_text, _font.FontFamily, (int)_font.Style, _font.Size, origin, _format);
                UpdateTextArea(origin);
            }
            else
            {
                _path.AddString(_text, _font.FontFamily, (int)_font.Style, _font.Size, _textArea, _format);

                System.Drawing.RectangleF stringBounds = _path.GetBounds();
            }

            _boundsPath = new System.Drawing.Drawing2D.GraphicsPath();
            _boundsPath.AddRectangle(_textArea);

            base.Update();
        }

        private void UpdateTextArea(System.Drawing.PointF origin)
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(System.IntPtr.Zero))
            {
                System.Drawing.StringFormat sf = (System.Drawing.StringFormat)_format.Clone();
                sf.FormatFlags |= System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
                sf.Trimming = System.Drawing.StringTrimming.Character;
                sf.SetMeasurableCharacterRanges(new System.Drawing.CharacterRange[] { new System.Drawing.CharacterRange(0, _text.Length) });

                g.PageUnit = System.Drawing.GraphicsUnit.Point;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                System.Drawing.SizeF textSize = g.MeasureString(_text, _font, origin, sf);

                float x, y;
                switch (_format.Alignment)
                {
                    case System.Drawing.StringAlignment.Far:
                        x = origin.X - textSize.Width;
                        break;

                    case System.Drawing.StringAlignment.Center:
                        x = origin.X - textSize.Width / 2;
                        break;

                    case System.Drawing.StringAlignment.Near:
                        x = origin.X;
                        break;

                    default:
                        throw new Aurigma.GraphicsMill.UnexpectedException();
                }

                switch (_format.LineAlignment)
                {
                    case System.Drawing.StringAlignment.Far:
                        y = origin.Y - textSize.Height;
                        break;

                    case System.Drawing.StringAlignment.Center:
                        y = origin.Y - textSize.Height / 2;
                        break;

                    case System.Drawing.StringAlignment.Near:
                        y = origin.Y;
                        break;

                    default:
                        throw new Aurigma.GraphicsMill.UnexpectedException();
                }

                _textArea = new System.Drawing.RectangleF(x, y, textSize.Width, textSize.Height);
            }
        }

        #endregion "Implementation of the functionality"

        #region "ISerializable interface implementation"

        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            base.GetObjectData(info, context);

            info.AddValue(SerializationNames.TextText, _text);
            info.AddValue(SerializationNames.TextFont, _font);
            info.AddValue(SerializationNames.TextFormat, BinarySerializer.Serialize(_format));
            info.AddValue(SerializationNames.TextBounds, _textArea);

            _textControlPointsProvider.GetObjectData(info, context);
        }

        #endregion "ISerializable interface implementation"

        #region "-------- Member variables ---------"

        private System.Drawing.Drawing2D.GraphicsPath _path;
        private System.Drawing.Drawing2D.GraphicsPath _boundsPath;

        private string _text;
        private System.Drawing.Font _font;
        private System.Drawing.StringFormat _format;
        private System.Drawing.RectangleF _textArea;

        private TextControlPointsProvider _textControlPointsProvider;

        #endregion "-------- Member variables ---------"
    }
}