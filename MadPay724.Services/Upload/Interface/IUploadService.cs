using MadPay724.Data.Dtos.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Upload.Interface
{
    public interface IUploadService
    {
        FileUploadedDto UploadProfileToCloudinary(IFormFile file, string UserId);
        Task<FileUploadedDto> UploadProfileToLocal(IFormFile file, string WebRootPath, string urlBegan, string UserId);
        Task<FileUploadedDto> UploadProfilePicture(IFormFile file, string UserId, string WebRootPath, string urlBegan);
        FileUploadedDto RemoveFileFromCloudinary(string PublicId);
        FileUploadedDto RemoveFileFromLocal(string PhotoName, string WebRootPath, string FilePath);
    }
}
