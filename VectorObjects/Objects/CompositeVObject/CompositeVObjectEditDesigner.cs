// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// CompositeVObjectDesigner. Provides multi-selection functionality.
    /// </summary>
    public class CompositeVObjectEditDesigner : GenericVObjectEditDesigner
    {
        #region "Member variables"

        private System.Drawing.Point _draggingStartPoint;
        private bool _draggingPerformed;
        private IVObject _objectToRemove;

        #endregion "Member variables"

        #region "Construction / destruction"

        public CompositeVObjectEditDesigner(IVObject obj)
            : base(obj)
        {
        }

        public CompositeVObjectEditDesigner(IVObject[] objects)
            : base()
        {
            if (objects == null)
                throw new System.ArgumentNullException("objects");
            if (objects.Length < 1)
                throw new System.ArgumentException(StringResources.GetString("ExStrArrayZeroLengthError"), "objects");

            base.ActualVObject = new CompositeVObject(objects);
            base.ActualVObject.Changed += new System.EventHandler(ObjectChangedHandler);
        }

        #endregion "Construction / destruction"

        #region "Public properties"

        public CompositeVObject CompositeVObject
        {
            get
            {
                return (CompositeVObject)base.ActualVObject;
            }
        }

        #endregion "Public properties"

        #region "Overloaded methods"

        public override IVObject[] VObjects
        {
            get
            {
                return ((CompositeVObject)base.ActualVObject).Children.ToArray();
            }
        }

        public override void UpdateSettings()
        {
            base.UpdateSettings();

            CompositeVObject castedObj = (CompositeVObject)base.ActualVObject;
            castedObj.MultipleVObjectsTransformationEnabled = VObjectsUtils.GetBoolDesignerProperty(base.VObjectHost, DesignerSettingsConstants.MultipleVObjectsTransformationEnabled, castedObj.MultipleVObjectsTransformationEnabled);
        }

        public override void NotifyDisconnect()
        {
            CompositeVObject compositeObj = (CompositeVObject)base.ActualVObject;
            compositeObj.EnableChangesTracking(false);

            try
            {
                foreach (IVObject child in CompositeVObject.Children)
                    child.Update();
            }
            finally
            {
                compositeObj.EnableChangesTracking(true);
            }

            base.NotifyDisconnect();
        }

        public override bool NotifyMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            bool result = false;
            base.Dragging = false;
            _draggingPerformed = false;
            _objectToRemove = null;

            //
            // Check for the control points click
            //
            System.Drawing.Point clickedPoint = new System.Drawing.Point(e.X, e.Y);
            if (base.GripsProvider != null)
            {
                base.DraggingPointIndex = base.GripsProvider.TestPoint(clickedPoint);
                if (base.DraggingPointIndex != GripsProvider.InvalidPointHandle)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        base.Dragging = true;
                        result = true;
                        _draggingStartPoint = clickedPoint;
                    }
                }

                if (base.GripsProvider.HitTest(clickedPoint))
                    result = true;
            }

            // If MultiSelect option is on we should also process Ctrl+Click action. If clicked object is
            // already selected - it should be removed from selection, otherwise it should be added to it.
            if (base.MultiSelect && (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control)
            {
                System.Drawing.PointF clickedPointF = base.VObjectHost.HostViewer.ControlToWorkspace(clickedPoint, Aurigma.GraphicsMill.Unit.Point);
                IVObject clickedObj = base.VObjectHost.CurrentLayer.Find(clickedPointF, VObject.SelectionPrecisionDelta / base.VObjectHost.HostViewer.Zoom);
                if (clickedObj != null && !clickedObj.Locked)
                {
                    if (!this.CompositeVObject.Children.Contains(clickedObj))
                    {
                        this.CompositeVObject.Children.Add(clickedObj);
                        base.VObjectHost.CurrentDesigner = this;
                    }
                    else
                        _objectToRemove = clickedObj;
                }

                result = true;
            }

            return result;
        }

        public override bool NotifyMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            bool result = base.NotifyMouseUp(e);

            if (base.MultiSelect && _objectToRemove != null && !_draggingPerformed)
            {
                if (this.CompositeVObject.Children.Count == 1)
                    base.VObjectHost.CurrentDesigner = base.VObjectHost.DefaultDesigner;
                else if (this.CompositeVObject.Children.Count == 2)
                {
                    if (this.VObjects[0] == _objectToRemove)
                        base.VObjectHost.CurrentDesigner = this.CompositeVObject.Children[1].Designer;
                    else
                        base.VObjectHost.CurrentDesigner = this.CompositeVObject.Children[0].Designer;
                }
                else
                {
                    this.CompositeVObject.Children.Remove(_objectToRemove);
                    base.VObjectHost.CurrentDesigner = this;
                }

                result = true;
            }

            return result;
        }

        public override bool NotifyMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new System.ArgumentNullException("e");

            if (e.X != _draggingStartPoint.X || e.Y != _draggingStartPoint.Y)
                _draggingPerformed = true;

            return base.NotifyMouseMove(e);
        }

        #endregion "Overloaded methods"
    }
}