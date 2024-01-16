using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class Movement
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public double PriceUnit { get; set; }

        public int Qtd { get; set; }

        public double PriceTotal { get; set; }

        public eMove Move { get; set; }

        public DateTime Date { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }


        public string UserId { get; set; }

        public User? User { get; set; }

        public string MoveName
        {
            get
            {
                switch (Move)
                {
                    case eMove.entrada:
                        return "entrada";
                    case eMove.saida:
                        return "saida";
                    default:
                        return "none";
                }
            }
        }
        public string? ProductName1
        {
            get
            {
                return Product != null ? Product.Name : null;
            }
        }
    }
}


