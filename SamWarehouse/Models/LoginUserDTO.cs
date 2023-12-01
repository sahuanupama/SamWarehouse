using System.ComponentModel.DataAnnotations.Schema;

namespace SamWarehouse.Models
    {
    [NotMapped]
    public class LoginUserDTO
        {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        }
    }
