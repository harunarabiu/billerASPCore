using System;
using System.Threading.Tasks;
using FirstApp.Services;
using Microsoft.AspNetCore.Hosting;
using Refit;

namespace FirstApp.Helpers
{
    public class KEDCO
    {
        private readonly IKEDCOApi _kedcoAPI;
        private readonly  string ACCESS_TOKEN;
        private readonly string MERCHANT_ID;
        private readonly string ACCESS_CODE;

        public KEDCO(IKEDCOApi kedcoApi) { 

            _kedcoAPI = kedcoApi;

            if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                ACCESS_TOKEN = Environment.GetEnvironmentVariable("KEDCO_ACCESS_TOKEN"); ;
                ACCESS_CODE = Environment.GetEnvironmentVariable("KEDCO_ACCESS_CODE");
                MERCHANT_ID = Environment.GetEnvironmentVariable("KEDCO_MERCHANT_ID");
            } else
            {
                ACCESS_TOKEN = Environment.GetEnvironmentVariable("KEDCO_TEST_ACCESS_TOKEN"); ;
                ACCESS_CODE = Environment.GetEnvironmentVariable("KEDCO_TEST_ACCESS_CODE");
                MERCHANT_ID = Environment.GetEnvironmentVariable("KEDCO_TEST_MERCHANT_ID");
            }
        }
            



        public async Task<string> GetToken ()
        {

            var token = await _kedcoAPI.Auth();

            return token.token;
        }

        public async Task<IdentificationRequest> Identification (string customerId, string paymentPlan)
        {

            bool isPostPaid = false;
            if (paymentPlan.ToLower() == "postpaid")
                isPostPaid = true;

            var response = new IdentificationRequest();
            try
            { 

                //var token = await GetToken();


                response = await _kedcoAPI.Identification(customerId: customerId, isPostPaid: isPostPaid, accessToken: ACCESS_TOKEN, merchantId: MERCHANT_ID) ;

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

            return response;
        }

        public async Task<PaymentRequest> Payment (string CustomerId, double Amount, string paymentPlan)
        {
            var payment = new PaymentRequest();
            try
            {

                //var token = await GetToken();
                Random random = new Random();
                var transactionref = $"BLR-{random.Next(0000000, 9999999).ToString()}";

                payment = await _kedcoAPI.Payment(customerId: CustomerId, amount: Amount, transactionRef: transactionref, paymentPlan: paymentPlan, accessToken: ACCESS_TOKEN, merchantId: MERCHANT_ID);

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

            return payment;
        }


        public async Task<PaymentRequest> PaymentVerification(string CustomerId, string TransactionRef)
        {
            var payment = new PaymentRequest();
            try
            {

                //var token = await GetToken();

                payment = await _kedcoAPI.PaymentVerification(customerId: CustomerId, transactionRef: TransactionRef);

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

            return payment;
        }

        public async Task<PaymentReversal> PaymentReversal(string TransactionRef)
        {
            var reversal = new PaymentReversal();
            try
            {

                var token = await GetToken();

                reversal = await _kedcoAPI.PaymentReversal(transactionRef: TransactionRef);

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

            return reversal;
        }


    }
}
