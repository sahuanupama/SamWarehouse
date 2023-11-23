using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SamWarehouse.Repositories;
using System.Security.Claims;
using SamWarehouse.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SamWarehouse.Controllers
    {
    public class LoginController : Controller
        {
        private readonly ItemDbContext _context;
        private readonly AuthRepository _authRepository;

        public LoginController(ItemDbContext context, AuthRepository authRepository)
            {
            _context = context;
            _authRepository = authRepository;
            }



        public IActionResult Login([FromQuery] string ReturnUrl, LoginUserDTO LoginUserDTO)
            {
            LoginUserDTO login = new LoginUserDTO
                {
                ReturnUrl = String.IsNullOrWhiteSpace(ReturnUrl) ? "/Home" : ReturnUrl
                };
            return View(login);
            }

        /* public IActionResult Login([FromQuery] string ReturnUrl)
             {
             LoginUserDTO login = new LoginUserDTO
                 {
                 ReturnUrl = String.IsNullOrWhiteSpace(ReturnUrl) ? "/Home" : ReturnUrl
                 };
             return View(login);
             }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserDTO login)
            {
            //Checks if the username and account match an existing user account.
            var account = _authRepository.Authenticate(login);
            //If no match is found, return to the view.
            if (account == null)
                {
                ViewBag.LoginMessage = "Username or Password is incorrect";
                return View(login);
                }

            //Create new claim list for the current user.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Role, account.Role)
            };

            //Create new identity using the created claims.
            var claimsIdenity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
                {
                //Allows the sliding expiration cookie rule to be used on this user login.
                AllowRefresh = true,
                //Lets the cookie persist over multiple sessions within the timout period, not just the next one.
                IsPersistent = true,
                //Takes back the redirect address for once the login is completed.
                RedirectUri = login.ReturnUrl
                };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                    new ClaimsPrincipal(claimsIdenity), authProperties);
            //Store the current users id in the session so we can retrieve it when we 
            //need to identify who's shopping cart to display.
            HttpContext.Session.SetInt32("ID", account.Id);

            return Redirect(login.ReturnUrl);
            }

        public async Task<IActionResult> Logoff()
            {
            await HttpContext.SignOutAsync();
            //If the user signs out, set the session value to -1 which will be used to indicate
            //there is no user cart to display.
            HttpContext.Session.SetInt32("ID", -1);
            return RedirectToAction("Index", "Home");
            }

        [Authorize(Roles = "Admin")]
        public IActionResult AddUser()
            {
            bool authenticated = HttpContext.User.Identity.IsAuthenticated;
            if (authenticated == false)
                {
                return RedirectToAction("Index", "Home");
                }

            var roles = Enum.GetValues(typeof(Roles)).Cast<Roles>().Select(
                e => new SelectListItem() { Text = e.ToString(), Value = e.ToString() }).ToList();

            ViewBag.Roles = roles;

            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //   [Authorize(Roles = "Admin")]
        public IActionResult AddUser(LoginUserDTO login)
            {
            bool authenticated = HttpContext.User.Identity.IsAuthenticated;
            if (authenticated == false)
                {
                return RedirectToAction("Index", "Home");
                }

            if (login.Password.Equals(login.PasswordConfirmation) == false)
                {
                ViewBag.NewUserMessage = "Password and confirmation do not match";
                return View();
                }

            var account = _authRepository.CreateUser(login, login.Role);
            if (account == null)
                {
                ViewBag.NewUserMessage = "Username already exists!";
                }

            ViewBag.NewUserMessage = "New user added!";
            ModelState.Clear();

            return View();
            }
        }
    }
