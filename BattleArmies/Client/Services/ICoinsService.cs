namespace BattleArmies.Client.Services
{
    public interface ICoinsService
    {
       public event Action OnChange;
        int Coins { get; set; }
        void SpendCoins(int amount);
        Task AddCoins(int amount);
        void RestoreCoins(int amount);
        Task GetCoins();
    }
}
