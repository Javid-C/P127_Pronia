using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using P127_Pronia.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P127_Pronia.Controllers
{
    public class PlantController:Controller
    {
        private readonly ApplicationDbContext _context;

        public PlantController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if(id is null || id == 0)
            {
                return NotFound();
            }
            Plant plant = await _context.Plants.Include(p => p.PlantImages)
                .Include(p => p.PlantInformation).Include(p => p.PlantCategories).ThenInclude(p=>p.Category)
                .FirstOrDefaultAsync(p=>p.Id == id);

            ViewBag.Plants = await _context.Plants.Include(p=>p.PlantImages).ToListAsync();
            if (plant is null) return NotFound();
            return View(plant);
        }

        public async Task<IActionResult> Partial()
        {
            List<Plant> plants = await _context.Plants.Include(p=>p.PlantImages).ToListAsync();

            return PartialView("_PlantsPartialView", plants);

        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Plant plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);
            if (plant == null) return NotFound();
            string basketStr = HttpContext.Request.Cookies["Basket"];

            BasketVM basket;

            if (string.IsNullOrEmpty(basketStr))
            {
                basket = new BasketVM();
                BasketCookieItemVM cookieItem = new BasketCookieItemVM
                {
                    Id = plant.Id,
                    Quantity = 1
                };
                basket.BasketCookieItemVMs = new List<BasketCookieItemVM>();
                basket.BasketCookieItemVMs.Add(cookieItem);
                basket.TotalPrice = plant.Price;

            }
            else
            {
                basket = JsonConvert.DeserializeObject<BasketVM>(basketStr);
                BasketCookieItemVM existed = basket.BasketCookieItemVMs.Find(p => p.Id == id);
                if(existed == null)
                {
                    BasketCookieItemVM cookieItem = new BasketCookieItemVM
                    {
                        Id = plant.Id,
                        Quantity = 1
                    };
                    basket.BasketCookieItemVMs.Add(cookieItem);
                    basket.TotalPrice += plant.Price;
                }
                else
                {
                    basket.TotalPrice += plant.Price;
                    existed.Quantity++;
                }
            }
            basketStr = JsonConvert.SerializeObject(basket);

            HttpContext.Response.Cookies.Append("Basket", basketStr);

            return RedirectToAction("Index","Home");
        }

        public IActionResult ShowBasket()
        {
            if (HttpContext.Request.Cookies["Basket"] == null) return NotFound();
            BasketVM basket = JsonConvert.DeserializeObject<BasketVM>(HttpContext.Request.Cookies["Basket"]);
            return Json(basket);
        }
    }
}
