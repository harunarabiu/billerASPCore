using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using FirstApp.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FirstApp.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FirstApp.Helpers;
using FirstApp.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Authorize]
    [Route("account")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Utils _utils;
        //private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;
        private readonly Monnify _monnify;
        private readonly IEmailService _emailService;

        public AccountController(
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            Monnify monnify,
            Utils utils,
            IEmailService emailService
         // RoleManager<ApplicationUser> roleManager
         )
        {
            _DB = DB;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _monnify = monnify;
            _utils = utils;
            _emailService = emailService;
        }

        //GET: /<controller>/
        [Route("users")]
        [HttpGet]
        public IActionResult Index()
        {
        

            var users = _userManager.Users.Take(5).ToArray();


            return View(users);
        }

        [Route("users/data")]
        [HttpGet]
        public IActionResult UsersData()
        {


            var users = _userManager.Users.Select(x => new ApplicationUser {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                UserName = x.UserName,
                LockoutEnabled = x.LockoutEnabled
            }).ToList();


            return Ok(new
            {
                Ok = true,
                data = users
            });
        }

        [Route("bank/BVN/verification")]
        [HttpGet]
        public IActionResult BVNVerification()
        {


            var users = _userManager.Users.Take(5).ToArray();


            return View(users);
        }


        //GET: /<controller>/
        [Route("balance")]
        [HttpGet]
        public async Task<IActionResult> AccountBalance()
        {

            var user = await GetCurrentUser();
            

            var Wallet = _DB.Wallets.FirstOrDefault(x => x.User.Id == user.Id);
            if (Wallet == null)
            {
                var wallet = new Wallet
                {
                    User = user,
                    Balance = 0.0,
                    BookBalance = 0.0,
                };

                _DB.Wallets.Add(wallet);
                _DB.SaveChanges();
                Wallet = wallet;
            }

            var BankAccount = _DB.ReservedAccounts.FirstOrDefault(x => x.User.Id == user.Id);

            var AccountBalance = new AccountBalanceVM
            {
                BankAccount = BankAccount,
                Wallet = Wallet
            };
            //var users = _userManager.Users.Take(5).ToArray();


            return View(AccountBalance);
        }
        [Route("bank/create")]
        [HttpPost]
        public async Task<IActionResult> CreateBankAccount ([FromForm]VerifyBVNVM request)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();

            //}

            var user = await  GetCurrentUser();

            var Wallet = _DB.Wallets.FirstOrDefault(x => x.User.Id == user.Id);
            if (Wallet == null)
            {
                var wallet = new Wallet
                {
                    User = user,
                    Balance = 0.0,
                    BookBalance = 0.0,
                };

                _DB.Wallets.Add(wallet);
                _DB.SaveChanges();
                Wallet = wallet;
            }

            
            var BankAccount = _DB.ReservedAccounts.FirstOrDefault(x => x.User.Id == user.Id);
            
            if (BankAccount == null || string.IsNullOrEmpty(BankAccount.AccountNumber) )
            {
                var newReserverdAccount = await _monnify.CreateReservedAccount(new CreateReservedAccount
                {
                    AccountName = $"{user.FirstName} {user.MiddleName} {user.LastName} ",
                    AccountReference = user.Id,
                    CustomerName = $"{user.FirstName} {user.MiddleName} {user.LastName} ",
                    CustomerEmail = user.Email,
                    CustomerBVN = request.BVN
                });


                if (newReserverdAccount == null  || String.IsNullOrEmpty(newReserverdAccount.accountNumber)) {
                    ModelState.AddModelError(String.Empty, "Couldn't create Bank Account at the moment. Please Try again later.");
                    return StatusCode(501, new
                    {
                        ok = false,
                        error = "BANK_ACCOUNT_CREATION_FAILED"
                    });

                }
                else
                {
                    var reservedAccount = new ReservedAccount
                    {
                        AccountNumber = newReserverdAccount.accountNumber,
                        AccountName = newReserverdAccount.accountName,
                        AccountCurrency = newReserverdAccount.currencyCode,
                        AccountRef = newReserverdAccount.accountReference,
                        BankName = newReserverdAccount.bankName,
                        BankCode = newReserverdAccount.bankCode,
                        AccountReservationRef = newReserverdAccount.reservationReference,
                        User = user,
                        Wallet = Wallet,
                        Status = newReserverdAccount.status

                    };

                    _DB.ReservedAccounts.Add(reservedAccount);

                    _DB.SaveChanges();

                    BankAccount = reservedAccount;

                }

            } else
            {
                return StatusCode(501, new
                {
                    ok = false,
                    error = "ACCOUNT_ALREADY_EXIST"
                });
            }

            var AccountBalance = new AccountBalanceVM
            {
                BankAccount = BankAccount,
                Wallet = Wallet
            };
            //var users = _userManager.Users.Take(5).ToArray();


            return RedirectToAction("AccountBalance", "Account");
        }

        //[Route("{id}")]
        //[HttpGet]
        //public IActionResult Account( int id)
        //{
        //    var account = _DB.Accounts.FirstOrDefault(x => x.Id == id);

        //    return View(account);
        //}



        [Route("create")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {

            return View(new CreateUserViewModel());
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {

            return View(new LoginViewModel());
        }
        [Route("auth")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Auth()
        {

            return View(new LoginViewModel());
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl=null)
        {
            if (!ModelState.IsValid)
                return View(login);


            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            ViewData["ReturnUrl"] = returnUrl;
            if (result.Succeeded)
            {
            

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                return RedirectToAction(returnUrl);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your account is not activated, please contact support. ");
                return View(login);
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked, please contact support. ");
                _logger.LogWarning(2, "User account locked out.");
                return View(login);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login details.");
                return View(login);
            }


           
            //_DB.Add(account);
            //_DB.SaveChanges();


            //return RedirectToAction("Account", "Account", new { id = account.Id });



        }

        [Route("security/password")]
        [HttpGet]
        public IActionResult ChangePassword()
        {

            return View();
        }

        [Route("security/password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM  model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);


            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                return RedirectToAction(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        [Route("security/password/reset")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword( string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token.");
            }
            return View();
        }


        [Route("security/password/reset")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);


            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }


            return View(model);
        }

        [Route("security/password/forgot")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {

            return View();
        }
        

        [Route("security/password/forgot")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
               
                ModelState.AddModelError(string.Empty, "User with email not found.");
                
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

            sendPasswordResetEmail(model.Email, passwordResetLink);
            _logger.Log(LogLevel.Warning, passwordResetLink);
           
            return RedirectToAction("Login", "Account");

        }



        [HttpPost]
        public async Task<IActionResult> Logout( string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        [Route("auth")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Auth(LoginViewModel login, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(login);


            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!result.Succeeded)
            {

                ModelState.AddModelError("", "Login Error");

                return View(login);

            }


            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(returnUrl);

        }


        [Route("create")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel account)
        {
            if (!ModelState.IsValid)
                return View(account);


            var newAccount = new ApplicationUser
            {
                UserName = account.Email,
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                MiddleName = account.MiddleName,
                PhoneNumber = account.Phone,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(newAccount, account.Password);


            if (!result.Succeeded)
            {
                foreach(var error in result.Errors.Select(x => x.Description))
                {
                    ModelState.AddModelError("", error);
                     
                    return View(account);
                }
                
            }

            var user = await GetCurrentUser();

            if (user !=null && (await _userManager.IsInRoleAsync(user, "Root") || await _userManager.IsInRoleAsync(user, "Administrator")))
            {
                if(await _userManager.IsInRoleAsync(user, "Administrator") && account.AccountType == "Root")
                {
                    await _userManager.AddToRoleAsync(newAccount, "Root");
                } else
                {
                    await _userManager.AddToRoleAsync(newAccount, account.AccountType);
                }
                

            } else
            {
                await _userManager.AddToRoleAsync(newAccount, "Customer");
            }
               

            var profile = _DB.UserProfiles.Add(new UserProfile
            {
                User = newAccount,
                CompanyName = account.CompanyName
            });

            var wallet = new Wallet
            {
                Balance = 0.0,
                BookBalance = 0.0,
                User = newAccount
            };

            var newWallet = _DB.Wallets.Add(wallet);


            _DB.SaveChanges();

            var newReserverdAccount = await _monnify.CreateReservedAccount(new CreateReservedAccount
            {
                AccountName = $"{newAccount.FirstName} {newAccount.MiddleName} {newAccount.LastName} ",
                AccountReference = newAccount.Id,
                CurrencyCode = "NGN",
                ContractCode = "5853271866",
                CustomerName = $"{newAccount.FirstName} {newAccount.MiddleName} {newAccount.LastName} ",
                CustomerEmail = newAccount.Email
            });

            var reservedAccount = _DB.ReservedAccounts.Add(new ReservedAccount
            {
                AccountNumber = newReserverdAccount.accountNumber,
                AccountName = newReserverdAccount.accountName,
                AccountCurrency = newReserverdAccount.currencyCode,
                AccountRef = newReserverdAccount.accountReference,
                BankName = newReserverdAccount.bankName,
                BankCode = newReserverdAccount.bankCode,
                AccountReservationRef = newReserverdAccount.reservationReference,
                User = newAccount,
                Wallet = wallet,
                Status = newReserverdAccount.status

            }) ;






            _DB.SaveChanges();

            return RedirectToAction("users");

            //_DB.Add(account);
            //_DB.SaveChanges();


            //return RedirectToAction("Account", "Account", new { id = account.Id });



        }
        private async Task<ApplicationUser> GetCurrentUser()
        {
            
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private void sendPasswordResetEmail(string Email, string PasswordResetLink)
        {
            string message;

            var resetUrl = PasswordResetLink;
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
   
           
            _emailService.Send(
                to: Email,
                subject: "Biller.ng - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}",
                from: "hello@biller.ng"
            );
        }




        public class CreateAccountRequest
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string AccountType { get; set; }
            public string Password { get; set; }
            public Boolean Active { get; set; }
            public DateTime Timestamp { get; set; }
        }
    } 
}
