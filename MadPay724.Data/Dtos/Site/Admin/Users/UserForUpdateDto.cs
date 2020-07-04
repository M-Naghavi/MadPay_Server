using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserForUpdateDto
    {
        [StringLength(maximumLength: 100, MinimumLength = 0)]
        [Required]
        public string Name { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 0)]
        [Required]
        public string PhoneNumber { get; set; }

        [StringLength(maximumLength: 500, MinimumLength = 0)]
        [Required]
        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 0)]
        public string City { get; set; }
    }
}
