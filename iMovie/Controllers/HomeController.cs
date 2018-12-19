using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iMovie.Models;
using iMovie.Data;
using iMovie.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace iMovie.Controllers
{
    
    public class HomeController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        int count = 1;
        bool flag = true;
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _usermanager = usermanager;
            this.roleManager = roleManager;
        
        }
        public IActionResult Index()
        {
            var getMovieList = _context.MovieDetails.ToList();
            return View(getMovieList);
        }
        public IActionResult Start()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult BookNow(int Id)
        {
            BookNowViewModel vm = new BookNowViewModel();

            var item = _context.MovieDetails.Where(a => a.ID == Id).FirstOrDefault();
            vm.Movie_Name = item.Movie_Name;
            vm.Movie_Date = item.DateAndTime;
            vm.MovieId = Id;
                return View(vm);
        }
        [Authorize(Roles ="Admin,user")]
        [HttpPost]
        public IActionResult BookNow(BookNowViewModel vm)
        {
            List<BookingTable> booking = new List<BookingTable>();
            List<Cart> carts = new List<Cart>();
            string seatno = vm.SeatNo.ToString();
            int movieId = vm.MovieId;

            string[] seatnoArray = seatno.Split(',');
            count = seatnoArray.Length;
            if (checkseat(seatno,movieId) == false)
            {
                foreach (var item in seatnoArray)
                {
                    carts.Add(new Cart { Amount = 35, MovieID = vm.MovieId, UserId = _usermanager.GetUserId(HttpContext.User), date = vm.Movie_Date, seatno = item });
                }
                foreach (var item in carts)
                {
                    _context.Cart.Add(item);
                    _context.SaveChanges();

                }
                TempData["Sucess"] = "Miejsce bez rezerwacji, Sprawdź swoją kartę!";
            }
            else
            {
                TempData["seatnomsg"] = "Miejsce zajęte, proszę zmienić numer miejsca!";
            }
            return RedirectToAction("BookNow");
        }

        private bool checkseat(string seatno, int movieId)
        {
            //throw new NotImplementedException();
            string seats = seatno;
            string[] seatreserve = seats.Split(',');
            var seatnolist = _context.BookingTable.Where(a => a.MovieDetailsId == movieId).ToList();
            foreach (var item in seatnolist)
            {
                string alreadybook = item.seatno;
                foreach (var item1 in seatreserve)
                {
                    if(item1==alreadybook)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag == false)
            {
                return true;
            }
            else
                return false;
        }
        [HttpPost]
        public IActionResult checkseat(DateTime Movie_Date, BookNowViewModel booknow)
        {
            string seatno = string.Empty;
            var movielist = _context.BookingTable.Where(a=>a.Datetopresent == Movie_Date).ToList();
            if (movielist!=null)
            {
                var getseatno = movielist.Where(b => b.MovieDetailsId == booknow.MovieId).ToList();
                if (getseatno!= null)
                {
                    foreach (var item in getseatno)
                    {
                        seatno = seatno + " " + item.seatno.ToString();
                    }
                    TempData["SNO"] = "Już zarezerwowane miejsca: " + seatno + "";
                }
            }


            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
