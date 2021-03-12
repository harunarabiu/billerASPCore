using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using FirstApp.Helpers;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Authorize]
    [Route("transactions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TransactionController : Controller
    {

        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly Monnify _monnify;

        public TransactionController(
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            Monnify monnify,
            IConfiguration configuration
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
        }

        // GET: /<controller>/
        [Route("")]
        public async Task<IActionResult> Index()
        {
            //var transactions = await GetTransactions();

            return View();
        }

        [Route("data")]
        public async Task<IActionResult> TransactionData([FromQuery] string PaymentPlan, [FromQuery] string Status, [FromQuery] string Service, [FromQuery] string Channel, [FromQuery] string DateFrom, [FromQuery] string DateTo)
        {

            DateTime _DateFrom;
            DateTime _DateTo;

            var transactions = await GetTransactions();


            if (!String.IsNullOrEmpty(Channel))
            {
                transactions = transactions.Where(x => x.Channel == Channel);
            }

            if (!String.IsNullOrEmpty(PaymentPlan))
            {
                transactions = transactions.Where(x => x.PaymentPlan.Key == PaymentPlan);
            }

            if (!String.IsNullOrEmpty(Status))
            {
                transactions = transactions.Where(x => x.Status == Status);
            }
            if (!String.IsNullOrEmpty(Service))
            {
                transactions = transactions.Where(x => x.Service.Key == Service);
            }

            if (!String.IsNullOrWhiteSpace(DateFrom))
            {
                _DateFrom = DateTime.ParseExact(DateFrom, "dd-MM-yyyy", null);
                transactions = transactions.Where(x => x.CreatedAt >= _DateFrom);

            }

            if (!String.IsNullOrWhiteSpace(DateTo))
            {
                _DateTo = DateTime.ParseExact(DateTo, "dd-MM-yyyy", null);
                transactions = transactions.Where(x => x.CreatedAt <= _DateTo);
            }




            return Ok(new { ok= true, data = transactions.ToList()});
        }
        [Route("data/{Start}/{End}")]
        public IActionResult TransactionFilter(string Start, string End)
        {
            DateTime StartDate = DateTime.ParseExact(Start, "dd-MM-yyyy",null);
            DateTime EndDate = DateTime.ParseExact(End, "dd-MM-yyyy", null);

            var transactions = _DB.Transactions.Where(x => x.CreatedAt.Date >= StartDate && x.CreatedAt.Date <= EndDate);

            return Ok(new { ok = true, data = transactions });
        }

        [HttpGet]
        [Route("{TransactionId}")]
        public IActionResult TransactionDetails(Guid TransactionId)
        {

            var transaction = _DB.Transactions.FirstOrDefault(x => x.Id == TransactionId);

            var KEDCOTransaction = new KEDCOTransactionVM();

            dynamic json;
            if (String.IsNullOrEmpty(transaction.RawData))
            {
                
                KEDCOTransaction = new KEDCOTransactionVM
                {
                    Amount = transaction.Amount,
                    ServiceFee = transaction.ServiceFee,
                    Commission = transaction.Commission,
                    Channel = transaction.Channel,
                    AmountCharged = transaction.AmountCharged,
                    TransactionRef = transaction.TransactionRef,
                    TransactionStatus = transaction.Status,
                    CustomerName = "N/A",
                    Address = "N/A",
                    BusinessUnit = "N/A",
                    AccountNumber = transaction.CustomerId,
                    MeterNumber = transaction.CustomerId,
                    TariffClass = "N/A",
                    TariffPlan = "N/A",
                    RechargeToken = "N/A",
                    Arrears = "N/A",
                    PhoneNumber = transaction.PhoneNumber,
                    Email = transaction.Email,
                    Service = transaction.Service,
                    PaymentPlan = transaction.PaymentPlan,
                    CreatedAt = transaction.CreatedAt
                };
            }
            else
            {
                json = JValue.Parse(transaction.RawData);
                KEDCOTransaction = new KEDCOTransactionVM
                {
                    Amount = transaction.Amount,
                    ServiceFee = transaction.ServiceFee,
                    Commission = transaction.Commission,
                    Channel = transaction.Channel,
                    AmountCharged = transaction.AmountCharged,
                    TransactionRef = transaction.TransactionRef,
                    TransactionStatus = transaction.Status,
                    CustomerName = json.customer.customerName,
                    Address = json.customer.address,
                    BusinessUnit = json.customer.businessUnit,
                    AccountNumber = json.customer.accountNumber,
                    MeterNumber = json.customer.meterNumber,
                    TariffClass = json.customer.tariffCode,
                    TariffPlan = json.customer.tariff,
                    RechargeToken = transaction.TransactionToken,
                    Arrears = json.customer.customerArrears,
                    PhoneNumber = transaction.PhoneNumber,
                    Email = transaction.Email,
                    Service = transaction.Service,
                    PaymentPlan = transaction.PaymentPlan,
                    CreatedAt = transaction.CreatedAt
                };
            }
            

            
           

            return View(KEDCOTransaction);
        }


        [HttpGet]
        //[Route("export")]
        [Route("export")]
        public IActionResult TransactionExportFilter([FromQuery] string PaymentPlan, [FromQuery] string Status, [FromQuery] string Service, [FromQuery] string Channel, [FromQuery] string DateFrom, [FromQuery]  string DateTo)
        {
            DateTime _DateFrom;
            DateTime _DateTo;


            var transactions = _DB.Transactions.AsQueryable().Where(x => x.Status == "Success");


            if (!String.IsNullOrEmpty(Channel))
            {
                transactions = transactions.Where(x => x.Channel == Channel);
            }

            if (!String.IsNullOrEmpty(PaymentPlan))
            {
                transactions = transactions.Where(x => x.PaymentPlan.Key == PaymentPlan);
            }

            if (!String.IsNullOrEmpty(Status))
            {
                transactions = transactions.Where(x => x.Status == Status);
            }
            if (!String.IsNullOrEmpty(Service))
            {
                transactions = transactions.Where(x => x.Service.Key == Service);
            }


            if (!String.IsNullOrWhiteSpace(DateFrom))
            {
                _DateFrom = DateTime.ParseExact(DateFrom, "dd-MM-yyyy", null);
                transactions = transactions.Where(x => x.CreatedAt >= _DateFrom);

            } 

            if (!String.IsNullOrWhiteSpace(DateTo))
            {
                _DateTo = DateTime.ParseExact(DateTo, "dd-MM-yyyy", null);
                transactions = transactions.Where(x => x.CreatedAt <= _DateTo);
            }
            
           

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Transactions");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Transaction ID";
                worksheet.Cell(currentRow, 2).Value = "Service";
                worksheet.Cell(currentRow, 3).Value = "Region";
                worksheet.Cell(currentRow, 4).Value = "Tariff Class";
                worksheet.Cell(currentRow, 5).Value = "Tariff";
                worksheet.Cell(currentRow, 6).Value = "Account No.";
                worksheet.Cell(currentRow, 7).Value = "Meter No.";
                worksheet.Cell(currentRow, 8).Value = "Payment Plan";
                worksheet.Cell(currentRow, 9).Value = "NetAmount";
                worksheet.Cell(currentRow, 10).Value = "commission";
                worksheet.Cell(currentRow, 11).Value = "Status";
                worksheet.Cell(currentRow, 12).Value = "Date";

                DateTime dateTimeNow = DateTime.UtcNow;
                DateTime dateNow = dateTimeNow.Date;


                //var transactions = _DB.Transactions.ToList();


                foreach (var transaction in transactions)
                {
                    dynamic json;
                    if (String.IsNullOrEmpty(transaction.RawData))
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = transaction.TransactionRef;
                        worksheet.Cell(currentRow, 2).Value = transaction.Service.Name;
                        worksheet.Cell(currentRow, 3).Value = "N/A";
                        worksheet.Cell(currentRow, 4).Value = "N/A";
                        worksheet.Cell(currentRow, 5).Value = "N/A";
                        worksheet.Cell(currentRow, 6).Value = "N/A";
                        worksheet.Cell(currentRow, 7).Value = "N/A";
                        worksheet.Cell(currentRow, 8).Value = transaction.PaymentPlan.ServicePlan.Name;
                        worksheet.Cell(currentRow, 9).Value = transaction.Amount - transaction.Commission;
                        worksheet.Cell(currentRow, 10).Value = transaction.Commission;
                        worksheet.Cell(currentRow, 11).Value = transaction.Status;
                        worksheet.Cell(currentRow, 12).Value = transaction.CreatedAt;
                    }
                    else
                    {
                        json = JValue.Parse(transaction.RawData);
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = transaction.TransactionRef;
                        worksheet.Cell(currentRow, 2).Value = transaction.Service.Name;
                        worksheet.Cell(currentRow, 3).Value = Convert.ToString(json.customer.businessUnit);
                        worksheet.Cell(currentRow, 4).Value = Convert.ToString(json.customer.tariffCode);
                        worksheet.Cell(currentRow, 5).Value = Convert.ToString(json.customer.tariff);
                        worksheet.Cell(currentRow, 6).Value = Convert.ToString(json.customer.accountNumber);
                        worksheet.Cell(currentRow, 7).Value = Convert.ToString(json.customer.meterNumber);
                        worksheet.Cell(currentRow, 8).Value = transaction.PaymentPlan.ServicePlan.Name;
                        worksheet.Cell(currentRow, 9).Value = transaction.Amount - transaction.Commission;
                        worksheet.Cell(currentRow, 10).Value = transaction.Commission;
                        worksheet.Cell(currentRow, 11).Value = transaction.Status;
                        worksheet.Cell(currentRow, 12).Value = transaction.CreatedAt;
                    }

                    
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Biller_Transactions_{DateTime.UtcNow.Date.ToString("dd-MM-yyyy")}.xlsx");
                }
            }
        }

        [HttpPost]
        //[Route("export")]
        [Consumes("application/json")]
        [Route("export")]
        public IActionResult TransactionExport([FromBody] IEnumerable<Transaction> transactions)
        {
            var data = transactions;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Transactions");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Transaction ID";
                worksheet.Cell(currentRow, 2).Value = "Service";
                worksheet.Cell(currentRow, 3).Value = "Region";
                worksheet.Cell(currentRow, 4).Value = "Tariff Class";
                worksheet.Cell(currentRow, 5).Value = "Tariff";
                worksheet.Cell(currentRow, 6).Value = "Account No.";
                worksheet.Cell(currentRow, 7).Value = "Meter No.";
                worksheet.Cell(currentRow, 8).Value = "Payment Plan";
                worksheet.Cell(currentRow, 9).Value = "Net Amount";
                worksheet.Cell(currentRow, 10).Value = "commission";
                worksheet.Cell(currentRow, 11).Value = "Gross Amount";
                worksheet.Cell(currentRow, 12).Value = "Status";
                worksheet.Cell(currentRow, 13).Value = "Date";

                DateTime dateTimeNow = DateTime.UtcNow;
                DateTime dateNow = dateTimeNow.Date;


                //var transactions = _DB.Transactions.ToList();
                if (transactions == null)
                {
                    return StatusCode(204);
                }

                foreach (var transaction in transactions)
                {
                    dynamic json;
                    if (String.IsNullOrEmpty(transaction.RawData))
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = transaction.TransactionRef;
                        worksheet.Cell(currentRow, 2).Value = transaction.Service.Name;
                        worksheet.Cell(currentRow, 3).Value = "N/A";
                        worksheet.Cell(currentRow, 4).Value = "N/A";
                        worksheet.Cell(currentRow, 5).Value = "N/A";
                        worksheet.Cell(currentRow, 6).Value = transaction.PaymentPlan.Key == "postpaid" ? transaction.CustomerId : "N/A";
                        worksheet.Cell(currentRow, 7).Value = transaction.PaymentPlan.Key == "prepaid" ? transaction.CustomerId : "N/A";
                        worksheet.Cell(currentRow, 8).Value = transaction.PaymentPlan.Key;
                        worksheet.Cell(currentRow, 9).Value = transaction.Amount - transaction.Commission;
                        worksheet.Cell(currentRow, 10).Value = transaction.Commission;
                        worksheet.Cell(currentRow, 11).Value = transaction.Amount;
                        worksheet.Cell(currentRow, 12).Value = transaction.Status;
                        worksheet.Cell(currentRow, 13).Value = transaction.CreatedAt;
                    }
                    else
                    {
                        json = JValue.Parse(transaction.RawData);
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = transaction.TransactionRef;
                        worksheet.Cell(currentRow, 2).Value = transaction.Service.Name;
                        worksheet.Cell(currentRow, 3).Value = Convert.ToString(json?.customer?.businessUnit);
                        worksheet.Cell(currentRow, 4).Value = Convert.ToString(json?.customer?.tariffCode);
                        worksheet.Cell(currentRow, 5).Value = Convert.ToString(json?.customer?.tariff);
                        worksheet.Cell(currentRow, 6).Value = Convert.ToString(json?.customer?.accountNumber);
                        worksheet.Cell(currentRow, 7).Value = Convert.ToString(json?.customer?.meterNumber);
                        worksheet.Cell(currentRow, 8).Value = transaction.PaymentPlan.Key;
                        worksheet.Cell(currentRow, 9).Value = transaction.Amount - transaction.Commission;
                        worksheet.Cell(currentRow, 10).Value = transaction.Commission;
                        worksheet.Cell(currentRow, 11).Value = transaction.Amount;
                        worksheet.Cell(currentRow, 12).Value = transaction.Status;
                        worksheet.Cell(currentRow, 13).Value = transaction.CreatedAt;
                    }

                    
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Biller_Transactions_{DateTime.UtcNow.Date.ToString("dd-MM-yyyy")}.xlsx");
                }
            }
        }





        [Route("webhook/bank/transfers")]
        [Consumes("application/json")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BankTransferWebhook([FromBody] WebhookBankTransfer Transfer )
        {

            if (!ModelState.IsValid)
                return BadRequest();

            //{clientSecret}|{paymentReference}|{amountPaid}|{paidOn}|{transactionReference}
            if(string.IsNullOrEmpty(Transfer.paymentReference) || string.IsNullOrEmpty(Transfer.transactionReference))
            {
               
                return BadRequest();
            }
                
            var transaction = await _monnify.processWebHook(Transfer);


               
            if(!string.IsNullOrEmpty(transaction.TransactionReference) && transaction.PaymentStatus == "PAID")
            {
                var UserId = Transfer.product.reference;
                

                ApplicationUser user = await _userManager.FindByIdAsync(UserId);


                transaction.User = user;

                _DB.BankTransfers.Add(transaction);

                var wallet = _DB.Wallets.FirstOrDefault(x => x.User == user);
                wallet.BookBalance = wallet.Balance;
                wallet.Balance = wallet.Balance + Convert.ToDouble(transaction.SettlementAmount);

                _DB.SaveChanges();
            }
          

            return Ok(Json(Transfer));
        }

        private async Task<IQueryable<Transaction>> GetTransactions()
        {
            var user = await GetCurrentUser();


            var transactions = _DB.Transactions.Select(x => new Transaction {
                Id = x.Id,
                Service = x.Service,
                User = new ApplicationUser
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                },
                CreatedAt = x.CreatedAt,
                Amount = x.Amount,
                AmountCharged = x.AmountCharged,
                Channel = x.Channel,
                Commission = x.Commission,
                PaymentPlan = x.PaymentPlan,
                ServiceRequestStatus = x.ServiceRequestStatus,
                Status = x.Status,
                PaymentStatus = x.PaymentStatus,
                TransactionRef = x.TransactionRef,
                TransactionToken = x.TransactionToken,
                Description = x.Description,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RawData = x.RawData


            }).AsQueryable();

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


        private Boolean isSupervisor()
        {

            return User.IsInRole("ExSupervisor");
        }

        private Boolean isCustomer()
        {

            return User.IsInRole("Customer");
        }
        private Boolean isMerchant()
        {

            return User.IsInRole("Merchant");
        }



        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }



    }


}
