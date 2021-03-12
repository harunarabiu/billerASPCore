using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FirstApp.Models;
using FirstApp.Services;
using FirstApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TransactionService _transactionService;

        public HomeController(
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            TransactionService transactionService
        )
        {
            _DB = DB;
            _userManager = userManager;
            _transactionService = transactionService;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            //return "First Controller Class.";

            //return new ContentResult { Content="First Controller Content"};

            ViewBag.Title = "Biller Dashboard";

            //ApplicationUser user = await _userManager.GetUserAsync(User);

            //ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            //var user = this.User.FindAll(c => string.Equals(c.Type, ClaimTypes.Role)).Select(c => c.Value);
            //var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            //var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var claimvalue = claims?.Value;
            //if (claims != null)
            //{
            //    claimvalue = claims?.Value;
            //}
            
          

            return View();
        }
        [Route("dashboard")]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            //return "First Controller Class.";

            //return new ContentResult { Content="First Controller Content"};
            ViewBag.Title = "Biller Dashboard";
            //var user = await _userManager.GetUserAsync(User);

            //ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}";
            

            var transactions = await _transactionService.GetTransactions();

            return View(transactions);
        }

        
    }
}
