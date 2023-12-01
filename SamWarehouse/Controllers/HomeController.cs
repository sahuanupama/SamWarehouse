using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamWarehouse.Models;
using SamWarehouse.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace SamWarehouse.Controllers
    {
    public class HomeController : Controller
        {
        private readonly ILogger<HomeController> _logger;
        private readonly FileUploaderService _uploader;

        public HomeController(ILogger<HomeController> logger, FileUploaderService uploader)
            {
            _logger = logger;
            _uploader = uploader;

            }

        public IActionResult Index()
            {
            //Gets the session values stored in the user session if they exist yet.
            //If not, set the varables indicated after the null coalescing operators(??)
            //on each line.
            string name = HttpContext.Session.GetString("Name") ?? "Guest";
            int viewed = HttpContext.Session.GetInt32("Viewed") ?? -1;

            if (name.Equals("Guest") == false)
                {
                HttpContext.Session.SetInt32("Viewed", ++viewed);

                }
            return View();
            }

        // [Authorize]
        public IActionResult Privacy()
            {
            HttpContext.Session.SetString("Name", "Troy");
            HttpContext.Session.SetInt32("Viewed", 0);
            return View();
            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
            {
            _uploader.SaveFile(file);
            return View("Index");
            }


        [HttpPost]
        public IActionResult DownLoadFile(string fileName)
            {
            //Get the specified file as a byte array.
            byte[] fileData = _uploader.DownloadFile(fileName);
            //if yhe byte array returened null
            if (fileData == null)
                {// redirect index page 
                return RedirectToAction(nameof(Index));
                }
            //otherwise return the desired file through the browser downloader.
            return File(fileData, "application/octet-strem", fileDownloadName: fileName);
            }
        }
    }