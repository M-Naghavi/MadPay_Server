using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MadPay724.Services.Upload.Service
{
    public class UploadService : IUploadService
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly Cloudinary _cloudinary;
        private readonly Setting _setting;
        public UploadService(IUnitOfWork<MalpayDbContext> dbContext)
        {
            _db = dbContext;
            _setting = _db.SettingRepository.GetById((short)1);
            Account account = new Account(
                _setting.CloudinaryCloudName,
                _setting.CloudinaryAPIKey,
                _setting.CloudinaryAPISecret
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<FileUploadedDto> UploadProfilePicture(IFormFile file, string UserId, string WebRootPath, string urlBegan)
        {
            if (_setting.UploadLocal)
            {
               return await UploadProfileToLocal(file,  WebRootPath,  urlBegan,  UserId);
            }
            else
            {
                return UploadProfileToCloudinary(file, UserId);
            }
        }
        public async Task<FileUploadedDto> UploadProfileToLocal(IFormFile file, string WebRootPath, string urlBegan,string UserId)
        {
            if (file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    string fileNewName = string.Format("{0}{1}", UserId, fileExtension);
                    string path = Path.Combine(WebRootPath, "Files\\Pic\\Profile");
                    string fullPath = Path.Combine(path, fileNewName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return new FileUploadedDto()
                    {
                        Status = true,
                        LocalUploaded = true,
                        Message = "فایل با موفقیت در لوکال اپلود شد",
                        PublicId = "0",
                        Url = string.Format("{0}/{1}",urlBegan, "wwwroot/Files/Pic/Profile/" + fileNewName)
                    };
                }
                catch (Exception ex)
                {
                    return new FileUploadedDto()
                    {
                        Status = false,
                        Message = ex.Message

                    };
                }
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false,
                    Message = "فایلی برای اپلود یافت نشد"

                };
            }
        }
        public FileUploadedDto UploadProfileToCloudinary(IFormFile file, string UserId)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(250).Height(250).Crop("fill").Gravity("face"),
                            Folder = "ProfilePic/" + UserId
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                        if (uploadResult.Error == null)
                        {
                            return new FileUploadedDto()
                            {
                                LocalUploaded = false,
                                Status = true,
                                Message = "فایل با موفقیت در فضای ابری اپلود شد",
                                PublicId = uploadResult.PublicId,
                                Url = uploadResult.Url.ToString()
                            };
                        }
                        else
                        {
                            return new FileUploadedDto()
                            {
                                Status = false,
                                Message = uploadResult.Error.Message

                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new FileUploadedDto()
                    {
                        Status = false,
                        Message = ex.Message

                    };

                }
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false,
                    Message = "فایلی برای اپلود یافت نشد"

                };
            }
        }
        public FileUploadedDto RemoveFileFromCloudinary(string PublicId)
        {
            var deleteParams = new DeletionParams(PublicId);
            var deleteResult = _cloudinary.Destroy(deleteParams);
            if (deleteResult.Result.ToLower() == "ok")
            {
                return new FileUploadedDto()
                {
                    Status = true,
                    Message = "فایل با موفقیت حذف شد"
                };
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false,
                    Message = deleteResult.Error.Message

                };
            }
        }
        public FileUploadedDto RemoveFileFromLocal(string PhotoName, string WebRootPath , string FilePath)
        {
            string path = Path.Combine(WebRootPath, FilePath);
            string fullPath = Path.Combine(path, PhotoName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return new FileUploadedDto()
                {
                    Status = true,
                    Message = "فایل با موفقیت حذف شد"
                };
            }
            else
            {
                return new FileUploadedDto()
                {
                    Status = false,
                    Message = "فایلی وچود نداشت"

                };
            }
        }
    }
}
