using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SamWarehouse.Models
    {
    public class ShoppingCartItem
        {
        [Key]
        public int ShoppingCartItemId { get; set; }
        public int ShoppingCartId { get; set; }
        public int ProductCode { get; set; }
        public int Quantity { get; set; }
        public string? ImangePath { get; set; }
        // public int? UserId { get; set; }

        //Linking refrences to the other tables that this table has a relationship with.
        //  [ForeignKey("ShoppingCartId")]
        public virtual ShoppingCart Cart { get; set; }
        //    [ForeignKey("ProductCode")]
        public virtual Product ProductItem { get; set; }
        // [ForeignKey("UserId")]
        // public virtual AppUser User { get; set; }

        }
    }
