using BattleArmies.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BattleArmies.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;
        private readonly ICoinsService _coinsService;

        public CustomAuthStateProvider(ILocalStorageService localStorageService,HttpClient http, ICoinsService coinsService)
        {
           _localStorageService = localStorageService;
            _http = http;
            _coinsService = coinsService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authToken = await _localStorageService.GetItemAsStringAsync("authToken");
            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                   await _coinsService.GetCoins();
                }
                catch (Exception)
                {

                    await _localStorageService.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
                
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);


            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
        private byte[] ParsBase64NoPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2: base64 += "==";break;
                case 3: base64 += "=";break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParsBase64NoPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>> (jsonBytes);
            var claims = keyValuePairs.Select(kvp=> new Claim(kvp.Key, kvp.Value.ToString()));

            return claims;
        }
    }
}
