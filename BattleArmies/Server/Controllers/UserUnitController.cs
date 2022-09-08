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
    public class UserUnitController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityService;

        public UserUnitController(DataContext context, IUtilityService utilityService)
        {
            _context = context;
            _utilityService = utilityService;
        }

        [HttpPost("revive")]
        public async Task<IActionResult> ReviveArmy()
        {
            var user = await _utilityService.GetUser();
            var userUnits = await _context.UserUnits
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Unit)
                .ToListAsync();
            int coinsCost = 5000;
            if(user.Coins < coinsCost)
            {
                return BadRequest("Not enough coins!You need 5000 coins to revive your army");
            }

            bool armyAlreadyAlive = true;
            foreach (var item in userUnits)
            {
                if(item.HitPoints <= 0)
                {
                    armyAlreadyAlive = false;
                    item.HitPoints = new Random().Next(0, item.Unit.HitPoints);
                }
            }

            if (armyAlreadyAlive == true)
            {
                return BadRequest("Your army is already alive!");
            }
            user.Coins -= coinsCost;
            await _context.SaveChangesAsync();
            return Ok("Army revived");
        }

        [HttpPost]
        public async Task<IActionResult> BuildUserUnit([FromBody] int unitId)
        {
            var unit = await _context.Units.FirstOrDefaultAsync<Unit>(x => x.Id == unitId);
            var user = await _utilityService.GetUser();
            if(user.Coins < unit.CoinsCost)
            {
                return BadRequest("Not enough coins!");
            }
            user.Coins -= unit.CoinsCost;
            var newUserUnit = new UserUnit
            {
                UnitId = unit.Id,
                UserId = user.Id,
                HitPoints = unit.HitPoints
            };
            _context.UserUnits.Add(newUserUnit);
            await _context.SaveChangesAsync();
            return Ok(newUserUnit);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserUnits()
        {
            var user = await _utilityService.GetUser();
            var userUnits = await _context.UserUnits.Where(x=>x.UserId == user.Id).ToListAsync();

            var response = userUnits.Select(x => new UserUnitResponse { UnitId = x.UnitId, HitPoints = x.HitPoints });
            return Ok(response);
            
        }
    }
}
