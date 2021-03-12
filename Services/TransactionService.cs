using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstApp.Controllers;
using FirstApp.Helpers;
using FirstApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FirstApp.Services
{
    public class TransactionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly Monnify _monnify;

        public TransactionService(
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            Monnify monnify,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
         // RoleManager<ApplicationUser> roleManager
         )
        {
            _DB = DB;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _monnify = monnify;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<Models.Transaction>> GetTransactions()
        {
            var user = await GetCurrentUser();


            var transactions = _DB.Transactions.AsQueryable();

            if (isSupervisor())
            {
                transactions = transactions.Where(x => x.Service.Key == "kedco");
            }

            if (isCustomer() || isMerchant())
            {
                transactions = transactions.Where(x => x.User.Id == user.Id);
            }



            return transactions;


        }


        public Boolean isSupervisor()
        {

            return _httpContextAccessor.HttpContext.User.IsInRole("ExSupervisor");
        }

        public Boolean isCustomer()
        {

            return _httpContextAccessor.HttpContext.User.IsInRole("Customer");
        }
        public Boolean isMerchant()
        {

            return _httpContextAccessor.HttpContext.User.IsInRole("Merchant");
        }


        public async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }
    }
}
