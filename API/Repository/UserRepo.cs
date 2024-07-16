using API.Data;
using API.Models;
using API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        public UserRepo(ApplicationDbContext context,IOptions<AppSettings> appSettings) 
        {
            _context = context;
            _appSettings = (AppSettings)appSettings;
        }
        public User Authenticate(string username, string password)
        {
            var userInDb=_context.Users.FirstOrDefault(u=>u.Username == username &&
            u.Password==password);
            if (userInDb == null) return null;
            //JWT
            var Tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokendecription = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,userInDb.Id.ToString()),
                    new Claim(ClaimTypes.Role,userInDb.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = Tokenhandler.CreateToken(tokendecription);
            userInDb.Token=Tokenhandler.WriteToken(token);
            //
            userInDb.Password = "";
            return userInDb;
        }

        public bool IsUniqueUser(string username)
        {
            var userIndb=_context.Users.FirstOrDefault(u=>u.Username == username);

            if (userIndb == null) return true; return false;
        }

        public User Register(string username, string password)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Role = "Admin"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
    }
}
