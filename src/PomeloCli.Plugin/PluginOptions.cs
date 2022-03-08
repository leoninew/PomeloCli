using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PomeloCli.Plugin {
    public class PluginOptions {
        [Required]
        public String PackageSource { get; set; }

        public String PackageDir { get; set; }

        public String ProjectDir { get; set; }

        public Boolean Disable { get; set; }

        public String GetPackageDir() {
            if (String.IsNullOrWhiteSpace(PackageDir)) {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                PackageDir = Path.Combine(userProfile, ".nuget", "packages");
            }

            return PackageDir;
        }
    }
}