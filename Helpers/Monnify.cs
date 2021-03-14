using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using FirstApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Refit;

namespace FirstApp.Helpers
{


    public class Monnify
    {
        private readonly DBDataContext _DB;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly ILogger _logger;
        private readonly IMonnifyApi _monnifyApi;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private string base64Credentials;
        private string MONNIFY_CLIENT_SECRET;
        private string MONNIFY_APIKEY;
        private string MONNIFY_CONTRACT_CODE;

        public Monnify(
            //ApplicationUser User,
            DBDataContext DB,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signinManager,
            ILoggerFactory loggerFactory,
            
            IMonnifyApi monnifyApi,
            IConfiguration configuration
        // RoleManager<ApplicationUser> roleManager)
        )
        {
            _DB = DB;
            _userManager = userManager;
            _signInManager = signinManager;
            _logger = loggerFactory.CreateLogger<Monnify>();
            _monnifyApi = monnifyApi;
            _configuration = configuration;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                MONNIFY_APIKEY = Environment.GetEnvironmentVariable("MONNIFY_APIKEY");
                MONNIFY_CLIENT_SECRET = Environment.GetEnvironmentVariable("MONNIFY_CLIENT_SECRET");
                MONNIFY_CONTRACT_CODE = Environment.GetEnvironmentVariable("MONNIFY_CONTRACT_CODE");
            }
            else
            {
                MONNIFY_APIKEY = Environment.GetEnvironmentVariable("MONNIFY_TEST_APIKEY");
                MONNIFY_CLIENT_SECRET = Environment.GetEnvironmentVariable("MONNIFY_TEST_CLIENT_SECRET");
                MONNIFY_CONTRACT_CODE = Environment.GetEnvironmentVariable("MONNIFY_TEST_CONTRACT_CODE");
            }

            byte[] credentialsInBytes = System.Text.Encoding.UTF8.GetBytes($"{MONNIFY_APIKEY}:{MONNIFY_CLIENT_SECRET}");
            base64Credentials = Convert.ToBase64String(credentialsInBytes);

           
        }


        public async Task<string> GetToken()
        {
            var token = string.Empty;
            try
            {

                //var api = RestService.For<IMonnifyApi>("https://sandbox.monnify.com/api/v1");
                //var response = await api.GetTokenAsync($"Basic {base64Credentials}");

                var response = await _monnifyApi.GetTokenAsync($"Basic {base64Credentials}");
                token = response.responseBody.accessToken;

                return token;

            }

            catch (ApiException exception)
            {
                new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
                // other exception handling
            }
            catch (Exception ex)
            {
                new Exception($"Content: {ex.Message}");

            }

            return token;

        }
        
       


        public async Task<ReservedAccountBody>  CreateReservedAccount(CreateReservedAccount AccountDetails)
        {

            ReservedAccountBody newReservedAccount = new ReservedAccountBody();

            try
            {


                //var api = RestService.For<IMonnifyApi>("https://sandbox.monnify.com/api/v1");
                //var response = await api.GetTokenAsync($"Basic {base64Credentials}");


                var token = await GetToken();
                AccountDetails.ContractCode = MONNIFY_CONTRACT_CODE;
                AccountDetails.CurrencyCode = "NGN";

                var newAccount = await _monnifyApi.CreateReservedAccountAsync($"Bearer {token}", AccountDetails);
                _logger.LogDebug(newAccount.ToString());
                newReservedAccount = newAccount.responseBody;

                return newReservedAccount;


            }
            
            catch (ApiException exception)
            {
                 new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
                _logger.LogDebug(exception.Content.ToString());
                // other exception handling
            } catch (Exception ex)
            {
                new Exception($"Content: {ex.Message}");
            }





            return newReservedAccount ;
        }

        public async Task<TransactionResponseBody> GetTransaction(string transactionReference)
        {
            TransactionResponseBody transactionResponse = new TransactionResponseBody();
            try
            {

                //var api = RestService.For<IMonnifyApi>("https://sandbox.monnify.com/api/v1");
                //var response = await api.GetTokenAsync($"Basic {base64Credentials}");
                var token = await GetToken();

                var response = await _monnifyApi.GetTransaction($"Bearer {token}", transactionReference);

                transactionResponse = response.responseBody;

                return transactionResponse;

            }

            catch (ApiException exception)
            {
                new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
                // other exception handling
            }
            catch (Exception ex)
            {
                new Exception($"Content: {ex.Message}");

            }

            return transactionResponse;

        }

        public async Task<BankTransfer> processWebHook(WebhookBankTransfer Transfer)
        {
            //{clientSecret}|{paymentReference}|{amountPaid}|{paidOn}|{transactionReference}

            var newTransfer = new BankTransfer();

            var HashText = $"{MONNIFY_CLIENT_SECRET}|{Transfer.paymentReference}|{Transfer.amountPaid}|{Transfer.paidOn}|{Transfer.transactionReference}";



            var Hash = Cryptography.GenerateSHA512String(HashText);

            var query = await this.GetTransaction(Transfer.transactionReference);

            if (Transfer.transactionHash == Hash.ToLower())
            {
                var status = query.paymentStatus;

                

                var CardDetails = String.Empty;
                var AccountDetails = String.Empty;

                if (query.cardDetails != null)
                {
                    CardDetails = $"{query.cardDetails.cardType}|{query.cardDetails.last4}|{query.cardDetails.bin}";
                }


                if (query.accountDetails != null)
                {
                    AccountDetails = $"{ query.accountDetails.accountName}|{ query.accountDetails.accountNumber}|{ query.accountDetails.bankCode}|{ query.accountDetails.amountPaid}";

                }


                if (status == "PAID")
                {
                    newTransfer = new BankTransfer
                    {
                        TransactionReference = query.transactionReference,
                        PaymentReference = query.paymentReference,
                        AmountPaid = query.amountPaid,
                        TotalPayable = query.totalPayable,
                        SettlementAmount = query.settlementAmount,
                        PaymentDate = query.paidOn,
                        PaymentStatus = query.paymentStatus,
                        PaymentDescription = query.paymentDescription,
                        Currency = query.currency,
                        PaymentMethod = query.paymentMethod,
                        CardPaymentDetails = CardDetails,
                        AccountPaymentDetails = AccountDetails,
                    };
                }

                return newTransfer; 
            }

            return newTransfer;
        }


    }
}
