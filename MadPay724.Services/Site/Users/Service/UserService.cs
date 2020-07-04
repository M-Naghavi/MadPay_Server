using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Users.Interface;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Users.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IUtilities _Utilities;

        public UserService(IUnitOfWork<MalpayDbContext> dbContext , IUtilities utilities)
        {
            _db = dbContext;
            _Utilities = utilities;
        }


        public async Task<User> GetUserForPassChange(string id, string password)
        {
            var user = await _db.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            if (!_Utilities.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<bool> UpdateUserPassword(User user, string newPassword)
        {
            byte[] passwordHash, passwordSalt;
            _Utilities.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _db.UserRepository.Update(user);
            return await _db.SaveAsync();
        }
    }
}
