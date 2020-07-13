using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Seed.Interface;
using Microsoft.AspNetCore.Identity;
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
        //private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly UserManager<User> _userManager;
        private readonly IUtilities _utilities;

        //public SeedService(IUnitOfWork<MalpayDbContext> dbContext , IUtilities utilities)
        public SeedService(UserManager<User> userManager, IUtilities utilities)
        {
            _userManager = userManager;
            _utilities = utilities;
        }

        public async Task SeedUsersAsync()
        {
            if (!_userManager.Users.Any())
            {
                string userData = System.IO.File.ReadAllText("wwwroot/Files/Json/Seed/UserSeedData.json");
                IList<User> users = JsonConvert.DeserializeObject<IList<User>>(userData);
                foreach (var user in users)
                {
                    await _userManager.CreateAsync(user, "password");
                }
            }
        }
        public void SeedUsers()
        {
            try
            {
                if (!_userManager.Users.Any())
                {
                    string userData = System.IO.File.ReadAllText("wwwroot/Files/Json/Seed/UserSeedData.json");
                    IList<User> users = JsonConvert.DeserializeObject<IList<User>>(userData);

                    foreach (var user in users.ToList())
                    {
                      _userManager.CreateAsync(user, "12345678").Wait();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
