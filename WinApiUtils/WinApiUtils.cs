// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Aurigma
{
    internal class NativeMethods
    {
        private NativeMethods()
        {
        }

        #region "Static functions"

        internal static int SignedHIWORD(IntPtr a)
        {
            return ((int)a) >> 8;
        }

        internal static int SignedLOWORD(IntPtr a)
        {
            return (int)a & 0x0000ffff;
        }

        #endregion "Static functions"

        //
        // ListView styles
        //
        public const uint LVS_DUMMY_ZERO = 0x0000;

        public const uint LVS_ICON = 0x0000;
        public const uint LVS_REPORT = 0x0001;
        public const uint LVS_SMALLICON = 0x0002;
        public const uint LVS_LIST = 0x0003;
        public const uint LVS_TYPEMASK = 0x0003;
        public const uint LVS_SINGLESEL = 0x0004;
        public const uint LVS_SHOWSELALWAYS = 0x0008;
        public const uint LVS_SORTASCENDING = 0x0010;
        public const uint LVS_SORTDESCENDING = 0x0020;
        public const uint LVS_SHAREIMAGELISTS = 0x0040;
        public const uint LVS_NOLABELWRAP = 0x0080;
        public const uint LVS_AUTOARRANGE = 0x0100;
        public const uint LVS_EDITLABELS = 0x0200;
        public const uint LVS_OWNERDATA = 0x1000;
        public const uint LVS_NOSCROLL = 0x2000;
        public const uint LVS_TYPESTYLEMASK = 0xfc00;
        public const uint LVS_ALIGNTOP = 0x0000;
        public const uint LVS_ALIGNLEFT = 0x0800;
        public const uint LVS_ALIGNMASK = 0x0c00;
        public const uint LVS_OWNERDRAWFIXED = 0x0400;
        public const uint LVS_NOCOLUMNHEADER = 0x4000;
        public const uint LVS_NOSORTHEADER = 0x8000;

        public const uint LVS_EX_GRIDLINES = 0x00000001;
        public const uint LVS_EX_SUBITEMIMAGES = 0x00000002;
        public const uint LVS_EX_CHECKBOXES = 0x00000004;
        public const uint LVS_EX_TRACKSELECT = 0x00000008;
        public const uint LVS_EX_HEADERDRAGDROP = 0x00000010;
        public const uint LVS_EX_FULLROWSELECT = 0x00000020;
        public const uint LVS_EX_ONECLICKACTIVATE = 0x00000040;
        public const uint LVS_EX_TWOCLICKACTIVATE = 0x00000080;
        public const uint LVS_EX_FLATSB = 0x00000100;
        public const uint LVS_EX_REGIONAL = 0x00000200;
        public const uint LVS_EX_INFOTIP = 0x00000400;
        public const uint LVS_EX_UNDERLINEHOT = 0x00000800;
        public const uint LVS_EX_UNDERLINECOLD = 0x00001000;
        public const uint LVS_EX_MULTIWORKAREAS = 0x00002000;
        public const uint LVS_EX_LABELTIP = 0x00004000;
        public const uint LVS_EX_BORDERSELECT = 0x00008000;
        public const uint LVS_EX_DOUBLEBUFFER = 0x00010000;
        public const uint LVS_EX_HIDELABELS = 0x00020000;
        public const uint LVS_EX_SINGLEROW = 0x00040000;
        public const uint LVS_EX_SNAPTOGRID = 0x00080000;
        public const uint LVS_EX_SIMPLESELECT = 0x00100000;

        //
        // ListView messages
        //
        public const int LVM_FIRST = 0x1000;

        public const int LVM_SETBKCOLOR = (LVM_FIRST + 1);
        public const int LVM_GETIMAGELIST = (LVM_FIRST + 2);
        public const int LVM_SETIMAGELIST = (LVM_FIRST + 3);
        public const int LVM_SETCALLBACKMASK = (LVM_FIRST + 11);
        public const int LVM_DELETEITEM = (LVM_FIRST + 8);
        public const int LVM_DELETEALLITEMS = (LVM_FIRST + 9);
        public const int LVM_ARRANGE = (LVM_FIRST + 22);
        public const int LVM_SETITEMCOUNT = (LVM_FIRST + 47);
        public const int LVM_GETITEMCOUNT = (LVM_FIRST + 4);
        public const int LVM_SETITEMSSTATE = (LVM_FIRST + 43);
        public const int LVM_DELETECOLUMN = (LVM_FIRST + 28);
        public const int LVM_SETTEXTCOLOR = (LVM_FIRST + 36);
        public const int LVM_SETTEXTBKCOLOR = (LVM_FIRST + 38);
        public const int LVM_UPDATE = (LVM_FIRST + 42);
        public const int LVM_SETCOLUMNW = (LVM_FIRST + 96);
        public const int LVM_INSERTCOLUMNW = (LVM_FIRST + 97);
        public const int LVM_REDRAWITEMS = (LVM_FIRST + 21);
        public const int LVM_ENSUREVISIBLE = (LVM_FIRST + 19);
        public const int LVM_GETITEMW = (LVM_FIRST + 75);
        public const int LVM_SETITEMW = (LVM_FIRST + 76);
        public const int LVM_INSERTITEMW = (LVM_FIRST + 77);
        public const int LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54);
        public const int LVM_GETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 55);
        public const int LVM_GETNEXTITEM = (LVM_FIRST + 12);
        public const int LVM_GETCOLUMNWIDTH = (LVM_FIRST + 29);
        public const int LVM_SETCOLUMNWIDTH = (LVM_FIRST + 30);
        public const int LVM_GETITEMRECT = (LVM_FIRST + 14);
        public const int LVM_GETEDITCONTROL = (LVM_FIRST + 24);
        public const int LVM_SORTITEMSEX = (LVM_FIRST + 81);
        public const int LVM_SETITEMPOSITION32 = (LVM_FIRST + 49);
        public const int LVM_GETITEMPOSITION = (LVM_FIRST + 16);
        public const int LVM_HITTEST = (LVM_FIRST + 18);
        public const int LVM_INSERTMARKHITTEST = (LVM_FIRST + 168);
        public const int LVM_SETINSERTMARK = (LVM_FIRST + 166);
        public const int LVM_GETINSERTMARK = (LVM_FIRST + 167);
        public const int LVM_FINDITEM = (LVM_FIRST + 83);
        public const int LVM_GETHEADER = (LVM_FIRST + 31);
        public const int LVM_SETBKIMAGE = (LVM_FIRST + 138);
        public const int LVM_GETBKIMAGE = (LVM_FIRST + 139);
        public const int LVM_GETITEMSPACING = (LVM_FIRST + 51);
        public const int LVM_SETICONSPACING = (LVM_FIRST + 53);
        public const int LVM_GETSTRINGWIDTH = (LVM_FIRST + 87);
        public const int LVM_SCROLL = (LVM_FIRST + 20);
        public const int LVM_SETOUTLINECOLOR = (LVM_FIRST + 177);
        public const int LVM_GETOUTLINECOLOR = (LVM_FIRST + 176);

        public const int LV_MAX_WORKAREAS = 16;
        public const int LVM_GETNUMBEROFWORKAREAS = (LVM_FIRST + 73);
        public const int LVM_GETWORKAREAS = (LVM_FIRST + 70);
        public const int LVM_SETWORKAREAS = (LVM_FIRST + 65);

        public const int LVN_FIRST = -100;
        public const int LVN_ITEMCHANGING = (LVN_FIRST - 0);
        public const int LVN_ITEMCHANGED = (LVN_FIRST - 1);
        public const int LVN_COLUMNCLICK = (LVN_FIRST - 8);
        public const int LVN_ITEMACTIVATE = (LVN_FIRST - 14);
        public const int LVN_GETDISPINFOA = (LVN_FIRST - 55);
        public const int LVN_GETDISPINFOW = (LVN_FIRST - 77);
        public const int LVN_BEGINLABELEDIT = (LVN_FIRST - 75);
        public const int LVN_ENDLABELEDIT = (LVN_FIRST - 76);
        public const int LVN_BEGINDRAG = (LVN_FIRST - 9);
        public const int LVN_BEGINRDRAG = (LVN_FIRST - 11);
        public const int LVN_BEGINSCROLL = (LVN_FIRST - 80);
        public const int LVN_ENDSCROLL = (LVN_FIRST - 81);

        public const int NM_FIRST = 0;
        public const int NM_CLICK = (NM_FIRST - 2);
        public const int NM_DBLCLK = (NM_FIRST - 3);
        public const int NM_RCLICK = (NM_FIRST - 5);
        public const int NM_RDBLCLK = (NM_FIRST - 6);

        public const int HDM_FIRST = 0x1200;
        public const int HDM_GETITEM = (HDM_FIRST + 11);
        public const int HDM_SETITEM = (HDM_FIRST + 12);

        public const int HDN_FIRST = -300;
        public const int HDN_BEGINTRACK = (HDN_FIRST - 26);
        public const int HDN_ENDTRACK = (HDN_FIRST - 27);

        //
        // ListView constants
        //
        public const uint LVA_ALIGNLEFT = 0x001;

        public const uint LVA_ALIGNTOP = 0x002;
        public const uint LVA_DEFAULT = 0x000;
        public const uint LVA_SNAPTOGRID = 0x005;

        public const uint LVNI_FOCUSED = 0x0001;

        public const int LVIR_BOUNDS = 0x0;
        public const int LVIR_ICON = 0x1;
        public const int LVIR_LABEL = 0x2;
        public const int LVIR_SELECTBOUNDS = 0x3;

        public const uint LVIF_TEXT = 0x0001;
        public const uint LVIF_IMAGE = 0x0002;
        public const uint LVIF_PARAM = 0x0004;
        public const uint LVIF_STATE = 0x0008;
        public const uint LVIF_NORECOMPUTE = 0x0800;

        public const uint LVCF_FMT = 0x0001;
        public const uint LVCF_WIDTH = 0x0002;
        public const uint LVCF_TEXT = 0x0004;
        public const uint LVCF_SUBITEM = 0x0008;
        public const uint LVCF_IMAGE = 0x0010;
        public const uint LVCF_ORDER = 0x0020;

        public const int LVCFMT_LEFT = 0x0000;
        public const int LVCFMT_RIGHT = 0x0001;
        public const int LVCFMT_CENTER = 0x0002;
        public const int LVCFMT_IMAGE = 0x0800;
        public const int LVCFMT_BITMAP_ON_RIGHT = 0x1000;
        public const int LVCFMT_COL_HAS_IMAGES = 0x8000;

        public const uint LVIS_FOCUSED = 0x0001;
        public const uint LVIS_SELECTED = 0x0002;
        public const uint LVIS_CUT = 0x0004;
        public const uint LVIS_DROPHILITED = 0x0008;
        public const uint LVIS_GLOW = 0x0010;
        public const uint LVIS_ACTIVATING = 0x0020;
        public const uint LVIS_OVERLAYMASK = 0x0F00;
        public const uint LVIS_STATEIMAGEMASK = 0xF000;

        public const uint LVHT_ABOVE = 0x0008;
        public const uint LVHT_BELOW = 0x0010;
        public const uint LVHT_TORIGHT = 0x0020;
        public const uint LVHT_TOLEFT = 0x0040;

        public const uint LVSIL_NORMAL = 0;
        public const uint LVSIL_SMALL = 1;
        public const uint LVSIL_STATE = 2;

        public const uint LVSICF_NOINVALIDATEALL = 0x1;
        public const uint LVSICF_NOSCROLL = 0x2;

        public const uint LVKF_ALT = 0x0001;
        public const uint LVKF_CONTROL = 0x0002;
        public const uint LVKF_SHIFT = 0x0004;

        public const uint LVFI_NEARESTXY = 0x0040;

        public const uint LVIM_AFTER = 0x01;

        public const uint VK_NEXT = 0x022;
        public const uint VK_RIGHT = 0x027;

        public const uint HDI_WIDTH = 0x0001;
        public const uint HDI_HEIGHT = HDI_WIDTH;
        public const uint HDI_TEXT = 0x0002;
        public const uint HDI_FORMAT = 0x0004;
        public const uint HDI_LPARAM = 0x0008;
        public const uint HDI_BITMAP = 0x0010;
        public const uint HDI_IMAGE = 0x0020;
        public const uint HDI_DI_SETITEM = 0x0040;
        public const uint HDI_ORDER = 0x0080;
        public const uint HDI_FILTER = 0x0100;

        public const int HDF_LEFT = 0x0000;
        public const int HDF_RIGHT = 0x0001;
        public const int HDF_CENTER = 0x0002;
        public const int HDF_JUSTIFYMASK = 0x0003;
        public const int HDF_RTLREADING = 0x0004;
        public const int HDF_OWNERDRAW = 0x8000;
        public const int HDF_STRING = 0x4000;
        public const int HDF_BITMAP = 0x2000;
        public const int HDF_BITMAP_ON_RIGHT = 0x1000;
        public const int HDF_IMAGE = 0x0800;
        public const int HDF_SORTUP = 0x0400;
        public const int HDF_SORTDOWN = 0x0200;

        public const uint LVBKIF_SOURCE_NONE = 0x0000;
        public const uint LVBKIF_SOURCE_HBITMAP = 0x0001;
        public const uint LVBKIF_SOURCE_URL = 0x0002;
        public const uint LVBKIF_SOURCE_MASK = 0x0003;
        public const uint LVBKIF_STYLE_NORMAL = 0x0000;
        public const uint LVBKIF_STYLE_TILE = 0x0010;
        public const uint LVBKIF_STYLE_MASK = 0x0010;
        public const uint LVBKIF_FLAG_TILEOFFSET = 0x00100;
        public const uint LVBKIF_TYPE_WATERMARK = 0x00000;

        //
        // WindowStyles
        //
        public const int WS_OVERLAPPED = 0x00000000;

        public const int WS_POPUP = unchecked((int)(0x80000000));
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_GROUP = 0x00020000;
        public const int WS_TABSTOP = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_TILED = WS_OVERLAPPED;
        public const int WS_ICONIC = WS_MINIMIZE;
        public const int WS_SIZEBOX = WS_THICKFRAME;
        public const int WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;
        public const int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
        public const int WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU;
        public const int WS_CHILDWINDOW = WS_CHILD;
        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;
        public const int WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_NOINHERITLAYOUT = 0x00100000;
        public const int WS_EX_LAYOUTRTL = 0x00400000;
        public const int WS_EX_COMPOSITED = 0x02000000;
        public const int WS_EX_NOACTIVATE = 0x0800000;

        //
        // WindowLongFlags
        //
        public const int GWL_EXSTYLE = -20;

        public const int GWL_STYLE = -16;
        public const int GWLP_WNDPROC = -4;
        public const int GWLP_HINSTANCE = -6;
        public const int GWLP_HWNDPARENT = -8;
        public const int GWLP_ID = -12;
        public const int GWLP_USERDATA = -21;
        public const int DWLP_MSGRESULT = 0;

        //
        // WindowPosFlags
        //
        public const uint SWP_NOSIZE = 0x0001;

        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_HIDEWINDOW = 0x0080;
        public const uint SWP_NOCOPYBITS = 0x0100;
        public const uint SWP_NOOWNERZORDER = 0x0200;
        public const uint SWP_NOSENDCHANGING = 0x0400;
        public const uint SWP_DRAWFRAME = SWP_FRAMECHANGED;
        public const uint SWP_NOREPOSITION = SWP_NOOWNERZORDER;
        public const uint SWP_DEFERERASE = 0x2000;
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;

        //
        // ScrollBarConstants
        //
        public const uint SB_HORZ = 0;

        public const uint SB_VERT = 1;
        public const uint SB_CTL = 2;
        public const uint SB_BOTH = 3;

        //
        // ScrollBarArrows
        //
        public const uint ESB_DISABLE_BOTH = 0x0003;

        public const uint ESB_DISABLE_DOWN = 0x0002;
        public const uint ESB_DISABLE_LEFT = 0x0001;
        public const uint ESB_DISABLE_LTUP = ESB_DISABLE_LEFT;
        public const uint ESB_DISABLE_RIGHT = 0x0002;
        public const uint ESB_DISABLE_RTDN = ESB_DISABLE_RIGHT;
        public const uint ESB_DISABLE_UP = 0x0001;
        public const uint ESB_ENABLE_BOTH = 0x0000;

        //
        // ScrollInfoFlags
        //
        public const uint SIF_RANGE = 0x0001;

        public const uint SIF_PAGE = 0x0002;
        public const uint SIF_POS = 0x0004;
        public const uint SIF_DISABLENOSCROLL = 0x0008;
        public const uint SIF_TRACKPOS = 0x0010;
        public const uint SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

        //
        // WM_Mesages
        //
        public const uint WM_VSCROLL = 0x0115;

        public const uint WM_HSCROLL = 0x0114;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_CHAR = 0x0102;
        public const uint WM_TIMER = 0x0113;
        public const uint WM_SETTINGCHANGE = 0x001A;
        public const uint WM_MOUSEWHEEL = 0x020A;
        public const uint WM_CTLCOLORSCROLLBAR = 0x0137;
        public const uint WM_NCCREATE = 0x0081;
        public const uint WM_CREATE = 0x0001;
        public const uint WM_DESTROY = 0x0002;
        public const uint WM_SETFOCUS = 0x0007;
        public const uint WM_KILLFOCUS = 0x0008;
        public const uint WM_SETREDRAW = 0x000B;
        public const uint WM_PAINT = 0x000F;
        public const uint WM_ERASEBKGND = 0x0014;
        public const uint WM_SETCURSOR = 0x0020;
        public const uint WM_WINDOWPOSCHANGED = 0x0047;
        public const uint WM_NOTIFY = 0x004E;
        public const uint WM_NCHITTEST = 0x0084;
        public const uint WM_MOUSEHOVER = 0x02a1;
        public const uint WM_MOUSEFIRST = 0x0200;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_LBUTTONDBLCLK = 0x0203;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
        public const uint WM_RBUTTONDBLCLK = 0x0206;
        public const uint WM_MBUTTONDOWN = 0x0207;
        public const uint WM_MBUTTONUP = 0x0208;
        public const uint WM_MBUTTONDBLCLK = 0x0209;

        //
        // ScrollBarCommand
        //
        public const uint SB_LEFT = 6;

        public const uint SB_LINELEFT = 0;
        public const uint SB_LINERIGHT = 1;
        public const uint SB_PAGELEFT = 2;
        public const uint SB_PAGERIGHT = 3;
        public const uint SB_RIGHT = 7;

        public const uint SB_TOP = 6;
        public const uint SB_LINEUP = 0;
        public const uint SB_LINEDOWN = 1;
        public const uint SB_PAGEUP = 2;
        public const uint SB_PAGEDOWN = 3;
        public const uint SB_BOTTOM = 7;

        public const uint SB_THUMBPOSITION = 4;
        public const uint SB_THUMBTRACK = 5;
        public const uint SB_ENDSCROLL = 8;

        //
        // RedrawWindowFlags
        //
        public const uint RDW_INVALIDATE = 0x0001;

        public const uint RDW_INTERNALPAINT = 0x0002;
        public const uint RDW_ERASE = 0x0004;
        public const uint RDW_VALIDATE = 0x0008;
        public const uint RDW_NOINTERNALPAINT = 0x0010;
        public const uint RDW_NOERASE = 0x0020;
        public const uint RDW_NOCHILDREN = 0x0040;
        public const uint RDW_ALLCHILDREN = 0x0080;
        public const uint RDW_UPDATENOW = 0x0100;
        public const uint RDW_ERASENOW = 0x0200;
        public const uint RDW_FRAME = 0x0400;
        public const uint RDW_NOFRAME = 0x0800;

        //
        // BorderFlags
        //
        public const uint BF_LEFT = 0x0001;

        public const uint BF_TOP = 0x0002;
        public const uint BF_RIGHT = 0x0004;
        public const uint BF_BOTTOM = 0x0008;
        public const uint BF_TOPLEFT = BF_TOP | BF_LEFT;
        public const uint BF_TOPRIGHT = BF_TOP | BF_RIGHT;
        public const uint BF_BOTTOMLEFT = BF_BOTTOM | BF_LEFT;
        public const uint BF_BOTTOMRIGHT = BF_BOTTOM | BF_RIGHT;
        public const uint BF_RECT = BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM;
        public const uint BF_DIAGONAL = 0x0010;
        public const uint BF_DIAGONAL_ENDTOPRIGHT = BF_DIAGONAL | BF_TOP | BF_RIGHT;
        public const uint BF_DIAGONAL_ENDTOPLEFT = BF_DIAGONAL | BF_TOP | BF_LEFT;
        public const uint BF_DIAGONAL_ENDBOTTOMLEFT = BF_DIAGONAL | BF_BOTTOM | BF_LEFT;
        public const uint BF_DIAGONAL_ENDBOTTOMRIGHT = BF_DIAGONAL | BF_BOTTOM | BF_RIGHT;
        public const uint BF_MIDDLE = 0x0800;
        public const uint BF_SOFT = 0x1000;
        public const uint BF_ADJUST = 0x2000;
        public const uint BF_FLAT = 0x4000;
        public const uint BF_MONO = 0x8000;

        //
        // BorderStyleFlags
        //
        public const uint BDR_RAISEDOUTER = 0x0001;

        public const uint BDR_SUNKENOUTER = 0x0002;
        public const uint BDR_RAISEDINNER = 0x0004;
        public const uint BDR_SUNKENINNER = 0x0008;
        public const uint EDGE_BUMP = BDR_RAISEDOUTER | BDR_SUNKENINNER;
        public const uint EDGE_ETCHED = BDR_SUNKENOUTER | BDR_RAISEDINNER;
        public const uint EDGE_RAISED = BDR_RAISEDOUTER | BDR_RAISEDINNER;
        public const uint EDGE_SUNKEN = BDR_SUNKENOUTER | BDR_SUNKENINNER;

        //
        // DeviceCapsConstants
        //
        public const uint LOGPIXELSX = 88;

        public const uint LOGPIXELSY = 90;

        //
        // SystemMetricsConstants
        //
        public const uint SM_CXEDGE = 45;

        public const uint SM_CYEDGE = 46;
        public const uint SM_CXVSCROLL = 2;
        public const uint SM_CYVSCROLL = 3;

        //
        // ButtonStates
        //
        public const int MK_LBUTTON = 0x0001;

        public const int MK_RBUTTON = 0x0002;
        public const int MK_SHIFT = 0x0004;
        public const int MK_CONTROL = 0x0008;
        public const int MK_MBUTTON = 0x0010;

        //
        // ControlsClasses
        //
        public const uint ICC_LISTVIEW_CLASSES = 0x00000001;

        public const uint ICC_TREEVIEW_CLASSES = 0x00000002;
        public const uint ICC_BAR_CLASSES = 0x00000004;
        public const uint ICC_TAB_CLASSES = 0x00000008;
        public const uint ICC_UPDOWN_CLASS = 0x00000010;
        public const uint ICC_PROGRESS_CLASS = 0x00000020;
        public const uint ICC_HOTKEY_CLASS = 0x00000040;
        public const uint ICC_ANIMATE_CLASS = 0x00000080;
        public const uint ICC_WIN95_CLASSES = 0x000000FF;
        public const uint ICC_DATE_CLASSES = 0x00000100;
        public const uint ICC_USEREX_CLASSES = 0x00000200;
        public const uint ICC_COOL_CLASSES = 0x00000400;
        public const uint ICC_INTERNET_CLASSES = 0x00000800;
        public const uint ICC_PAGESCROLLER_CLASS = 0x00001000;
        public const uint ICC_NATIVEFNTCTL_CLASS = 0x00002000;
        public const uint ICC_STANDARD_CLASSES = 0x00004000;
        public const uint ICC_LINK_CLASS = 0x00008000;

        #region "Pidl enums"

        //
        // SFGAOF
        //
        public const uint SFGAO_CANCOPY = 0x1;          // Objects can be copied    (DROPEFFECT_COPY)

        public const uint SFGAO_CANMOVE = 0x2;          // Objects can be moved     (DROPEFFECT_MOVE)
        public const uint SFGAO_CANLINK = 0x4;          // Objects can be linked    (DROPEFFECT_LINK)
        public const uint SFGAO_STORAGE = 0x00000008;       // supports BindToObject(IID_IStorage)
        public const uint SFGAO_CANRENAME = 0x00000010;     // Objects can be renamed
        public const uint SFGAO_CANDELETE = 0x00000020;     // Objects can be deleted
        public const uint SFGAO_HASPROPSHEET = 0x00000040;      // Objects have property sheets
        public const uint SFGAO_DROPTARGET = 0x00000100;        // Objects are drop target
        public const uint SFGAO_CAPABILITYMASK = 0x00000177;
        public const uint SFGAO_ENCRYPTED = 0x00002000;     // object is encrypted (use alt color)
        public const uint SFGAO_ISSLOW = 0x00004000;        // 'slow' object
        public const uint SFGAO_GHOSTED = 0x00008000;       // ghosted icon
        public const uint SFGAO_LINK = 0x00010000;      // Shortcut (link)
        public const uint SFGAO_SHARE = 0x00020000;     // shared
        public const uint SFGAO_READONLY = 0x00040000;      // read-only
        public const uint SFGAO_HIDDEN = 0x00080000;        // hidden object
        public const uint SFGAO_DISPLAYATTRMASK = 0x000FC000;
        public const uint SFGAO_FILESYSANCESTOR = 0x10000000;       // may contain children with SFGAO_FILESYSTEM
        public const uint SFGAO_FOLDER = 0x20000000;        // support BindToObject(IID_IShellFolder)
        public const uint SFGAO_FILESYSTEM = 0x40000000;        // is a win32 file system object (file/folder/root)
        public const uint SFGAO_HASSUBFOLDER = 0x80000000;      // may contain children with SFGAO_FOLDER
        public const uint SFGAO_CONTENTSMASK = 0x80000000;
        public const uint SFGAO_VALIDATE = 0x01000000;      // invalidate cached information
        public const uint SFGAO_REMOVABLE = 0x02000000;     // is this removeable media?
        public const uint SFGAO_COMPRESSED = 0x04000000;        // Object is compressed (use alt color)
        public const uint SFGAO_BROWSABLE = 0x08000000;     // supports IShellFolder, but only implements CreateViewObject() (non-folder view)
        public const uint SFGAO_NONENUMERATED = 0x00100000;     // is a non-enumerated object
        public const uint SFGAO_NEWCONTENT = 0x00200000;        // should show bold in explorer tree
        public const uint SFGAO_CANMONIKER = 0x00400000;        // defunct
        public const uint SFGAO_HASSTORAGE = 0x00400000;        // defunct
        public const uint SFGAO_STREAM = 0x00400000;        // supports BindToObject(IID_IStream)
        public const uint SFGAO_STORAGEANCESTOR = 0x00800000;       // may contain children with SFGAO_STORAGE or SFGAO_STREAM
        public const uint SFGAO_STORAGECAPMASK = 0x70C50008;        // for determining storage capabilities, ie for open/save semantics

        //
        // SHGNO
        //
        public const uint SHGDN_NORMAL = 0x0000;        // default (display purpose)

        public const uint SHGDN_INFOLDER = 0x0001;      // displayed under a folder (relative)
        public const uint SHGDN_FOREDITING = 0x1000;        // for in-place editing
        public const uint SHGDN_FORADDRESSBAR = 0x4000;     // UI friendly parsing name (remove ugly stuff)
        public const uint SHGDN_FORPARSING = 0x8000;        // parsing name for ParseDisplayName()

        //
        // SHCONTF
        //
        public const uint SHCONTF_FOLDERS = 0x0020;     // only want folders enumerated (SFGAO_FOLDER)

        public const uint SHCONTF_NONFOLDERS = 0x0040;      // include non folders
        public const uint SHCONTF_INCLUDEHIDDEN = 0x0080;       // show items normally hidden
        public const uint SHCONTF_INIT_ON_FIRST_NEXT = 0x0100;      // allow EnumObject() to return before validating enum
        public const uint SHCONTF_NETPRINTERSRCH = 0x0200;      // hint that client is looking for printers
        public const uint SHCONTF_SHAREABLE = 0x0400;       // hint that client is looking sharable resources (remote shares)
        public const uint SHCONTF_STORAGE = 0x0800;     // include all items with accessible storage and their ancestors

        //
        // SHGFI
        //
        public const uint SHGFI_ICON = 0x000000100;     // get icon

        public const uint SHGFI_DISPLAYNAME = 0x000000200;      // get display name
        public const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        public const uint SHGFI_ATTRIBUTES = 0x000000800;       // get attributes
        public const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        public const uint SHGFI_EXETYPE = 0x000002000;      // return exe type
        public const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        public const uint SHGFI_LINKOVERLAY = 0x000008000;      // put a link overlay on icon
        public const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;       // get only specified attributes
        public const uint SHGFI_LARGEICON = 0x000000000;        // get large icon
        public const uint SHGFI_SMALLICON = 0x000000001;        // get small icon
        public const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        public const uint SHGFI_SHELLICONSIZE = 0x000000004;        // get shell size icon
        public const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;        // use passed dwFileAttribute
        public const uint SHGFI_ADDOVERLAYS = 0x000000020;      // apply the appropriate overlays
        public const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay

        //
        // FILE_ATTRIBUTE
        //
        public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;

        public const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        public const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        public const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        public const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        public const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        public const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        public const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        public const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        #endregion "Pidl enums"

        internal sealed class WindowPosHandleValues
        {
            private WindowPosHandleValues()
            {
                throw new NotImplementedException();
            }

            internal static readonly System.IntPtr HWND_TOP = new System.IntPtr(0);
            internal static readonly System.IntPtr HWND_BOTTOM = new System.IntPtr(1);
            internal static readonly System.IntPtr HWND_TOPMOST = new System.IntPtr(-1);
            internal static readonly System.IntPtr HWND_NOTOPMOST = new System.IntPtr(-2);
        }

        internal class NMHDR
        {
            private NMHDRx64 hdrx64;
            private NMHDRx86 hdrx86;

            public IntPtr hwndFrom
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return hdrx64.hwndFrom;
                    else
                        return hdrx86.hwndFrom;
                }
            }

            public uint idFrom
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return hdrx64.idFrom;
                    else
                        return hdrx86.idFrom;
                }
            }

            public uint code
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return hdrx64.code;
                    else
                        return hdrx86.code;
                }
            }

            public NMHDR(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    hdrx64 = (NativeMethods.NMHDRx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMHDRx64));
                else
                    hdrx86 = (NativeMethods.NMHDRx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMHDRx86));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct NMHDRx64
        {
            [FieldOffset(0)] public IntPtr hwndFrom;
            [FieldOffset(8)] public uint idFrom;
            [FieldOffset(16)] public uint code;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct NMHDRx86
        {
            [FieldOffset(0)]
            public IntPtr hwndFrom;

            [FieldOffset(4)]
            public uint idFrom;

            [FieldOffset(8)]
            public uint code;
        }

        internal class NMHEADER
        {
            private NMHEADERx86 headerx86;
            private NMHEADERx64 headerx64;

            public int iItem
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return headerx64.iItem;
                    else
                        return headerx86.iItem;
                }
            }

            public int iButton
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return headerx64.iButton;
                    else
                        return headerx86.iButton;
                }
            }

            public HDITEM hItem
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return headerx64.hItem;
                    else
                        return headerx86.hItem;
                }
            }

            public NMHEADER(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    headerx64 = (NativeMethods.NMHEADERx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMHEADERx64));
                else
                    headerx86 = (NativeMethods.NMHEADERx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMHEADERx86));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMHEADERx86
        {
            public NMHDRx86 hdr;
            public int iItem;
            public int iButton;
            public HDITEM hItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMHEADERx64
        {
            public NMHDRx64 hdr;
            public int iItem;
            public int iButton;
            public HDITEM hItem;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct HDITEM
        {
            public uint mask;
            public int cxy;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public IntPtr lParam;
            public int iImage;        // index of bitmap in ImageList
            public int iOrder;
            public uint type;           // [in] filter type (defined what pvFilter is a pointer to)
            public IntPtr pvFilter;       // [in] fillter data see above
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LVBKIMAGE
        {
            public uint ulFlags;
            public IntPtr hbm;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszImage;
            public uint cchImageMax;
            public int xOffsetPercent;
            public int yOffsetPercent;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVITEMW
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public uint state;
            public uint stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
        }

        internal class LVDISPINFOW
        {
            private LVDISPINFOWx86 dispinfox86;
            private LVDISPINFOWx64 dispinfox64;

            public LVITEMW item
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return dispinfox64.item;
                    else
                        return dispinfox86.item;
                }
                set
                {
                    if (IntPtr.Size == 8)
                        dispinfox64.item = value;
                    else
                        dispinfox86.item = value;
                }
            }

            public LVDISPINFOW(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    dispinfox64 = (NativeMethods.LVDISPINFOWx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.LVDISPINFOWx64));
                else
                    dispinfox86 = (NativeMethods.LVDISPINFOWx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.LVDISPINFOWx86));
            }

            public void StructToPointer(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    Marshal.StructureToPtr(dispinfox64, ptr, false);
                else
                    Marshal.StructureToPtr(dispinfox86, ptr, false);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVDISPINFOWx86
        {
            public NMHDRx86 hdr;
            public LVITEMW item;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVDISPINFOWx64
        {
            public NMHDRx64 hdr;
            public LVITEMW item;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LVCOLUMNW
        {
            public uint mask;
            public int fmt;
            public int cx;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszText;
            public int cchTextMax;
            public int iSubItem;
            public int iImage;
            public int iOrder;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVINSERTMARK
        {
            public uint cbSize;
            public uint dwFlags;
            public int iItem;
            public uint dwReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVHITTESTINFO
        {
            public POINT pt;
            public uint flags;
            public int iItem;
            public int iSubItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVFINDINFO
        {
            public uint flags;
            public IntPtr psz;
            public uint lParam;
            public POINT pt;
            public uint vkDirection;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCROLLINFO
        {
            internal uint cbSize;
            internal uint fMask;
            internal int nMin;
            internal int nMax;
            internal int nPage;
            internal int nPos;
            internal int nTrackPos;

            internal SCROLLINFO(uint fMask, int nMin, int nMax, int nPage, int nPos)
            {
                this.fMask = fMask;
                this.nMin = nMin;
                this.nMax = nMax;
                this.nPage = nPage;
                this.nPos = nPos;
                this.nTrackPos = 0;
                this.cbSize = 0;
                SetSizeField();
            }

            internal void SetSizeField()
            {
                this.cbSize = (uint)Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        internal class NMITEMACTIVATE
        {
            private NMITEMACTIVATEx86 itemActivatex86;
            private NMITEMACTIVATEx64 itemActivatex64;

            public NMITEMACTIVATE(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    itemActivatex64 = (NativeMethods.NMITEMACTIVATEx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMITEMACTIVATEx64));
                else
                    itemActivatex86 = (NativeMethods.NMITEMACTIVATEx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMITEMACTIVATEx86));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMITEMACTIVATEx86
        {
            public NMHDRx86 hdr;
            public int iItem;
            public int iSubItem;
            public uint uNewState;
            public uint uOldState;
            public uint uChanged;
            public POINT ptAction;
            public IntPtr lParam;
            public int iIndent;
            public uint uKeyFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMITEMACTIVATEx64
        {
            public NMHDRx64 hdr;
            public int iItem;
            public int iSubItem;
            public uint uNewState;
            public uint uOldState;
            public uint uChanged;
            public POINT ptAction;
            public IntPtr lParam;
            public int iIndent;
            public uint uKeyFlags;
        }

        internal class NMLISTVIEW
        {
            private NMLISTVIEWx86 listViewx86;
            private NMLISTVIEWx64 listViewx64;

            public NMLISTVIEW(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    listViewx64 = (NativeMethods.NMLISTVIEWx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMLISTVIEWx64));
                else
                    listViewx86 = (NativeMethods.NMLISTVIEWx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMLISTVIEWx86));
            }

            public int iItem
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.iItem;
                    else
                        return listViewx86.iItem;
                }
            }

            public int iSubItem
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.iSubItem;
                    else
                        return listViewx86.iSubItem;
                }
            }

            public uint uNewState
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.uNewState;
                    else
                        return listViewx86.uNewState;
                }
            }

            public uint uOldState
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.uOldState;
                    else
                        return listViewx86.uOldState;
                }
            }

            public uint uChanged
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.uChanged;
                    else
                        return listViewx86.uChanged;
                }
            }

            public POINT ptAction
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.ptAction;
                    else
                        return listViewx86.ptAction;
                }
            }

            public IntPtr lParam
            {
                get
                {
                    if (IntPtr.Size == 8)
                        return listViewx64.lParam;
                    else
                        return listViewx86.lParam;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMLISTVIEWx86
        {
            public NMHDRx86 hdr;
            public int iItem;
            public int iSubItem;
            public uint uNewState;
            public uint uOldState;
            public uint uChanged;
            public POINT ptAction;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMLISTVIEWx64
        {
            public NMHDRx64 hdr;
            public int iItem;
            public int iSubItem;
            public uint uNewState;
            public uint uOldState;
            public uint uChanged;
            public POINT ptAction;
            public IntPtr lParam;
        }

        internal class NMLVSCROLL
        {
            private NMLVSCROLLx64 scrollx64;
            private NMLVSCROLLx86 scrollx86;

            public NMLVSCROLL(IntPtr ptr)
            {
                if (IntPtr.Size == 8)
                    scrollx64 = (NativeMethods.NMLVSCROLLx64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMLVSCROLLx64));
                else
                    scrollx86 = (NativeMethods.NMLVSCROLLx86)Marshal.PtrToStructure(ptr, typeof(NativeMethods.NMLVSCROLLx86));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMLVSCROLLx64
        {
            public NMHDRx64 hdr;
            public int dx;
            public int dy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NMLVSCROLLx86
        {
            public NMHDRx86 hdr;
            public int dx;
            public int dy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CREATESTRUCT : IDisposable
        {
            internal System.IntPtr lpCreateParams;
            internal System.IntPtr hInstance;
            internal System.IntPtr hMenu;
            internal System.IntPtr hwndParent;
            internal int cy;
            internal int cx;
            internal int y;
            internal int x;
            internal int style;
            internal System.IntPtr lpszName;
            internal System.IntPtr lpszClass;
            internal uint dwExStyle;

            public void Dispose()
            {
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct INITCOMMONCONTROLSEX
        {
            internal uint dwSize;
            internal uint dwICC;

            internal INITCOMMONCONTROLSEX(uint controls)
            {
                dwSize = 0;
                dwICC = 0;

                if (controls != 0)
                    dwICC = controls;
                else
                    dwICC = NativeMethods.ICC_PAGESCROLLER_CLASS | NativeMethods.ICC_STANDARD_CLASSES;

                SetSizeField();
            }

            internal void SetSizeField()
            {
                this.dwSize = (uint)Marshal.SizeOf(this);
            }
        }

        #region "Icons processing"

        [ComVisible(false)]
        internal struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        #endregion "Icons processing"

        #region "Pidl Structures"

        [StructLayout(LayoutKind.Explicit)]
        internal struct STRRET
        {
            [FieldOffset(0)] public UInt32 uType;              // One of the STRRET_* values
            [FieldOffset(4)] public IntPtr pOleStr;            // must be freed by caller of GetDisplayNameOf
            [FieldOffset(4)] public IntPtr pStr;               // NOT USED
            [FieldOffset(4)] public UInt32 uOffset;            // Offset into SHITEMID
            [FieldOffset(4)] public IntPtr cStr;               // Buffer to fill in (ANSI)
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SHFILEINFO
        {
            public SHFILEINFO(bool dummy)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = new string('\0', 260);
                szTypeName = new string('\0', 80);
            }

            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        #endregion	"Pidl Structures"

        #region "Pidl interfaces"

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F2-0000-0000-C000-000000000046")]
        internal interface IEnumIDList
        {
            [PreserveSig()]
            uint Next(
                uint celt,
                out IntPtr rgelt,
                out IntPtr pceltFetched);

            [PreserveSig()]
            uint Skip(uint celt);

            [PreserveSig()]
            uint Reset();

            [PreserveSig()]
            uint Clone(out IEnumIDList ppenum);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        internal interface IShellFolder
        {
            [PreserveSig()]
            uint ParseDisplayName(
                IntPtr hwnd,
                IntPtr pbc,
                [In(), MarshalAs(UnmanagedType.LPWStr)]
                string pszDisplayName,
                out uint pchEaten,
                out IntPtr ppidl,
                ref uint pdwAttributes);

            [PreserveSig()]
            uint EnumObjects(
                IntPtr hwnd,
                uint grfFlags,
                out IEnumIDList ppenumIDList);

            [PreserveSig()]
            uint BindToObject(
                IntPtr pidl,
                IntPtr pbc,
                [In()]
                ref Guid riid,
                out IntPtr ppv);

            [PreserveSig()]
            uint BindToStorage(
                IntPtr pidl,
                IntPtr pbc,
                [In()]
                ref Guid riid,
                [MarshalAs(UnmanagedType.Interface)]
                out object ppv);

            [PreserveSig()]
            int CompareIDs(
                int lParam,
                IntPtr pidl1,
                IntPtr pidl2);

            [PreserveSig()]
            uint CreateViewObject(
                IntPtr hwndOwner,
                [In()]
                ref Guid riid,
                [MarshalAs(UnmanagedType.Interface)]
                out object ppv);

            [PreserveSig()]
            uint GetAttributesOf(
                int cidl,
                [In(), MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
                [MarshalAs(UnmanagedType.LPArray)] uint[] rgfInOut);

            [PreserveSig()]
            uint GetUIObjectOf(
                IntPtr hwndOwner,
                int cidl,
                [In(), MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
                [In()]
                ref Guid riid,
                IntPtr rgfReserved,
                [MarshalAs(UnmanagedType.Interface)]
                out object ppv);

            [PreserveSig()]
            uint GetDisplayNameOf(
                IntPtr pidl,
                uint uFlags,
                out NativeMethods.STRRET pName);

            [PreserveSig()]
            uint SetNameOf(
                IntPtr hwnd,
                IntPtr pidl,
                [In(), MarshalAs(UnmanagedType.LPWStr)]
                string pszName,
                uint uFlags,
                out IntPtr ppidlOut);
        }

        #endregion "Pidl interfaces"

        #region "SendMessage overloads"

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.Dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, uint lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [In, Out] ref RECT lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, IntPtr lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, ref int lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVCOLUMNW lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref POINT lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVITEMW lvitem);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVINSERTMARK lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, ref NativeMethods.POINT wParam, ref LVINSERTMARK lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVHITTESTINFO lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVFINDINFO lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVBKIMAGE lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, [In, Out] [MarshalAs(UnmanagedType.LPArray)] RECT[] lParam);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, [In, Out] [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("User32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SendMessage(IntPtr hWnd, int Msg, int wParam, ref HDITEM lParam);

        internal delegate int ListViewSortHandler(int index0, int index1, int dummy);

        [DllImport("User32.Dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ListViewSortHandler lParam);

        #endregion "SendMessage overloads"

        [DllImport("comctl32.dll", EntryPoint = "ImageList_GetIcon")]
        internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern int GetWindowLong32(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        internal static extern int SetWindowLongPtr(System.IntPtr hWnd, int nIndex, System.IntPtr newLong);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetWindowLong(System.IntPtr hWnd, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetWindowLong(HandleRef hWnd, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int flags);

        [DllImport("user32.dll")]
        internal static extern int SetScrollInfo(System.IntPtr hWnd, uint fnBar, ref SCROLLINFO si, [MarshalAs(UnmanagedType.Bool)]bool fRedraw);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetScrollInfo(System.IntPtr hWnd, uint fnBar, ref SCROLLINFO si);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnableScrollBar(System.IntPtr hWnd, uint fnBar, uint fnArrows);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowScrollBar(System.IntPtr hWnd, uint fnBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RedrawWindow(System.IntPtr hWnd, System.IntPtr lprcUpdate, System.IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetScrollRange(System.IntPtr hWnd, uint fnBar, int nMinPos, int nMaxPos, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetScrollPos(System.IntPtr hWnd, uint fnBar, int position);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DrawEdge(System.IntPtr hWnd, ref RECT qrc, uint edge, uint grfFlags);

        [DllImport("user32.dll", EntryPoint = "DrawEdge")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DrawEdge(IntPtr hdc, ref RECT qrc, int edge, int grfFlags);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(uint nIndex);

        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(System.IntPtr hDc, uint nIndex);

        [DllImport("user32.dll")]
        internal static extern System.IntPtr GetDC(System.IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InvalidateRect(System.IntPtr hWnd, ref RECT lpRect, [MarshalAs(UnmanagedType.Bool)]bool bErase);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitCommonControlsEx(ref INITCOMMONCONTROLSEX pStruct);

        [DllImport("ole32.dll")]
        internal static extern IntPtr CoTaskMemAlloc(uint size);

        [DllImport("ole32.dll")]
        internal static extern void CoTaskMemFree(IntPtr ptr);

        [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
        internal static extern int CopyMemory(IntPtr dest, IntPtr src, int count);

        #region "Pidl imported functions"

        [DllImport("Shell32.DLL")]
        internal static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, out IntPtr ppidl);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)]StringBuilder name, IntPtr pbc, out IntPtr pidlResult, uint flagsIn, out uint flagsOut);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Path);

        [DllImport("Shell32.dll")]
        internal static extern int SHGetDesktopFolder(out IShellFolder ppshf);

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SHGetFileInfo(IntPtr pidl, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, uint uFlags);

        #endregion "Pidl imported functions"

        [DllImport("Gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("Gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteDC(IntPtr hDC);

        #region	"Icons processing"

        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        internal static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        #endregion    //"Icons processing"
    }
}