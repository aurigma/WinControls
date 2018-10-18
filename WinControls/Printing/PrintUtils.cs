// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class PrintUtils
    {
        private PrintUtils()
        {
        }

        public static int InchHundredthsToPixels(int value, int printerResolution)
        {
            return (int)Math.Round((float)value / 100.0f * printerResolution);
        }

        public static Size FitSizeToBounds(Size value, Size bounds)
        {
            if (value.Width <= bounds.Width && value.Height <= bounds.Height)
                return value;

            return ResizeProportionally(value, bounds);
        }

        public static Size ResizeUpToBounds(Size value, Size bounds)
        {
            if ((value.Width > bounds.Width && value.Height >= bounds.Height) || (value.Width >= bounds.Width && value.Height > bounds.Height))
                throw new ArgumentException(StringResources.GetString("ValueShouldBeLessThanBounds"), "objValue");

            return ResizeProportionally(value, bounds);
        }

        public static Size ResizeProportionally(Size value, Size bounds)
        {
            if (value.Width <= 0 || value.Height <= 0)
                throw new ArgumentOutOfRangeException("objValue", StringResources.GetString("DimensionsShouldBeAboveZero"));
            if (bounds.Width <= 0 || bounds.Height <= 0)
                throw new ArgumentOutOfRangeException("objBounds", StringResources.GetString("DimensionsShouldBeAboveZero"));

            float boundsAspect = (float)bounds.Width / bounds.Height,
                  valueAspect = (float)value.Width / value.Height;

            Size result = new Size();
            if (valueAspect > boundsAspect)
            {
                result.Width = bounds.Width;
                result.Height = (int)Math.Round(result.Width / valueAspect);

                if (result.Height < 1)
                    result.Height = 1;
            }
            else
            {
                result.Height = bounds.Height;
                result.Width = (int)Math.Round(result.Height * valueAspect);

                if (result.Width < 1)
                    result.Width = 1;
            }

            return result;
        }

        public static int RecalculateResolution(int pixelValue, float srcResolution, float dstResolution)
        {
            if (srcResolution < Eps)
                throw new ArgumentOutOfRangeException("fSrcResolution", StringResources.GetString("ResolutionShouldBePositive"));
            if (dstResolution < Eps)
                throw new ArgumentOutOfRangeException("fDstResolution", StringResources.GetString("ResolutionShouldBePositive"));

            if (srcResolution - dstResolution < Eps)
                return pixelValue;

            float units = (float)pixelValue / srcResolution;
            return (int)Math.Round(units * dstResolution);
        }

        public const float Eps = 0.0001f;
    }
}