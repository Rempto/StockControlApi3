using StockControlApi.Domain.Enums;

namespace StockControlApi.Application.Models
{
    public class MovementModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }

        public double PriceUnit { get; set; }

        public int Qtd { get; set; }

        public double PriceTotal { get; set; }

        public eMove Move { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public Guid ProductId { get; set; }
        
        public string UserId { get; set; }
    }
}
