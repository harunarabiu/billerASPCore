using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstApp.Models;
using FirstApp.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FirstApp.Helpers;
using FirstApp.Models.ViewModels;
using Newtonsoft.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class V1Controller : Controller
    {
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;
        private readonly KEDCO _kedco;

        public V1Controller(DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            KEDCO kedco
            )
        {
            _configuration = configuration;
            _DB = DB;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _kedco = kedco;
        }


        /// <summary>
        /// Verify Customer Id (Meter Number, SmartCard Number, etc)
        /// </summary>
        /// <returns>
        /// hello
        /// </returns>
        [AllowAnonymous]
        [Route("{service}/verify")]
        [HttpPost]
        public async Task <dynamic> Verify([FromBody] IdentificationRequest request)
        {

           
            var response = await _kedco.Identification(customerId: request.CustomerId, paymentPlan: request.PaymentPlan);

            return StatusCode(200, new { ok = true, data = response });

            //return new string[] { service, "value2" };
        }

        /// <summary>
        /// Make Payments
        /// </summary>
        /// /// <returns>
        /// hello
        /// </returns>
        [Route("{serviceId}/payment")]
        [HttpPost]
        public async Task<dynamic> Payment([FromBody]PaymentRequest request)
        {

            var _payment =  await _kedco.Payment(request.CustomerId, request.Amount, request.PaymentPlan);


            var service_ = _DB.Services.FirstOrDefault(x => x.Key == request.Service.ToLower());

            var _user = await GetCurrentUser();
            ApplicationUser user = _user;

            var customerPaymentPlan = _DB.CustomerPaymentPlans.FirstOrDefault(x => x.PaymentPlan.ServicePlan.Service == service_ && x.PaymentPlan.ServicePlan.Key == request.PaymentPlan.ToLower() && x.User == user);

            var paymentPlan = _DB.PaymentPlans.FirstOrDefault(x => x.ServicePlan.Key == request.PaymentPlan && x.IsDefault == true);

            if (customerPaymentPlan != null)
            {
                paymentPlan = customerPaymentPlan.PaymentPlan;

            }

            var wallet = _DB.Wallets.FirstOrDefault(x => x.User == user);

            var customerCommisson = 0.0;
            var Commisson = paymentPlan.ServicePlan.CommissionType == "Variable" ? request.Amount * paymentPlan.ServicePlan.Commission : paymentPlan.ServicePlan.Commission;

            if (wallet != null && wallet.Balance > request.Amount)
            {
                customerCommisson = paymentPlan.CommissionType == "Variable" ? request.Amount * paymentPlan.Commission : paymentPlan.Commission;
                wallet.BookBalance = wallet.Balance;
                wallet.Balance = wallet.Balance - ((request.Amount - customerCommisson) + paymentPlan.ConvienceFee);
                _DB.SaveChanges();

            }
            else
            {
                ViewData["Message"] = "Insufficient Wallet Balance.";
           
                return StatusCode(402, new { ok = false, error="INSUFFICIENT_WALLET_BALANCE"});
            }



            var payment = new KEDCOPaymentReponse
            {
                TransactionRef = _payment.transactionReference,
                CustomerName = _payment.customer.customerName,
                Address = _payment.customer.address,
                MeterNumber = _payment.customer.meterNumber,
                AccountNumber = _payment.customer.accountNumber,
                Amount = Convert.ToDouble(_payment.paidamount),
                AmountCharged = Convert.ToDouble(_payment.paidamount),
                CustomerArrears = _payment.customer.customerArrears,
                BusinessUnit = _payment.customer.businessUnit,
                Status = _payment.transactionStatus,
                RechargeToken = _payment.recieptNumber,
                Tariff = _payment.customer.tariff,
                TariffCode = _payment.customer.tariffCode,
            };

            var RawData = JsonConvert.SerializeObject(_payment);
            var _rawData = JsonConvert.DeserializeObject<dynamic>(RawData);

            var transaction = new Transaction
            {
                TransactionRef = _payment.transactionReference,
                Amount = Convert.ToDouble(_payment.paidamount),
                Status = _payment.transactionStatus,
                User = _user,
                CustomerId = _payment.customer.meterNumber,
                TransactionToken = _payment.recieptNumber,
                Commission = Commisson,
                AmountCharged = payment.AmountCharged,
                RawData = RawData,
                Channel ="API",
                PaymentPlan = paymentPlan,
                Service = service_
            };
            


            _DB.Transactions.Add(transaction);
            _DB.SaveChanges();

            return StatusCode(200, new { ok=true, data = payment });
        }

        //[Route("{service}/transaction/requery")]
        //[HttpGet]
        //public IEnumerable<string> Query(string service, int? TransactionId)
        //{
        //    switch (service)
        //    {
        //        case "kedco":

        //            return new string[] { service, "Query" };
        //        case "kano":
        //            return new string[] { service, "Query" };

        //        default:
        //            return new string[] { "no service", "Query" };


        //    }

        //    //return new string[] { service, "value2" };
        //}

        /// <summary>
        /// Get Token
        /// </summary>
        /// <returns>
        /// hello
        /// </returns>
        [AllowAnonymous]
        [Route("auth/token")]
        [HttpPost]
        public async Task<IActionResult> Auth([FromBody]AuthViewModel auth)
        {

            var user = await _userManager.FindByEmailAsync(auth.username);

            if(user != null && await _userManager.CheckPasswordAsync(user, auth.password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                };

                var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }



                var token = new JwtSecurityToken(
                       issuer: _configuration["JWT:ValidIssuer"],
                       audience: _configuration["JWT:ValidAudience"],
                       expires: DateTime.Now.AddMinutes(90),
                       claims: authClaims,
                       signingCredentials: new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256)

                    );

                return Ok(

                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    }
                    

                );

            }
            else
            {
                return StatusCode(
                    400,
                   new
                   {
                       ok = false,
                       error = "INVALID_CREDENTIALS"
                   }


               ) ;
            }
            

        }

        

      

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }

    

    public class IdentificationRequest
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string Service { get; set; }
        [Required]
        public string PaymentPlan { get; set; }
    }

    public enum PaymentPlanVM
    {
        prepaid,
        postpaid,
    }


    public enum ServiceProviderVM
    {
        kedco,
        other,
    }

    
}
