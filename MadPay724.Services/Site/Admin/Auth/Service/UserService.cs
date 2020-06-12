using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Admin.Auth.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        public UserService(IUnitOfWork<MalpayDbContext> dbContext)
        {
            _db = dbContext;
        }

        public async Task<User> GetUserForPassChange(string id, string password)
        {
            var user = await _db.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            if (!Utilities.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<bool> UpdateUserPassword(User user, string newPassword)
        {
            byte[] passwordHash, passwordSalt;
            Utilities.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _db.UserRepository.Update(user);
            return await _db.SaveAsync();
        }
    }
}
