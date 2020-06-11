using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models
{
    public class User : BaseEntity<string>
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [StringLength(maximumLength: 0, MinimumLength = 100)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string UserName { get; set; }

        [StringLength(maximumLength: 0, MinimumLength = 100)]
        [Required]
        public string PhoneNumber { get; set; }

        [StringLength(maximumLength: 0, MinimumLength = 500)]
        [Required]
        public string Address { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime LastActive { get; set; }

        [StringLength(maximumLength: 0, MinimumLength = 100)]
        public string City { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool Status { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<BankCard> BankCards { get; set; }
    }
}
