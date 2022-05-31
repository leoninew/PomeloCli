using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PomeloCli.DemoApp.Diagnosis {
    public class DiagnosisMessage {
        public DiagnosisEnvironment Environment { get; set; }
        public String Directory { get; set; }
        public String User { get; set; }
        public String[] Args { get; set; }
        public Version Version { get; set; }
        public String Exception { get; set; }

        public static DiagnosisMessage GetDiagnosisMessage() {
            var hostName = default(String);
            String[] hostAddress = null;

            try {
                hostName = Dns.GetHostName();
                hostAddress = GetHostAddresses(hostName).ToArray();
            }
            catch (Exception) {
                // ignored
            }

            var info = new DiagnosisMessage {
                Environment = new DiagnosisEnvironment {
                    CLR = System.Environment.Version,
                    OS = System.Environment.OSVersion,
                    Host = hostName,
                    Addresses = hostAddress
                },
                User = System.Environment.UserName,
                Version = VersionHelper.GetCurrentVersion(),
                Directory = System.IO.Directory.GetCurrentDirectory()
            };
            return info;
        }

        private static IEnumerable<String> GetHostAddresses(String hostName) {
            var host = Dns.GetHostEntry(hostName);
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    yield return ip.ToString();
                }
            }
        }
    }

    public class DiagnosisEnvironment {
        public String Host { get; set; }
        public String[] Addresses { get; set; }
        public Version CLR { get; set; }
        public OperatingSystem OS { get; set; }
    }
}