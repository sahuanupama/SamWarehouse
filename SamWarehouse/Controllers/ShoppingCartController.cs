using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
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
            if (id == null || id < 0 || HttpContext.Session.GetString("IsAuthenticated") != "true")
                {
                return BadRequest();
                }
            var shoppingCart = _context.ShoppingCarts.Include(c => c.CartUser)
                   .Where(c => c.UserId == id).FirstOrDefault();

            //If the user doesn;t currently have an open cart return to the partial view with
            //no data provided
            if (shoppingCart == null)
                {
                return PartialView("_ShoppingCartPartial");
                }
            //Get the shopping cart lines for the selected cart and add them to its cart items.
            shoppingCart.CartItems = _context.ShoppingCartItems.Include(ci => ci.ShoppingCartId)
                    .Where(ci => ci.ShoppingCartId == shoppingCart.ShoppingCartId).ToList();

            //Return the shopping cart partial view with any data we hand over to it.
            return PartialView("_ShoppingCartPartial", shoppingCart);
            }

        public async Task<ActionResult> UpdateQuantity([FromBody] ShoppingCartItem item)
            {
            //Pass the model to Entity Framework to be used. By using the attach command
            //we can tell entity framwork to only change the field we specify in the next step.
            _context.ShoppingCartItems.Attach(item);
            //Tell entity frameworok to find the Quantity property of the item
            //ans sets the IsMOdified property to true. This will mark this property
            //as one that needs to be updated int the database.
            _context.Entry(item).Property(x => x.Quantity).IsModified = true;

            await _context.SaveChangesAsync();
            return Ok();

            }

        public async Task<ActionResult> RemoveFromCart([FromBody] ShoppingCartItem item)
            {
            //Tell Entiry framework to remove the specified item.
            _context.ShoppingCartItems.Remove(item);
            //Save the changes
            await _context.SaveChangesAsync();
            //Send a response back to the javascript where this method was called.
            return Ok();
            }

        public async Task<ActionResult> AddToCart(int Id)
            {
            //Check if there is a user ID stored in the session data. This will
            //default the ID to -1 if there isnlt one saved in the session.
            var userId = HttpContext.Session.GetInt32("ID") ?? -1;
            //Check if the user is logged in and the ID is valid.
            if (userId < 0 || HttpContext.Session.GetString("IsAuthenticated") != "true")
                {
                return Unauthorized();
                }
            //Check if the user has a cart that is still unfinalised and if they do get it from
            //the database.
            var cart = _context.ShoppingCarts.Where(c => c.ShoppingCartId == userId && c.IsFinalised == false)
                                             .Include(c => c.CartItems).FirstOrDefault();
            //Create a new cart item with the details we will need to add it later when it's needed.
            var cartItem = new ShoppingCartItem
                {
                ProductCode = Id,
                Quantity = 1
                };
            //If the uer currently doesn't have an open cart
            if (cart == null)
                {
                //Create a new cart with the user's id and start a new cart item list with the
                //created item as it's first entry.
                cart = new ShoppingCart
                    {
                    UserId = userId,
                    CartItems = new List<ShoppingCartItem> { cartItem }
                    };
                //Add the new cart to Entity Framwork as well as the details of the new cartItem from within
                //the cart. This will also add the cartItem's foreign key reference to the shopping cart.
                _context.ShoppingCarts.Add(cart);
                }
            //If the user already has an open cart
            else
                {
                //Check if ther cart already has a copy of the item in it.
                var item = cart.CartItems.Where(ci => ci.ProductCode == Id).FirstOrDefault();
                //If the cart already has the item
                if (item != null)
                    {
                    //Increate the items quantiyt by one
                    item.Quantity++;
                    //Hand the item over to entiry framwork.
                    _context.ShoppingCartItems.Attach(item);
                    //Tell entity framework to find the Quantity property of the item
                    //ans sets the IsModified property to true. This will mark this property
                    //as one that needs to be updated int the database.
                    _context.Entry(item).Property(x => x.Quantity).IsModified = true;
                    }
                else
                    {
                    //Add the cart's ID to the shopping cart item
                    cartItem.ShoppingCartId = cart.ShoppingCartId;
                    //Pass the cart item to entity framework to be saved.
                    _context.ShoppingCartItems.Add(cartItem);
                    }

                }
            //Save the change to the database
            await _context.SaveChangesAsync();
            //Send an OK response to the user.
            return Ok();
            }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<ActionResult> FinaliseCart(int id)
            {
            if (id < 1)
                {
                return BadRequest();
                }
            var cart = new ShoppingCart { ShoppingCartId = id, Total = CalculateCartTotal(id), IsFinalised = true };
            // pushing for the update.
            _context.ShoppingCarts.Attach(cart);
            _context.Entry(cart).Property(c => c.IsFinalised).IsModified = true;
            _context.Entry(cart).Property(c => c.Total).IsModified = true;
            await _context.SaveChangesAsync();
            return Ok();
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double CalculateCartTotal(int id)
            {
            var cartItems = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == id).Include(c => c.ProductCode).ToList();

            double total = 0;
            foreach (var cartItem in cartItems)
                {
                total += (double)cartItem.ProductItem.ProductPrice * cartItem.Quantity;
                }
            return total;
            }

        }
    }
