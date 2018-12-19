using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iMovie.Data;
using iMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace iMovie.Controllers
{
    [Authorize(Roles ="Admin,user")]
    public class CartTController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _usermanager;
        public CartTController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _usermanager = usermanager;
            _context = context;
        }

        public IActionResult Index()
        {
            var item = _context.Cart.Where(a => a.UserId == _usermanager.GetUserId(HttpContext.User)).ToList();
            return View(item);
        }
        [HttpGet]
        public IActionResult cartEmpty()
        {
            TempData["cartempty"] = "Empty Cart";
            return View();
        }
        [HttpGet] 
        public IActionResult proceed(Cart cart)
        {
            var CartList = _context.Cart.Where(a => a.UserId == _usermanager.GetUserId(HttpContext.User)).ToList();
            if (CartList.Count ==0)
            {
                return RedirectToAction("cartEmpty", "CartT");
            }
            else
            {
                return View(CartList);
            }

        }

        public IActionResult BookTicket(Cart cart)
        {
            List<BookingTable> bt = new List<BookingTable>();
            var CartList = _context.Cart.Where(a => a.UserId == _usermanager.GetUserId(HttpContext.User)).ToList();
            foreach (var item in CartList)
            {
                bt.Add(new BookingTable { Datetopresent = item.date, MovieDetailsId = item.MovieID, seatno = item.seatno, UserID = item.UserId, Amount = item.Amount });
            }
            foreach (var item in bt)
            {
                _context.BookingTable.Add(item);
                _context.SaveChanges();
            }
            if(cart!=null)
            {
                var itemList = _context.Cart.Where(a => a.UserId == _usermanager.GetUserId(HttpContext.User)).ToList();
                foreach(var item in itemList)
                {
                    _context.Cart.Remove(item);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _context.Cart.Where(a => a.ID == id).SingleOrDefault();
            return View(item);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCartItem(int id)
        {
            var item = _context.Cart.Where(a => a.ID == id).SingleOrDefault();
            _context.Cart.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index", "CartT");
        }


    }
}