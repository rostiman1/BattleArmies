using BattleArmies.Server.Data;
using BattleArmies.Server.Services;
using BattleArmies.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattleArmies.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityServer;

        public BattleController(DataContext context, IUtilityService utilityServer)
        {
            _context = context;
            _utilityServer = utilityServer;
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] int opponentId)
        {
            var attacker = await _utilityServer.GetUser();
            var opponent = await _context.Users.FindAsync(opponentId);
            if(opponent == null || opponent.IsDeleted)
            {
                return NotFound("Opponent not available");
            }
            var result = new BattleResults();
            await Fight(attacker, opponent, result);

            return Ok(result);
        }
        private async Task Fight(User attacker, User opponent, BattleResults result)
        {
            var attackerArmy = await _context.UserUnits
                .Where(x => x.UserId == attacker.Id && x.HitPoints > 0)
                .Include(x => x.Unit)
                .ToListAsync();
            var opponentArmy = await _context.UserUnits
                .Where(x => x.UserId == opponent.Id && x.HitPoints > 0)
                .Include(x => x.Unit)
                .ToListAsync();

            var attackerDmgSum = 0;
            var opponentDmgSum = 0;
            var currentRound = 0;

            while (attackerArmy.Count > 0 && opponentArmy.Count > 0)
            {
                currentRound++;
                if(currentRound % 2 != 0) 
                {
                    attackerDmgSum += FightRound(attacker, opponent, attackerArmy, opponentArmy,result);
                }
                else
                {
                    opponentDmgSum += FightRound(attacker, opponent, attackerArmy, opponentArmy,result);
                }
                result.IsVictory = opponentArmy.Count == 0;
                result.RoundsFought = currentRound;

                if(result.RoundsFought > 0)
                {
                    await FinishFight(attacker, opponent, result, attackerDmgSum, opponentDmgSum);
                }
            }
        }

        private int FightRound(User attacker, User opponent, List<UserUnit> attackerArmy, List<UserUnit> opponentArmy, BattleResults result)
        {
            int randomAttackerIndex = new Random().Next(attackerArmy.Count);  
            int randomOpponentIndex = new Random().Next(opponentArmy.Count);

            var randomAttacker = attackerArmy[randomAttackerIndex];
            var randomOpponent = opponentArmy[randomOpponentIndex];

            var damage = new Random().Next(randomAttacker.Unit.Attack) - new Random().Next(randomOpponent.Unit.Defense);
            if(damage < 0)
            {
                damage = 0;
            }
            if(damage <= randomOpponent.HitPoints)
            {
                randomOpponent.HitPoints -= damage;
                result.Log.Add(
                    $"{attacker.Username}'s {randomAttacker.Unit.Title} attacks" +
                    $"{opponent.Username}'s {randomOpponent.Unit.Title} the amount is {damage}");
                return damage;
            }
            else
            {
                damage = randomOpponent.HitPoints;
                randomOpponent.HitPoints = 0;
                opponentArmy.Remove(randomOpponent);
                result.Log.Add(
                    $"{attacker.Username}'s {randomAttacker.Unit.Title} kills" +
                    $"{opponent.Username}'s {randomOpponent.Unit.Title}!!!");
                return damage;
            }
        }
        private async Task FinishFight(User attacker, User opponent, BattleResults result, int attackerDmgSum, int opponentDmgSum)
        {
            result.AttackerDmgSum = attackerDmgSum;
            result.OpponentDmgSum = opponentDmgSum;

            attacker.Battles++;
            opponent.Battles++;

           if(result.IsVictory)
            {
                attacker.Victories++;
                opponent.Defeats++;
            }
            else
            {
                opponent.Victories++;
                attacker.Defeats++;
            }


            StoreBattleHistory(attacker, opponent, result);

            await _context.SaveChangesAsync();
        }

        private void StoreBattleHistory(User attacker, User opponent, BattleResults result)
        {
            var battle = new Battle();
            battle.Attacker = attacker;
            battle.Opponent = opponent;
            battle.RoundsFought = result.RoundsFought;
            battle.WinnerDamage = result.IsVictory ? result.AttackerDmgSum : result.OpponentDmgSum;
            battle.Winner = result.IsVictory ? attacker : opponent;

            _context.Battles.Add(battle);
        }
    }
}
