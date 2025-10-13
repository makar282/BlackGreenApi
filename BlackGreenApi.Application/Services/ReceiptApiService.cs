namespace SaveNature.Services
{
    public class ReceiptApiService : IReceiptApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly ILogger<ReceiptApiService> _logger;

        public ReceiptApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ReceiptApiService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://proverkacheka.com/api/v1/");
            _httpClient.DefaultRequestHeaders.Add("Cookie", "ENGID=1.1");
            _apiToken = "33812.cIfyRu0WPp22Y8btm";
            _logger = logger;
        }

        public async Task<string> FetchReceiptAsync(QrCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.QrRaw) && string.IsNullOrEmpty(request.QrUrl))
                throw new ArgumentException("QrRaw or QrUrl must be provided");

            var formData = new Dictionary<string, string>
            {
                { "token", _apiToken },
                { request.QrRaw != null ? "qrraw" : "qrurl", request.QrRaw ?? request.QrUrl! }
            };

            using var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("check/get", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API request failed: {StatusCode}, {Response}", response.StatusCode, responseContent);
                throw new HttpRequestException($"API request failed with status {response.StatusCode}: {responseContent}");
            }

            if (string.IsNullOrEmpty(responseContent))
            {
                _logger.LogWarning("API response is empty");
                throw new InvalidOperationException("API response is empty");
            }

            return responseContent;
        }
    }
}