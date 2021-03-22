using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstApp.Helpers;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Route("payment")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : Controller
    {
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;
        private readonly KEDCO _kedco;
        private readonly GladePay _gladePay;
        private readonly MultiTexter _MultiTexter;
        private readonly SmartSMS _smartSMS;

        public PaymentController(
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            Monnify monnify,
            IConfiguration configuration,
            KEDCO kedco,
            GladePay gladePay,
            MultiTexter multiTexter,
            SmartSMS smartSMS
            )
        {
            _configuration = configuration;
            _DB = DB;
            _MultiTexter = multiTexter;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _kedco = kedco;
            _gladePay = gladePay;
            _smartSMS = smartSMS;
            
        }

        [Route("")]
        [HttpGet]
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }



        [Route("MakePayment")]
        [HttpPost]
        public async Task<dynamic> MakePayment([FromForm] PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var _user = await GetCurrentUser();
            ApplicationUser user = _user;

            var _payment = await _kedco.Payment(request.CustomerId, request.Amount, request.PaymentPlan);


            var service_ = _DB.Services.FirstOrDefault(x => x.Key == request.Service);

            
            
            var customerPaymentPlan = _DB.CustomerPaymentPlans.FirstOrDefault(x => x.PaymentPlan.ServicePlan.Service == service_ && x.PaymentPlan.ServicePlan.Key == request.PaymentPlan && x.User == user);

            var paymentPlan = _DB.PaymentPlans.FirstOrDefault(x => x.ServicePlan.Key == request.PaymentPlan && x.IsDefault == true);

            if (customerPaymentPlan != null)
            {
                paymentPlan = customerPaymentPlan.PaymentPlan;
                
            }

            var wallet = _DB.Wallets.FirstOrDefault(x => x.User == user);

            if (wallet != null && wallet.Balance > request.Amount)
            {
                var customerCommisson = paymentPlan.CommissionType == "Variable" ? request.Amount * paymentPlan.Commission : paymentPlan.Commission;
                wallet.BookBalance = wallet.Balance;
                wallet.Balance = wallet.Balance - ((request.Amount - customerCommisson) + paymentPlan.ConvienceFee);
                _DB.SaveChanges();

            }
            else
            {
                ViewData["Message"] = "Insufficient Wallet Balance.";
                return View();
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
                Commission = 0,
                AmountCharged = payment.AmountCharged,
                RawData = RawData,
                Channel = "WEB",
                PaymentPlan = paymentPlan,
                Service = service_
            };



            _DB.Transactions.Add(transaction);
            _DB.SaveChanges();

            return RedirectToAction("TransactionDetails", "Transaction", new { TransactionId = transaction.Id });
        }


        [HttpPost]
        [Route("otpayment")]
        public async Task<dynamic> OneTimePayment([FromForm] PaymentRequest request)
        {
            var _user = await GetCurrentUser();
            ApplicationUser user = _user;

            if (!ModelState.IsValid)
                return View(request);

            var _payment = await _kedco.Payment(request.CustomerId, request.Amount, request.PaymentPlan);

          
            var service_ = _DB.Services.FirstOrDefault(x => x.Key == request.Service);
            var paymentPlan_ = _DB.PaymentPlans.FirstOrDefault(x => x.ServicePlan.Key == request.PaymentPlan && x.IsDefault == true);

            
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
                Commission = 0,
                AmountCharged = payment.AmountCharged,
                RawData = RawData,
                Channel = "WEB_OTP",
                PaymentPlan = paymentPlan_,
                Service = service_
            };



            _DB.Transactions.Add(transaction);
            _DB.SaveChanges();

            var Text = $"Your KEDCO token is {transaction.TransactionToken}. Thank you for using Biller.NG";

            var SMS = new MultiTexterSendSMSVM
            {
                Message = Text,
                Recipients = request.PhoneNumber,
                Sender_name = "Biller.NG"
            };

            var SMSResponse = _MultiTexter.SendSMS(SMS);

            return RedirectToAction("TransactionDetails", "Transaction", new { TransactionId = transaction.Id });
        }


        [Route("otprepayment", Name = "otprepayment")]
        [Consumes("application/json")]
        [HttpPost]
        public async Task<dynamic> OneTimePrePayment([FromBody] PaymentRequest request)
        {

            var temp = request;

            if (!ModelState.IsValid)
                return StatusCode(400);

            temp = request;

            var transactionRef = Utils.GenerateTransactionId();
            

            var service_ = _DB.Services.FirstOrDefault(x => x.Key == "kedco");
            var paymentPlan_ = _DB.PaymentPlans.FirstOrDefault(x => x.ServicePlan.Key == request.PaymentPlan && x.IsDefault == true);
            var serviceFee = paymentPlan_.ConvienceFee;

            var _user = await GetCurrentUser();
            ApplicationUser user = _user;

            var transaction = new Transaction
            {
                TransactionRef = transactionRef,
                Amount = Convert.ToDouble(request.Amount),
                Status = "Initiated",
                PaymentStatus = "Pending",
                CustomerId = request.CustomerId,
                AmountCharged = Convert.ToDouble(request.Amount) + serviceFee,
                Channel = "WEB",
                PaymentPlan = paymentPlan_,
                User= _user,
                Service = service_,
                Email = request.Email,
                ServiceFee = serviceFee,
                PhoneNumber = request.PhoneNumber,
            };





            _DB.Transactions.Add(transaction);
            _DB.SaveChanges();
            if(transaction.User !=null){
                transaction.User = new ApplicationUser
                {
                    Id = transaction.User.Id,
                    FirstName = transaction.User.FirstName,
                    LastName = transaction.User.LastName
                };
            }
            

            return Ok(new { data = transaction });
        }

        [Route("processotpayment")]
        [Consumes("application/json")]
        [HttpPost]
        public async Task<dynamic> ProcessOTPayment([FromBody] OTPProcessingRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(400);

            var transaction = _DB.Transactions.FirstOrDefault(x=> x.TransactionRef == request.TxnRef);
            var cashpayment = await _gladePay.VerifyPayment(request.PaymentTxnRef);

            if(cashpayment.status == 200 && cashpayment.txnStatus == "successful")
            {
                if(transaction.AmountCharged == Convert.ToDouble(cashpayment.chargedAmount))
                {


                    try {
                        //return StatusCode(500);
                        var _payment = await _kedco.Payment(transaction.CustomerId, transaction.Amount, transaction.PaymentPlan.ServicePlan.Key);

                        var payment = new KEDCOPaymentReponse
                        {
                            TransactionRef = transaction.TransactionRef,
                            CustomerName = _payment.customer.customerName,
                            Address = _payment.customer.address,
                            MeterNumber = _payment.customer.meterNumber,
                            AccountNumber = _payment.customer.accountNumber,
                            Amount = Convert.ToDouble(transaction.Amount),
                            AmountCharged = Convert.ToDouble(transaction.AmountCharged),
                            CustomerArrears = _payment.customer.customerArrears,
                            BusinessUnit = _payment.customer.businessUnit,
                            Status = _payment.transactionStatus,
                            RechargeToken = _payment.recieptNumber,
                            Tariff = _payment.customer.tariff,
                            TariffCode = _payment.customer.tariffCode,
                        };

                        //TODO: handle transaction failure after payment is received

                        var RawData = JsonConvert.SerializeObject(_payment);
                        var _rawData = JsonConvert.DeserializeObject<dynamic>(RawData);



                        transaction.Status = "Success";
                        transaction.ServiceRequestStatus = "Success";
                        transaction.PaymentStatus = "Success";
                        transaction.CustomerId = String.IsNullOrEmpty(_payment.customer.meterNumber) ? _payment.customer.accountNumber : _payment.customer.meterNumber;
                        transaction.Commission = transaction.Amount * (3.5 / 100);
                        transaction.TransactionToken = _payment.recieptNumber;
                        transaction.RawData = RawData;

                        _DB.Transactions.Update(transaction);
                        _DB.SaveChanges();

                        var Text = "";

                        if (transaction.Service.Key == "kedco" && transaction.PaymentPlan.ServicePlan.Key == "prepaid")
                        {
                            Text = $"Your KEDCO token is {transaction.TransactionToken}. Thank you for using Biller.NG";
                        }
                        else
                        {
                            Text = $"Your transaction for {transaction.Service.Name} {transaction.PaymentPlan.ServicePlan.Name} {transaction.CustomerId} is completed Successful. Thank you for using Biller.NG";

                        }
                        var SMSResponse = _smartSMS.SendSMS(Text, transaction.PhoneNumber);



                    }
                    catch (ApiException exception)
                    {
                        new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
                        // other exception handling
                        transaction.PaymentStatus = "Success";
                        transaction.ServiceRequestStatus = "Failed";
                        transaction.Status = "Pending";

                        _DB.Transactions.Update(transaction);
                        _DB.SaveChanges();

                        return StatusCode(500);
                    }
                    catch (Exception ex)
                    {
                        new Exception($"Content: {ex.Message}");

                        transaction.PaymentStatus = "Success";
                        transaction.ServiceRequestStatus = "Failed";
                        transaction.Status = "Pending";

                        _DB.Transactions.Update(transaction);
                        _DB.SaveChanges();

                        return StatusCode(500);
                    }
                    if(transaction.User !=null){

                        transaction.User = new ApplicationUser
                        {
                            Id = transaction.User.Id,
                            FirstName = transaction.User.FirstName,
                            LastName = transaction.User.LastName
                        };
                    }

                    return Ok(new { data = transaction, ok=true });

                } else
                {
                    transaction.PaymentStatus = "Failed";
                    transaction.Status = "Failed";

                    _DB.Transactions.Update(transaction);
                    _DB.SaveChanges();
                    return StatusCode(409);
                }
            } else
            {
                transaction.PaymentStatus = "Failed";
                    transaction.Status = "Failed";

                    _DB.Transactions.Update(transaction);
                    _DB.SaveChanges();
            }
            
           

            return StatusCode(402, new { ok=false});
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public class OTPProcessingRequest
        {
            public string PaymentTxnRef { get; set; }
            public string TxnRef { get; set; }

        }
    }
}
