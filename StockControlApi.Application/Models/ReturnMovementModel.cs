using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;

namespace StockControlApi.Application.Models
{
    public class ReturnMovementModel
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public int Qtd { get; set; }
 
        public eMove Move { get; set; }

    }
}
