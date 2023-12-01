using Microsoft.AspNetCore.Mvc;
using SamWarehouse.Models;
using Ganss.Xss;

namespace SamWarehouse.Controllers
    {
    public class CommentController : Controller
        {
        private readonly ItemDbContext _context;

        public CommentController(ItemDbContext context)
            {
            _context = context;
            }

        [HttpGet]
        public IActionResult CommentList(int id)
            {
            if (!isLoggedin())
                {
                TempData["ProductError"] = "User not logged in.";
                return RedirectToAction("Index", "Product");
                }

            var userId = HttpContext.Session.GetInt32("userId");
            List<Comment> comments = _context.Comments.Where(c => (c.ProductCode == id && c.UserId == userId)).ToList();

            if (comments == null)
                {
                return RedirectToAction("Index", "Product");
                }

            return View(comments);

            }


        // Show a form to add a new comment
        [HttpGet]
        public IActionResult AddComment(int productCode)
            {
            Comment comment = new Comment();
            comment.ProductCode = productCode;
            return View(comment);
            }

        // Receive and handle the newly created comment data
        [HttpPost]
        public IActionResult AddComment(Comment comment)
            {
            // creating new instane of Html Sanitizer
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            if (!isLoggedin())
                {
                return RedirectToAction("Index", "Product");
                }

            var userId = HttpContext.Session.GetInt32("userId");
            comment.UserId = (int)userId;
            comment.CreatedDate = DateTime.Now;
            // Sanitizing the comment text
            comment.CommentText = sanitizer.Sanitize(comment.CommentText);

            _context.Comments.Add(comment);
            _context.SaveChanges();

            // A session id is only set once a value has been added!
            // adding a value here to ensure the session is created
            HttpContext.Session.SetString("CommentText", comment.CommentText);

            return RedirectToAction("Index", "Product");
            }

        // Receive and handle a request to Delete a comment
        public IActionResult RemoveComment(int commentId)
            {
            var comment = _context.Comments.Find(commentId);

            // Check if the admin is logged in
            //  string authStatus = HttpContext.Session.GetString("Authenticated");
            //  bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            if (!isLoggedin())
                {
                return RedirectToAction("Index", "Product");
                }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("CommentList", "Comment", new { id = comment.ProductCode });
            }

        // Show a existing comment details in a form to allow for editing
        [HttpGet]
        public IActionResult EditComment(int commentId)
            {
            Comment comment = _context.Comments.Find(commentId);
            return View(comment);
            }

        // Receive and handle the edited comment data
        [HttpPost]
        public IActionResult EditComment(Comment comment)
            {
            if (comment == null)
                {
                return RedirectToAction("Index", "Product");
                }

            // Check if the admin is logged in
            if (!isLoggedin())
                {
                return RedirectToAction("Index", "Product");
                }

            var userId = HttpContext.Session.GetInt32("userId");
            comment.UserId = (int)userId;
            comment.CreatedDate = DateTime.Now;
            _context.Comments.Update(comment);
            _context.SaveChanges();

            return RedirectToAction("CommentList", "Comment", new { id = comment.ProductCode });
            }

        public IActionResult Index()
            {
            var comments = _context.Comments.AsEnumerable();
            return View(comments);
            }

        private bool isLoggedin()
            {
            string authStatus = HttpContext.Session.GetString("IsAuthenticated");
            return !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("true");
            }
        }
    }
