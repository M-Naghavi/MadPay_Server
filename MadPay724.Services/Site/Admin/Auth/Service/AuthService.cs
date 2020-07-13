using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Service.Site.Admin.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IUtilities _utilities;

        public AuthService(IUnitOfWork<MalpayDbContext> dbContext, IUtilities utilities)
        {
            _db = dbContext;
            _utilities = utilities;
        }

        public async Task<User> Login(string username, string password)
        {
            var users = await _db.UserRepository.GetManyAsync(x => x.UserName == username, null, "Photos,BankCards");
            var user  = users.SingleOrDefault();

            if (user == null)
            {
                return null;
            }

            //if (!_utilities.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //{
            //    return null;
            //}

            return user;
        }

        public async Task<User> Register(User user,Photo photo, string password)
        {
            byte[] passwordHash, passwordSalt;
            _utilities.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            await _db.UserRepository.InsertAsync(user);
            await _db.PhotoRepository.InsertAsync(photo);
            if (await _db.SaveAsync())
            {
                return user;
            }
            else
            {
                return null;
            }

            
        }
    }
}
