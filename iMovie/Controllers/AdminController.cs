using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileUploadControl;
using iMovie.Data;
using iMovie.Models;
using iMovie.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace iMovie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private UploadInterface _upload;

        public AdminController(ApplicationDbContext context, UploadInterface upload)
        {
            _upload = upload;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IList<IFormFile> files, MovieDetailViewmodel vmodel, MovieDetails movie)
        {
            string path = string.Empty;
            movie.Movie_Name = vmodel.Name;
            movie.Movie_Description = vmodel.Description;
            movie.DateAndTime = vmodel.DateofMovie;
            foreach (var item in files)
            {
                path = Path.GetFileName(item.FileName.Trim());
                movie.MoviePicture = "~/uploads/" + path;

            }
            _upload.uploadfilemultiple(files);
            _context.MovieDetails.Add(movie);
            _context.SaveChanges();
            TempData["Success"] = "Save Your Movie";
            return RedirectToAction("Create", "Admin");
        }
        [HttpGet]
        public IActionResult CheckBookSeat()
        {

            var getBookingTable = _context.BookingTable.ToList().OrderByDescending(a => a.Datetopresent);
            return View(getBookingTable);
        }

        [HttpGet]
        public IActionResult DeleteMovie(int id)
        {

            var item = _context.MovieDetails.Where(a => a.ID == id).SingleOrDefault();
            return View(item);
        }
        [HttpPost, ActionName("DeleteMovie")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMovieItem(int id)
        {
            var item = _context.MovieDetails.Where(a => a.ID == id).SingleOrDefault();
            _context.MovieDetails.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
         
            var item = _context.BookingTable.Where(a => a.ID == id).SingleOrDefault();
            return View(item);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.BookingTable.Where(a => a.ID == id).SingleOrDefault();
            _context.BookingTable.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

       

        [HttpGet]
        public IActionResult GetUserDetails()
        {
            var getUserTable = _context.Users.ToList();
            return View(getUserTable);
        }

    }
}