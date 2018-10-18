// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class MultipleItemsDistributor : IPrintItemDistributorImpl
    {
        public MultipleItemsDistributor()
        {
            _nonPlacedItems = new System.Collections.ArrayList(5);
        }

        public override void Initialize(ImagePrintDocument document, PrintPageEventArgs e)
        {
            base.Initialize(document, e);
            _nonPlacedItems.Clear();
        }

        public override void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords)
        {
            System.Diagnostics.Debug.Assert(items != null, "pItems should be allocated by caller");
            System.Diagnostics.Debug.Assert(coords != null, "pCoords should be allocated by caller");

            int printerResolutionX, printerResolutionY;
            GetValidatedResolution(e, out printerResolutionX, out printerResolutionY);

            ConvertSpacings(printerResolutionX, printerResolutionY);

            int curX = 0,
                curY = 0,
                leftMargin = PrintUtils.InchHundredthsToPixels(e.MarginBounds.X, printerResolutionX),
                topMargin = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Y, printerResolutionY),
                maxHeight = -1,
                pageWidth = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Width, printerResolutionX),
                pageHeight = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Height, printerResolutionY);

            Size pageSize = new Size(pageWidth, pageHeight);

            bool isPageFilled = false;
            while (!isPageFilled)
            {
                bool isRowFilled = false;
                maxHeight = -1;

                while (!isRowFilled)
                {
                    ImagePrintItem item = GetNextItem(e);
                    if (e.Cancel == true)
                        return;

                    if (item == null)
                    {
                        isPageFilled = true;
                        break;
                    }

                    item.FitSize(pageSize, _printOptions.PlaceholderAutoRotate);
                    Size itemSize = item.GetSize();
                    Size rotatedSize = item.GetSizeOfRotated();

                    // Trying to insert (maybe with rotation, if it allowed)
                    int freeSpaceWidth = pageWidth - curX,
                        freeSpaceHeight = pageHeight - curY;

                    if (itemSize.Width <= freeSpaceWidth && itemSize.Height <= freeSpaceHeight)
                    {
                        // Item placed successfully
                        items.Add(item);
                        coords.Add(new Point(curX + leftMargin, curY + topMargin));

                        curX += itemSize.Width;

                        if (itemSize.Height > maxHeight)
                            maxHeight = itemSize.Height;
                    }
                    else if (_printOptions.PlaceholderAutoRotate && (rotatedSize.Width <= freeSpaceWidth && rotatedSize.Height <= freeSpaceHeight))
                    {
                        // Item placed successfully after a 90-degree rotate.
                        items.Add(item);
                        coords.Add(new Point(curX + leftMargin, curY + topMargin));

                        item.SetSize(rotatedSize);

                        curX += rotatedSize.Width;

                        if (rotatedSize.Height > maxHeight)
                            maxHeight = rotatedSize.Height;
                    }
                    else
                    {
                        // Item cannot be inserted into current row - putting it into
                        // NonPlacedItems queue for trying to insert into next rows.
                        AddToNonPlacedItems(item);
                        isRowFilled = true;
                    }

                    curX += _horizontalSpacing;
                    if (curX >= pageWidth)
                        isRowFilled = true;
                }

                // Shifting to next row. If intMaxHeight == -1 - this means that
                // no one Item has been inserted on current step => current Item
                // of the queue can be placed only on next page => this page is filled.
                if (maxHeight == -1)
                    isPageFilled = true;
                else
                {
                    curX = 0;
                    curY += maxHeight + _verticalSpacing;
                    if (curY > pageHeight)
                        isPageFilled = true;
                }
            }

            e.HasMorePages = HasMorePages();
        }

        protected new ImagePrintItem GetNextItem(PrintPageEventArgs e)
        {
            ImagePrintItem item;

            if (_nonPlacedItems.Count == 0)
            {
                return base.GetNextItem(e);
            }
            else
            {
                item = (ImagePrintItem)_nonPlacedItems[_nonPlacedItems.Count - 1];
                _nonPlacedItems.RemoveAt(_nonPlacedItems.Count - 1);
            }

            return item;
        }

        protected void ConvertSpacings(int printerResolutionX, int printerResolutionY)
        {
            _horizontalSpacing = PrintUtils.InchHundredthsToPixels(_printOptions.HorizontalSpacing, printerResolutionX);
            _verticalSpacing = PrintUtils.InchHundredthsToPixels(_printOptions.VerticalSpacing, printerResolutionY);
        }

        protected void AddToNonPlacedItems(ImagePrintItem item)
        {
            _nonPlacedItems.Add(item);
        }

        protected new bool HasMorePages()
        {
            if (_nonPlacedItems.Count != 0)
                return true;

            return base.HasMorePages();
        }

        protected int _verticalSpacing;     // Spacings that are already converted
        protected int _horizontalSpacing;   // into pixels (using current page resolution).
        protected ArrayList _nonPlacedItems;
    }
}