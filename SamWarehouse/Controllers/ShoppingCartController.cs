using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamWarehouse.Models;

namespace SamWarehouse.Controllers
    {
    public class ShoppingCartController : Controller
        {
        public readonly ItemDbContext _context;

        public ShoppingCartController(ItemDbContext context)
            {
            _context = context;
            }



        // GET: ItemController1
        public ActionResult Index()
            {
            //Get the user id out of the session ansd store it.
            var id = HttpContext.Session.GetInt32("ID");
            //Check if the user is not logged in and an ivalid ID has been stored.
            if (id == null || id < 0 || HttpContext.User.Identity.IsAuthenticated == false)
                {
                return BadRequest();
                }


            var shoppingCart = _context.ShoppingCarts.Include(c => c.CartUser)
                   .Where(c => c.UserId == id).FirstOrDefault();
            //Get the shopping cart lines for the selected cart and add them to its cart items.
            shoppingCart.CartItems = _context.ShoppingCartItems.Include(ci => ci.ItemId)
                    .Where(ci => ci.ShoppingCartId == shoppingCart.Id).ToList();

            //Return the shopping cart partial view with any data we hand over to it.
            return PartialView("_ShoppingCartPartial", shoppingCart);

            }

        }
    }
