using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Repository.Repositories.Repo
{
    public class PhotoRepository:  Repository<Photo>, IPhotoRepository
    {
        private readonly DbContext _db;
        public PhotoRepository(DbContext dbContext) : base(dbContext)
        {
            _db = (_db ?? (MalpayDbContext)_db);
        }


    }
}
