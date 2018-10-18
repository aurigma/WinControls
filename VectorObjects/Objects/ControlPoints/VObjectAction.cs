// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    public abstract class VObjectAction : IVObjectAction
    {
        #region "Action names"

        public static readonly int Resize = 0;
        public static readonly int Skew = 1;
        public static readonly int Rotate = 2;
        public static readonly int Drag = 3;
        public static readonly int MoveNode = 4;
        public static readonly int ChangeTextArea = 5;

        #endregion "Action names"

        protected VObjectAction(int id, string name)
        {
            if (name == null)
                throw new System.ArgumentNullException("name");

            _id = id;
            _name = name;
        }

        #region IVObjectAction Members

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public abstract bool Enabled
        {
            get;
            set;
        }

        #endregion IVObjectAction Members

        #region "Member variables"

        private int _id;
        private string _name;

        #endregion "Member variables"
    }
}