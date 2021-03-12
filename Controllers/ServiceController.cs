using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FirstApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Authorize]
    [Route("services")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServiceController : Controller
    {

        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;


        public ServiceController (
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            IConfiguration configuration
         )
        {
            _configuration = configuration;
            _DB = DB;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: /<controller>/

        
        public IActionResult Index()
        {
            var services = _DB.Services.ToList();
            return View(services);
        }

        [HttpPost]
        public IActionResult Service()
        {


            return View();
        }


        [Route("type")]
        [HttpPost]
        public IActionResult ServiceType()
        {


            return View();
        }

        [Route("data")]
        [HttpGet]
        public IActionResult ServiceData()
        {
            var services = _DB.Services.ToList();

            return Ok(new
            {
                Ok = true,
                data = services
            });
        }

        [Route("plan")]
        [HttpPost]
        public IActionResult ServicePlan()
        {
            var servicePlans = _DB.ServicePlans.ToList();
            return View(servicePlans);
        }

        [Route("plan/data")]
        [HttpGet]
        public IActionResult ServicePlanData()
        {
            var servicePlans = _DB.ServicePlans.ToList();

            return Ok(new
            {
                Ok = true,
                data = servicePlans
            });
        }

        [Route("payment/plan/data")]
        [HttpGet]
        public IActionResult PaymentPlanData()
        {

            var paymentPlans = _DB.PaymentPlans.ToList();
            return Ok(new
            {
                Ok = true,
                data = paymentPlans
            });

            
        }

        [Route("payment/plan/")]
        [HttpGet]
        public IActionResult PaymentPlan()
        {

            var paymentPlans = _DB.PaymentPlans.ToList();
            return View(paymentPlans);
        }
    }
}
