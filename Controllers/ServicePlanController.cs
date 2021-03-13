using System.Linq;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace FirstApp.Controllers {

    [Authorize]
    [Route("services/plan")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServicePlanController : Controller {

        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;


        public ServicePlanController (
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

        [HttpGet]
        [Route("")]
        public IActionResult Index(){
            return View();
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create(){
            var service = new CreateServicePlanVM();
            return View(service);
        }

        [HttpPost]
        public IActionResult Create(CreatePaymentPlanVM plan){

            return View();
        }
        
        [HttpGet]
        [Route("edit")]
        public IActionResult Update(int PlanId){

            var service = _DB.Services.FirstOrDefault(x => x.Id == PlanId);
            
            return View("New", service);
        }
    }
}