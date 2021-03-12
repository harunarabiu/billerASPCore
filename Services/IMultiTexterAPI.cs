using System;
using FirstApp.Models.ViewModels;
using Refit;
using System.Threading.Tasks;
using System.Net.Http;

namespace FirstApp.Services
{
    public interface IMultiTexterAPI
    {

        [Post("/sendsms")]
        //Task<MultiTexterSendSMSResponse> SendSMS([Body] MultiTexterSendSMSVM request, [Header("Bearer")] string token = "2D6FLZlfxLAIssWV8j6oTBqsPu1w6SILh2PDL9BLhasDOVMPOl59AK6r5ZsY");
        Task<ApiResponse<MultiTexterSendSMSResponse>> SendSMS([Body] MultiTexterSendSMSVM request, [Header("Bearer")] string token = "2D6FLZlfxLAIssWV8j6oTBqsPu1w6SILh2PDL9BLhasDOVMPOl59AK6r5ZsY");
    }

    public class MultiTexterSendSMSResponse
    {
        public string status { get; set; }
        public string msg { get; set; }
        public string msgid { get; set; }
        public string units { get; set; }
        public string balance { get; set; }
    }
}
