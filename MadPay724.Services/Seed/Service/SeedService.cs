using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Seed.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MadPay724.Services.Seed.Service
{
    public class SeedService : ISeedService
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IUtilities _utilities;

        public SeedService(IUnitOfWork<MalpayDbContext> dbContext , IUtilities utilities)
        {
            _db = dbContext;
            _utilities = utilities;
        }

        public async Task SeedUsersAsync()
        {
            if (_db.UserRepository.GetAll().Count() <= 0)
            {
                string userData = System.IO.File.ReadAllText("wwwroot/Files/Json/Seed/UserSeedData.json");
                IList<User> users = JsonConvert.DeserializeObject<IList<User>>(userData);

                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    _utilities.CreatePasswordHash("123456", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    user.UserName = user.UserName.ToLower();
                    await _db.UserRepository.InsertAsync(user);
                }

                await _db.SaveAsync();
            }
        }
        public void SeedUsers()
        {
            if (_db.UserRepository.GetAll().Count() <= 0)
            {
                string userData = System.IO.File.ReadAllText("Files/Json/Seed/UserSeedData.json");
                IList<User> users = JsonConvert.DeserializeObject<IList<User>>(userData);

                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    _utilities.CreatePasswordHash("123456", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    user.UserName = user.UserName.ToLower();
                    _db.UserRepository.Insert(user);
                }
                _db.Save();
            }
        }
    }
}
