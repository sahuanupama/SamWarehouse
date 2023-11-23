using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SamWarehouse.Models;
using System.Security.Claims;

namespace SamWarehouse.Controllers
    {
    public class LoginController1 : Controller
        {
        private readonly ItemDbContext _context;
        public LoginController1(ItemDbContext context)
            {
            _context = context;
            }


        public IActionResult Login([FromQuery] string returnUrl)
            {
            //Pass the returnUrl into a new LoginDTO
            LoginUserDTO user = new LoginUserDTO
                {
                //Check the login Url is not empty before passing it over.
                //If it is change it to '/Home'
                ReturnUrl = String.IsNullOrWhiteSpace(returnUrl) ? "/Home" : returnUrl
                };
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginUserDTO user)
            {
            var account = _context.AppUsers.Where(a => a.UserName == user.Username).FirstOrDefault();
            if (account == null)
                {
                ViewBag.LoginError = "User or Password incorrect";
                return View(user);
                }
            if (BCrypt.Net.BCrypt.EnhancedVerify(user.Password, account.PasswordHash) == false)
                {
                ViewBag.LoginError = "Username or Password incorrect";
                return View(user);
                }

            //Create a list of claims associated with the logged in user. These will normally
            //be taken from their user profile.
            var claims = new List<Claim>
            {
                //Add the claim details for the user.
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim("Department","Management")
            };

            var authProperties = new AuthenticationProperties
                {
                //Sets whether the cliding expiry is allowed for this user. Default is true.
                AllowRefresh = true,
                //Whether the login is persitent across all requests
                IsPersistent = true
                };
            //Creates a user identity which will be used for the authentication system.
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Signs in the user using the previously setup details.
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimIdentity),
                                    authProperties);
            //Redirect the user back to where they were trying to go before being made to log in.
            return Redirect(user.ReturnUrl);
            }

        public IActionResult LogOff()
            {
            //Logs the current user out of the system
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
            }

        public IActionResult CreateUser()
            {
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(CreateUserDTO user)
            {

            //Check that the username and confirmation are the same
            if (user.Password.Equals(user.PasswordConfirmation) == false)
                {
                //Create message and return if not matching
                ViewBag.CreateUserError = "Password and Password confirmation do not match";
                return View(user);
                }
            //Check if the username is already taken
            if (_context.AppUser.Any(a => a.UserName == user.UserName))
                {
                //Create message and return it is taken
                ViewBag.CreateUserError = "Username already exists.";
                return View(user);
                }
            //Create new admin user object and fill it our using bcrypt
            AppUser newUser = new AppUser
                {
                UserName = user.UserName,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password),
                Role = "Customer"
                };
            //Add the new user to the database and save it.
            _context.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
            }

        public IActionResult AccessDenied()
            {
            return View();
            }

        }

    }