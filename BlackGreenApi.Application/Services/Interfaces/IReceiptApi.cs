namespace SaveNature.Services
{
    public interface IReceiptApi
    {
        Task<string> FetchReceiptAsync(QrCodeRequest request);
    }
}