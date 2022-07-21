using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P127_Pronia.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(ApplicationDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            List<BasketItem> items = await _context.BasketItems.Include(b => b.AppUser).Include(b => b.Plant).Where(b=>b.AppUserId == user.Id).ToListAsync();

            order.BasketItems = items;
            order.AppUser = user;
            order.Date = DateTime.Now;
            order.Status = null;
            order.TotalPrice = default;
            foreach (var item in items)
            {
                order.TotalPrice += item.Price * item.Quantity;
            }
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

    }
}
