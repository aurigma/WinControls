// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// The class provides several static functions.
    /// </summary>
    internal class VObjectsUtils
    {
        internal static void Swap(ref float val0, ref float val1)
        {
            float tmp = val0;
            val0 = val1;
            val1 = tmp;
        }

        internal static void TransformPoint(System.Drawing.Drawing2D.Matrix matrix, ref System.Drawing.PointF point)
        {
            float[] elements = matrix.Elements;
            System.Drawing.PointF original = point;
            point.X = elements[0] * original.X + elements[2] * original.Y + elements[4];
            point.Y = elements[1] * original.X + elements[3] * original.Y + elements[5];
        }

        internal static void TransformVector(System.Drawing.Drawing2D.Matrix matrix, ref System.Drawing.PointF point)
        {
            float[] elements = matrix.Elements;
            System.Drawing.PointF original = point;
            point.X = elements[0] * original.X + elements[2] * original.Y;
            point.Y = elements[1] * original.X + elements[3] * original.Y;
        }

        internal static System.Drawing.PointF[] TransformPoints(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.PointF[] points)
        {
            if (points == null)
                throw new System.ArgumentNullException("points");

            float[] elements = matrix.Elements;
            System.Drawing.PointF[] result = new System.Drawing.PointF[points.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i].X = elements[0] * points[i].X + elements[2] * points[i].Y + elements[4];
                result[i].Y = elements[1] * points[i].X + elements[3] * points[i].Y + elements[5];
            }

            return result;
        }

        internal static void TransformPointsInplace(System.Drawing.Drawing2D.Matrix matrix, System.Drawing.PointF[] points)
        {
            if (points == null)
                throw new System.ArgumentNullException("points");

            float[] elements = matrix.Elements;
            for (int i = 0; i < points.Length; i++)
            {
                System.Drawing.PointF original = points[i];
                points[i].X = elements[0] * original.X + elements[2] * original.Y + elements[4];
                points[i].Y = elements[1] * original.X + elements[3] * original.Y + elements[5];
            }
        }

        internal static int FitToBounds(int value, int minValue, int maxValue)
        {
            return System.Math.Min(System.Math.Max(minValue, value), maxValue);
        }

        internal static float FitToBounds(float value, float minValue, float maxValue)
        {
            return System.Math.Min(System.Math.Max(minValue, value), maxValue);
        }

        internal static System.Drawing.RectangleF GetBoundingRectangle(System.Drawing.PointF[] points)
        {
            float l = float.MaxValue, t = float.MaxValue, r = float.MinValue, b = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X > r)
                    r = points[i].X;
                if (points[i].X < l)
                    l = points[i].X;
                if (points[i].Y > b)
                    b = points[i].Y;
                if (points[i].Y < t)
                    t = points[i].Y;
            }

            return System.Drawing.RectangleF.FromLTRB(l, t, r, b);
        }

        internal static System.Drawing.Rectangle GetBoundingRectangle(System.Drawing.Point[] points)
        {
            int l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X > r)
                    r = points[i].X;
                if (points[i].X < l)
                    l = points[i].X;
                if (points[i].Y > b)
                    b = points[i].Y;
                if (points[i].Y < t)
                    t = points[i].Y;
            }

            return System.Drawing.Rectangle.FromLTRB(l, t, r, b);
        }

        internal static System.Drawing.RectangleF GetBoundingRectangle(System.Drawing.PointF point0, System.Drawing.PointF point1)
        {
            float minX, minY, maxX, maxY;
            if (point0.X < point1.X)
            {
                minX = point0.X;
                maxX = point1.X;
            }
            else
            {
                minX = point1.X;
                maxX = point0.X;
            }

            if (point0.Y < point1.Y)
            {
                minY = point0.Y;
                maxY = point1.Y;
            }
            else
            {
                minY = point1.Y;
                maxY = point0.Y;
            }

            return System.Drawing.RectangleF.FromLTRB(minX, minY, maxX, maxY);
        }

        internal static System.Drawing.RectangleF GetBoundingRectangle(System.Drawing.RectangleF rect, System.Drawing.Drawing2D.Matrix matrix)
        {
            System.Drawing.PointF[] corners = new System.Drawing.PointF[4];
            corners[0] = rect.Location;
            corners[1] = new System.Drawing.PointF(rect.Right, rect.Top);
            corners[2] = new System.Drawing.PointF(rect.Right, rect.Bottom);
            corners[3] = new System.Drawing.PointF(rect.Left, rect.Bottom);

            TransformPointsInplace(matrix, corners);
            return GetBoundingRectangle(corners);
        }

        internal static void SetBrushMatrix(System.Drawing.Brush brush, System.Drawing.Drawing2D.Matrix matrix)
        {
            if (brush == null)
                throw new System.ArgumentNullException("brush");

            if (brush.GetType() == typeof(System.Drawing.TextureBrush))
            {
                System.Drawing.TextureBrush textureBrush = (System.Drawing.TextureBrush)brush;
                textureBrush.Transform = matrix;
            }
            else if (brush.GetType() == typeof(System.Drawing.Drawing2D.LinearGradientBrush))
            {
                System.Drawing.Drawing2D.LinearGradientBrush linearGradientBrush = (System.Drawing.Drawing2D.LinearGradientBrush)brush;
                linearGradientBrush.Transform = matrix;
            }
            else if (brush.GetType() == typeof(System.Drawing.Drawing2D.PathGradientBrush))
            {
                System.Drawing.Drawing2D.PathGradientBrush pathGradientBrush = (System.Drawing.Drawing2D.PathGradientBrush)brush;
                pathGradientBrush.Transform = matrix;
            }
        }

        internal static System.Drawing.Drawing2D.Matrix GetBrushMatrix(System.Drawing.Brush brush)
        {
            if (brush == null)
                throw new System.ArgumentNullException("brush");

            System.Drawing.Drawing2D.Matrix result = null;

            if (brush.GetType() == typeof(System.Drawing.TextureBrush))
                result = ((System.Drawing.TextureBrush)brush).Transform;
            else if (brush.GetType() == typeof(System.Drawing.Drawing2D.LinearGradientBrush))
                result = ((System.Drawing.Drawing2D.LinearGradientBrush)brush).Transform;
            else if (brush.GetType() == typeof(System.Drawing.Drawing2D.PathGradientBrush))
                result = ((System.Drawing.Drawing2D.PathGradientBrush)brush).Transform;
            return result;
        }

        #region "IObjectHost.DesignerOptions hash helpers"

        internal static bool GetBoolDesignerProperty(IVObjectHost objectHost, string key, bool defaultValue)
        {
            if (!objectHost.DesignerOptions.ContainsKey(key))
                return defaultValue;

            object value = objectHost.DesignerOptions[key];
            if (value.GetType() == typeof(bool))
                return (bool)value;

            return defaultValue;
        }

        internal static System.Windows.Forms.Cursor GetCursorDesignerProperty(IVObjectHost objectHost, string key)
        {
            object result = VObjectsUtils.GetObjectDesignerProperty(objectHost, key);
            if (result != null && result.GetType() == typeof(System.Windows.Forms.Cursor))
                return (System.Windows.Forms.Cursor)result;
            else
                return null;
        }

        internal static object GetObjectDesignerProperty(IVObjectHost objectHost, string key)
        {
            if (!objectHost.DesignerOptions.ContainsKey(key))
                return null;

            return objectHost.DesignerOptions[key];
        }

        internal static void SetObjectDesignerProperty(IVObjectHost objectHost, object value, string key)
        {
            if (value == null && objectHost.DesignerOptions.ContainsKey(key))
                objectHost.DesignerOptions.Remove(key);
            else
                objectHost.DesignerOptions[key] = value;
        }

        #endregion "IObjectHost.DesignerOptions hash helpers"

        /// <summary>
        /// Subtracts one rectangle from another. Returns an array of four rectangles (some of them may
        /// be zero-size) than make up the difference between the two rectangles areas. Indexes of the
        /// resulting rectangles are shown on the picture below. "s" is subtrahend
        ///
        ///   -----------
        ///  |     0     |
        ///  |-----------|
        ///  | 2 | s | 3 |
        ///  |-----------|
        ///  |     1     |
        ///   -----------
        ///
        /// </summary>
        public static System.Drawing.Rectangle[] SubstractRectangle(System.Drawing.Rectangle minuend, System.Drawing.Rectangle subtrahend)
        {
            System.Drawing.Rectangle[] result = new System.Drawing.Rectangle[4];
            if (subtrahend.Contains(minuend) || minuend.IsEmpty)
            {
                result[0] = result[1] = result[2] = result[3] = System.Drawing.Rectangle.Empty;
            }
            else if (subtrahend.Width < 1 || subtrahend.Height < 1 || !subtrahend.IntersectsWith(minuend))
            {
                result[0] = minuend;
                result[1] = result[2] = result[3] = System.Drawing.Rectangle.Empty;
            }
            else
            {
                // Rectangle above already subtrahend.
                result[0] = minuend;
                result[0].Height = subtrahend.Top - minuend.Y;

                // Rectangle below subtrahend.
                result[1] = minuend;
                result[1].Y = subtrahend.Bottom;
                result[1].Height = minuend.Bottom - subtrahend.Bottom;

                // Rectangle to the left from subtrahend.
                result[2] = subtrahend;
                result[2].X = minuend.Left;
                result[2].Width = subtrahend.Left - minuend.Left;

                // And at last - rectangle to the right from subtrahend.
                result[3] = subtrahend;
                result[3].X = subtrahend.Right;
                result[3].Width = minuend.Right - subtrahend.Right;
            }

            return result;
        }

        private VObjectsUtils()
        {
        }
    }
}