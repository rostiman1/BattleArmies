using BattleArmies.Shared;

namespace BattleArmies.Client.Services
{
    public interface IBattleService
    {
        BattleResults LastBattle { get; set; }
        Task<BattleResults> StartBattle(int opponentId);
    }
}
