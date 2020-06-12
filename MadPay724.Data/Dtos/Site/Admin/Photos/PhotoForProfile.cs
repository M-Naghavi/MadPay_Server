using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Photos
{
    public class PhotoForProfile
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string PublicId { get; set; }

        public string Description { get; set; } = "profile pic";
        public string Alt { get; set; } = "profile pic";
        public bool IsMain { get; set; } = true;
    }
}
