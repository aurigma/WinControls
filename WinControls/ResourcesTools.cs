// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// A class for managing resources
    /// </summary>
    internal sealed class StringResources
    {
        private StringResources()
        {
        }

        /// <summary>
        /// Returns resource string by its name
        /// </summary>
        /// <param name="name">Resource string name</param>
        /// <returns>Resource string value</returns>
        public static string GetString(string name)
        {
            System.Resources.ResourceManager resourceManager =
                new System.Resources.ResourceManager("Aurigma.GraphicsMill.WinControls.Messages", typeof(StringResources).Assembly);

            return resourceManager.GetString(name);
        }
    }

    /// <summary>
    /// Resource based description attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResDescriptionAttribute : System.ComponentModel.DescriptionAttribute
    {
        private bool _stringReplaced;

        public ResDescriptionAttribute(string description)
            : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this._stringReplaced)
                {
                    this._stringReplaced = true;
                    base.DescriptionValue = StringResources.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Class)]
    internal sealed class AdaptiveToolboxBitmapAttribute : System.Drawing.ToolboxBitmapAttribute
    {
        public AdaptiveToolboxBitmapAttribute(System.Type t, string name)
            : base(t, GetResourceName(name))
        {
        }

        private static string GetResourceName(string name)
        {
            if (System.Environment.Version.Major == 1)
            {
                return "Resources.NET11." + name;
            }
            else
            {
                return "Resources.NET20." + name;
            }
        }
    }
}