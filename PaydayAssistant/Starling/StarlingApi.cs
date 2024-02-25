using System.Net.Http.Headers;
using System.Text.Json;

namespace AutoBanker
{
    public sealed class StarlingApi : HttpClient
    {
        private readonly JsonSerializerOptions _opt;
        public StarlingApi(string token)
        {

            BaseAddress = new Uri("https://api.starlingbank.com");
            DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
            _opt = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
        

        public async Task<StarlingBalance?> GetBalance(string accountId)
        {
            return await Get<StarlingBalance>( $"/api/v2/accounts/{accountId}/balance" );
        }

        public async Task<SavingsGoalsResponse> GetSavingsGoals(string accountId)
        {
            return await Get<SavingsGoalsResponse>( $"/api/v2/account/{accountId}/savings-goals");
        }

        public async Task<SavingsGoal> GetSavingsGoal( string accountId, string goalId )
        {
            return await Get<SavingsGoal>( $"/api/v2/account/{accountId}/savings-goals/{goalId}" );
        }

        public async Task<AccountsResponse> GetAccounts()
        {
            return await Get<AccountsResponse>("api/v2/accounts");
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

            return await Put<StarlingTransferResponse, StarlingTransferRequest>($"api/v2/account/{accountId}/savings-goals/{spaceId}/add-money/{Guid.NewGuid()}", body);
        }

        private async Task<T?> Get<T>( string url )
        {
            var response = await GetAsync( url );

            if( response.IsSuccessStatusCode )
                return JsonSerializer.Deserialize<T>( await response.Content.ReadAsStringAsync(), _opt );
            throw new ApplicationException( response.ReasonPhrase );
        }

        private async Task<TOut?> Put<TOut, TIn>( string url, TIn body)
        {
            await Task.Delay( new TimeSpan( 0, 0, 0, 5 ) );
            var response = await PutAsync( url, new StringContent( JsonSerializer.Serialize( body ), new MediaTypeHeaderValue( "application/json" ) ) );
            if( response.IsSuccessStatusCode )
                return JsonSerializer.Deserialize<TOut>( await response.Content.ReadAsStringAsync() );
            throw new ApplicationException( response.ReasonPhrase );
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
