using Microsoft.AspNetCore.Mvc;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P127_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SliderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SliderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slider> model = _context.Sliders.ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            return RedirectToAction(nameof(Index));
        }
    }
}
