using System;
using System.Threading.Tasks;
using Refit;
namespace FirstApp.Services
{
    [Headers("Authorization: Bearer")]
    public interface IMonnifyClient
    {
        [Post("/auth/login")]
        Task<string> GetTokenAsync();

        [Post("/bank-transfer/reserved-accounts")]
        Task<string> CreateReservedAccountAsync();


        //bank-transfer/reserved-accounts/transactions?accountReference={accountReference}&page=0&size=10}

        [Get("/bank-transfer/reserved-accounts/transactions?accountReference={accountReference}&page=0&size=10}")]
        Task<string> GetAllTransactionsAsync();
    }


}