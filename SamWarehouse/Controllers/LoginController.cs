using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SamWarehouse.Models;
using System.Security.Claims;

namespace SamWarehouse.Controllers
    {
    public class LoginController : Controller
        {
        private readonly ItemDbContext _context;

        public LoginController(ItemDbContext context)
            {
            _context = context;
            }
        public IActionResult Login()
            {
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginUserDTO user)
            {
            var account = _context.AppUsers.Where(a => a.UserName == user.UserName).FirstOrDefault();
            if (account == null)
                {
                ViewBag.LoginError = "Username or Password incorrect";
                return View(user);
                }

            if (BCrypt.Net.BCrypt.EnhancedVerify(user.Password, account.Password))
                {
                /*var claims = new List<Claim>{
                 //Add the claim details for the user.
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim("Department","Management")};

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
                                            new ClaimsPrincipal(claimIdentity), authProperties);*/
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Name", account.UserName);
                HttpContext.Session.SetInt32("Viewed", 0);
                HttpContext.Session.SetInt32("ID", account.UserId);


                return RedirectToAction("Index", "Home");
                }

            ViewBag.LoginError = "Username or Password incorrect";
            return View(user);
            }

        //Create a list of claims associated with the logged in user. These will normally
        //be taken from their user profile.
        /*var claims = new List<Claim>
            {
                //Add the claim details for the user.
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim("Department","Management")
            };*/



        //Set any additional properties fo rthis user's login

        /*var authProperties = new AuthenticationProperties
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
*/

        public IActionResult LogOff()
            {
            HttpContext.Session.SetString("IsAuthenticated", "false");
            return RedirectToAction("Index", "Home");
            }

        public IActionResult CreateUser()
            {
            //Gets the value of the IsAuthenticated string from the session.
            //If empty, set the variable to blank.
            string authenticated = HttpContext.Session.GetString("IsAuthenticated");
            //If the user is not authenticated, redirect them to the login page.
            if (authenticated.Equals("false"))
                {
                ViewBag.LoginError = "Login required to access this page.";
                return View("Login");
                }

            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(CreateUserDTO user)
            {
            string authenticted = HttpContext.Session.GetString("IsAuthenticated") ?? "false";
            if (authenticted.Equals("false"))
                {
                ViewBag.LoginError = "Login requires to access this page.";
                return RedirectToAction("Login");
                }
            if (user.Password.Equals(user.PasswordConfirmation) == false)
                {
                ViewBag.CreateUserError = "Password and Password conformation do not match";
                return View(user);
                }
            if (_context.AppUsers.Any(adminUser => adminUser.UserName == user.UserName))
                {
                ViewBag.CreateUserError = "Username already exist.";
                return View(user);
                }
            AppUser newUser = new AppUser
                {
                UserName = user.UserName,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password)
                };
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

