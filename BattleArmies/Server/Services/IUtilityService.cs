using BattleArmies.Shared;

namespace BattleArmies.Server.Services
{
    public interface IUtilityService
    {
        Task<User> GetUser();
    }
}
