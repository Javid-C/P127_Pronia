using Microsoft.AspNetCore.Mvc;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P127_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Category> model = _context.Categories.OrderByDescending(x => x.Id).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View();
            Category existed = _context.Categories.FirstOrDefault(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (existed != null)
            {
                ModelState.AddModelError("Name", "You can not duplicate category name");
                return View();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(int? id,Category newCategory)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Category existed = _context.Categories.FirstOrDefault(c=>c.Id == id);
            if (existed == null) return NotFound();
            bool duplicate = _context.Categories.Any(c => c.Name == newCategory.Name);
            if (duplicate)
            {
                ModelState.AddModelError("Name","You cannot duplicate name");
                return View();
            }

            //existed.Name = newCategory.Name;
            _context.Entry(existed).CurrentValues.SetValues(newCategory);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Category category = await _context.Categories.FindAsync(id);
            if (category is null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
