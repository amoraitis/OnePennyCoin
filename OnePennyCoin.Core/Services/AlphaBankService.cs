using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Refit;

namespace OnePennyCoin.Core.Services
{
    public interface IAlphaBankService : IService
    {
        Task<ApiResponse<CreateAccountResponse>> CreateAccountAuthorizationRequest();
        string GetAuthenticationUrl(string redirectUri, string accountRequestId);
        Task<AccessTokenResponse> GetTokenForAuthorizationCode(string code, string redirectUri);
        Task<AccountData[]> GetAccountDetails(string token);
        Task<AccountTransaction.TransactionData[]> GetTransactionsFor(string accountId);
    }
    public class AlphaBankService : IAlphaBankService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<EBankingUser> _userManager;
        private readonly IAlphaBankAuthApi _authApi;
        private readonly IAlphaBankAccountsApi _accountsApi;
        private const string AuthUrl = "https://gw.api.alphabank.eu/sandbox/auth/authorize";
        private string _subscriptionKey;

        public AlphaBankService(IConfiguration configuration, IUserAccessor userAccessor, UserManager<EBankingUser> userManager)
        {
            _configuration = configuration;
            _userAccessor = userAccessor;
            _userManager = userManager;
            
            var authClient = new HttpClient
            {
                BaseAddress = new Uri("https://gw.api.alphabank.eu/sandbox")
                //BaseAddress = new Uri("https://secure.alpha.gr")
            };

            var accountsClient = new HttpClient
            {
                BaseAddress = new Uri("https://gw.api.alphabank.eu/api/sandbox/accounts/v1"/*"https://gw.api.alphabank.eu/gr/api/accounts/v1"*/)
            };

            _subscriptionKey = _configuration["Providers:AlphaBank:SubscriptionKey"];
            _authApi = RestService.For<IAlphaBankAuthApi>(authClient);
            _accountsApi = RestService.For<IAlphaBankAccountsApi>(accountsClient);
        }

        public async Task<ApiResponse<CreateAccountResponse>> CreateAccountAuthorizationRequest()
        {
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuration["Providers:AlphaBank:ClientId"]}:{_configuration["Providers:AlphaBank:ClientSecret"]}"));
            var result = await _authApi.GetAccessTokenForClientAsync(
                new AccessTokenBody
                {
                    GrantType = "client_credentials",
                    Scope = "account-info-setup"
                },
                $"Basic {authHeader}");

            return await _accountsApi.CreateAccountRequestAsync("{\"Risk\": {},\"ProductIdentifiers\":null}", $"Bearer {result.Content.AccessToken}", _configuration["Providers:AlphaBank:SubscriptionKey"]);
        }

        public string GetAuthenticationUrl(string redirectUri, string accountRequestId)
        {
            var clientId = _configuration["Providers:AlphaBank:ClientId"];
            return
                $"{AuthUrl}?client_id={clientId}&response_type=code&scope=account-info&redirect_uri={redirectUri}&request={accountRequestId}";
        }

        public async Task<AccessTokenResponse> GetTokenForAuthorizationCode(string code, string redirectUri)
        {
            var res = await _authApi.ExchangeAuthorizationCodeAsync(new ExchangeAuthorizationCodeRequest
            {
                ClientId = _configuration["Providers:AlphaBank:ClientId"],
                ClientSecret = _configuration["Providers:AlphaBank:ClientSecret"],
                Code = code,
                RedirectUrl = redirectUri,
                GrantType = "authorization_code"
            });

            return res.Content;
        }

        public async Task<AccountData[]> GetAccountDetails(string token)
        {
            var accountDataResponse = await this._accountsApi.GetAccountDetailsAsync($"Bearer {token}",
                _subscriptionKey);

            var user = await _userManager.FindByIdAsync(_userAccessor.GetCurrentUser());

            user.AuthToken = token;

            await _userManager.UpdateAsync(user);

            return accountDataResponse.Content;
        }

        public async Task<AccountTransaction.TransactionData[]> GetTransactionsFor(string accountId)
        {
            var user = await _userManager.FindByIdAsync(_userAccessor.GetCurrentUser());
            var getTransactionsResponse =
                await this._accountsApi.GetTransactionsForAccountAsync(accountId, null /*"2020-01-01"*/, $"Bearer {user.AuthToken}", _subscriptionKey);
            
            return getTransactionsResponse.Content;
        }
    }
}
