﻿using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Users.Interface
{
    public interface IUserService
    {
        Task<User> GetUserForPassChange(string id, string password);
        Task<bool> UpdateUserPassword(User user, string password);
    }
}
