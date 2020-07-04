using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.v1;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [Authorize]
    //[Route("api/v1/site/admin/users/{userId}/photos")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1_Site_Admin")]
    public class PhotosController : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IUploadService _uploadService { get; }
        public PhotosController(IUnitOfWork<MalpayDbContext> dbContext,
                               IMapper mapper,
                               IUploadService uploadService,
                               IWebHostEnvironment webHostEnvironment)
        {
            _db = dbContext;
            _mapper = mapper;
            _uploadService = uploadService;
            _webHostEnvironment = webHostEnvironment;
        }

        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpPost(ApiV1Routes.Photos.ChangeUserPhoto)]
        public async Task<IActionResult> ChangeUserPhoto(string userId, [FromForm] PhotoForProfile photoForProfile)
        {
            var path = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value ?? "", Request.PathBase.Value ?? "");
            var uploadResult =await _uploadService.UploadProfilePicture(photoForProfile.File, userId, _webHostEnvironment.WebRootPath,path);
            if (uploadResult.Status)
            {
                if (uploadResult.LocalUploaded)
                    photoForProfile.PublicId = "1";
                else
                    photoForProfile.PublicId = uploadResult.PublicId;
                photoForProfile.Url = uploadResult.Url;


                var oldPhoto = await _db.PhotoRepository.GetAsync(x => x.UserId == userId && x.IsMain);
                if (oldPhoto.PublicId != null && oldPhoto.PublicId != "0" && oldPhoto.PublicId != "1")
                {
                    _uploadService.RemoveFileFromCloudinary(oldPhoto.PublicId);
                }
                if (oldPhoto.PublicId == photoForProfile.PublicId && photoForProfile.Url.Split('/').Last() != oldPhoto.Url.Split('/').Last())
                {
                    _uploadService.RemoveFileFromLocal(oldPhoto.Url.Split('/').Last(), _webHostEnvironment.WebRootPath, "Files\\Pic\\Profile");
                }
                if (oldPhoto.PublicId == "1" && photoForProfile.PublicId != "1")
                {
                    _uploadService.RemoveFileFromLocal(oldPhoto.Url.Split('/').Last(), _webHostEnvironment.WebRootPath, "Files\\Pic\\Profile");
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
            else
            {
                return BadRequest(uploadResult.Message);
            }
        }

        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Photos.GetPhoto, Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(string id)
        {
            var photoFromRepo = await _db.PhotoRepository.GetByIdAsync(id);
            var photo = _mapper.Map<PhotoForReturnProfileDto>(photoFromRepo);
            return Ok(photo);
        }

    }
}
