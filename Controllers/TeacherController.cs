using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Core;
using SchoolApp.DTO;
using SchoolApp.Services;

namespace SchoolApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IApplicationService applicationService;
        public List<Error> ErrorArray { get; set; } = [];


        public TeacherController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        [HttpGet]
        [Authorize(Roles = "TEACHER")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Policy = "CanInsertTeacher")]
        public IActionResult Signup()
        {
           return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(TeacherSignupDTO teacherSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(teacherSignupDTO);
            }
            
            try
            {
                await applicationService.TeacherService.SignUpUserAsync(teacherSignupDTO);
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
