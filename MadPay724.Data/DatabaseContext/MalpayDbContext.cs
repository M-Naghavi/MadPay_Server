using MadPay724.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.DatabaseContext
{
    public class MalpayDbContext : IdentityDbContext<User,
                                                     Role,
                                                     string,
                                                     IdentityUserClaim<string>, 
                                                     UserRole , 
                                                     IdentityUserLogin<string>, 
                                                     IdentityRoleClaim<string>, 
                                                     IdentityUserToken<string>>
    {
        public MalpayDbContext()
        {

        }

        public MalpayDbContext(DbContextOptions<MalpayDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=MadPay724db; Integrated Security=true; MultipleActiveResultSets=true");
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<Setting> Settings { get; set; }

    }
}
