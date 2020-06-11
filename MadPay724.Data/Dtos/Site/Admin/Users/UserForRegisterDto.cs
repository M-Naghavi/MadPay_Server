using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserForRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "email not exist")]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 4, ErrorMessage = "paswword must 4 character")]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PnoneNumber { get; set; }
    }
}
