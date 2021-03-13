using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FirstApp.Models;
using Newtonsoft.Json;

namespace FirstApp.Controllers {

    public class BaseController : Controller {
        [Route("ignored")]
        public void Notify(string message, string title = "Message", NotificationType type = NotificationType.sucess){

            var msg = new {
                message = message,
                title = title,
                icon = type.ToString(),
                type = type.ToString(),
                provider = getProvider(),
            };

            TempData["Message"] = JsonConvert.SerializeObject(msg);

            
        }


        private string getProvider(){

                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
                .AddEnvironmentVariables();


                IConfiguration configuration = builder.Build();
                var value = configuration["NotificationProvider"];
                return value;


        }
    }
}