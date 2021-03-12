using System;
using System.Threading.Tasks;
using FirstApp.Models.ViewModels;
using Newtonsoft.Json;
using Refit;

namespace FirstApp.Services
{
    public interface ISmartSMSAPI
    {
        [Get("/json.php?message={message}&to={to}&sender={sender}&type={type}&routing={routing}&token={token}")]
        Task<SmartSMSSendSMSResponse> SendSMS(string message, string to, string sender="Biller.NG", int type=0, int routing=3, string token= "KFDay3NhfIyINOM7p4yTogC8HkzFfaE3nyKswvDhGYkWrgvVoIrn7OTDYvFuknnxpUGS2lXgjUrM4SPZmlAjBziF86vjSIotxYTJ");
    }

    public class SmartSMSSendSMSResponse
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "failed")]
        public string Failed { get; set; }
        [JsonProperty(PropertyName = "successful")]
        public string Successful { get; set; }
        [JsonProperty(PropertyName = "message_id ")]
        public string MessageID { get; set; }
        [JsonProperty(PropertyName = "error")]
        public Boolean Error { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
