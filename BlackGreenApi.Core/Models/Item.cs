using System.ComponentModel.DataAnnotations.Schema;

namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Товар из чека
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int? RecommendationId { get; set; }

        [ForeignKey("ReceiptId")]
        public required Receipt Receipt { get; set; }

        [ForeignKey("RecommendationId")]
        public virtual Recommendation? Recommendation { get; set; }
    }
}