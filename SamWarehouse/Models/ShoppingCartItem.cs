namespace SamWarehouse.Models
    {
    public class ShoppingCartItem
        {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        //Linking refrences to the other tables that this table has a relationship with.
        public virtual ShoppingCart Cart { get; set; }
        public virtual Item ShoppingItem { get; set; }

        }
    }
