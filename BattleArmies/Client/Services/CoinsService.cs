using System.Net.Http.Json;

namespace BattleArmies.Client.Services
{
    public class CoinsService : ICoinsService
    {
        private readonly HttpClient _http;

        public event Action OnChange;
        public int Coins { get; set; } = 1000;

        public CoinsService(HttpClient http)
        {
            _http = http;
        }
        public void SpendCoins(int amount)
        {
            Coins -= amount;
            CoinsChanged();
        }
        void CoinsChanged()
        {
            OnChange.Invoke();
        }

        public async Task AddCoins(int amount)
        {
            var result = await _http.PutAsJsonAsync<int>("api/user/addcoins", amount);
            Coins = await result.Content.ReadFromJsonAsync<int>();
            CoinsChanged();
        }
        public void RestoreCoins(int amount)
        {
            Coins += amount;
            CoinsChanged();
        }

        public async Task GetCoins()
        {
            Coins = await _http.GetFromJsonAsync<int>("api/user/getcoins");
            CoinsChanged();
        }
    }
}
