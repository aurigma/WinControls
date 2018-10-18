# Graphics Mill Windows Controls

## Description

Graphics Mill Windows Controls is a set of Windows Forms controls which helps you create an image processing user interface in desktop applications. The Bitmap Viewer control displays a bitmap on the screen with zoom, scroll, and crop functionality. The Vector Objects module allows working with composite images consisting of multiple elements such as bitmaps, texts, and vector data. The Thumbnail List View control displays a collection of images no matter if they are stored in a file system or memory.

## History

Initially, Windows Controls was an integral part of Graphics Mill. It was based on the old Windows Forms class library, which is no longer popular among developers due to such modern technologies as WPF and UWP. Since 9.x versions, [WinControls package](https://www.nuget.org/packages/Aurigma.GraphicsMill.WinControls.x64/) stopped being a part of the Graphics Mill release. The last available version on NuGet is 8.2.17. However, to enable our customers to maintain existing code in long-term, we decided to publish the full source code of the Windows Controls package on GitHub. 

## Important notes

 - We continue to provide support for existing customers of Windows Controls as long as their maintenance contract is active. If the maintenance contract of Graphics Mill is renewed after Nov 1, 2018, then we continue to provide support for Graphis Mill Core but not for Graphics Mill Windows Controls.

 - Small fixes and pull requests are possible but not guaranteed.

 - We are not going to remove the existing required API in the Graphics Mill Core component, but we don't guarantee that further Core versions will be fully compatible with existing sources of Windows Controls.

 - You still need a valid license for Graphics Mill Core to use Windows Controls.

## How to build

Just build GraphicsMill.WinControls.sln.

The default target platform is x64. If you need x86, please replace the NuGet package manually.

## In this repository

- VectorObjects assembly.
- WinControls assembly.
- ImageEditor tool, previously available as a part of [GraphicsMill samples](https://github.com/aurigma/GraphicsMill.Samples).
- Windows Control Reference in XML format.

## Documentation

The most recent documentation is available on the [Graphics Mill](https://www.graphicsmill.com/docs/gm5/UsingGraphicsMillinWindowsApplications.htm) web site.

## License

This repository is licensed with the [MIT](LICENSE) license.