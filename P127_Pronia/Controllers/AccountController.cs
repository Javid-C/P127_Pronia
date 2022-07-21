using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P127_Pronia.Models;
using P127_Pronia.ViewModels;
using System.Threading.Tasks;

namespace P127_Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            if (!register.Terms)
            {
                ModelState.AddModelError("Terms", "Please check our terms and conditions");
                return View();
            }

            AppUser user = new AppUser
            {
                Firstname = register.Firstname,
                Lastname = register.Lastname,
                UserName = register.Username,
                Email = register.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user,"Member");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(login.Username);
            if (user is null) return View();


            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Due to overtrying you have been blocked about 10 minutes");
                    return View();
                }

                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
             await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public JsonResult ShowAuthentication()
        {
            return Json(User.Identity.IsAuthenticated);
        }

        //public async Task CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));
        //    await _roleManager.CreateAsync(new IdentityRole("Moderator"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //}
    }
}
