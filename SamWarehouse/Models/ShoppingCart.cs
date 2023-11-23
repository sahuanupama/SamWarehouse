namespace SamWarehouse.Models
    {
    public class ShoppingCart
        {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public bool isFinnalised { get; set; } = false;

        //Linking refrences to the other tables that this table has a relationship with.
        public virtual AppUser CartUser { get; set; }
        public virtual List<ShoppingCartItem> CartItems { get; set; }
        }
    }
