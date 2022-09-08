using BattleArmies.Server.Data;
using BattleArmies.Shared;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BattleArmies.Server.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpAccessor;

        public UtilityService(DataContext context, IHttpContextAccessor httpAccessor)
        {
            _context = context;
            _httpAccessor = httpAccessor;
        }
        public async Task<User> GetUser()
        {
            var userID = int.Parse(_httpAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == userID);
            return user;
        }
    }
}
