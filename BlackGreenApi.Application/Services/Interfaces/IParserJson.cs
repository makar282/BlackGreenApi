namespace SaveNature.Services.Interfaces
{
    public interface IParserJson
    {
        Task ParseReceiptDataAndSaveItems(Receipt receipt, string responseContent);
    }
}
