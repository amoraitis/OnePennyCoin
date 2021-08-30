using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using OnePennyCoin.Core;
using OnePennyCoin.Core.Services;

namespace OnePennyCoin.Controllers
{
    [Authorize]
    public class BankProviderController : Controller
    {
        private readonly IAlphaBankService _alphaBankService;
        private readonly IConfiguration _configuration;

        public BankProviderController(IAlphaBankService alphaBankService, IConfiguration configuration)
        {
            _alphaBankService = alphaBankService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddBank()
        {
            ViewData["CreateCallbackWorkflowUrl"] = this.Url.ActionLink("CreateCallbackWorkflow");
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Callback([FromQuery]string code)
        {
            var result = await _alphaBankService.GetTokenForAuthorizationCode(code, this.Url.ActionLink("Callback"));
            return RedirectToAction("GetAccountDetails", new RouteValueDictionary(result));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult NewCallback([FromQuery] string accessToken)
        {
            return this.Ok(accessToken);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCallbackWorkflow()
        {
            var clientId = _configuration["Providers:AlphaBank:ClientId"];
            var baseUrl = this.Url.ActionLink("Callback");
            var request = await _alphaBankService.CreateAccountAuthorizationRequest();
            return Ok($"https://gw.api.alphabank.eu/sandbox/auth/authorize?client_id={clientId}&response_type=code&scope=account-info&redirect_uri={baseUrl}&request={request.Content.AccountRequestId}");
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountDetails([FromRoute] AccessTokenResponse accessTokenInfo)
        {
            if (this.Request.Query.TryGetValue("AccessToken", out var value))
            {
                var accountData = await this._alphaBankService.GetAccountDetails(value);

                this.ViewData["GetTransactionsUrl"] = this.Url.ActionLink("GetTransactionsFor");
                
                return this.View(accountData?.Where(a => a.Scheme.Equals("Account") || a.Scheme.Equals("Card"))?.ToArray());
            }

            return this.View(new AccountData[]{});
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsFor(string id)
        {
                var transactions = await this._alphaBankService.GetTransactionsFor(id);
            return Ok(transactions);
        }
    }
}
