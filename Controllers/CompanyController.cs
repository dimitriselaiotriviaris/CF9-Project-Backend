using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CF9Project.Core;
using CF9Project.DTO;
using CF9Project.Services;

namespace CF9Project.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IApplicationService applicationService;
        public List<Error> ErrorArray { get; set; } = [];


        public CompanyController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        [HttpGet]
        [Authorize(Roles = "COMPANY")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Policy = "CanInsertCompany")]
        public IActionResult Signup()
        {
           return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(CompanySignupDTO companySignupDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(companySignupDTO);
            }
            
            try
            {
                await applicationService.CompanyService.SignUpUserAsync(companySignupDTO);
                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View();
            }
        }
    }
}
