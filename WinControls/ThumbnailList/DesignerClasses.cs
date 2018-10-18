// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// TypeConverter for ListColumn class. Used by Visual Studio while working with the component in design time.
    /// </summary>
    public class ListColumnTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of one type to the type of this converter.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A type that represents the type you want to convert to.</param>
        /// <returns>Returns true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="contex">An ITypeDescriptorContext that provides a format context. </param>
        /// <param name="culture">A CultureInfo object. If a null reference is passed, the current culture is assumed.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">Destination type.</param>
        /// <returns>Returns an object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ListColumn column = value as ListColumn;
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if (column != null && destinationType == typeof(InstanceDescriptor))
            {
                MemberInfo memberInfo = typeof(ListColumn).GetConstructor(new Type[] { typeof(int), typeof(string), typeof(int), typeof(HorizontalAlignment) });
                Object[] arguments = new Object[] { column.TextInfoId, column.Caption, column.Width, column.TextAlignment };

                return new InstanceDescriptor(memberInfo, arguments);
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}