using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _singInManager;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(
            SignInManager<ApplicationUser> singInManager,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _singInManager = singInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl,
            };
            return View(loginVM);
        }


        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

           
            RegisterVM registeVM = new()
            {
                RoleList = _roleManager.Roles.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Name
                    }),
                RedirectUrl = returnUrl
            };

            return View(registeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = new ApplicationUser
                {
                    CreatedAt = DateTime.Now,
                    Email = registerVM.Email,
                    Name = registerVM.Name,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = registerVM.Email

                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    await _singInManager.SignInAsync(user, isPersistent: false);
                    if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(registerVM.RedirectUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });


            return View(registerVM);
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _singInManager
                    .PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RemmberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user =await _userManager.FindByEmailAsync(loginVM.Email);

                    if(await _userManager.IsInRoleAsync(user,SD.Role_Admin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

                    if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(loginVM.RedirectUrl);
                    }


                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }

            }
            return View(loginVM);
        }
    }
}
