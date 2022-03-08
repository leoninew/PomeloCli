using System;
using System.IO;

namespace PomeloCli.Plugin.Native {
    static class DotnetHelper {
        public static String DetectExecutable() {
            var dotnetHome = DetectFromDepsFile() ?? DetectFromEnvironment();
            //Assertion.NotNull(dotnetHome, "dotnet home not resolved");
            //Assertion.Ensure(Directory.Exists(dotnetHome), "dotnet home not found");
            return Path.Combine(dotnetHome, "dotnet");
        }

        private static String DetectFromEnvironment() {
            String dotnetHome = null;
            if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
                dotnetHome = "/usr/local/share/";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix) {
                dotnetHome = "/usr/share/dotnet/";
            }
            else /* if (OperatingSystem.IsWindows())*/ {
                // %ProgramFiles%\dotnet\dotnet.exe
                if (Environment.Is64BitOperatingSystem) {
                    dotnetHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet");
                }
                // ProgramFiles(x86)\dotnet\dotnet.exe
                else {
                    dotnetHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet");
                }
            }

            return dotnetHome;
        }

        private static String DetectFromDepsFile() {
            // https://github.com/Azure/azure-webjobs-sdk/blob/dev/src/Analyzer/ExtensionViewer/DotNetMuxer.cs
            var fxDepsFile = AppContext.GetData("FX_DEPS_FILE") as String;
            if (fxDepsFile == null) {
                return null;
            }

            return new FileInfo(fxDepsFile) // Microsoft.NETCore.App.deps.json
                .Directory? // (version)
                .Parent? // Microsoft.NETCore.App
                .Parent? // shared
                .Parent
                .FullName;
        }
    }
}