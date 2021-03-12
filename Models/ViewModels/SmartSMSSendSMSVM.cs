using System;
namespace FirstApp.Models.ViewModels
{
    public class SmartSMSSendSMSVM
    {

            public string Sender { get; set; }
            public string To { get; set; }
            public string Message { get; set; }
            public string Type { get; set; }
            public string Routing { get; set; }
            public string token { get; set; }
    }
}
