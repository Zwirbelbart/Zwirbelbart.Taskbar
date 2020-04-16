using System;

namespace Zwirbelbart.Taskbar.Util {
	static class WindowsVersionUtil {
		private static readonly Version Win10InsiderPreview14328 = new Version(10, 0, 14328);
		private static readonly Version Win10InsiderPreview14971 = new Version(10, 0, 14971);

		public static bool IsWin10Build14328OrNewer() {
			return Environment.OSVersion.Version >= Win10InsiderPreview14328;
		}

		public static bool IsWin10Build14971OrNewer() {
			return Environment.OSVersion.Version >= Win10InsiderPreview14971;
		}
	}
}
