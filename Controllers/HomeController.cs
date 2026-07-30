using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SchoolApp.Controllers
{
    /*
     * Η κλάση Controller  δίνει πρόσβαση σε όλα τα "εργαλεία" που χρειάζεσαι μέσα σε ένα MVC controller:

        View() → επιστρέφει HTML view
        RedirectToAction() → ανακατεύθυνση
        User → τα claims του logged-in χρήστη
        HttpContext → το request/response
        ViewData, ViewBag, TempData → περνάς δεδομένα στο view
        ModelState → validation errors
        Json() → επιστρέφει JSON
     * */
    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            //ClaimsPrincipal represents the currently authenticated user in ASP.NET Core.
            // HttpContext.User gives you access to it —
            // it's the equivalent of SecurityContextHolder.getContext().getAuthentication() in Spring Security.
            ClaimsPrincipal? principal = HttpContext.User;

            if (!principal!.Identity!.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // tells the browser and proxies to never cache error page. 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
