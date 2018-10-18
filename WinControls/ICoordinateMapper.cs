// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Interface supplies methods for conversion between viewport and workspace coordinate systems.
    /// </summary>
    public interface ICoordinateMapper
    {
        System.Drawing.RectangleF ControlToWorkspace(System.Drawing.Rectangle controlRectangle, Aurigma.GraphicsMill.Unit workspaceUnit);

        System.Drawing.Rectangle WorkspaceToControl(System.Drawing.RectangleF workspaceRectangle, Aurigma.GraphicsMill.Unit workspaceUnit);

        System.Drawing.PointF ControlToWorkspace(System.Drawing.Point controlPoint, Aurigma.GraphicsMill.Unit workspaceUnit);

        System.Drawing.Point WorkspaceToControl(System.Drawing.PointF workspacePoint, Aurigma.GraphicsMill.Unit workspaceUnit);

        float GetControlPixelsPerUnitX(Aurigma.GraphicsMill.Unit unit);

        float GetControlPixelsPerUnitY(Aurigma.GraphicsMill.Unit unit);
    }
}