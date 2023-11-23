namespace SamWarehouse.Models
    {
    public class AppUser
        {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        //Linking references to the other tables that this table has a relationship with.
        public virtual List<ShoppingCart> Carts { get; set; }
        }
    }
