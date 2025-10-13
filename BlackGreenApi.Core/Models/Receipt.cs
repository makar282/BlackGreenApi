namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Чек
    /// </summary>
    public class Receipt
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Item> Items { get; set; } = new List<Item>(); 
    }
}