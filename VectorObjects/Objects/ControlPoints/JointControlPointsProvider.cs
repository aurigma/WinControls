// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Internal class for uniting multiple control point providers.
    /// </summary>
    public class JointControlPointsProvider : IControlPointsProvider
    {
        #region "Construction / destruction"

        public JointControlPointsProvider(IControlPointsProvider provider)
        {
            _controlPointsProviders = new System.Collections.ArrayList();
            _controlPointsProviders.Add(provider);

            _controlPoints = new JointControlPointCollection(this);
            _supportedActions = new JointVObjectActionCollection(this);
        }

        #endregion "Construction / destruction"

        #region "IControlPointsProvider interface implmentation"

        public IControlPointCollection ControlPoints
        {
            get
            {
                return _controlPoints;
            }
        }

        public IVObjectActionCollection SupportedActions
        {
            get
            {
                return _supportedActions;
            }
        }

        public int MaxControlPointRadius
        {
            get
            {
                int max = 0;
                for (int i = 0; i < _controlPointsProviders.Count; i++)
                {
                    int val = ((IControlPointsProvider)_controlPointsProviders[i]).MaxControlPointRadius;
                    if (val > max)
                        max = val;
                }

                return max;
            }
        }

        public System.Windows.Forms.Cursor GetPointCursor(int index)
        {
            int translatedIndex;
            IControlPointsProvider provider = GetProviderByIndex(index, out translatedIndex);
            return provider.GetPointCursor(translatedIndex);
        }

        public System.Drawing.RectangleF GetControlPointsBounds()
        {
            if (_controlPointsProviders.Count == 0)
                return System.Drawing.RectangleF.Empty;

            System.Drawing.RectangleF result = ((IControlPointsProvider)_controlPointsProviders[0]).GetControlPointsBounds();
            for (int i = 1; i < _controlPointsProviders.Count; i++)
                result = System.Drawing.RectangleF.Union(result, ((IControlPointsProvider)_controlPointsProviders[i]).GetControlPointsBounds());

            return result;
        }

        public void DragPoint(int index, System.Drawing.PointF newPosition)
        {
            int translatedIndex;
            IControlPointsProvider provider = GetProviderByIndex(index, out translatedIndex);
            provider.DragPoint(translatedIndex, newPosition);
        }

        public void ClickPoint(int index)
        {
            int translatedIndex;
            IControlPointsProvider provider = GetProviderByIndex(index, out translatedIndex);
            provider.ClickPoint(translatedIndex);
        }

        #endregion "IControlPointsProvider interface implmentation"

        #region "Internal methods"

        internal IControlPointsProvider GetProviderByIndex(int index, out int translatedIndex)
        {
            int pointCount = 0;
            for (int i = 0; i < _controlPointsProviders.Count; i++)
            {
                IControlPointsProvider provider = (IControlPointsProvider)_controlPointsProviders[i];
                if (index < pointCount + provider.ControlPoints.Count)
                {
                    translatedIndex = index - pointCount;
                    return provider;
                }

                pointCount += provider.ControlPoints.Count;
            }

            throw new System.ArgumentException(StringResources.GetString("ExStrCannotFindIndex"), "index");
        }

        internal System.Collections.ArrayList ControlPointsProviders
        {
            get
            {
                return _controlPointsProviders;
            }
        }

        #endregion "Internal methods"

        #region "Public methods"

        public void AddProvider(IControlPointsProvider provider)
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _controlPointsProviders.Add(provider);
        }

        public IControlPointsProvider GetProviderAt(int index)
        {
            return (IControlPointsProvider)_controlPointsProviders[index];
        }

        public void InsertProvider(int index, IControlPointsProvider provider)
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _controlPointsProviders.Insert(index, provider);
        }

        public void RemoveProvider(IControlPointsProvider provider)
        {
            if (provider == null)
                throw new System.ArgumentNullException("provider");

            _controlPointsProviders.Remove(provider);
        }

        public void RemoveProviderAt(int index)
        {
            _controlPointsProviders.RemoveAt(index);
        }

        #endregion "Public methods"

        #region "Member variables"

        private System.Collections.ArrayList _controlPointsProviders;
        private JointControlPointCollection _controlPoints;
        private JointVObjectActionCollection _supportedActions;

        #endregion "Member variables"
    }
}