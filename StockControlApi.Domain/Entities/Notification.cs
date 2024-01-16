using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public eType Type { get; set; }
        public string UserId { get; set; } = string.Empty;
        [NotMapped]
        public User? User { get; set; }
        public string RecipientUserId { get; set; } = string.Empty;
        [NotMapped]
        public User? RecipientUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsVisualized { get; set; }
        public string TypeName
        {
            get
            {
                switch (Type)
                {   
                    case eType.registro:
                        return "registro";
                    case eType.edicao:
                        return "edicao";
                    case eType.exclusaoProduto:
                        return "exclusaoProduto";
                    case eType.saida:
                        return "saida";
                    case eType.entrada:
                        return "entrada";
                    case eType.exclusaoMovimentacao:
                        return "exclusaoMovimentacao";
                    default:
                        return "none";
                }
            }
        }


    }
}
