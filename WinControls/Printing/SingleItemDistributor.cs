// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Drawing;
using System.Drawing.Printing;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class SingleItemDistributor : IPrintItemDistributorImpl
    {
        public SingleItemDistributor()
        {
        }

        public override void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords)
        {
            System.Diagnostics.Debug.Assert(items != null, "pItems should be allocated by caller");
            System.Diagnostics.Debug.Assert(coords != null, "pCoords should be allocated by caller");

            ImagePrintItem item = GetNextItem(e);
            if (e.Cancel == true || item == null)
            {
                e.HasMorePages = HasMorePages();
                return;
            }

            int printerResolutionX, printerResolutionY;
            GetValidatedResolution(e, out printerResolutionX, out printerResolutionY);

            int pageWidth = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Width, printerResolutionX),
                pageHeight = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Height, printerResolutionY),
                marginX = PrintUtils.InchHundredthsToPixels(e.MarginBounds.X, printerResolutionX),
                marginY = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Y, printerResolutionY);

            Size pageSize = new Size(pageWidth, pageHeight);
            item.FitSize(pageSize, _printOptions.PlaceholderAutoRotate);
            Size itemSize = item.GetSize();

            Point coord = new Point();
            coord.X = marginX + (pageWidth - itemSize.Width) / 2;
            coord.Y = marginY + (pageHeight - itemSize.Height) / 2;

            items.Add(item);
            coords.Add(coord);
            e.HasMorePages = HasMorePages();
        }
    }
}