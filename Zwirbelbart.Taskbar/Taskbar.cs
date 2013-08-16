using System;
using System.Drawing;
using Zwirbelbart.Taskbar.Core;
using Zwirbelbart.Taskbar.Util;

namespace Zwirbelbart.Taskbar {
    public class Taskbar {
        private const string RegistryPath =
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

        public static bool IsLocked {
            get { return InternalGetIsLocked(); }
            set { InternalSetIsLocked(value); }
        }

        public static bool AutoHide {
            get { return InternalGetAutoHide(); }
            set { InternalSetAutoHide(value); }
        }

        public static bool UseSmallIcons {
            get { return InternalGetUseSmallIcons(); }
            set { InternalSetUseSmallIcons(value); }
        }

        public static TaskbarButtonAppearance ButtonAppearance {
            get { return InternalGetTaskbarButtonAppearance(); }
            set { InternalSetTaskbarButtonAppearance(value); }
        }

        public static int RecentJumplistItemCount {
            get { return InternalGetRecentJumplistItemCount(); }
            set { InternalSetRecentJumplistItemCount(value); }
        }

        public static Rectangle Rectangle {
            get { return InternalGetRectangle(); }
        }

        public static IntPtr Handle {
            get { return NativeMethods.FindWindow("Shell_TrayWnd", null); }
        }

        public static TaskbarLocation Location {
            get { return InternalGetLocation(); }
        }

        //implementation
        private static bool InternalGetIsLocked() {
            return (int) RegistryUtil.GetValue(RegistryPath, "TaskbarSizeMove") == 0;
        }

        private static void InternalSetIsLocked(bool isLocked) {
            RegistryUtil.SetValue(RegistryPath, "TaskbarSizeMove", isLocked ? 0 : 1);
        }

        private static bool InternalGetAutoHide() {
            var data = new NativeMethods.APPBARDATA();
            var result = NativeMethods.SHAppBarMessage(NativeMethods.ABM.GetState, ref data);

            return (result.ToInt32() & NativeMethods.ABS.Autohide) == NativeMethods.ABS.Autohide;
        }

        private static void InternalSetAutoHide(bool hide) {
            var data = new NativeMethods.APPBARDATA {
                                                        lParam =
                                                            hide
                                                                ? NativeMethods.ABS.Autohide
                                                                : NativeMethods.ABS.AlwaysOnTop
                                                    };
            NativeMethods.SHAppBarMessage(NativeMethods.ABM.SetState, ref data);
        }

        private static bool InternalGetUseSmallIcons() {
            return (int) RegistryUtil.GetValue(RegistryPath, "TaskbarSmallIcons") != 0;
        }

        private static void InternalSetUseSmallIcons(bool useSmallIcons) {
            RegistryUtil.SetValue(RegistryPath, "TaskbarSmallIcons", useSmallIcons ? 1 : 0);
        }

        private static TaskbarButtonAppearance InternalGetTaskbarButtonAppearance() {
            return (TaskbarButtonAppearance) ((int) RegistryUtil.GetValue(RegistryPath, "TaskbarGlomLevel"));
        }

        private static void InternalSetTaskbarButtonAppearance(TaskbarButtonAppearance appearance) {
            RegistryUtil.SetValue(RegistryPath, "TaskbarGlomLevel", (int) appearance);
        }

        private static int InternalGetRecentJumplistItemCount() {
            return (int) RegistryUtil.GetValue(RegistryPath, "Start_JumpListItems");
        }

        private static void InternalSetRecentJumplistItemCount(int value) {
            RegistryUtil.SetValue(RegistryPath, "Start_JumpListItems", value);
        }

        private static Rectangle InternalGetRectangle() {
            NativeMethods.RECT rect;

            if (!NativeMethods.GetWindowRect(Handle, out rect))
                throw new Exception("GetWindowRect failed");

            return new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        private static TaskbarLocation InternalGetLocation() {
            var data = new NativeMethods.APPBARDATA();
            NativeMethods.SHAppBarMessage(NativeMethods.ABM.GetTaskbarPos, ref data);

            return (TaskbarLocation) data.uEdge;
        }
    }
}