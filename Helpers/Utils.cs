using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FirstApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FirstApp.Helpers
{
    public class Utils
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Utils(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public static string GenerateTransactionId()
        {
            Random random = new Random();
            var transactionref = $"BLR-{random.Next(0000000, 9999999).ToString()}";

            return transactionref;
        }

        //public static string GetClaim(this ClaimsIdentity claimsIdentity, string claimType)
        //{
        //    var claim = claimsIdentity.Claims.GetType(x => x.Type == claimType);

        //    return (claim != null) ? claim.Value : string.Empty;
        //

        public string CurrentUserId(ClaimsPrincipal User) {


            var user = _userManager?.GetUserId(User);

            return user;
        }

        //public async Task<ApplicationUser> GetCurrentUser()
        //{
        //    return await _userManager.GetUserAsync(HttpContext.User);
        //}

    }
}
