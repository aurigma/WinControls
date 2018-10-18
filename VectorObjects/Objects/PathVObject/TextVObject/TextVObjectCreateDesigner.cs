// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Text vector object create designer. Allows specifying text bounds by click and drag action.
    /// </summary>
    public class TextVObjectCreateDesigner : ClickDragCreateDesigner
    {
        #region "Construction / destruction"

        public TextVObjectCreateDesigner()
        {
            _text = "Text sample...";
            _brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0));
            _font = new System.Drawing.Font("Arial", 16);
            _format = (System.Drawing.StringFormat)System.Drawing.StringFormat.GenericDefault.Clone();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
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

                    if (_font != null)
                    {
                        _font.Dispose();
                        _font = null;
                    }

                    if (_format != null)
                    {
                        _format.Dispose();
                        _format = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion "Construction / destruction"

        protected override IVObject CreateObject(System.Drawing.RectangleF destinationRectangle)
        {
            if (destinationRectangle.Width < 10)
                destinationRectangle.Width = 300;
            if (destinationRectangle.Height < 10)
                destinationRectangle.Height = 200;

            TextVObject obj = new TextVObject();
            obj.Text = _text;
            obj.Font = _font;
            obj.Brush = _brush;
            obj.Format = _format;
            obj.TextArea = destinationRectangle;

            return obj;
        }

        public System.Drawing.Brush Brush
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

        public System.Drawing.Pen Pen
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
            }
        }

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
            }
        }

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
            }
        }

        #region "Member variables"

        private System.Drawing.Pen _pen;
        private System.Drawing.Brush _brush;
        private System.Drawing.Font _font;
        private System.Drawing.StringFormat _format;
        private string _text;

        #endregion "Member variables"
    }
}