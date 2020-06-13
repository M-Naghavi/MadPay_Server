using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [Route("site/admin/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Site")]
    public class Auth : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IAuthService _authService;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;

        public Auth(IUnitOfWork<MalpayDbContext> dbContext, 
                    IAuthService authService, 
                    IConfiguration configuration,
                    IMapper mapper)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
            _db = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _db.UserRepository.UserExist(userForRegisterDto.UserName))
            {
                return BadRequest(new ReturnMessage()
                {
                    Message = "Username is exist",
                    Status = false,
                    Title = "Error"
                });
            }

            var userToCreate = new User
            {
                UserName = userForRegisterDto.UserName,
                Address = "",
                City = "",
                Gender = "male",
                BirthDate = DateTime.Now,
                IsActive = true,
                Status = true,
                Name = userForRegisterDto.Name,
                PhoneNumber = userForRegisterDto.PnoneNumber
            };
            //var uri = Server.MapPath("~/Files/Pic/ProfilePic.png");
            var photoToCreate = new Photo
            {
               UserId = userToCreate.Id,
               //Url = "https://res.cloudinary.com/dlfpc2qk8/image/upload/v1591948165/download_lksuyt.png",
               Url = string.Format("{0}://{1}{2}/{3}",
                                    Request.Scheme , 
                                    Request.Host.Value , 
                                    Request.PathBase.Value,
                                    "wwwroot/Files/Pic/ProfilePic.png"),
               Description = "Profile Pic",
               Alt = "Profile Pic",
               IsMain = true,
               PublicId = "0"
            };

            var createdUser = _authService.Register(userToCreate, photoToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForRegisterDto)
        {
            var userFromRepo = await _authService.Login(userForRegisterDto.UserName, userForRegisterDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized("کاربری با این یوزر و پسورد وجود ندارد");
                //return Unauthorized(new ReturnMessage()
                //{
                //    Message = "user not found",
                //    Status = false,
                //    Title = "Error"
                //});
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name , userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettingToken:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDes = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = userForRegisterDto.IsRemember ? DateTime.Now.AddDays(1) : DateTime.Now.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDes);

            UserForDetailedDto user = _mapper.Map<UserForDetailedDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

    }
}
