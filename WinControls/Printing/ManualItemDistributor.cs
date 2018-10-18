// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System.Drawing;
using System.Drawing.Printing;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class ManualItemDistributor : MultipleItemsDistributor
    {
        public override void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords)
        {
            while (true)
            {
                ImagePrintItem item = GetNextItem(e);

                if (e.Cancel == true || item == null)
                    break;

                if (_beginNewPage)
                {
                    AddToNonPlacedItems(item);
                    _beginNewPage = false;
                    break;
                }

                int printerResolutionX, printerResolutionY;
                GetValidatedResolution(e, out printerResolutionX, out printerResolutionY);

                Point location = new Point(PrintUtils.InchHundredthsToPixels(_placeholderLocation.X, printerResolutionX), PrintUtils.InchHundredthsToPixels(_placeholderLocation.Y, printerResolutionY));
                items.Add(item);
                coords.Add(location);
            }

            e.HasMorePages = HasMorePages();
        }

        protected override void OnQueryImageEvent(QueryImageEventArgs queryImageEventArgs)
        {
            base.OnQueryImageEvent(queryImageEventArgs);

            _beginNewPage = queryImageEventArgs.BeginNewPage;
            _placeholderLocation = queryImageEventArgs.PrintPlaceholder.Location;
        }

        protected bool _beginNewPage;
        protected Point _placeholderLocation;
    }
}