// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Summary description for IVObjectAction.
    /// </summary>
    public interface IVObjectAction
    {
        string Name
        {
            get;
        }

        int Id
        {
            get;
        }

        bool Enabled
        {
            get;
            set;
        }
    }
}