namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Запрос
    /// </summary>
    public class QrCodeRequest
    {
        public string? QrRaw { get; set; }
        public string? QrUrl { get; set; }
        public string? UserName { get; set; }
    }
}
