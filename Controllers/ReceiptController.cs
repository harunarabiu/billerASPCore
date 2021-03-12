using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using FirstApp.Services;
using FirstApp.Models;
using FirstApp.Helpers;
using FirstApp.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApp.Controllers
{
    [Route("receipt")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReceiptController : Controller
    {
        private readonly string webRootPath;
        private readonly DocumentService _documentService;
        private readonly DBDataContext _DB;
        private readonly MultiTexter _MultiTexter;
        private readonly SmartSMS _smartSMS;

        public ReceiptController(IWebHostEnvironment webHostEnvironment, DBDataContext DB, DocumentService documentService, MultiTexter multiTexter, SmartSMS smartSMS)
        {
            webRootPath = webHostEnvironment.WebRootPath;
            _documentService = documentService;
            _DB = DB;
            _MultiTexter = multiTexter;
            _smartSMS = smartSMS;
        }

        // GET: /<controller>/
        [HttpGet]
        [Route("{TransactionId}")]
        public IActionResult Index(string TransactionId)
        {
            

            if (!string.IsNullOrEmpty(TransactionId))
            {
                var transaction = _DB.Transactions.FirstOrDefault(x => x.TransactionRef == TransactionId);
                if (transaction !=null && transaction.Status == "Success")
                {
                    //var DocumentService = new DocumentService();

                    var pdfFile = _documentService.GeneratePdfFromRazorView(transaction);
                    var FileName = $"Biller.NG_{transaction.TransactionRef}({transaction.Service.Name}).pdf";
                    return File(pdfFile, "application/octet-stream", FileName);

                } else
                {
                    return NotFound();
                }
                
            } else
            {
                return NotFound();
            }
            

            

        }
    }
}
