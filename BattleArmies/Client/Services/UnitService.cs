using BattleArmies.Shared;
using Blazored.Toast.Services;
using System.Net.Http.Json;

namespace BattleArmies.Client.Services
{
    public class UnitService : IUnitService
    {
        private readonly IToastService _toastService;
        private readonly HttpClient _http;
        private readonly ICoinsService _coinsService;

        public UnitService(IToastService toastService,HttpClient http, ICoinsService coinsService)
        {
            _toastService = toastService;
            _http = http;
            _coinsService = coinsService;
        }
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

        public async Task AddUnit(int unitId)
        {
            var unit = Units.First(x => x.Id == unitId);
            var result = await _http.PostAsJsonAsync<int>("api/UserUnit", unitId);
            if(result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _toastService.ShowError(await result.Content.ReadAsStringAsync());  
            }
            // if (MyUnits.Count > 14)
            //{
            //    _toastService.ShowInfo($"{MyUnits.Count} soldiers! Your army is ready go and conquer the universe","Done");
            //     return;
            //}
            else
            {
                await _coinsService.GetCoins();
                _toastService.ShowSuccess($"Your {unit.Title} has been built!");
            }
        }
        public async Task LoadUnitsAsync()
        {
            if (Units.Count == 0)
            {
                Units = await _http.GetFromJsonAsync<IList<Unit>>("api/unit");
            }
        }

        public async Task LoadUserFromUnitsAsync()
        {
            MyUnits = await _http.GetFromJsonAsync<IList<UserUnit>>("api/UserUnit");
        }

        public async Task ReviveArmy()
        {
            var result = await _http.PostAsJsonAsync<string>("api/userunit/revive", null);
            if(result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _toastService.ShowSuccess(await result.Content.ReadAsStringAsync());
            }
            else
            {
                _toastService.ShowError(await result.Content.ReadAsStringAsync());
            }
            await LoadUnitsAsync();
            await _coinsService.GetCoins();
        }
    }
}
