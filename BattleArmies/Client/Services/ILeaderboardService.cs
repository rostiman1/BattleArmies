using BattleArmies.Shared;

namespace BattleArmies.Client.Services
{
    public interface ILeaderboardService
    {
        IList<UserStatistic> Leaderboard { get; set; }
        Task GetLeaderboard();
    }
}
