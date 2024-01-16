using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class UserPushNotifications
    {
        public Guid UserPushNotificationsId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        [NotMapped]
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
