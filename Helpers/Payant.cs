using System;
namespace FirstApp.Helpers
{
    public class Payant
    { 

        //private readonly IGladePayAPI _gladePayAPI;
        //private readonly string GLADE_MID;
        //private readonly string GLADE_MKEY;
        //public Payant(IGladePayAPI gladePayAPI)
        //{
        //    _gladePayAPI = gladePayAPI;
        //    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        //    {
        //        GLADE_MID = Environment.GetEnvironmentVariable("GLADE_MID");
        //        GLADE_MKEY = Environment.GetEnvironmentVariable("GLADE_MKEY");
        //    }
        //    else
        //    {
        //        GLADE_MID = Environment.GetEnvironmentVariable("GLADE_TEST_MID");
        //        GLADE_MKEY = Environment.GetEnvironmentVariable("GLADE_TEST_MKEY");
        //    }
        //}

        //public async Task<PaymentVerify> VerifyPayment(string TxnRef)
        //{
        //    var payment = new PaymentVerify();
        //    try
        //    {
        //        payment = await _gladePayAPI.verify(new GladePaymentVerificationRequest
        //        {
        //            action = "verify",
        //            txnRef = TxnRef
        //        }, key: GLADE_MKEY, mid: GLADE_MID);
        //    }
        //    catch (ApiException exception)
        //    {
        //        new Exception($"Content: {exception.Content} Message: ${exception.Message} Headers: ${exception.Headers}");
        //        // other exception handling
        //    }
        //    catch (Exception ex)
        //    {
        //        new Exception($"Content: {ex.Message}");

        //    }

        //    return payment;
        //}
    }
}
