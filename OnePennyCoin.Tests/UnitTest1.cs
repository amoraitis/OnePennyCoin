using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OnePennyCoin.Core;
using Refit;
using Waher.Security.JWS;

namespace OnePennyCoin.Tests
{
    public class AlphaBankApiTests
    {
        private IAlphaBankAuthApi _authApi;
        private IAlphaBankAccountsApi _accountsApi;

        [SetUp]
        public void Setup()
        {
            var authClient = new HttpClient
            {
                BaseAddress = new Uri("https://gw.api.alphabank.eu/sandbox")
            };

            var accountsClient = new HttpClient
            {
                BaseAddress = new Uri("https://gw.api.alphabank.eu/api/sandbox/accounts/v1")
            };

            _authApi = RestService.For<IAlphaBankAuthApi>(authClient);
            _accountsApi = RestService.For<IAlphaBankAccountsApi>(accountsClient);
        }

        [Test]
        public async Task GetAccessToken_Success()
        {
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes("26fabc70181c4609bec982489c4f00f1:7fUkFO4YX4hHJMgdxMdysarkTsJXEyUo"));
            var result = await _authApi.GetAccessTokenForClientAsync(
                new AccessTokenBody
                {
                    GrantType = "client_credentials",
                    Scope = "account-info-setup"
                },
                $"Basic {authHeader}");

            Assert.IsTrue(result.IsSuccessStatusCode);
            //var jwsSignature = JwsAlgorithm.TryGetAlgorithm("as", out var algorithm);
                
                //.SignData(b, jwsPriKey, "", SigAlgorithm.Rsa_Sha256, Sig.SigOptions.Default, Sig.Encoding.Base64url);
            var res = await _accountsApi.CreateAccountRequestAsync("{\"Risk\": {},\"ProductIdentifiers\":null}", $"Bearer {result.Content.AccessToken}", "6f85fa31327f49609d769cfbfa8f321f");

            Assert.IsTrue(res.IsSuccessStatusCode);
        }
    }
}