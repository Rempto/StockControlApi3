
namespace StockControlApi.Application.Models
{
    public class LoginModel
    {
        public UserReturnModel? User { get; set; }

        public string Token { get; set; }

        public DateTime DateExpires { get; set; }

    }
}
