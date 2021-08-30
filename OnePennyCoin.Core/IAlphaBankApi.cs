using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;

namespace OnePennyCoin.Core
{
    public class AccessTokenBody
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public class ExchangeAuthorizationCodeRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
        [JsonPropertyName("redirect_uri")]
        public string RedirectUrl { get; set; }
    }

    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class Risk
    {
        [JsonProperty("PaymentContextCode")]
        public object PaymentContextCode { get; set; }

        [JsonProperty("MerchantCategoryCode")]
        public object MerchantCategoryCode { get; set; }

        [JsonProperty("MerchantCustomerIdentification")]
        public object MerchantCustomerIdentification { get; set; }

        [JsonProperty("DeliveryAddress")]
        public object DeliveryAddress { get; set; }
    }

    public class CreateAccountResponse
    {
        [JsonProperty("AccountRequestId")]
        public string AccountRequestId { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Risk")]
        public Risk Risk { get; set; }

        [JsonProperty("CreationDateTime")]
        public DateTime CreationDateTime { get; set; }

        [JsonProperty("RequestedProductKeys")]
        public object RequestedProductKeys { get; set; }
    }

    public class AccountData
    {
        public string AccountId { get; set; }
        public string Scheme { get; set; }
        public string Currency { get; set; }
        public string Alias { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public bool AllowDebit { get; set; }
        public bool AllowCredit { get; set; }
        public bool AllowQuery { get; set; }
        public string AccountCode { get; set; }
    }

    public interface IAlphaBankAuthApi
    {
        [Headers("Content-Type: application/x-www-form-urlencoded",
            "Authorization: :authContent:")]
        [Post("/auth/token")]
        Task<ApiResponse<AccessTokenResponse>> GetAccessTokenForClientAsync([Body(BodySerializationMethod.UrlEncoded)] AccessTokenBody body,
            [Header("Authorization")] string authContent);

        [Post("/auth/token")]
        [Headers("Content-Type: application/x-www-form-urlencode")]
        Task<ApiResponse<AccessTokenResponse>> ExchangeAuthorizationCodeAsync([Body(BodySerializationMethod.UrlEncoded)] ExchangeAuthorizationCodeRequest body);
    }

    public interface IAlphaBankAccountsApi
    {
        [Headers("Authorization: :authContent:",
            "Ocp-Apim-Subscription-Key: :subscriptionKey:",
            "Content-Type: application/json")]
        [Post("/account-requests")]
        Task<ApiResponse<CreateAccountResponse>> CreateAccountRequestAsync([Body] string rawBody, [Header("Authorization")] string authContent,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

        [Headers("Authorization: :authContent:",
            "Ocp-Apim-Subscription-Key: :subscriptionKey:",
            "x-ab-bank-id: CRBAGRAA",
            "Content-Type: application/x-www-form-urlencode")]
        [Get("/details")]
        Task<ApiResponse<AccountData[]>> GetAccountDetailsAsync([Header("Authorization")] string authContent, [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

        [Headers(
            "Authorization: :authContent:",
            "Ocp-Apim-Subscription-Key: :subscriptionKey:",
            "x-ab-bank-id: CRBAGRAA")]
        [Get("/{accountId}/transactions")]
        Task<ApiResponse<AccountTransaction.TransactionData[]>> GetTransactionsForAccountAsync(
            string accountId,
            [Query] string fromDate,
            [Header("Authorization")] string authContent,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);
    }

    public interface IAlphaBankPaymentsApi
    {
        [Headers("Authorization: :authContent:",
            "Ocp-Apim-Subscription-Key: :subscriptionKey:",
            "Content-Type: application/json")]
        [Post("/account-requests")]
        Task<ApiResponse<CreateAccountResponse>> CreateDomesticTransferIntentAsync(
            [Body] string rawBody,
            [Header("Authorization")] string authContent,
            [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey);

    }

    public class AccountTransaction
    {
        public class Amount
        {
            public object Value { get; set; }
            public string Currency { get; set; }
        }

        public class MerchantDetails
        {
            public object MerchantName { get; set; }
            public object MerchantCategoryCode { get; set; }
        }

        public class TransactionData
        {
            public string AccountId { get; set; }
            public string Scheme { get; set; }
            public string OriginatorAccount { get; set; }
            public string TransactionId { get; set; }
            public Amount Amount { get; set; }
            public string CreditDebitIndicator { get; set; }
            public DateTime BookingDateTime { get; set; }
            public DateTime ValueDateTime { get; set; }
            public string TransactionInformation { get; set; }
            public MerchantDetails MerchantDetails { get; set; }
            public string EndToEndIdentification { get; set; }
            public string InstructionIdentification { get; set; }
        }
    }
}
