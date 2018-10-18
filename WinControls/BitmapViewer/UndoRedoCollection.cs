// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

namespace Aurigma.GraphicsMill.WinControls
{
    public class UndoRedoBitmapCollection : System.Collections.Generic.LinkedList<Aurigma.GraphicsMill.Bitmap>
    {
        public int MaxCount = 5;

        private System.Collections.Generic.LinkedListNode<Bitmap> _current;

        public void Add(Aurigma.GraphicsMill.Bitmap bitmap)
        {
            if (this.MaxCount <= 0)
                return;

            if (_current != null)
            {
                while (base.Last != _current)
                    base.RemoveLast();
            }

            base.AddLast(bitmap);

            _current = base.Last;

            if (base.Count > this.MaxCount)
                base.RemoveFirst();
        }

        public bool CanUndo
        {
            get
            {
                return (_current != null) && (_current != base.First);
            }
        }

        public bool CanRedo
        {
            get
            {
                return (_current != null) && (_current != base.Last);
            }
        }

        public Bitmap Undo()
        {
            if (_current == null)
                return null;

            if (_current.Previous == null)
                return null;

            _current = _current.Previous;

            return _current.Value;
        }

        public Bitmap Redo()
        {
            if (_current == null)
                return null;

            if (_current.Next == null)
                return null;

            _current = _current.Next;

            return _current.Value;
        }
    }
}