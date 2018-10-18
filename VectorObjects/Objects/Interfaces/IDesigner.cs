// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Designers are special classes which handle user input.
    /// They are used to create new or edit existing vector objects.
    /// </summary>
    public interface IDesigner
    {
        void NotifyConnect(IVObjectHost objectHost);

        void UpdateSettings();

        void NotifyDisconnect();

        void Draw(System.Drawing.Graphics g);

        bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e);

        bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e);

        bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e);

        bool NotifyMouseDoubleClick(System.EventArgs e);

        bool NotifyKeyUp(System.Windows.Forms.KeyEventArgs e);

        bool NotifyKeyDown(System.Windows.Forms.KeyEventArgs e);

        bool Connected
        {
            get;
        }

        IVObject[] VObjects
        {
            get;
        }
    }
}