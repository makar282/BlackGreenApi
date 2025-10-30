namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IEcoRatingManager
    {
		  Task<int> GetEcoRatingAsync(string userName);
		  Task<int> CalculateAndSaveEcoRatingAsync(string userName);
	 }
}
