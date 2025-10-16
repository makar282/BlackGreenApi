using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IParserJson
    {
        Task ParseReceiptDataAndSaveItems(Receipt receipt, string responseContent);
    }
}
