// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;

// If however you require an assembly name that is different from the default namespace such
// as an asembly called "Foo.dll" and a namespace of "My.Imaginary.Corporate.Namespace",
// which is not an unlikely scenario, you're going to be out of luck because a bug in the
// GetImageFromResource method prepends the name of the assembly and makes a resource string
// such as "MyControl.MyControl.MyBitmap.bmp" which won't ever exist in the assembly.
// To overcome this problem and to simplify the process of finding a resource you can do the following:
// #1 Create an internal class called "resfinder" outside of the root namespace of your source code.
// #2 Use "resfinder" in the toolbox bitmap attribute instead of your control name.
internal class ResourceFinder
{
}

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
                return "Aurigma.GraphicsMill.WinControls.Resources.NET11." + name;
            }
            else
            {
                return "Aurigma.GraphicsMill.WinControls.Resources.NET20." + name;
            }
        }
    }
}