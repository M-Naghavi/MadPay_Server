using MadPay724.Repository.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Repository.Infrastructure
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        bool Save();
        Task<bool> SaveAsync();

        IUserRepository UserRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        ISettingRepository SettingRepository { get; }
    }
}
