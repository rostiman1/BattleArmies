using BattleArmies.Shared;

namespace BattleArmies.Client.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegister request);
        Task<ServiceResponse<string>> Login(UserLogin request);
    }
}
