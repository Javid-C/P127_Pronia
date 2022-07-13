using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P127_Pronia.DAL;
using P127_Pronia.Models;
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
            if (plant is null) return NotFound();
            return View(plant);
        }
    }
}
