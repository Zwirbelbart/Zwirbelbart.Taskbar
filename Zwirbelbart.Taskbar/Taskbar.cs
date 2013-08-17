using System;
using System.Drawing;
using Zwirbelbart.Taskbar.Core;
using Zwirbelbart.Taskbar.Util;

namespace Zwirbelbart.Taskbar {
    public class Taskbar {
        private const string RegistryPath =
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

        private const string TaskbarClassName = "Shell_TrayWnd";
        private const string IsLockedKey = "TaskbarSizeMove";
        private const string UseSmallIconsKey = "TaskbarSmallIcons";
        private const string ItemAppearanceKey = "TaskbarGlomLevel";
        private const string RecentJumplistItemCountKey = "Start_JumpListItems";
        private const string UsePeekPreviewKey = "DisablePreviewDesktop";

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

        public static TaskbarItemAppearance ItemAppearance {
            get { return InternalGetTaskbarItemAppearance(); }
            set { InternalSetTaskbarItemAppearance(value); }
        }

        public static int RecentJumplistItemCount {
            get { return InternalGetRecentJumplistItemCount(); }
            set { InternalSetRecentJumplistItemCount(value); }
        }

        public static Rectangle Bounds {
            get { return InternalGetBounds(); }
        }

        public static IntPtr Handle {
            get { return NativeMethods.FindWindow(TaskbarClassName, null); }
        }

        public static TaskbarLocation Location {
            get { return InternalGetLocation(); }
        }

        public static bool UsePeekPreview {
            get { return InternalGetUsePeekPreview(); }
            set { InternalSetUsePeekPreview(value); }
        }

        //implementation
        private static bool InternalGetIsLocked() {
            return (int) RegistryUtil.GetValue(RegistryPath, IsLockedKey) == 0;
        }

        private static void InternalSetIsLocked(bool isLocked) {
            RegistryUtil.SetValue(RegistryPath, IsLockedKey, isLocked ? 0 : 1);
            UpdateTaskbar();
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

            UpdateTaskbar();
        }

        private static bool InternalGetUseSmallIcons() {
            return (int) RegistryUtil.GetValue(RegistryPath, UseSmallIconsKey) != 0;
        }

        private static void InternalSetUseSmallIcons(bool useSmallIcons) {
            RegistryUtil.SetValue(RegistryPath, UseSmallIconsKey, useSmallIcons ? 1 : 0);
            UpdateTaskbar();
        }

        private static TaskbarItemAppearance InternalGetTaskbarItemAppearance() {
            return (TaskbarItemAppearance) ((int) RegistryUtil.GetValue(RegistryPath, ItemAppearanceKey));
        }

        private static void InternalSetTaskbarItemAppearance(TaskbarItemAppearance appearance) {
            RegistryUtil.SetValue(RegistryPath, ItemAppearanceKey, (int) appearance);

            UpdateTaskbar();
        }

        private static int InternalGetRecentJumplistItemCount() {
            return (int) RegistryUtil.GetValue(RegistryPath, RecentJumplistItemCountKey);
        }

        private static void InternalSetRecentJumplistItemCount(int value) {
            RegistryUtil.SetValue(RegistryPath, RecentJumplistItemCountKey, value);
        }

        private static Rectangle InternalGetBounds() {
            NativeMethods.RECT rect;

            if (!NativeMethods.GetWindowRect(Handle, out rect))
                throw new Exception("GetWindowRect failed");

            return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        private static TaskbarLocation InternalGetLocation() {
            var data = new NativeMethods.APPBARDATA();
            NativeMethods.SHAppBarMessage(NativeMethods.ABM.GetTaskbarPos, ref data);

            return (TaskbarLocation) data.uEdge;
        }

        private static bool InternalGetUsePeekPreview() {
            return (int)RegistryUtil.GetValue(RegistryPath, UsePeekPreviewKey) == 0;
        }

        private static void InternalSetUsePeekPreview(bool usePeekPreview) {
            RegistryUtil.SetValue(RegistryPath, UsePeekPreviewKey, usePeekPreview ? 0 : 1);

            UpdateTaskbar();
        }

        private static void UpdateTaskbar() {
            NativeMethods.SendMessage(Handle, NativeMethods.WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}