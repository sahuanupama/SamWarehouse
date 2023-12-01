using System.ComponentModel.DataAnnotations;

namespace SamWarehouse.Models
    {
    public class AppUser
        {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        //Linking references to the other tables that this table has a relationship with.
        public virtual List<ShoppingCart> Carts { get; set; }
        }
    }
