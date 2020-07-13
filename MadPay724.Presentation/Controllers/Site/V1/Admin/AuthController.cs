using System;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MadPay724.Data.Dtos.Site.Admin.Users;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MadPay724.Repository.Infrastructure;
using MadPay724.Presentation.Routes.v1;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [AllowAnonymous]
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
        private readonly IUtilities _utilities;
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IUnitOfWork<MalpayDbContext> dbContext,
                    IAuthService authService,
                    IConfiguration configuration,
                    IMapper mapper,
                    ILogger<AuthController> logger,
                    IUtilities utilities,
                    UserManager<User> userManager,
                    SignInManager<User> signInManager)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
            _db = dbContext;
            _logger = logger;
            _UserManager = userManager;
            _signInManager = signInManager;
            _utilities = utilities;
        }

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
                }, userForReturn);
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

        [HttpPost(ApiV1Routes.Auth.Login)]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _UserManager.FindByNameAsync(userForLoginDto.UserName);
            if (user == null)
            {
                _logger.LogWarning($"{userForLoginDto.UserName} درخواست لاگین ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پس وجود ندارد");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = _UserManager.Users.Include(p => p.Photos)
                    .FirstOrDefault(u => u.NormalizedUserName == userForLoginDto.UserName.ToUpper());

                var userForReturn = _mapper.Map<UserForDetailedDto>(appUser);

                _logger.LogInformation($"{userForLoginDto.UserName} لاگین کرده است");
                return Ok(new
                {
                    token = _utilities.GenerateJwtToken(appUser, userForLoginDto.IsRemember),
                    user = userForReturn
                });
            }
            else
            {
                _logger.LogWarning($"{userForLoginDto.UserName} درخواست لاگین ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پس وجود ندارد");

            }
        }

    }
}
