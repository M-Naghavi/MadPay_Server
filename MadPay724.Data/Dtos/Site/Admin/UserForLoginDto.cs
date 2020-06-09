using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin
{
    public class UserForLoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "email not exist")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsRemember { get; set; }
    }
}
