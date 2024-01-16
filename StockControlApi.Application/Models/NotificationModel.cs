using StockControlApi.Domain.Enums;

namespace StockControlApi.Application.Models
{
    public class NotificationModel
    {
            public string CreationUserId { get; set; }
            public string? ProductName { get; set; }
            public int? Qtd { get; set; }
            public Guid? MoveId { get; set; }
            public eType Type { get; set; }
    }
}
