using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using P127_Pronia.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P127_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    [Authorize(Roles ="Moderator,Admin")]
    public class PlantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PlantController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Plant> model = _context.Plants
                .Include(p => p.PlantInformation)
                .Include(p => p.PlantCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.PlantImages).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Information = _context.PlantInformations.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Plant plant)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Information = _context.PlantInformations.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }
            if (plant.MainPhoto == null || plant.HoverPhoto == null || plant.Photos == null)
            {

                ViewBag.Information = _context.PlantInformations.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                ModelState.AddModelError(string.Empty, "You must choose 1 main photo and 1 hover photo and 1 another photo");
                return View();
            }

            if (!plant.MainPhoto.ImageIsOkay(2) || !plant.HoverPhoto.ImageIsOkay(2))
            {
                ViewBag.Information = _context.PlantInformations.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                ModelState.AddModelError(string.Empty, "Please choose valid image file");
                return View();
            }

            plant.PlantImages = new List<PlantImage>();
            TempData["FileName"] = "";
            List<IFormFile> removeable = new List<IFormFile>();
            foreach (var photo in plant.Photos.ToList())
            {
                if (!photo.ImageIsOkay(2))
                {
                    removeable.Add(photo);
                    TempData["FileName"] += photo.FileName + ",";
                    continue;
                }
                PlantImage another = new PlantImage
                {
                    Name = await photo.FileCreate(_env.WebRootPath, "assets/images/website-images"),
                    IsMain = false,
                    Alternative = photo.Name,
                    Plant = plant
                };
                plant.PlantImages.Add(another);
            }

            plant.Photos.RemoveAll(p=>removeable.Any(r=>r.FileName == p.FileName));

            PlantImage main = new PlantImage
            {
                Name = await plant.MainPhoto.FileCreate(_env.WebRootPath, "assets/images/website-images"),
                IsMain = true,
                Alternative = plant.Name,
                Plant = plant
            };
            PlantImage hover = new PlantImage
            {
                Name = await plant.HoverPhoto.FileCreate(_env.WebRootPath, "assets/images/website-images"),
                IsMain = null,
                Alternative = plant.Name,
                Plant = plant
            };

            plant.PlantImages.Add(main);
            plant.PlantImages.Add(hover);

            plant.PlantCategories = new List<PlantCategory>();
            foreach (int id in plant.CategoryIds)
            {
                PlantCategory category = new PlantCategory
                {
                    CategoryId = id,
                    Plant = plant
                };
                plant.PlantCategories.Add(category);
            }

            await _context.Plants.AddAsync(plant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Information = _context.PlantInformations.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            if (id is null || id == 0) return NotFound();
            Plant plant = await _context.Plants
                .Include(p=>p.PlantImages)
                .Include(p => p.PlantInformation)
                .Include(p => p.PlantCategories)
                .ThenInclude(p => p.Category).SingleOrDefaultAsync(p=>p.Id == id);
            if (plant == null) return NotFound();
            return View(plant);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Edit(int? id, Plant plant)
        {
            if (id is null || id == 0) return NotFound();
            Plant existed = await _context.Plants
                .Include(p => p.PlantImages)
                .Include(p => p.PlantInformation)
                .Include(p => p.PlantCategories)
                .ThenInclude(p => p.Category).SingleOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();

            List<PlantImage> removeable = existed.PlantImages.Where(p => p.IsMain == false && !plant.ImagesId.Contains(p.Id)).ToList();
            existed.PlantImages.RemoveAll(p => removeable.Any(r => p.Id == r.Id));

            return Json(existed.PlantImages);
        }
    }
}
