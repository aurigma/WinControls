// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    internal class SerializationNames
    {
#if DEBUG
		internal const string Name								= "Name";
		internal const string Locked							= "Locked";
		internal const string Path								= "Path";
		internal const string Matrix							= "Matrix";
		internal const string Pen								= "Pen";
		internal const string Brush								= "Brush";

		internal const string GcppResizeMode					= "GcppResizeMode";
		internal const string GcppResizeEnabled					= "GcppResizeEnabled";
		internal const string GcppRotateEnabled					= "GcppRotateEnabled";
		internal const string GcppSkewEnabled					= "GcppSkewEnabled";
		internal const string GcppDragEnabled					= "GcppDragEnabled";
		internal const string GcppResizeWECursor				= "GcppResizeWECursor";
		internal const string GcppResizeNSCursor				= "GcppResizeNSCursor";
		internal const string GcppResizeNWSECursor				= "GcppResizeNWSECursor";
		internal const string GcppResizeNESWCursor				= "GcppResizeNESWCursor";
		internal const string GcppRotateCenterCursor			= "GcppRotateCenterCursor";
		internal const string GcppRotatePointCursor				= "GcppRotatePointCursor";
		internal const string GcppSkewCursor					= "GcppSkewCursor";
		internal const string GcppDragCursor					= "GcppDragCursor";
		internal const string GcppMajorResizePointPrototype		= "GcppMajorResizePointPrototype";
		internal const string GcppMinorResizePointPrototype		= "GcppMinorResizePointPrototype";
		internal const string GcppRotatePointPrototype			= "GcppRotatePointPrototype";
		internal const string GcppRotateCenterPointPrototype	= "GcppRotateCenterPointPrototype";
		internal const string GcppSkewPointPrototype			= "GcppSkewPointPrototype";

		internal const string FreehandPoints					= "FreehandPoints";
		internal const string FreehandClosePath					= "FreehandClosePath";
		internal const string FreehandFillMode					= "FreehandFillMode";

		internal const string TextText							= "TextText";
		internal const string TextFont							= "TextFont";
		internal const string TextFormat						= "TextFormat";
		internal const string TextBounds						= "TextBounds";
		internal const string TextAreaPointsEnabled				= "TextAreaPointsEnabled";
		internal const string TextAreaPointPrototype			= "TextAreaPointPrototype";

		internal const string ImageBitmap						= "ImageBitmap";
		internal const string ImageScaleToActual				= "ImageScaleToActual";
		internal const string ImageRectangle					= "ImageRectangle";

		internal const string LayerVisible						= "LayerVisible";
		internal const string LayerObjects						= "LayerObjects";

		internal const string LayerCollection					= "LayerCollection";

		internal const string ControlPointSize					= "ControlPointSize";
		internal const string ControlPointLocation				= "ControlPointLocation";
		internal const string ControlPointEnabled				= "ControlPointEnabled";
#else
        internal const string Name = "q";
        internal const string Locked = "w";
        internal const string Path = "e";
        internal const string Matrix = "r";
        internal const string Pen = "t";
        internal const string Brush = "y";

        internal const string GcppResizeMode = "u";
        internal const string GcppResizeEnabled = "i";
        internal const string GcppRotateEnabled = "o";
        internal const string GcppSkewEnabled = "p";
        internal const string GcppDragEnabled = "a";
        internal const string GcppResizeWECursor = "s";
        internal const string GcppResizeNSCursor = "d";
        internal const string GcppResizeNWSECursor = "f";
        internal const string GcppResizeNESWCursor = "g";
        internal const string GcppRotateCenterCursor = "h";
        internal const string GcppRotatePointCursor = "j";
        internal const string GcppSkewCursor = "k";
        internal const string GcppDragCursor = "l";
        internal const string GcppMajorResizePointPrototype = "z";
        internal const string GcppMinorResizePointPrototype = "x";
        internal const string GcppRotatePointPrototype = "c";
        internal const string GcppRotateCenterPointPrototype = "v";
        internal const string GcppSkewPointPrototype = "b";

        internal const string FreehandPoints = "n";
        internal const string FreehandClosePath = "m";
        internal const string FreehandFillMode = "q0";

        internal const string TextText = "w0";
        internal const string TextFont = "e0";
        internal const string TextFormat = "r0";
        internal const string TextBounds = "t0";
        internal const string TextAreaPointsEnabled = "y0";
        internal const string TextAreaPointPrototype = "q1";

        internal const string ImageBitmap = "u0";
        internal const string ImageScaleToActual = "i0";
        internal const string ImageRectangle = "o0";

        internal const string LayerVisible = "p0";
        internal const string LayerObjects = "a0";

        internal const string LayerCollection = "s0";

        internal const string ControlPointSize = "d0";
        internal const string ControlPointLocation = "f0";
        internal const string ControlPointEnabled = "g0";
#endif

        private SerializationNames()
        {
        }
    }

    internal class VObjectSerializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public VObjectSerializationBinder()
        {
            System.Reflection.Assembly vObjectsAssembly = System.Reflection.Assembly.GetAssembly(typeof(VObjectSerializationBinder));
            _assemblyName = vObjectsAssembly.FullName;
        }

        public override System.Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName.StartsWith("Aurigma.GraphicsMill.WinControls.VectorObjects"))
                assemblyName = _assemblyName;

            return System.Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
        }

        private string _assemblyName;
    }

    public class BinarySerializer
    {
        #region "Types id"

        private const byte NullId = 0x01;
        private const byte IntArrayId = 0x02;
        private const byte ByteArrayId = 0x03;
        private const byte FloatArrayId = 0x04;
        private const byte PointArrayId = 0x05;
        private const byte PointFArrayId = 0x06;
        private const byte ColorArrayId = 0x07;
        private const byte MatrixId = 0x08;
        private const byte PenId = 0x09;
        private const byte PathId = 0x0a;
        private const byte BrushId = 0x0b;
        private const byte SolidBrushId = 0x0c;
        private const byte TextureBrushId = 0x0d;
        private const byte LinearGradientBrushId = 0x0e;
        private const byte PathGradientBrushId = 0x0f;
        private const byte HatchBrushId = 0x10;
        private const byte CustomBrushId = 0x11;
        private const byte ImageId = 0x12;
        private const byte BlendId = 0x13;
        private const byte ColorBlendId = 0x14;
        private const byte StringFormatId = 0x15;
        private const byte ObjectArrayId = 0x16;

        #endregion "Types id"

        private const int InitialMemoryStreamCapacity = 2048;

        private BinarySerializer()
        {
        }

        public static object Deserialize(byte[] buffer)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, false);
            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);

            object result;

            byte typeId = ReadTypeId(br);
            br.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            switch (typeId)
            {
                case NullId:
                    result = null;
                    break;

                case IntArrayId:
                    result = DeserializeIntArray(br);
                    break;

                case ByteArrayId:
                    result = DeserializeByteArray(br);
                    break;

                case FloatArrayId:
                    result = DeserializeFloatArray(br);
                    break;

                case PointArrayId:
                    result = DeserializePointArray(br);
                    break;

                case PointFArrayId:
                    result = DeserializePointFArray(br);
                    break;

                case ColorArrayId:
                    result = DeserializeColorArray(br);
                    break;

                case MatrixId:
                    result = DeserializeMatrix(br);
                    break;

                case PenId:
                    result = DeserializePen(br);
                    break;

                case BrushId:
                    result = DeserializeBrush(br);
                    break;

                case SolidBrushId:
                    result = DeserializeSolidBrush(br);
                    break;

                case HatchBrushId:
                    result = DeserializeHatchBrush(br);
                    break;

                case TextureBrushId:
                    result = DeserializeTextureBrush(br);
                    break;

                case LinearGradientBrushId:
                    result = DeserializeLinearGradientBrush(br);
                    break;

                case PathGradientBrushId:
                    result = DeserializePathGradientBrush(br);
                    break;

                case ImageId:
                    result = DeserializeImage(br);
                    break;

                case BlendId:
                    result = DeserializeBlend(br);
                    break;

                case ColorBlendId:
                    result = DeserializeColorBlend(br);
                    break;

                case StringFormatId:
                    result = DeserializeStringFormat(br);
                    break;

                case ObjectArrayId:
                    result = DeserializeObjectArray(br);
                    break;

                default:
                    throw new System.ArgumentException(StringResources.GetString("ExStrSerializationUnexpectedBufferContent"), "buffer");
            }

            return result;
        }

        #region "byte[] <-> object [de]serialization methods"

        #region "Arrays"

        public static byte[] Serialize(int[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static int[] DeserializeIntArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            int[] result;
            try
            {
                result = DeserializeIntArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(byte[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static byte[] DeserializeByteArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            byte[] result;
            try
            {
                result = DeserializeByteArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(float[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static float[] DeserializeFloatArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            float[] result;
            try
            {
                result = DeserializeFloatArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.PointF[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.PointF[] DeserializePointFArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.PointF[] result;
            try
            {
                result = DeserializePointFArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Point[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Point[] DeserializePointArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Point[] result;
            try
            {
                result = DeserializePointArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Color[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Color[] DeserializeColorArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Color[] result;
            try
            {
                result = DeserializeColorArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(object[] array)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, array);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static object[] DeserializeObjectArray(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            object[] result;
            try
            {
                result = DeserializeObjectArray(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        #endregion "Arrays"

        #region "Image, Path, Matrix, Pen"

        public static byte[] Serialize(System.Drawing.Image image)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, image);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Image DeserializeImage(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Image result;
            try
            {
                result = DeserializeImage(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.Matrix matrix)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, matrix);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.Matrix DeserializeMatrix(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.Matrix result;
            try
            {
                result = DeserializeMatrix(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.GraphicsPath path)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, path);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.GraphicsPath DeserializePath(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.GraphicsPath result;
            try
            {
                result = DeserializePath(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Pen pen)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, pen);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Pen DeserializePen(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Pen result;
            try
            {
                result = DeserializePen(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        #endregion "Image, Path, Matrix, Pen"

        #region "Brushes"

        public static byte[] Serialize(System.Drawing.Brush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Brush DeserializeBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Brush result;
            try
            {
                result = DeserializeBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.SolidBrush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.SolidBrush DeserializeSolidBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.SolidBrush result;
            try
            {
                result = DeserializeSolidBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.TextureBrush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.TextureBrush DeserializeTextureBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.TextureBrush result;
            try
            {
                result = DeserializeTextureBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.LinearGradientBrush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.LinearGradientBrush DeserializeLinearGradientBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.LinearGradientBrush result;
            try
            {
                result = DeserializeLinearGradientBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.PathGradientBrush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.PathGradientBrush DeserializePathGradientBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.PathGradientBrush result;
            try
            {
                result = DeserializePathGradientBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.HatchBrush brush)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, brush);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.HatchBrush DeserializeHatchBrush(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.HatchBrush result;
            try
            {
                result = DeserializeHatchBrush(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        #endregion "Brushes"

        #region "Blend && ColorBlend objects"

        public static byte[] Serialize(System.Drawing.Drawing2D.Blend blend)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, blend);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.Blend DeserializeBlend(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.Blend result;
            try
            {
                result = DeserializeBlend(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        public static byte[] Serialize(System.Drawing.Drawing2D.ColorBlend blend)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, blend);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.Drawing2D.ColorBlend DeserializeColorBlend(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.Drawing2D.ColorBlend result;
            try
            {
                result = DeserializeColorBlend(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        #endregion "Blend && ColorBlend objects"

        #region "StringFormat"

        public static byte[] Serialize(System.Drawing.StringFormat format)
        {
            System.IO.BinaryWriter bw;
            System.IO.MemoryStream stream;
            CreateBinaryWriter(out bw, out stream);

            try
            {
                Serialize(bw, format);
            }
            finally
            {
                bw.Close();
            }

            byte[] result = stream.ToArray();
            stream.Close();
            stream.Dispose();

            return result;
        }

        public static System.Drawing.StringFormat DeserializeStringFormat(byte[] buffer)
        {
            System.IO.BinaryReader br;
            CreateBinaryReader(buffer, out br);

            System.Drawing.StringFormat result;
            try
            {
                result = DeserializeStringFormat(br);
            }
            finally
            {
                br.Close();
            }

            return result;
        }

        #endregion "StringFormat"

        #endregion "byte[] <-> object [de]serialization methods"

        #region "BinaryWriter/Binary reader [de]serialization"

        #region "Arrays"

        public static void Serialize(System.IO.BinaryWriter bw, int[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, IntArrayId);

                bw.Write((int)array.Length);
                for (int i = 0; i < array.Length; i++)
                    bw.Write(array[i]);
            }
        }

        public static int[] DeserializeIntArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            int[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == IntArrayId)
            {
                int length = br.ReadInt32();
                result = new int[length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = br.ReadInt32();
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, byte[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, ByteArrayId);

                bw.Write((int)array.Length);
                if (array.Length > 0)
                    bw.Write(array);
            }
        }

        public static byte[] DeserializeByteArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            byte[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == ByteArrayId)
            {
                int length = br.ReadInt32();
                if (length > 0)
                    result = br.ReadBytes(length);
                else
                    result = new byte[0];
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, float[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, FloatArrayId);

                bw.Write((int)array.Length);
                for (int i = 0; i < array.Length; i++)
                    bw.Write(array[i]);
            }
        }

        public static float[] DeserializeFloatArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            float[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == FloatArrayId)
            {
                int length = br.ReadInt32();
                result = new float[length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = br.ReadSingle();
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.PointF[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, PointFArrayId);

                bw.Write((int)array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    bw.Write(array[i].X);
                    bw.Write(array[i].Y);
                }
            }
        }

        public static System.Drawing.PointF[] DeserializePointFArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.PointF[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == PointFArrayId)
            {
                int length = br.ReadInt32();
                result = new System.Drawing.PointF[length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = new System.Drawing.PointF(br.ReadSingle(), br.ReadSingle());
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Point[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, PointArrayId);

                bw.Write((int)array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    bw.Write(array[i].X);
                    bw.Write(array[i].Y);
                }
            }
        }

        public static System.Drawing.Point[] DeserializePointArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.Point[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == PointArrayId)
            {
                int length = br.ReadInt32();
                result = new System.Drawing.Point[length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = new System.Drawing.Point(br.ReadInt32(), br.ReadInt32());
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Color[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, ColorArrayId);

                bw.Write((int)array.Length);
                for (int i = 0; i < array.Length; i++)
                    bw.Write(array[i].ToArgb());
            }
        }

        public static System.Drawing.Color[] DeserializeColorArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.Color[] result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == ColorArrayId)
            {
                int length = br.ReadInt32();
                result = new System.Drawing.Color[length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = System.Drawing.Color.FromArgb(br.ReadInt32());
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, object[] array)
        {
            if (array == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, ObjectArrayId);
                bw.Write((int)array.Length);

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                for (int i = 0; i < array.Length; i++)
                    formatter.Serialize(bw.BaseStream, array[i]);
            }
        }

        public static object[] DeserializeObjectArray(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            object[] result;
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == ObjectArrayId)
            {
                int length = br.ReadInt32();
                result = new object[length];

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                for (int i = 0; i < length; i++)
                    result[i] = formatter.Deserialize(br.BaseStream);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        #endregion "Arrays"

        #region "Image, Matrix, Path, Pen"

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.Matrix matrix)
        {
            if (matrix != null)
            {
                WriteTypeId(bw, MatrixId);

                float[] elements = matrix.Elements;
                for (int i = 0; i < elements.Length; i++)
                    bw.Write(elements[i]);
            }
            else
            {
                WriteTypeId(bw, NullId);
            }
        }

        public static System.Drawing.Drawing2D.Matrix DeserializeMatrix(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.Drawing2D.Matrix result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == MatrixId)
            {
                float e0, e1, e2, e3, e4, e5;
                e0 = br.ReadSingle();
                e1 = br.ReadSingle();
                e2 = br.ReadSingle();
                e3 = br.ReadSingle();
                e4 = br.ReadSingle();
                e5 = br.ReadSingle();
                result = new System.Drawing.Drawing2D.Matrix(e0, e1, e2, e3, e4, e5);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.GraphicsPath path)
        {
            if (path != null)
            {
                WriteTypeId(bw, PathId);

                Serialize(bw, path.PathPoints);
                Serialize(bw, path.PathTypes);
                bw.Write((int)path.FillMode);
            }
            else
            {
                WriteTypeId(bw, NullId);
            }
        }

        public static System.Drawing.Drawing2D.GraphicsPath DeserializePath(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.Drawing2D.GraphicsPath result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == PathId)
            {
                System.Drawing.PointF[] points = DeserializePointFArray(br);
                byte[] types = DeserializeByteArray(br);
                System.Drawing.Drawing2D.FillMode fillMode = (System.Drawing.Drawing2D.FillMode)br.ReadInt32();

                result = new System.Drawing.Drawing2D.GraphicsPath(points, types, fillMode);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Pen pen)
        {
            if (pen != null)
            {
                WriteTypeId(bw, PenId);

                bw.Write(pen.Color.ToArgb());
                bw.Write(pen.Width);
                bw.Write((int)pen.Alignment);
                Serialize(bw, pen.Brush);
                Serialize(bw, pen.CompoundArray);
                bw.Write((int)pen.DashCap);
                bw.Write(pen.DashOffset);
                bw.Write((int)pen.DashStyle);
                if (pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
                    Serialize(bw, pen.DashPattern);
                bw.Write((int)pen.EndCap);
                bw.Write((int)pen.LineJoin);
                bw.Write(pen.MiterLimit);
                Serialize(bw, pen.Transform);
            }
            else
            {
                WriteTypeId(bw, NullId);
            }
        }

        public static System.Drawing.Pen DeserializePen(System.IO.BinaryReader br)
        {
            byte typeId = ReadTypeId(br);

            System.Drawing.Pen result;

            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == PenId)
            {
                result = new System.Drawing.Pen(System.Drawing.Color.FromArgb(br.ReadInt32()), br.ReadSingle());
                result.Alignment = (System.Drawing.Drawing2D.PenAlignment)br.ReadInt32();
                result.Brush = DeserializeBrush(br);

                float[] compoundArray = DeserializeFloatArray(br);
                if (compoundArray.Length > 0)
                    result.CompoundArray = compoundArray;

                result.DashCap = (System.Drawing.Drawing2D.DashCap)br.ReadInt32();
                result.DashOffset = br.ReadSingle();
                result.DashStyle = (System.Drawing.Drawing2D.DashStyle)br.ReadInt32();

                if (result.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
                    result.DashPattern = DeserializeFloatArray(br);

                result.EndCap = (System.Drawing.Drawing2D.LineCap)br.ReadInt32();
                result.LineJoin = (System.Drawing.Drawing2D.LineJoin)br.ReadInt32();
                result.MiterLimit = br.ReadSingle();
                result.Transform = DeserializeMatrix(br);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Image image)
        {
            if (image == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, ImageId);

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                formatter.Serialize(bw.BaseStream, image);
            }
        }

        public static System.Drawing.Image DeserializeImage(System.IO.BinaryReader br)
        {
            System.Drawing.Image result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == ImageId)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                result = (System.Drawing.Image)formatter.Deserialize(br.BaseStream);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        #endregion "Image, Matrix, Path, Pen"

        #region "Brushes"

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Brush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, BrushId);

                System.Type type = brush.GetType();
                if (type == typeof(System.Drawing.SolidBrush))
                {
                    WriteTypeId(bw, SolidBrushId);
                    Serialize(bw, (System.Drawing.SolidBrush)brush);
                }
                else if (type == typeof(System.Drawing.TextureBrush))
                {
                    WriteTypeId(bw, TextureBrushId);
                    Serialize(bw, (System.Drawing.TextureBrush)brush);
                }
                else if (type == typeof(System.Drawing.Drawing2D.LinearGradientBrush))
                {
                    WriteTypeId(bw, LinearGradientBrushId);
                    Serialize(bw, (System.Drawing.Drawing2D.LinearGradientBrush)brush);
                }
                else if (type == typeof(System.Drawing.Drawing2D.PathGradientBrush))
                {
                    WriteTypeId(bw, PathGradientBrushId);
                    Serialize(bw, (System.Drawing.Drawing2D.PathGradientBrush)brush);
                }
                else if (type == typeof(System.Drawing.Drawing2D.HatchBrush))
                {
                    WriteTypeId(bw, HatchBrushId);
                    Serialize(bw, (System.Drawing.Drawing2D.HatchBrush)brush);
                }
                else
                {
                    WriteTypeId(bw, CustomBrushId);
                    SerializeCustomBrush(bw, brush);
                }
            }
        }

        public static System.Drawing.Brush DeserializeBrush(System.IO.BinaryReader br)
        {
            System.Drawing.Brush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == BrushId)
            {
                typeId = ReadTypeId(br);
                switch (typeId)
                {
                    case SolidBrushId:
                        result = DeserializeSolidBrush(br);
                        break;

                    case TextureBrushId:
                        result = DeserializeTextureBrush(br);
                        break;

                    case LinearGradientBrushId:
                        result = DeserializeLinearGradientBrush(br);
                        break;

                    case PathGradientBrushId:
                        result = DeserializePathGradientBrush(br);
                        break;

                    case HatchBrushId:
                        result = DeserializeHatchBrush(br);
                        break;

                    case CustomBrushId:
                        result = DeserializeCustomBrush(br);
                        break;

                    default:
                        throw new System.ArgumentException(StringResources.GetString("ExStrSerializationUnsupportedBrushType"), "br");
                }
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.SolidBrush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, SolidBrushId);
                bw.Write(brush.Color.ToArgb());
            }
        }

        public static System.Drawing.SolidBrush DeserializeSolidBrush(System.IO.BinaryReader br)
        {
            System.Drawing.SolidBrush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
                result = null;
            else
                result = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(br.ReadInt32()));

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.HatchBrush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, HatchBrushId);
                bw.Write(brush.BackgroundColor.ToArgb());
                bw.Write(brush.ForegroundColor.ToArgb());
                bw.Write((int)brush.HatchStyle);
            }
        }

        public static System.Drawing.Drawing2D.HatchBrush DeserializeHatchBrush(System.IO.BinaryReader br)
        {
            System.Drawing.Drawing2D.HatchBrush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else
            {
                System.Drawing.Color backColor = System.Drawing.Color.FromArgb(br.ReadInt32());
                System.Drawing.Color foreColor = System.Drawing.Color.FromArgb(br.ReadInt32());
                System.Drawing.Drawing2D.HatchStyle style = (System.Drawing.Drawing2D.HatchStyle)br.ReadInt32();

                result = new System.Drawing.Drawing2D.HatchBrush(style, foreColor, backColor);
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.TextureBrush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, TextureBrushId);
                Serialize(bw, brush.Image);
                Serialize(bw, brush.Transform);
                bw.Write((int)brush.WrapMode);
            }
        }

        public static System.Drawing.TextureBrush DeserializeTextureBrush(System.IO.BinaryReader br)
        {
            System.Drawing.TextureBrush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == TextureBrushId)
            {
                System.Drawing.Image image = DeserializeImage(br);
                System.Drawing.Drawing2D.Matrix matrix = DeserializeMatrix(br);
                System.Drawing.Drawing2D.WrapMode wrapMode = (System.Drawing.Drawing2D.WrapMode)br.ReadInt32();

                result = new System.Drawing.TextureBrush(image, wrapMode);
                result.Transform = matrix;
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.LinearGradientBrush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, LinearGradientBrushId);
                bw.Write(brush.Rectangle.Left);
                bw.Write(brush.Rectangle.Top);
                bw.Write(brush.Rectangle.Width);
                bw.Write(brush.Rectangle.Height);
                Serialize(bw, brush.Blend);
                bw.Write(brush.GammaCorrection);

                System.Drawing.Drawing2D.ColorBlend cb = null;
                try
                {
                    cb = brush.InterpolationColors;
                }
                catch (System.ArgumentException)
                {
                    cb = null;
                }
                Serialize(bw, (System.Drawing.Drawing2D.ColorBlend)cb);

                Serialize(bw, brush.LinearColors);
                Serialize(bw, brush.Transform);
                bw.Write((int)brush.WrapMode);
            }
        }

        public static System.Drawing.Drawing2D.LinearGradientBrush DeserializeLinearGradientBrush(System.IO.BinaryReader br)
        {
            System.Drawing.Drawing2D.LinearGradientBrush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == LinearGradientBrushId)
            {
                System.Drawing.RectangleF rect = new System.Drawing.RectangleF(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                result = new System.Drawing.Drawing2D.LinearGradientBrush(rect, System.Drawing.Color.Black, System.Drawing.Color.Black, 0.0f, false);
                result.Blend = DeserializeBlend(br);
                result.GammaCorrection = br.ReadBoolean();

                System.Drawing.Drawing2D.ColorBlend cb = DeserializeColorBlend(br);
                if (cb != null)
                    result.InterpolationColors = cb;

                result.LinearColors = DeserializeColorArray(br);
                result.Transform = DeserializeMatrix(br);
                result.WrapMode = (System.Drawing.Drawing2D.WrapMode)br.ReadInt32();
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.PathGradientBrush brush)
        {
            if (brush == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, PathGradientBrushId);
                bw.Write(brush.CenterColor.ToArgb());
                bw.Write(brush.CenterPoint.X);
                bw.Write(brush.CenterPoint.Y);
                bw.Write(brush.FocusScales.X);
                bw.Write(brush.FocusScales.Y);
                Serialize(bw, brush.Blend);

                if (brush.InterpolationColors.Colors.Length > 1)
                    Serialize(bw, brush.InterpolationColors);
                else
                    Serialize(bw, (System.Drawing.Drawing2D.ColorBlend)null);

                Serialize(bw, brush.SurroundColors);
                Serialize(bw, brush.Transform);
                bw.Write((int)brush.WrapMode);
            }
        }

        public static System.Drawing.Drawing2D.PathGradientBrush DeserializePathGradientBrush(System.IO.BinaryReader br)
        {
            System.Drawing.Drawing2D.PathGradientBrush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == PathGradientBrushId)
            {
                result = new System.Drawing.Drawing2D.PathGradientBrush(new System.Drawing.PointF[] { System.Drawing.PointF.Empty, new System.Drawing.PointF(1.0f, 1.0f) });
                result.CenterColor = System.Drawing.Color.FromArgb(br.ReadInt32());
                result.CenterPoint = new System.Drawing.PointF(br.ReadSingle(), br.ReadSingle());
                result.FocusScales = new System.Drawing.PointF(br.ReadSingle(), br.ReadSingle());
                result.Blend = DeserializeBlend(br);

                System.Drawing.Drawing2D.ColorBlend cb = DeserializeColorBlend(br);
                if (cb != null)
                    result.InterpolationColors = cb;

                try
                {
                    // It is possible to create and serialize a brush which will crush deserialization
                    // on this step => Using try/catch to work around this issue.
                    result.SurroundColors = DeserializeColorArray(br);
                }
                catch (System.ArgumentException)
                {
                }

                result.Transform = DeserializeMatrix(br);
                result.WrapMode = (System.Drawing.Drawing2D.WrapMode)br.ReadInt32();
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        private static void SerializeCustomBrush(System.IO.BinaryWriter bw, System.Drawing.Brush brush)
        {
            if (brush.GetType().IsSerializable)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                formatter.Serialize(bw.BaseStream, brush);
            }
            else
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationCannotSerializeBrush"), "brush");
        }

        private static System.Drawing.Brush DeserializeCustomBrush(System.IO.BinaryReader br)
        {
            System.Drawing.Brush result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == CustomBrushId)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new VObjectSerializationBinder();
                result = (System.Drawing.Brush)formatter.Deserialize(br.BaseStream);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        #endregion "Brushes"

        #region "Blend & ColorBlend objects"

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.Blend blend)
        {
            if (blend == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, BlendId);
                Serialize(bw, blend.Factors);
                Serialize(bw, blend.Positions);
            }
        }

        public static System.Drawing.Drawing2D.Blend DeserializeBlend(System.IO.BinaryReader br)
        {
            System.Drawing.Drawing2D.Blend result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == BlendId)
            {
                result = new System.Drawing.Drawing2D.Blend();
                result.Factors = DeserializeFloatArray(br);
                result.Positions = DeserializeFloatArray(br);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.Drawing2D.ColorBlend blend)
        {
            if (blend == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, ColorBlendId);
                Serialize(bw, blend.Colors);
                Serialize(bw, blend.Positions);
            }
        }

        public static System.Drawing.Drawing2D.ColorBlend DeserializeColorBlend(System.IO.BinaryReader br)
        {
            System.Drawing.Drawing2D.ColorBlend result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == ColorBlendId)
            {
                result = new System.Drawing.Drawing2D.ColorBlend();
                result.Colors = DeserializeColorArray(br);
                result.Positions = DeserializeFloatArray(br);
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        #endregion "Blend & ColorBlend objects"

        #region "StringFormat"

        public static void Serialize(System.IO.BinaryWriter bw, System.Drawing.StringFormat format)
        {
            if (format == null)
            {
                WriteTypeId(bw, NullId);
            }
            else
            {
                WriteTypeId(bw, StringFormatId);
                bw.Write((int)format.Alignment);
                bw.Write((int)format.FormatFlags);
                bw.Write((int)format.HotkeyPrefix);
                bw.Write((int)format.LineAlignment);
                bw.Write((int)format.Trimming);
            }
        }

        public static System.Drawing.StringFormat DeserializeStringFormat(System.IO.BinaryReader br)
        {
            System.Drawing.StringFormat result;

            byte typeId = ReadTypeId(br);
            if (typeId == NullId)
            {
                result = null;
            }
            else if (typeId == StringFormatId)
            {
                result = (System.Drawing.StringFormat)System.Drawing.StringFormat.GenericDefault.Clone();
                result.Alignment = (System.Drawing.StringAlignment)br.ReadInt32();
                result.FormatFlags = (System.Drawing.StringFormatFlags)br.ReadInt32();
                result.HotkeyPrefix = (System.Drawing.Text.HotkeyPrefix)br.ReadInt32();
                result.LineAlignment = (System.Drawing.StringAlignment)br.ReadInt32();
                result.Trimming = (System.Drawing.StringTrimming)br.ReadInt32();
            }
            else
            {
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
            }

            return result;
        }

        #endregion "StringFormat"

        #endregion "BinaryWriter/Binary reader [de]serialization"

        #region "Aux methods"

        private static void WriteTypeId(System.IO.BinaryWriter bw, byte typeId)
        {
            bw.Write(typeId);
            bw.Write(typeId);
            bw.Write(typeId);
        }

        private static byte ReadTypeId(System.IO.BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(3);
            if (bytes[0] != bytes[1] || bytes[1] != bytes[2])
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderDataIncorrect"), "br");

            return bytes[0];
        }

        private static void CheckTypeId(System.IO.BinaryReader br, byte typeId)
        {
            if (ReadTypeId(br) != typeId)
                throw new System.ArgumentException(StringResources.GetString("ExStrSerializationReaderContentTypeError"), "br");
        }

        private static void CreateBinaryWriter(out System.IO.BinaryWriter bw, out System.IO.MemoryStream stream)
        {
            stream = new System.IO.MemoryStream(InitialMemoryStreamCapacity);
            bw = new System.IO.BinaryWriter(stream);
        }

        private static void CreateBinaryReader(byte[] buffer, out System.IO.BinaryReader br)
        {
            if (buffer == null)
                throw new System.ArgumentNullException("buffer");
            if (buffer.Length == 0)
                throw new System.ArgumentException(StringResources.GetString("ExStrArrayZeroLengthError"), "buffer");

            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, false);
            br = new System.IO.BinaryReader(stream);
        }

        #endregion "Aux methods"
    }
}