using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public int StockAmount { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
    }

}
