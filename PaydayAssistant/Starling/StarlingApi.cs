using System.Net.Http.Headers;
using System.Text.Json;

namespace AutoBanker
{
    public sealed class StarlingApi : HttpClient
    {
        public StarlingApi(string token)
        {

            BaseAddress = new Uri("https://api.starlingbank.com");
            DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
        }
        

        public async Task<StarlingBalance?> GetBalance(string accountId)
        {
            var response = await GetAsync($"/api/v2/accounts/{accountId}/balance");

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<StarlingBalance>(await response.Content.ReadAsStringAsync());
            throw new ApplicationException(response.ReasonPhrase);
        }

        public async Task<SavingsGoalsResponse> GetSavingsGoals(string accountId)
        {
            var response = await GetAsync($"/api/v2/account/{accountId}/savings-goals");
            
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<SavingsGoalsResponse>(await response.Content.ReadAsStringAsync());
            throw new ApplicationException(response.ReasonPhrase);
        }

        public async Task<AccountsResponse> GetAccounts()
        {
            var response = await GetAsync("api/v2/accounts");
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<AccountsResponse>(await response.Content.ReadAsStringAsync());
            throw new ApplicationException(response.ReasonPhrase);
        }

        public async Task<StarlingTransferResponse> TransferToSpace(string accountId, string spaceId, decimal amount)
        {
            var body = new StarlingTransferRequest
            {
                amount = new StarlingAmount()
                {
                    currency = "GBP",
                    minorUnits = Convert.ToInt32(amount * 100)
                }
            };

            var response = await PutAsync($"api/v2/account/{accountId}/savings-goals/{spaceId}/add-money/{Guid.NewGuid()}", new StringContent(JsonSerializer.Serialize(body), new MediaTypeHeaderValue("application/json")));
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<StarlingTransferResponse>(await response.Content.ReadAsStringAsync());
            throw new ApplicationException(response.ReasonPhrase);
        }
    }

    public class StarlingTransferRequest
    {
        public StarlingAmount amount { get; set; }
    }

    public class StarlingTransferResponse
    {
        public string TransferUid { get; set; }
        public bool Success { get; set; }
    }

    public record AccountsResponse
    {
        public IEnumerable<Account> Accounts { get; set; }
    }
    public record Account
    {
        public string AccountUid { get; set; }
        public string AccountType { get; set; }
        public string DefaultCategory { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
    }
}
