using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
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
    public class ServiceController : BaseController
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

        [Route("")]
        public IActionResult Index()
        {
            Notify("this should be ignored");
            var services = _DB.Services.ToList();
            return View(services);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            var service = new CreateServiceVM();

            return View(service);
        }



        [HttpPost]
        public IActionResult Create(CreateServiceVM data)
        {
            var serviceType = _DB.ServiceTypes.FirstOrDefault(x => x.Id == data.ServiceType);

            var service = _DB.Services.Add(new Service {
                Name = data.Name,
                ServiceType = serviceType,
                Key = data.Name
            });
            return View(service);
        }



        [HttpGet]
        [Route("update")]
        public IActionResult Update (long ServiceId)
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

        [Route("plan/{Id}")]
        [HttpGet]
        public IActionResult ServicePlanUpdate(int Id)
        {
        
            var servicePlan = _DB.ServicePlans.FirstOrDefault(x => x.Id == Id);
            var services = _DB.Services.ToList();

            var editModel = new ServicePlanVM {
                Services = services,
                Plan = servicePlan
            };

            return View(editModel);
        }

        [HttpPost]
        [Route("plan/{Id}")]
        public IActionResult ServicePlanUpdate(ServicePlan plan)
        {
            _logger.LogError(string.Empty, plan);
            _DB.ServicePlans.Update(plan);
            _DB.SaveChanges();

           
            var services = _DB.Services.ToList();

            var editModel = new ServicePlanVM {
                Services = services,
                Plan = plan
            };

           
            return View(editModel);
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
