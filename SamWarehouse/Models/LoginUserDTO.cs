using System.ComponentModel.DataAnnotations.Schema;

namespace SamWarehouse.Models
    {
    [NotMapped]
    public class LoginUserDTO
        {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Role { get; set; }
        public string ReturnUrl { get; set; }
        }
    }
