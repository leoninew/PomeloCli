using System;
using System.Collections.Generic;

namespace PomeloCli.Plugin.Runtime {
    class DotnetPackageManager {
        public static void AddPackage(DotnetPackage package, String workingDir, String packageDir = null) {
            var args = new List<String>();
            args.Add("add");
            args.Add("package");
            args.Add(package.Name);

            if (package.Version != null) {
                args.Add("-v");
                args.Add(package.Version);
            }

            if (package.Source != null) {
                args.Add("-s");
                args.Add(package.Source);
            }

            if (packageDir != null) {
                args.Add("--package-directory");
                args.Add(packageDir);
            }

            CommandExecutor.Start("dotnet", String.Join(' ', args), workingDir: workingDir);
        }

        public static void RemovePackage(String name, String workingDir) {
            var args = new List<String>();
            args.Add("remove");
            args.Add("package");
            args.Add(name);

            CommandExecutor.Start("dotnet", String.Join(' ', args), workingDir: workingDir);
        }
        
        public static void ListPackage(String source, String workingDir) {
            var args = new List<String>();
            args.Add("list");
            args.Add("package");
            
            if (source != null) {
                args.Add("-s");
                args.Add(source);
            }

            // Console.WriteLine("dotnet {0}", String.Join(' ', args));
            CommandExecutor.Start("dotnet", String.Join(' ', args), workingDir: workingDir);
        }
    }
}