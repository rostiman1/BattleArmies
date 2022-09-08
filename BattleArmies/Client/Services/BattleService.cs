using BattleArmies.Shared;
using System.Net.Http.Json;

namespace BattleArmies.Client.Services
{
    public class BattleService : IBattleService
    {
        private readonly HttpClient _http;

        public BattleService(HttpClient http)
        {
            _http = http;
        }
        public BattleResults LastBattle { get; set; } = new BattleResults();
        public async Task<BattleResults> StartBattle(int opponentId)
        {
            var result = await _http.PostAsJsonAsync("api/battle", opponentId);
            LastBattle =  await result.Content.ReadFromJsonAsync<BattleResults>();
            return LastBattle;
        }
    }
}
