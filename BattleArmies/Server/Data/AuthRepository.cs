using BattleArmies.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BattleArmies.Server.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthRepository(DataContext context,  IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found!";
            }
            else if (!VerifyPassHash(password,user.PasswordHash,user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Not the correct pass!";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password,int startUnitId)
        {
            if(await UserExists(user.Email))
            {
                return new ServiceResponse<int> {Success = false, Message = "User already exists" };
            }
            CreatePassHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
             _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await AddStartingUnit(user, startUnitId);

            return new ServiceResponse<int> { Data = user.Id, Message = "Registration successful!"};
        }

      

        public async Task<bool> UserExists(string email)
        {
           if(await _context.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower())))
            {
                return true;
            }
            return false;
        }
        private void CreatePassHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassHash(string password,  byte[] passwordHash,  byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        private async Task AddStartingUnit(User user, int startUnitId)
        {
            var unit = await _context.Units.FirstOrDefaultAsync<Unit>(x => x.Id == startUnitId);
          await _context.UserUnits.AddAsync(new UserUnit
            {
                UnitId = unit.Id,
                UserId = user.Id,
                HitPoints = unit.HitPoints
            });
            await _context.SaveChangesAsync();
        }
    }
}
