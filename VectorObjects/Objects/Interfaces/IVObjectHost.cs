// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Provides functionality for vector objects storing, accessing.
    /// </summary>
    public interface IVObjectHost : Aurigma.GraphicsMill.IStateNavigable
    {
        Aurigma.GraphicsMill.Bitmap RenderWorkspace(float renderingResolution);

        LayerCollection Layers
        {
            get;
        }

        Layer CurrentLayer
        {
            get;
            set;
        }

        int CurrentLayerIndex
        {
            get;
            set;
        }

        IDesigner CurrentDesigner
        {
            get;
            set;
        }

        IDesigner DefaultDesigner
        {
            get;
        }

        System.Collections.Hashtable DesignerOptions
        {
            get;
        }

        ViewerBase HostViewer
        {
            get;
        }

        bool UndoRedoTrackingEnabled
        {
            get;
            set;
        }

        event DesignerChangedEventHandler DesignerChanged;

        event System.EventHandler CurrentLayerChanged;
    }

    #region "Designer changed event stuff"

    public delegate void DesignerChangedEventHandler(object sender, DesignerChangedEventArgs e);

    public class DesignerChangedEventArgs : System.EventArgs
    {
        public DesignerChangedEventArgs(IDesigner oldDesigner, IDesigner newDesigner)
        {
            _oldDesigner = oldDesigner;
            _newDesigner = newDesigner;
        }

        public IDesigner OldDesigner
        {
            get
            {
                return _oldDesigner;
            }
        }

        public IDesigner NewDesigner
        {
            get
            {
                return _newDesigner;
            }
        }

        private IDesigner _oldDesigner;
        private IDesigner _newDesigner;
    }

    #endregion "Designer changed event stuff"
}