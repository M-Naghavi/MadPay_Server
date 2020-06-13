using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models
{
    public class Setting : BaseEntity<short>
    {
        public Setting()
        {
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [Required]
        [StringLength(maximumLength: 0, MinimumLength = 200)]
        public string CloudinaryCloudName { get; set; }

        [Required]
        [StringLength(maximumLength: 0, MinimumLength = 200)]
        public string CloudinaryAPIKey { get; set; }

        [Required]
        [StringLength(maximumLength: 0, MinimumLength = 200)]
        public string CloudinaryAPISecret { get; set; }

        [Required]
        public bool UploadLocal { get; set; }

    }
}
