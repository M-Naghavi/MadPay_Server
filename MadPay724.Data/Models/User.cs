using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models
{
    public class User : IdentityUser
    {
        [StringLength(maximumLength: 100, MinimumLength = 0)]
        [Required]
        public string Name { get; set; }

        [StringLength(maximumLength: 500, MinimumLength = 0)]
        [Required]
        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime LastActive { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 0)]
        public string City { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool Status { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<BankCard> BankCards { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
