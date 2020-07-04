using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Routes.v1;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [Authorize]
    //[Route("api/v1/site/admin/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1_Site_Admin")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IAuthService _authService;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUnitOfWork<MalpayDbContext> dbContext,
                    IAuthService authService,
                    IConfiguration configuration,
                    IMapper mapper,
                    ILogger<AuthController> logger)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
            _db = dbContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost(ApiV1Routes.Auth.Register)]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _db.UserRepository.UserExist(userForRegisterDto.UserName))
            {
                _logger.LogWarning($"{userForRegisterDto.Name}-{userForRegisterDto.UserName} میخواهد دوباره ثبت نام کند");
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
                                    Request.Scheme,
                                    Request.Host.Value ?? "",
                                    Request.PathBase.Value ?? "",
                                    "wwwroot/Files/Pic/ProfilePic.png"),
                Description = "Profile Pic",
                Alt = "Profile Pic",
                IsMain = true,
                PublicId = "0"
            };

            var createdUser = await _authService.Register(userToCreate, photoToCreate, userForRegisterDto.Password);
            if (createdUser != null)
            {
                var userForReturn = _mapper.Map<UserForDetailedDto>(createdUser);
                _logger.LogInformation($"{userForRegisterDto.Name}-{userForRegisterDto.UserName} ثبت نام کرد");
                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    id = createdUser.Id
                },userForReturn);
            }
            else
            {
                _logger.LogWarning($"{userForRegisterDto.Name}-{userForRegisterDto.UserName} میخواهد دوباره ثبت نام کند");
                return BadRequest(new ReturnMessage()
                {
                    Message = "Username is exist",
                    Status = false,
                    Title = "ثبت در دیتابیس"
                });
            }
        }

        [AllowAnonymous]
        [HttpPost(ApiV1Routes.Auth.Login)]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _authService.Login(userForLoginDto.UserName, userForLoginDto.Password);

            if (userFromRepo == null)
            {
                _logger.LogWarning($"{userForLoginDto.UserName} درخواست ورود ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پسورد وجود ندارد");
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
                Expires = userForLoginDto.IsRemember ? DateTime.Now.AddDays(1) : DateTime.Now.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDes);

            UserForDetailedDto user = _mapper.Map<UserForDetailedDto>(userFromRepo);

            _logger.LogInformation($"{userForLoginDto.UserName} وارد سایت شده است");

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

    }
}
