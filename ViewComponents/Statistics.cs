using Microsoft.AspNetCore.Mvc;
using System;

namespace FirstApp.ViewComponents
{
    public class Statistics : ViewComponent
    {
        public Statistics()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
