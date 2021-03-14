using System;
using System.Threading.Tasks;
using FirstApp.Models.ViewModels;
using FirstApp.Services;
using Refit;

namespace FirstApp.Helpers
{
    public class SmartSMS
    {
        

        private readonly ISmartSMSAPI _smartSMSAPI;
        private string SMARTSMS_ACCESS_TOKEN;

        public SmartSMS(ISmartSMSAPI smartSMSAPI)
        {
            _smartSMSAPI = smartSMSAPI;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                SMARTSMS_ACCESS_TOKEN = Environment.GetEnvironmentVariable("SMARTSMS_TEST_ACCESS_TOKEN");

            }
            else
            {
                SMARTSMS_ACCESS_TOKEN = Environment.GetEnvironmentVariable("SMARTSMS_ACCESS_TOKEN");
                
            }
        }

        public async Task<dynamic> Send(SmartSMSSendSMSVM request)
        {
            //var response = new (ApiResponse)MultiTexterSendSMSResponse();

            try
            {

                var response = await _smartSMSAPI.SendSMS(message: request.Message, to: request.To);
                var _response = response;
                return response;

            }
            catch (ApiException exception)
            {
                new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
                // other exception handling
                return "";
            }
            catch (Exception ex)
            {

                new Exception($"Content: {ex.Message}");
                return "";
            }

            //return response;
        }

        public dynamic SendSMS(string Text, string Phone)
        {
            //var Text = "Your KEDCO token is . Thank you for using Biller.NG";

            //var SMS = new MultiTexterSendSMSVM
            //{
            //    Message = Text,
            //    Recipients = "08189931773",
            //    Sender_name = "Biller.NG"
            //};


            var SMS = new SmartSMSSendSMSVM
            {
                Sender = "Biller.NG",
                To = Phone,
                Message = Text,
                Routing = "3",
                Type = "0",
                token = SMARTSMS_ACCESS_TOKEN
            };

            var SMSResponse = this.Send(SMS);

            return SMSResponse;
        }
    }
}
