using System;
using System.Threading.Tasks;

namespace PomeloCli.DemoApp.Diagnosis {
    public interface IDiagnosisService {
        Task<Boolean> ReportAsync(String[] args, Exception exception);
    }
}