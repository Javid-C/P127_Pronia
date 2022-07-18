using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P127_Pronia.DAL;
using P127_Pronia.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P127_Pronia.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FooterViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Setting> model = await _context.Settings.ToListAsync();

            return View(model);
        }
    }
}
