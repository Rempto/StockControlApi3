using StockControlApi.Domain.Enums;

namespace StockControlApi.Application.Models
{
    public class UserReturnModel
    {
        public string Id { get; set; }  
        public string Name { get; set; }
        public string Email { get; set; }

        public eUserPermission Permission { get; set; }
       
    }
}
