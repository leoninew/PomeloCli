using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace PomeloCli.DemoApp.Diagnosis {
    public class DiagnosisService : IDiagnosisService {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<DiagnosisOptions> _options;

        public DiagnosisService(
            IHttpClientFactory httpClientFactory,
            IOptions<DiagnosisOptions> options) {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public async Task<Boolean> ReportAsync(String[] args, Exception exception) {
            if (_options.Value.Enable == false || _options.Value.Url == null) {
                return false;
            }

            var message = DiagnosisMessage.GetDiagnosisMessage();
            message.Args = args;
            message.Exception = exception?.ToString();

            var httpClient = _httpClientFactory.CreateClient();
            var url = new Uri(_options.Value.Url);

            var jsonSerializerOptions = new JsonSerializerOptions() {
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
#if DEBUG
                WriteIndented = true,
#endif
            };

            // 对于 net5.0 VersionConverter
            // 对于 netcoreapp3.1 将返回 ObjectDefaultConverter<Version>
            // jsonSerializerOptions.GetConverter(typeof(Version));
            jsonSerializerOptions.Converters.Add(new VersionConvert());

            var content = JsonSerializer.Serialize(GetDiagnosisContent(message, jsonSerializerOptions), jsonSerializerOptions);
            var response = await httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        protected virtual Object GetDiagnosisContent(DiagnosisMessage message, JsonSerializerOptions jsonSerializerOptions) {
            return new { Value = JsonSerializer.Serialize(message, jsonSerializerOptions)};
        }
    }
}