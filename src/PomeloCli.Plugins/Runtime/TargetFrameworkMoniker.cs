using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomeloCli.Plugins.Runtime
{
    class TargetFrameworkMoniker
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/frameworks
        public static String Current()
        {
            return Resolve(Environment.Version);
        }

        // https://learn.microsoft.com/en-us/dotnet/standard/frameworks
        public static String Resolve(Version version)
        {
            return (Environment.Version.Major, Environment.Version.Minor) switch
            {
                (8, _) => "net8.0",
                (7, _) => "net7.0",
                (6, _) => "net6.0",
                (5, _) => "net5.0",
                (3, 1) => "netcoreapp3.1",
                (2, 1) => "netstandard2.1",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
