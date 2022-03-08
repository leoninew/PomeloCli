using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PomeloCli.DemoApp.Diagnosis {
    static class VersionHelper {
        public static Version GetCurrentVersion() {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName.Version;
        }

        // 0.8.3-beta
        public static Boolean TryParseVersion(String input, out Version version) {
            const String pattern = @"^(\d+(?:\.\d+){1,3})(?=-.+)?";
            version = null;
            var match = Regex.Match(input, pattern);
            if (match.Success == false) {
                return false;
            }

            version = Version.Parse(match.Value);
            return true;
        }
    }
}