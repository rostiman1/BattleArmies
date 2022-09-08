using BattleArmies.Server.Data;
using BattleArmies.Server.Services;
using BattleArmies.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattleArmies.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly DataContext _context;

        public UnitController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            var units = await _context.Units.ToListAsync();
            return Ok(units);
        }

        [HttpPost]
        public async Task<IActionResult> AddUnit(Unit unit)
        {
            await _context.Units.AddAsync(unit);
            await _context.SaveChangesAsync();
            return Ok(await _context.Units.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, Unit unit)
        {
            Unit dbUnit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id);
            if (dbUnit == null)
            {
                return NotFound("Unit with the given id doesn't exist.");
            }

            dbUnit.Title = unit.Title;
            dbUnit.Attack = unit.Attack;
            dbUnit.Defense = unit.Defense;
            dbUnit.CoinsCost = unit.CoinsCost;
            dbUnit.HitPoints = unit.HitPoints;

            await _context.SaveChangesAsync();

            return Ok(dbUnit);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            Unit dbUnit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id);
            if (dbUnit == null)
            {
                return NotFound("Unit with the given id doesn't exist.");
            }

            _context.Units.Remove(dbUnit);
            await _context.SaveChangesAsync();

            return Ok(await _context.Units.ToListAsync());
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> CreateAUnitOnSpecifiedId(int id)
        {
            Unit dbUnit = await _context.Units.FirstOrDefaultAsync(u => u.Id == id);
            if (dbUnit == null)
            {
                _context.Units.Add(dbUnit);
                await _context.SaveChangesAsync();
            }
            return Ok(await _context.Units.ToListAsync());
        }
    }
}
