﻿using MadPay724.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Seed.Interface
{
    public interface ISeedService
    {
        Task SeedUsersAsync();
        void SeedUsers();
    }
}
