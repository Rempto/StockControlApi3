using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Models
{
    public class UsersChatModel
    {
        public string UserId { get; set; } = string.Empty;
        [NotMapped]
        public string RecipientUserId { get; set; } = string.Empty;
    }
}
