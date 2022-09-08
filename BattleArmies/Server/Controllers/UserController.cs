using BattleArmies.Server.Data;
using BattleArmies.Server.Services;
using BattleArmies.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BattleArmies.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityService;

        public UserController(DataContext context, IUtilityService utilityService)
        {
            _context = context;
            _utilityService = utilityService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private async Task<User> GetUser() => await _context.Users.FirstOrDefaultAsync(x => x.Id == GetUserId());

        [HttpGet("getcoins")]
        public async Task<IActionResult> GetCoins()
        {
            var user = await _utilityService.GetUser();
            return Ok(user.Coins);
        }

        [HttpPut("addcoins")]
        public async Task<IActionResult> AddCoins([FromBody] int coins)
        {
            var user = await _utilityService.GetUser();
            user.Coins += coins;
            await _context.SaveChangesAsync();
            return Ok(user.Coins);
        }
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var users = await _context.Users.Where(x => !x.IsDeleted && x.IsConfirmed).ToListAsync();

            users = users.OrderByDescending(x => x.Victories).ThenBy(x => x.Battles).ThenBy(x => x.DateOfCreation).ToList();

            int rank = 1;
            var response = users.Select(user => new UserStatistic
            {
                Rank = rank++,
                UserId = user.Id,
                Username = user.Username,
                Battles = user.Battles,
                Victories = user.Victories,
                Defeats = user.Defeats,
            });
            return Ok(response);
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var user = await _utilityService.GetUser();
            var battles = await _context.Battles.Where(x => x.AttackerId == user.Id || x.OpponentId == user.Id)
                .Include(x => x.Attacker)
                .Include(x => x.Opponent)
                .Include(x => x.Winner)
                .ToListAsync();
            var history = battles.Select(x => new BattleHistoryEntry
            {
                BattleId = x.Id,
                AttackerId = x.AttackerId,
                OpponentId = x.OpponentId,
                IsVictory = x.WinnerId == user.Id,
                AttackerName = x.Attacker.Username,
                OpponentName = x.Opponent.Username,
                RoundsFought = x.RoundsFought,
                VictoriousDamage = x.WinnerDamage,
                BattleDate = x.BattleTime
            });
            return Ok(history.OrderByDescending(x=>x.BattleDate));
        }
    }
}
