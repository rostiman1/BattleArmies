using BattleArmies.Shared;

namespace BattleArmies.Client.Services
{
    public interface IUnitService
    {
        IList<Unit> Units { get; set; }
        IList<UserUnit>? MyUnits { get; set; }
        Task AddUnit(int unitId);
        Task LoadUnitsAsync();
        Task LoadUserFromUnitsAsync();
        Task ReviveArmy();
    }
}
