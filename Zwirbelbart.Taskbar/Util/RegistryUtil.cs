using System;
using Microsoft.Win32;

namespace Zwirbelbart.Taskbar.Util
{
    internal class RegistryUtil
    {
        public static object GetValue(string path, string valueName) {
            var result = Registry.GetValue(path, valueName, null);
            if (result == null)
                throw new Exception("Could not get registry value");

            return result;
        }

        public static void SetValue(string path, string valueName, object value) {
            Registry.SetValue(path, valueName, value);
        }
    }
}
