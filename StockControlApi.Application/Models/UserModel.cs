using StockControlApi.Domain.Enums;

namespace StockControlApi.Application.Models
{
    public class UserModel
    {   
       
        public string Name { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        public eUserPermission Permission { get; set; }
    }

}
