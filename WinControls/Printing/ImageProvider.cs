// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Aurigma.GraphicsMill.WinControls
{
    internal interface IImageProvider
    {
        bool IsEmpty();

        PrintPlaceholder GetNext();
    }

    internal class FromIEnumeratorProvider : IImageProvider
    {
        public FromIEnumeratorProvider(IEnumerator enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("pEnumerator");

            _enumerator = enumerator;
            _isEmpty = !enumerator.MoveNext();
            if (!_isEmpty)
            {
                object obj = enumerator.Current;
                if (obj == null)
                {
                    _isEmpty = true;
                    return;
                }

                Type type = obj.GetType();
                if (type == typeof(Bitmap))
                    _receivesOnlyBitmaps = true;
                else if (type != typeof(PrintPlaceholder))
                    throw new ArgumentException(StringResources.GetString("UnexpectedParameterType"), "pEnumerator");

                _next = new PrintPlaceholder();
                FillImagePrintInfo(_next, obj);
            }
        }

        public virtual bool IsEmpty()
        {
            return _isEmpty;
        }

        public virtual PrintPlaceholder GetNext()
        {
            _isEmpty = !_enumerator.MoveNext();
            PrintPlaceholder result = _next;
            _next = null;

            if (!_isEmpty)
            {
                Object source = _enumerator.Current;
                if (source != null)
                {
                    _next = new PrintPlaceholder();
                    FillImagePrintInfo(_next, source);
                }
                else
                    _isEmpty = true;
            }

            return result;
        }

        protected void FillImagePrintInfo(PrintPlaceholder placeholder, object source)
        {
            if (source == null)
                return;

            if (_receivesOnlyBitmaps)
            {
                placeholder.Image = (Bitmap)source;
                placeholder.Location = Point.Empty;
                placeholder.Size = Size.Empty;
                placeholder.Header = null;
                placeholder.Footer = null;
            }
            else
            {
                PrintPlaceholder receivedPlaceholder = (PrintPlaceholder)source;
                placeholder.Image = receivedPlaceholder.Image;
                placeholder.Location = receivedPlaceholder.Location;
                placeholder.Size = receivedPlaceholder.Size;
                placeholder.Header = receivedPlaceholder.Header;
                placeholder.Footer = receivedPlaceholder.Footer;
            }
        }

        private IEnumerator _enumerator;
        private bool _isEmpty;
        private bool _receivesOnlyBitmaps;
        private PrintPlaceholder _next;
    }
}