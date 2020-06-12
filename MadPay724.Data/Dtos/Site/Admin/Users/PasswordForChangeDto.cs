using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class PasswordForChangeDto
    {
        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 4, ErrorMessage = "paswword must 4 character")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 4, ErrorMessage = "paswword must 4 character")]
        public string NewPassword { get; set; }
    }
}
