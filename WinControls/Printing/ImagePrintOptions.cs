// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using Aurigma.GraphicsMill.Drawing;
using Aurigma.GraphicsMill.Transforms;
using System;
using System.ComponentModel;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    public class PrintOptions
    {
        public PrintOptions()
        {
            _imageAutoRotate = true;
            _placeholderSize = Size.Empty;

            _imageFitMode = ImageFitMode.ResizeToFit;
            _horizontalSpacing = 25;
            _verticalSpacing = 25;

            _interpolationMode = ResizeInterpolationMode.Medium;

            _headerFont = new Font("Arial", 10).ToGdiPlusFont();
            _footerFont = new Font("Arial", 10).ToGdiPlusFont();
            _headerColor = System.Drawing.Color.Black;
            _footerColor = System.Drawing.Color.Black;
            _headerAlignment = System.Drawing.StringAlignment.Center;
            _footerAlignment = System.Drawing.StringAlignment.Center;
            _headerTrimming = System.Drawing.StringTrimming.EllipsisCharacter;
            _footerTrimming = System.Drawing.StringTrimming.EllipsisCharacter;

            _borderColor = System.Drawing.Color.Black;
            _borderWidth = 2;
        }

        #region "Properties"

        [Browsable(true)]
        [DefaultValue(typeof(Aurigma.GraphicsMill.WinControls.ImageFitMode), "ResizeToFit")]
        [ResDescription("PrintOptions_ImageFitMode")]
        public ImageFitMode ImageFitMode
        {
            get
            {
                return _imageFitMode;
            }
            set
            {
                _imageFitMode = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [ResDescription("PrintOptions_ImageAutoRotate")]
        public bool ImageAutoRotate
        {
            get
            {
                return _imageAutoRotate;
            }
            set
            {
                _imageAutoRotate = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [ResDescription("PrintOptions_PlaceholderAutoRotate")]
        public bool PlaceholderAutoRotate
        {
            get
            {
                return _placeholderAutoRotate;
            }
            set
            {
                _placeholderAutoRotate = value;
            }
        }

        [Browsable(true)]
        [ResDescription("PrintOptions_PlaceholderSize")]
        public Size PlaceholderSize
        {
            get
            {
                return _placeholderSize;
            }
            set
            {
                _placeholderSize = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(25)]
        [ResDescription("PrintOptions_VerticalSpacing")]
        public int VerticalSpacing
        {
            get
            {
                return _verticalSpacing;
            }
            set
            {
                _verticalSpacing = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(25)]
        [ResDescription("PrintOptions_HorizontalSpacing")]
        public int HorizontalSpacing
        {
            get
            {
                return _horizontalSpacing;
            }
            set
            {
                _horizontalSpacing = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Aurigma.GraphicsMill.Transforms.ResizeInterpolationMode), "Medium")]
        [ResDescription("PrintOptions_InterpolationMode")]
        public ResizeInterpolationMode InterpolationMode
        {
            get
            {
                return _interpolationMode;
            }
            set
            {
                _interpolationMode = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [ResDescription("PrintOptions_HeaderEnabled")]
        public bool HeaderEnabled
        {
            get
            {
                return _headerEnabled;
            }
            set
            {
                _headerEnabled = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [ResDescription("PrintOptions_FooterEnabled")]
        public bool FooterEnabled
        {
            get
            {
                return _footerEnabled;
            }
            set
            {
                _footerEnabled = value;
            }
        }

        [Browsable(true)]
        [ResDescription("PrintOptions_HeaderFont")]
        public System.Drawing.Font HeaderFont
        {
            get
            {
                return _headerFont;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _headerFont = value;
            }
        }

        [Browsable(true)]
        [ResDescription("PrintOptions_FooterFont")]
        public System.Drawing.Font FooterFont
        {
            get
            {
                return _footerFont;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _footerFont = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.StringAlignment), "Center")]
        [ResDescription("PrintOptions_HeaderAlignment")]
        public System.Drawing.StringAlignment HeaderAlignment
        {
            get
            {
                return _headerAlignment;
            }
            set
            {
                _headerAlignment = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.StringAlignment), "Center")]
        [ResDescription("PrintOptions_FooterAlignment")]
        public System.Drawing.StringAlignment FooterAlignment
        {
            get
            {
                return _footerAlignment;
            }
            set
            {
                _footerAlignment = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.StringTrimming), "EllipsisCharacter")]
        [ResDescription("PrintOptions_HeaderTrimming")]
        public System.Drawing.StringTrimming HeaderTrimming
        {
            get
            {
                return _headerTrimming;
            }
            set
            {
                _headerTrimming = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.StringTrimming), "EllipsisCharacter")]
        [ResDescription("PrintOptions_FooterTrimming")]
        public System.Drawing.StringTrimming FooterTrimming
        {
            get
            {
                return _footerTrimming;
            }
            set
            {
                _footerTrimming = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "Black")]
        [ResDescription("PrintOptions_HeaderColor")]
        public System.Drawing.Color HeaderColor
        {
            get
            {
                return _headerColor;
            }
            set
            {
                _headerColor = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "Black")]
        [ResDescription("PrintOptions_FooterColor")]
        public System.Drawing.Color FooterColor
        {
            get
            {
                return _footerColor;
            }
            set
            {
                _footerColor = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [ResDescription("PrintOptions_BorderEnabled")]
        public bool BorderEnabled
        {
            get
            {
                return _borderEnabled;
            }
            set
            {
                _borderEnabled = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(System.Drawing.Color), "Black")]
        [ResDescription("PrintOptions_BorderColor")]
        public System.Drawing.Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }

        [Browsable(true)]
        [DefaultValue(2)]
        [ResDescription("PrintOptions_BorderWidth")]
        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

        #endregion "Properties"

        private ResizeInterpolationMode _interpolationMode;
        private ImageFitMode _imageFitMode;
        private bool _imageAutoRotate;
        private bool _placeholderAutoRotate;

        private Size _placeholderSize;

        private int _verticalSpacing;
        private int _horizontalSpacing;

        private bool _borderEnabled;
        private System.Drawing.Color _borderColor;
        private int _borderWidth;

        private bool _headerEnabled;
        private System.Drawing.Font _headerFont;
        private System.Drawing.StringAlignment _headerAlignment;
        private System.Drawing.StringTrimming _headerTrimming;
        private System.Drawing.Color _headerColor;
        private bool _footerEnabled;
        private System.Drawing.Font _footerFont;
        private System.Drawing.StringAlignment _footerAlignment;
        private System.Drawing.StringTrimming _footerTrimming;
        private System.Drawing.Color _footerColor;
    }
}