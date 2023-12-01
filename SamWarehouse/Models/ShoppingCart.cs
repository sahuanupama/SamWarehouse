using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SamWarehouse.Models
    {
    public class ShoppingCart
        {
        [Key]
        public int ShoppingCartId { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public bool IsFinalised { get; set; } = false;

        //Linking refrences to the other tables that this table has a relationship with.
        // [ForeignKey("UserId")]
        public virtual AppUser CartUser { get; set; }
        //  public virtual Product ProductItem { get; set; }
        public virtual List<ShoppingCartItem> CartItems { get; set; }
        }
    }
