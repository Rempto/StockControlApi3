using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
 
        public User? User { get; set; }
        public string RecipientUserId { get; set; } = string.Empty;
    
        public User? RecipientUser { get; set; }
        public List<Message>? Messages { get; set; }

    }
        

    }
