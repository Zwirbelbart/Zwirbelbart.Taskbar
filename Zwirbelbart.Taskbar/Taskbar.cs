﻿using System;
using System.Drawing;
using Zwirbelbart.Taskbar.Core;
using Zwirbelbart.Taskbar.Util;

namespace Zwirbelbart.Taskbar {
	public class Taskbar {
		private const string RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

		private const string TaskbarClassName = "Shell_TrayWnd";

		private const string IsLockedKey = "TaskbarSizeMove";
		private const string UseSmallIconsKey = "TaskbarSmallIcons";
		private const string ItemAppearanceKey = "TaskbarGlomLevel";
		private const string RecentJumplistItemCountKey = "Start_JumpListItems";
		private const string UsePeekPreviewKey = "DisablePreviewDesktop";
		private const string UsePowerShellKey = "DontUsePowerShellOnWinX";
		private const string ShowTaskViewButtonKey = "ShowTaskViewButton";
		private const string AutoHideInTabletModeKey = "TaskbarAutoHideInTabletMode";
		private const string ShowBadgesKey = "TaskbarBadges";

		private const string ShowOnAllDisplaysKey = "MMTaskbarEnabled";
		private const string OtherTaskbarsMode = "MMTaskbarMode";
		private const string OtherTaskbarsItemAppearanceKey = "MMTaskbarGlomLevel";

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

		public static bool IsUsePowerShellSupported {
			get { return WindowsVersionUtil.IsWin10Build14971OrNewer(); }
		}

		public static bool UsePowerShell {
			get { return InternalGetUsePowerShell(); }
			set { InternalSetUsePowerShell(value); }
		}
		public static bool ShowTaskViewButton {
			get { return InternalGetShowTaskViewButton(); }
			set { InternalSetShowTaskViewButton(value); }
		}

		//Available with build 14328
		//https://www.tenforums.com/tutorials/48127-turn-off-auto-hide-taskbar-tablet-mode-windows-10-a.html
		public static bool IsAutoHideInTabletModeSupported {
			get { return WindowsVersionUtil.IsWin10Build14328OrNewer(); }
		}

		public static bool AutoHideInTabletMode {
			get { return InternalGetAutoHideInTabletMode(); }
			set { InternalSetAutoHideInTabletMode(value); }
		}
		public static bool ShowBadges {
			get { return InternalGetShowBadges(); }
			set { InternalSetShowBadges(value); }
		}
		public static bool ShowOnAllDisplays {
			get { return InternalGetShowOnAllDisplays(); }
			set { InternalSetShowOnAllDisplays(value); }
		}
		public static MultiMonitorTaskbarItemVisibility MultiMonitorItemVisibility {
			get { return InternalGetMultiMonitorItemVisibility(); }
			set { InternalSetMultiMonitorItemVisibility(value); }
		}
		public static TaskbarItemAppearance OtherTaskbarsItemAppearance {
			get { return InternalGetOtherTaskbarsItemAppearance(); }
			set { InternalSetOtherTaskbarsItemAppearance(value); }
		}

		//implementation
		private static bool InternalGetIsLocked() {
			return (int)RegistryUtil.GetValue(RegistryPath, IsLockedKey) == 0;
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
				lParam = hide ? NativeMethods.ABS.Autohide : NativeMethods.ABS.AlwaysOnTop
			};

			NativeMethods.SHAppBarMessage(NativeMethods.ABM.SetState, ref data);
			UpdateTaskbar();
		}

		private static bool InternalGetUseSmallIcons() {
			return (int)RegistryUtil.GetValue(RegistryPath, UseSmallIconsKey) != 0;
		}

		private static void InternalSetUseSmallIcons(bool useSmallIcons) {
			RegistryUtil.SetValue(RegistryPath, UseSmallIconsKey, useSmallIcons ? 1 : 0);
			UpdateTaskbar();
		}

		private static TaskbarItemAppearance InternalGetTaskbarItemAppearance() {
			return (TaskbarItemAppearance)((int)RegistryUtil.GetValue(RegistryPath, ItemAppearanceKey));
		}

		private static void InternalSetTaskbarItemAppearance(TaskbarItemAppearance appearance) {
			RegistryUtil.SetValue(RegistryPath, ItemAppearanceKey, (int)appearance);
			UpdateTaskbar();
		}

		private static int InternalGetRecentJumplistItemCount() {
			return (int)RegistryUtil.GetValue(RegistryPath, RecentJumplistItemCountKey, 0);
		}

		private static void InternalSetRecentJumplistItemCount(int value) {
			RegistryUtil.SetValue(RegistryPath, RecentJumplistItemCountKey, value);
		}

		private static Rectangle InternalGetBounds() {
			if (!NativeMethods.GetWindowRect(Handle, out var rect))
				throw new Exception("GetWindowRect failed");

			return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		private static TaskbarLocation InternalGetLocation() {
			var data = new NativeMethods.APPBARDATA();
			NativeMethods.SHAppBarMessage(NativeMethods.ABM.GetTaskbarPos, ref data);

			return (TaskbarLocation)data.uEdge;
		}

		private static bool InternalGetUsePeekPreview() {
			return (int)RegistryUtil.GetValue(RegistryPath, UsePeekPreviewKey) == 0;
		}

		private static void InternalSetUsePeekPreview(bool usePeekPreview) {
			RegistryUtil.SetValue(RegistryPath, UsePeekPreviewKey, usePeekPreview ? 0 : 1);
			UpdateTaskbar();
		}

		private static bool InternalGetUsePowerShell() {
			if (IsUsePowerShellSupported)
				return (int)RegistryUtil.GetValue(RegistryPath, UsePowerShellKey) == 0;

			return false;
		}

		private static void InternalSetUsePowerShell(bool usePowerShell) {
			if (IsUsePowerShellSupported)
				RegistryUtil.SetValue(RegistryPath, UsePowerShellKey, usePowerShell ? 0 : 1);

			//UpdateTaskbar();
			//todo: this update method does not seem to work on this
		}
		private static bool InternalGetShowTaskViewButton() {
			//todo: update does not work here, is it the right key?
			return (int)RegistryUtil.GetValue(RegistryPath, ShowTaskViewButtonKey) == 1;
		}

		private static void InternalSetShowTaskViewButton(bool showTaskViewButton) {
			RegistryUtil.SetValue(RegistryPath, ShowTaskViewButtonKey, showTaskViewButton ? 1 : 0);
			UpdateTaskbar();
		}
		private static bool InternalGetAutoHideInTabletMode() {
			if (IsAutoHideInTabletModeSupported)
				return (int)RegistryUtil.GetValue(RegistryPath, AutoHideInTabletModeKey) == 1;

			return false;
		}

		private static void InternalSetAutoHideInTabletMode(bool hide) {
			//todo: is update needed here?
			if (IsAutoHideInTabletModeSupported)
				RegistryUtil.SetValue(RegistryPath, AutoHideInTabletModeKey, hide ? 1 : 0);
		}
		private static bool InternalGetShowBadges() {
			return (int)RegistryUtil.GetValue(RegistryPath, ShowBadgesKey) == 1;
		}

		private static void InternalSetShowBadges(bool value) {
			RegistryUtil.SetValue(RegistryPath, ShowBadgesKey, value ? 1 : 0);
			UpdateTaskbar();
		}

		private static bool InternalGetShowOnAllDisplays() {
			return (int)RegistryUtil.GetValue(RegistryPath, ShowOnAllDisplaysKey) == 1;
		}

		private static void InternalSetShowOnAllDisplays(bool value) {
			RegistryUtil.SetValue(RegistryPath, ShowOnAllDisplaysKey, value ? 1 : 0);
			UpdateTaskbar();
		}
		private static MultiMonitorTaskbarItemVisibility InternalGetMultiMonitorItemVisibility() {
			return (MultiMonitorTaskbarItemVisibility)((int)RegistryUtil.GetValue(RegistryPath, OtherTaskbarsMode));
		}

		private static void InternalSetMultiMonitorItemVisibility(MultiMonitorTaskbarItemVisibility value) {
			RegistryUtil.SetValue(RegistryPath, OtherTaskbarsMode, (int)value);
			UpdateTaskbar();
		}
		private static TaskbarItemAppearance InternalGetOtherTaskbarsItemAppearance() {
			return (TaskbarItemAppearance)((int)RegistryUtil.GetValue(RegistryPath, OtherTaskbarsItemAppearanceKey));
		}
		private static void InternalSetOtherTaskbarsItemAppearance(TaskbarItemAppearance value) {
			RegistryUtil.SetValue(RegistryPath, OtherTaskbarsItemAppearanceKey, (int)value);
			UpdateTaskbar();
		}

		private static void UpdateTaskbar() {
			NativeMethods.SendMessage(Handle, NativeMethods.WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
