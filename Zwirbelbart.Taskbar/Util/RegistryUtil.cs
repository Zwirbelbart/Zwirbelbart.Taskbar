using System;
using Microsoft.Win32;

namespace Zwirbelbart.Taskbar.Util {
	internal class RegistryUtil {
		public static object GetValue(string path, string valueName, object defaultValue) {
			var result = Registry.GetValue(path, valueName, defaultValue);
			if (result == null)
				throw new Exception("Could not get registry value '" + path + "\\" + valueName + "'");

			return result;
		}

		public static object GetValue(string path, string valueName) {
			return GetValue(path, valueName, null);
		}

		public static void SetValue(string path, string valueName, object value) {
			Registry.SetValue(path, valueName, value);
		}
	}
}
