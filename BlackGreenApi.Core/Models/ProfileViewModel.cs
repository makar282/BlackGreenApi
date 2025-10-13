namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Поля пользователя для отобржаения
    /// </summary>
    public class ProfileViewModel
    {
        public int EcoPurchasesCount { get; set; }
        public double EcoRating { get; set; }
        public DateTime? FirstReceiptDate { get; set; }
    }
}