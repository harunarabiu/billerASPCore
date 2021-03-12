using FirstApp.Services;
using FirstApp.Models.ViewModels;
using FirstApp.Models;
using Newtonsoft.Json.Linq;
using Api2PdfLibrary;

namespace FirstApp.Helpers
{
    public class DocumentService
    {
        
        private readonly IRazorRenderer _razorRenderer;


        public DocumentService(
            IRazorRenderer razorRenderer)
        {

            _razorRenderer = razorRenderer;
        }

        
        public byte[] GeneratePdfFromRazorView(Models.Transaction transaction)
        {
            var receiptViewModel = GetReceiptDetails(transaction);
            var partialName = "/Views/PdfTemplate/KEDCOReceipt.cshtml";
            var htmlContent = _razorRenderer.RenderPartialToString(partialName, receiptViewModel);

            var a2pClient = new Api2Pdf("1ac86803-378d-4934-855b-bfdb7d9219cc");


            var FileName = $"Biller.NG_{transaction.TransactionRef}({transaction.Service.Name})";

            var apiResponse = a2pClient.HeadlessChrome.FromHtml(htmlContent, outputFileName:FileName);

            return apiResponse.GetPdfBytes();
        }



        private KEDCOTransactionVM GetReceiptDetails(Models.Transaction transaction)
        {

            var KEDCOTransaction = new KEDCOTransactionVM();

            if (transaction.Status == "Failed" || string.IsNullOrEmpty(transaction.RawData)) {

                KEDCOTransaction = new KEDCOTransactionVM
                {
                    Amount = transaction.Amount,
                    ServiceFee = transaction.ServiceFee,
                    Commission = transaction.Commission,
                    Channel = transaction.Channel,
                    AmountCharged = transaction.AmountCharged,
                    TransactionRef = transaction.TransactionRef,
                    TransactionStatus = transaction.Status,
                    CustomerName = "N/A",
                    Address = "N/A",
                    BusinessUnit = "N/A",
                    AccountNumber = "N/A",
                    MeterNumber = "N/A",
                    TariffClass = "N/A",
                    TariffPlan = "N/A",
                    RechargeToken = transaction.TransactionToken,
                    Arrears = "N/A",
                    PhoneNumber = transaction.PhoneNumber,
                    Email = transaction.Email,
                    Service = transaction.Service,
                    PaymentPlan = transaction.PaymentPlan,
                    CreatedAt = transaction.CreatedAt
                };

            } else
            {
                dynamic json = JValue.Parse(transaction.RawData);


                KEDCOTransaction = new KEDCOTransactionVM
                {
                    Amount = transaction.Amount,
                    ServiceFee = transaction.ServiceFee,
                    Commission = transaction.Commission,
                    Channel = transaction.Channel,
                    AmountCharged = transaction.AmountCharged,
                    TransactionRef = transaction.TransactionRef,
                    TransactionStatus = transaction.Status,
                    CustomerName = json?.customer?.customerName,
                    Address = json?.customer?.address,
                    BusinessUnit = json?.customer?.businessUnit,
                    AccountNumber = json?.customer?.accountNumber,
                    MeterNumber = json?.customer?.meterNumber,
                    TariffClass = json?.customer?.tariffCode,
                    TariffPlan = json?.customer?.tariff,
                    RechargeToken = transaction.TransactionToken,
                    Arrears = json?.customer?.customerArrears,
                    PhoneNumber = transaction.PhoneNumber,
                    Email = transaction.Email,
                    Service = transaction.Service,
                    PaymentPlan = transaction.PaymentPlan,
                    CreatedAt = transaction.CreatedAt
                };
            }


            

            //var receiptViewModel = new KEDCOReceiptVM
            //{
            //    TransactionDate = transaction.CreatedAt,
            //    TransactionRef = transaction.TransactionRef,
            //    Amount= transaction.Amount,
            //    AmountCharged = transaction.AmountCharged,
            //    ServiceFee = transaction.ServiceId,
            //    Customer = "Haruna Muhd",
            //    Service = transaction.Service.Name,
            //    ServiceType = transaction.Service.ServiceType.Name,
            //    PaymentPlan = transaction.PaymentPlan.Name,
            //    RechargeToken = transaction.TransactionToken


            //};


            KEDCOTransaction.Commission = KEDCOTransaction.Amount * 0.03;

            return KEDCOTransaction;
        }
    }
}
