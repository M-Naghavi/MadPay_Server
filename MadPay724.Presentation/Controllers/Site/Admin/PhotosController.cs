using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [Route("site/admin/users/{userId}/photos")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Site")]
    public class PhotosController : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IUnitOfWork<MalpayDbContext> dbContext,
                               IMapper mapper,
                               IOptions<CloudinarySettings> cloudinarySettings)
        {
            _db = dbContext;
            _mapper = mapper;

            _cloudinarySettings = cloudinarySettings;
            Account account = new Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.APIKey,
                _cloudinarySettings.Value.APISecret
                );
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserPhoto(string userId, [FromForm] PhotoForProfile photoForProfile)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != userId)
                return Unauthorized("شما اجازه تغییر تصویر این کاربر را ندارید");

            //var userFromRepo = await _db.UserRepository.GetByIdAsync(userId);

            var file = photoForProfile.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(250).Height(250).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForProfile.Url = uploadResult.Url.ToString();
            photoForProfile.PublicId = uploadResult.PublicId;

            var oldPhoto = await _db.PhotoRepository.GetAsync(x => x.UserId == userId && x.IsMain);
            if (oldPhoto.PublicId != null && oldPhoto.PublicId != "0")
            {
                var deleteParams = new DeletionParams(oldPhoto.PublicId);
                var deleteResult = _cloudinary.Destroy(deleteParams);
                //if (deleteResult.Result.ToLower() == "ok")
                //{

                //}
            }


            _mapper.Map(photoForProfile, oldPhoto);

            _db.PhotoRepository.Update(oldPhoto);
            if (await _db.SaveAsync())
            {
                var photoForReturn = _mapper.Map<PhotoForReturnProfileDto>(oldPhoto);
                return CreatedAtRoute("GetPhoto", new { id = oldPhoto.Id }, photoForReturn);
            }
            else
            {
                return BadRequest("خطایی در اپلود، دوباره امتحان کنید");
            }
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(string id)
        {
            var photoFromRepo = await _db.PhotoRepository.GetByIdAsync(id);
            var photo = _mapper.Map<PhotoForReturnProfileDto>(photoFromRepo);
            return Ok(photo);
        }

    }
}
