// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class BestFitDistributor : MultipleItemsDistributor
    {
        private const int CachedSquareCoefficient = 3;
        private const int MaxBranchingDeep = 0;

        private struct ItemPlacement
        {
            public bool Rotate;
            public int QueueIndex;
            public Position ItemPosition;
            public ItemRectangle Item;
        }

        private class SearchStackFrame
        {
            public SearchStackFrame()
            {
                PlacedRectangles = new ArrayList();
            }

            public int ProcessState;
            public int Coverage;
            public int Square;
            public int InitialQueueIndex;
            public int ActualQueueIndex;
            public Position ItemPosition;
            public ItemRectangle Item;
            public bool Rotate;
            public ArrayList PlacedRectangles;
        }

        public BestFitDistributor()
        {
            _queuedItems = new ArrayList();
        }

        protected void InitializePage(PrintPageEventArgs e)
        {
            int printerResolutionX, printerResolutionY;
            GetValidatedResolution(e, out printerResolutionX, out printerResolutionY);

            ConvertSpacings(printerResolutionX, printerResolutionY);

            _pageWidth = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Width, printerResolutionX);
            _pageHeight = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Height, printerResolutionY);

            _pageLeft = PrintUtils.InchHundredthsToPixels(e.MarginBounds.X, printerResolutionX);
            _pageTop = PrintUtils.InchHundredthsToPixels(e.MarginBounds.Y, printerResolutionY);

            // Taking into account shared touch-borders
            _pageLeft -= 1;
            _pageTop -= 1;
            _pageWidth += 1;
            _pageHeight += 1;

            _pageRight = _pageLeft + _pageWidth - 1;
            _pageBottom = _pageTop + _pageHeight - 1;
        }

        // Filling queue with ImagePrintItems that we will try to place into page.
        protected int FillQueue(PrintPageEventArgs e)
        {
            ImagePrintItem printItem;
            Size pageSize = new Size(_pageWidth - 1, _pageHeight - 1);
            int maxCachedSquare = _pageWidth * _pageHeight * CachedSquareCoefficient,
                queuedSquare = 0;

            if (queuedSquare < maxCachedSquare)
                while ((printItem = base.GetNextItem(e)) != null && queuedSquare < maxCachedSquare)
                {
                    printItem.FitSize(pageSize, _printOptions.PlaceholderAutoRotate);

                    ItemRectangle itemRect = new ItemRectangle(printItem, _verticalSpacing, _horizontalSpacing);
                    queuedSquare += itemRect.Square;
                    _queuedItems.Add(itemRect);
                }

            // If print was canceled
            if (e.Cancel == true)
            {
                _queuedItems.Clear();
                return 0;
            }

            _queuedItems.Sort();
            return _queuedItems.Count;
        }

        protected ItemRectangle GetQueuedItemAt(ref int index)
        {
            for (; index < _queuedItems.Count; index++)
            {
                ItemRectangle item = (ItemRectangle)_queuedItems[index];
                if (!item.Locked)
                {
                    item.Locked = true;
                    return item;
                }
            }
            return null;
        }

        protected void FlushQueueNonPlaced()
        {
            foreach (ItemRectangle rect in _queuedItems)
                if (!rect.Locked)
                    AddToNonPlacedItems(rect.Item);

            _queuedItems.Clear();
        }

        public override void FillPage(PrintPageEventArgs e, System.Collections.ArrayList items, System.Collections.ArrayList coords)
        {
            // Getting page parameters, filling queue of elements for placement
            InitializePage(e);
            int i, itemCount = FillQueue(e);
            if (itemCount == 0)
                return;

            // Preparing stack
            SearchStackFrame[] stack = new SearchStackFrame[_queuedItems.Count + 1];
            for (i = 0; i < stack.Length; i++)
                stack[i] = new SearchStackFrame();

            // Adding fake rectangle with first position
            ItemRectangle fakeRect = new ItemRectangle();
            fakeRect.Location = new Point(-100000, -100000);
            fakeRect.HostedPositions.Add(new Position(_pageLeft, _pageTop));
            stack[0].PlacedRectangles.Add(fakeRect);

            // Go! - there we will emulate recursion in plain loop. Deep is restricted by s_intMaxBranchingDepth,
            // after this level - just going straight forward without recursive search.
            ItemPlacement[] bestPlacement = new ItemPlacement[_queuedItems.Count];       // best reached result
            ArrayList positions = new ArrayList(256);                                    // search positions for current level

            int bestSquare = -1,
                framePointer = 0,
                placedItemCount = 0;

            while (true)
            {
                // if the dip is over
                if (framePointer < 0)
                    break;

                // extracting and cleaning current level of the stack
                SearchStackFrame stackFrame = stack[framePointer];
                if (stackFrame.Item != null)
                {
                    stackFrame.Item.Locked = false;
                    stackFrame.Item = null;
                }
                stackFrame.ActualQueueIndex = stackFrame.InitialQueueIndex;

                // searching at this level
                if (stackFrame.ProcessState < 2)
                {
                    positions.Clear();
                    foreach (ItemRectangle tmp1 in stackFrame.PlacedRectangles)
                        foreach (Position tmp2 in tmp1.HostedPositions)
                        {
                            tmp2.Close(framePointer, false);
                            if (!tmp2.Closed(framePointer))
                                positions.Add(tmp2);
                        }

                    bool placed = false;
                    while (!placed)
                    {
                        int maxCoverage = -1;
                        int coverage;

                        Position resPosition;

                        stackFrame.Item = GetQueuedItemAt(ref stackFrame.ActualQueueIndex);
                        if (stackFrame.Item == null)
                            break;

                        // Original item orientation
                        stackFrame.Rotate = stackFrame.Item.Rotated = (stackFrame.ProcessState == 1);
                        for (i = 0; i < positions.Count; i++)
                        {
                            Position position = (Position)positions[i];
                            if (TryToPlaceRectangle(framePointer, stackFrame.PlacedRectangles, position, stackFrame.Item, out coverage, out resPosition) && coverage > maxCoverage)
                            {
                                stackFrame.ItemPosition = resPosition;
                                maxCoverage = coverage;
                                placed = true;
                            }
                        }

                        // Rotated orientation (performed if we cannot use full-search on current level)
                        if (_printOptions.PlaceholderAutoRotate && framePointer > MaxBranchingDeep)
                        {
                            stackFrame.Item.Rotated = true;
                            for (i = 0; i < positions.Count; i++)
                            {
                                Position position = (Position)positions[i];
                                if (position.Closed(framePointer))
                                    continue;

                                if (TryToPlaceRectangle(framePointer, stackFrame.PlacedRectangles, position, stackFrame.Item, out coverage, out resPosition) && coverage > maxCoverage)
                                {
                                    stackFrame.ItemPosition = resPosition;
                                    maxCoverage = coverage;
                                    stackFrame.Rotate = true;
                                    placed = true;
                                }
                            }
                        }

                        if (placed)
                        {
                            // Goto next level
                            if (framePointer <= MaxBranchingDeep && _printOptions.PlaceholderAutoRotate)
                                stackFrame.ProcessState++;
                            else
                                stackFrame.ProcessState = 2;

                            int nextFrame = framePointer + 1;
                            stack[nextFrame].ProcessState = 0;
                            stack[nextFrame].Item = null;
                            stack[nextFrame].Coverage = stackFrame.Coverage + maxCoverage;
                            stack[nextFrame].Square = stackFrame.Square + stackFrame.Item.Square;
                            stack[nextFrame].InitialQueueIndex = stackFrame.ActualQueueIndex + 1;
                            stack[nextFrame].PlacedRectangles.AddRange(stackFrame.PlacedRectangles);

                            stackFrame.Item.Rotated = stackFrame.Rotate;
                            PlaceRectangle(framePointer, stackFrame.ItemPosition, stackFrame.Item, stack[nextFrame].PlacedRectangles);
                            stackFrame.Item.Neighbours.Clear();

                            framePointer++;
                        }
                        else
                        {
                            stackFrame.Item.Locked = false;
                            stackFrame.Item = null;
                            stackFrame.ActualQueueIndex++;
                        }
                    }

                    if (!placed)
                    {
                        if (stackFrame.Square > bestSquare)
                        {
                            placedItemCount = framePointer;
                            bestSquare = stackFrame.Square;

                            for (i = 0; i < framePointer; i++)
                            {
                                bestPlacement[i].QueueIndex = stack[i].ActualQueueIndex;
                                bestPlacement[i].ItemPosition = stack[i].ItemPosition;
                                bestPlacement[i].Rotate = stack[i].Rotate;
                                bestPlacement[i].Item = stack[i].Item;
                            }
                        }

                        framePointer--;
                    }
                }
                else
                {
                    framePointer--;
                }
            }

            // Returning best result to caller
            if (bestSquare > 0)
            {
                for (i = 0; i < placedItemCount; i++)
                {
                    ItemPlacement element = bestPlacement[i];

                    GetQueuedItemAt(ref element.QueueIndex);
                    element.Item.Rotated = element.Rotate;

                    coords.Add(ItemRectangle.GetItemLeftTopCorner(element.ItemPosition));
                    items.Add(element.Item.Detach());
                }
            }

            FlushQueueNonPlaced();
            e.HasMorePages = HasMorePages();
        }

        // Trying to place the rectangle at specified position and also checks some alternatives based
        // on specified position. The best position will be chosen.
        protected bool TryToPlaceRectangle(int dip, ArrayList placedRectangles, Position initialPosition, ItemRectangle itemRectangle, out int coverage, out Position resultPosition)
        {
            Point initialCoords = initialPosition.Coordinates;
            bool placed = false;

            Position[] altPositions = new Position[3];
            int altCount = 0;
            coverage = 0;
            resultPosition = null;

            // Filling array of possible variants for item placement.
            // ...original position
            if (!initialPosition.Closed(dip))
                altPositions[altCount++] = initialPosition;

            // ...trying to go left and top
            int minLeftDistance = initialCoords.X + 1,
                minUpperDistance = initialCoords.Y + 1,
                left,
                upper;
            bool leftPointMatch = true,
                upperPointMatch = false;

            // ...check position relative to other rectangles
            foreach (ItemRectangle rect in placedRectangles)
            {
                if (rect.Y <= initialCoords.Y && rect.BottomY >= initialCoords.Y && rect.RightX <= initialCoords.X)
                {
                    left = initialCoords.X - rect.RightX;
                    if (left < minLeftDistance)
                    {
                        minLeftDistance = left;
                        if (initialCoords.Y == rect.Y || initialCoords.Y == rect.BottomY)
                            leftPointMatch = true;
                        else
                            leftPointMatch = false;
                    }
                }
                else if (rect.X <= initialCoords.X && rect.RightX >= initialCoords.X && rect.BottomY <= initialCoords.Y)
                {
                    upper = initialCoords.Y - rect.BottomY;
                    if (upper < minUpperDistance)
                    {
                        minUpperDistance = upper;
                        if (initialCoords.X == rect.X || initialCoords.X == rect.RightX)
                            upperPointMatch = true;
                        else
                            upperPointMatch = false;
                    }
                }
            }

            // ...storing existing additional positions
            if (minLeftDistance > 0 && !leftPointMatch)
                altPositions[altCount++] = new Position(initialCoords.X - minLeftDistance, initialCoords.Y);
            if (minUpperDistance > 0 && !upperPointMatch)
                altPositions[altCount++] = new Position(initialCoords.X, initialCoords.Y - minUpperDistance);

            // Choosing best position among alternatives
            int maxCoverage = 0;
            for (int i = 0; i < altCount; i++)
                if (ExaminePosition(placedRectangles, altPositions[i], itemRectangle, out coverage) && coverage > maxCoverage)
                {
                    placed = true;
                    maxCoverage = coverage;
                    resultPosition = altPositions[i];
                }

            coverage = maxCoverage;
            return placed;
        }

        // Returns true if rectangle could be placed into position. (also calculates perimeter coverage in the position).
        protected bool ExaminePosition(ArrayList placedRectangles, Position position, ItemRectangle itemRectangle, out int coverage)
        {
            // ----- Trying to place exact at specified position -----
            itemRectangle.MoveToPosition(position);
            itemRectangle.Neighbours.Clear();
            coverage = 0;

            // ...intersects with page borders?
            if (!IsInPageBorders(itemRectangle))
                return false;

            // ...intersects with other rectangles?
            foreach (ItemRectangle placedRect in placedRectangles)
            {
                if (itemRectangle.Intersects(placedRect))
                    return false;

                bool haveTouch;
                coverage += itemRectangle.CalculateCoverage(placedRect, out haveTouch);
                if (haveTouch)
                    itemRectangle.Neighbours.Add(placedRect);
            }

            // ...additional coverage values - when contacts page borders
            if (itemRectangle.X == _pageLeft)
                coverage += itemRectangle.Height;
            if (itemRectangle.Y == _pageTop)
                coverage += itemRectangle.Width;
            if (itemRectangle.RightX == _pageRight)
                coverage += itemRectangle.Height;
            if (itemRectangle.BottomY == _pageBottom)
                coverage += itemRectangle.Width;

            return true;
        }

        protected static void PlaceRectangle(int dip, Position dstPosition, ItemRectangle itemRectangle, ArrayList placedRectangles)
        {
            itemRectangle.MoveToPosition(dstPosition);

            // ...Generate all possible positions
            Position[] hostedPoints = new Position[4];

            // Left-top corner
            Point pnt = itemRectangle.Location;
            hostedPoints[0] = new Position(pnt);

            // Left-bottom corner
            pnt.Y += itemRectangle.Height - 1;
            hostedPoints[1] = new Position(pnt);

            // Right-bottom corner
            pnt.X += itemRectangle.Width - 1;
            hostedPoints[2] = new Position(pnt);

            // Right-top corner
            pnt = itemRectangle.Location;
            pnt.X += itemRectangle.Width - 1;
            hostedPoints[3] = new Position(pnt);

            // ...Closing position that is closed by the pItemRectangle himself
            hostedPoints[0].Close(dip, true);

            for (int i = 0; i < 4; i++)
            {
                // Closing closed positions in pPositionsArray
                IEnumerator neighboursEnumerator = itemRectangle.Neighbours.GetEnumerator();
                bool toContinue = true;
                while (neighboursEnumerator.MoveNext() && toContinue)
                {
                    ItemRectangle neighbour = (ItemRectangle)neighboursEnumerator.Current;
                    for (int j = 0; j < neighbour.HostedPositions.Count; j++)
                    {
                        Position neighbourPosition = (Position)neighbour.HostedPositions[j];

                        if (hostedPoints[i].EqualCoordinates(neighbourPosition))
                        {
                            if (hostedPoints[i].Closed(dip) || neighbourPosition.Closed(dip))
                            {
                                hostedPoints[i].Close(dip, true);
                                neighbourPosition.Close(dip, true);

                                toContinue = false;
                                break;
                            }
                            else
                                hostedPoints[i] = neighbourPosition;
                        }
                    }
                }
            }

            // ...Adding new possible positions into pPositions array, adding new placed rect.
            foreach (Position pos in hostedPoints)
            {
                if (!pos.Closed(dip))
                    itemRectangle.HostedPositions.Add(pos);
            }
            placedRectangles.Add(itemRectangle);
        }

        protected bool IsInPageBorders(ItemRectangle itemRectangle)
        {
            if (itemRectangle.X < _pageLeft ||
                itemRectangle.Y < _pageTop ||
                itemRectangle.RightX > _pageRight + _horizontalSpacing ||
                itemRectangle.BottomY > _pageBottom + _verticalSpacing)
                return false;

            return true;
        }

        protected ArrayList _queuedItems;

        protected int _pageWidth;
        protected int _pageHeight;
        protected int _pageLeft;
        protected int _pageTop;
        protected int _pageRight;
        protected int _pageBottom;
    }

    internal class ItemRectangle : IComparable
    {
        public ItemRectangle()
        {
            _neighbours = new ArrayList();
            _hostedPositions = new ArrayList();
        }

        public ItemRectangle(ImagePrintItem item, int verticalSpacing, int horizontalSpacing)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _horizontalSpacing = horizontalSpacing;
            _verticalSpacing = verticalSpacing;
            _item = item;

            Size size = _item.GetSize();
            size.Width += _horizontalSpacing + 1/*shared border*/;
            size.Height += _verticalSpacing + 1/*shared border*/;
            _rect = new Rectangle(new Point(0, 0), size);

            _square = _rect.Width * _rect.Height;

            _neighbours = new ArrayList();
            _hostedPositions = new ArrayList();
        }

        #region "Properties"

        public int Square
        {
            get
            {
                return _square;
            }
        }

        public ImagePrintItem Item
        {
            get
            {
                return _item;
            }
        }

        public int X
        {
            get
            {
                return _rect.X;
            }
            set
            {
                _rect.X = value;
            }
        }

        public int Y
        {
            get
            {
                return _rect.Y;
            }
            set
            {
                _rect.Y = value;
            }
        }

        public Point Location
        {
            get
            {
                return _rect.Location;
            }
            set
            {
                _rect.Location = value;
            }
        }

        public int Width
        {
            get
            {
                return _rect.Width;
            }
        }

        public int Height
        {
            get
            {
                return _rect.Height;
            }
        }

        public int RightX
        {
            get
            {
                return _rect.X + _rect.Width - 1;
            }
        }

        public int BottomY
        {
            get
            {
                return _rect.Y + _rect.Height - 1;
            }
        }

        public ArrayList Neighbours
        {
            get
            {
                return _neighbours;
            }
        }

        public ArrayList HostedPositions
        {
            get
            {
                return _hostedPositions;
            }
        }

        public bool Locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
            }
        }

        // Changes Width and Height in _rect to reflect 90-degree rotation.
        // Values of X & Y coordinates of _rect will NOT re-calculated.
        public bool Rotated
        {
            set
            {
                if (value)
                    _rect.Size = _item.GetSizeOfRotated();
                else
                    _rect.Size = _item.GetSize();

                _rect.Width += _horizontalSpacing + 1;
                _rect.Height += _verticalSpacing + 1;
            }
        }

        #endregion "Properties"

        // Shifts X & Y coordinates of _rect to place rectangle into specified position.
        public void MoveToPosition(Position position)
        {
            _rect.Location = position.Coordinates;
        }

        public virtual int CompareTo(object obj)
        {
            return (((ItemRectangle)obj).Square - _square);
        }

        // Intersects or not 2 rectangles? Contact != intersection
        public bool Intersects(ItemRectangle rectangle)
        {
            if (rectangle.X >= RightX ||
                rectangle.Y >= BottomY ||
                rectangle.RightX <= _rect.X ||
                rectangle.BottomY <= _rect.Y)
                return false;

            return true;
        }

        public int CalculateCoverage(ItemRectangle rectangle, out bool haveTouch)
        {
            int coverage = 0;
            int left0 = this.X,
                right0 = this.RightX,
                top0 = this.Y,
                bottom0 = this.BottomY;
            int left1 = rectangle.X,
                right1 = rectangle.RightX,
                top1 = rectangle.Y,
                bottom1 = rectangle.BottomY;

            // ...If no touch - exit
            if (left1 > right0 ||
                right1 < left0 ||
                top1 > bottom0 ||
                bottom1 < top0)
            {
                haveTouch = false;
                return 0;
            }

            // Top/bottom touch
            int max, min;
            if ((top0 == bottom1 || bottom0 == top1) && left1 <= right0 && right1 >= left0)
            {
                max = Math.Max(left0, left1);
                min = Math.Min(right0, right1);
                coverage += min - max + 1;
            }

            // Left/right touch
            if ((left0 == right1 || right0 == left1) && top1 <= bottom0 && bottom1 >= top0)
            {
                max = Math.Max(top0, top1);
                min = Math.Min(bottom0, bottom1);
                coverage += min - max + 1;
            }

            haveTouch = true;
            return coverage;
        }

        public ImagePrintItem Detach()
        {
            Size size = _rect.Size;
            size.Width -= _horizontalSpacing + 1;
            size.Height -= _verticalSpacing + 1;

            _item.SetSize(size);
            ImagePrintItem item = _item;
            _item = null;

            return item;
        }

        public static Point GetItemLeftTopCorner(Position position)
        {
            Point result = position.Coordinates;

            result.X += 1;
            result.Y += 1;
            return result;
        }

        //------------------------------------------
        protected int _verticalSpacing;

        protected int _horizontalSpacing;

        protected bool _locked;

        protected Rectangle _rect;
        protected int _square;
        protected ImagePrintItem _item;
        protected ArrayList _neighbours;
        protected ArrayList _hostedPositions;
    }

    internal class Position
    {
        public Position()
        {
            _closingDeep = -1;
        }

        public Position(Point obj)
        {
            _closingDeep = -1;
            _coords = obj;
        }

        public Position(int x, int y)
            : this(new Point(x, y))
        {
        }

        public bool EqualCoordinates(Position position)
        {
            if (_coords.Equals(position.Coordinates))
                return true;

            return false;
        }

        public bool Closed(int dip)
        {
            if (_closingDeep == -1 || _closingDeep >= dip)
                return false;

            return true;
        }

        public void Close(int dip, bool close)
        {
            if (close)
            {
                if (_closingDeep > dip)
                    _closingDeep = dip;
            }
            else if (!close && _closingDeep >= dip)
                _closingDeep = -1;
        }

        #region "Properties & stuff"

        public Point Coordinates
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }

        #endregion "Properties & stuff"

        protected Point _coords;
        protected int _closingDeep;
    }
}