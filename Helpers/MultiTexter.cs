using System;
using System.Threading.Tasks;
using FirstApp.Models.ViewModels;
using FirstApp.Services;
using Refit;

namespace FirstApp.Helpers
{
    public class MultiTexter
    {
        private readonly IMultiTexterAPI _MultiTexterAPI;

        public MultiTexter(IMultiTexterAPI multiTexterAPI)
        {
            _MultiTexterAPI = multiTexterAPI;
        }

        public async Task<dynamic> SendSMS(MultiTexterSendSMSVM request)
        {
            //var response = new (ApiResponse)MultiTexterSendSMSResponse();

            try
            {

                var response = await _MultiTexterAPI.SendSMS(request: request);
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
    }
}
