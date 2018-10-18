// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    internal class FilteringExpandableObjectConverter : System.ComponentModel.ExpandableObjectConverter
    {
        private static bool ShowProperty(System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.IsReadOnly)
                return false;

            System.Type propertyType = propertyDescriptor.PropertyType;

            if (propertyType == typeof(System.Drawing.Drawing2D.Matrix) ||
                propertyType == typeof(System.Drawing.Drawing2D.Blend) ||
                propertyType == typeof(System.Drawing.Drawing2D.ColorBlend) ||
                propertyType == typeof(System.Drawing.Drawing2D.CustomLineCap))
                return false;

            if (propertyDescriptor.ComponentType == typeof(System.Drawing.Pen) && !ShowPenProperty(propertyDescriptor))
                return false;

            return true;
        }

        private static bool ShowPenProperty(System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name == "DashPattern" ||
                propertyDescriptor.Name == "Brush" ||
                propertyDescriptor.Name == "CompoundArray")
                return false;

            return true;
        }

        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection propertyDescriptors = base.GetProperties(context, value, attributes);

            int propertyCount = 0;
            for (int i = 0; i < propertyDescriptors.Count; i++)
            {
                if (ShowProperty(propertyDescriptors[i]))
                    propertyCount++;
            }

            int k = 0;
            System.ComponentModel.PropertyDescriptor[] filteredProperties = new System.ComponentModel.PropertyDescriptor[propertyCount];
            for (int i = 0; i < propertyDescriptors.Count; i++)
            {
                if (ShowProperty(propertyDescriptors[i]))
                    filteredProperties[k++] = propertyDescriptors[i];
            }

            return new System.ComponentModel.PropertyDescriptorCollection(filteredProperties);
        }
    }
}