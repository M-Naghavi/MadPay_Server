using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.ION;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.v1;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Users.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    //[System.Web.Mvc.RequireHttps]
    //[ServiceFilter(typeof(LogFilter))]
    //[Route("api/v1/site/admin/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1_Site_Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUnitOfWork<MalpayDbContext> dbContext,
                               IMapper mapper,
                               IUserService userService,
                               ILogger<UsersController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet(ApiV1Routes.Users.GetUsers)]
        [ResponseCache(Duration = 60 )]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.UserRepository.GetManyAsync(null, null, "Photos,BankCards");
            IEnumerable<UserForListDto> usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            var collectionLink = Link.ToCollection(nameof(GetUsers));
            var collection = new Collection<UserForListDto>()
            {
                Self = collectionLink , 
                Value = usersToReturn.ToArray()
            };
            return Ok(collection);
        }

        [HttpGet(ApiV1Routes.Users.GetUser, Name = nameof(GetUser))]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _db.UserRepository.GetManyAsync(p => p.Id == id, null, "Photos");
            UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user.SingleOrDefault());
            return Ok(userToReturn);
        }

        [HttpPut(ApiV1Routes.Users.UpdateUser)]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        public async Task<IActionResult> UpdateUser(string id, UserForUpdateDto userForUpdateDto)
        {
            var userFromRepo = await _db.UserRepository.GetByIdAsync(id);
            _mapper.Map(userForUpdateDto, userFromRepo);
            _db.UserRepository.Update(userFromRepo);
            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                _logger.LogError($"{userForUpdateDto.Name} : user can not update");
                return BadRequest(new ReturnMessage()
                {
                    Message = $"ویرایش برای کاربر {userForUpdateDto.Name} انجام نشد.",
                    Status = false,
                    Title = "خطا"
                });
            }
        }

        [HttpPut(ApiV1Routes.Users.ChangeUserPassword)]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        public async Task<IActionResult> ChangeUserPassword(string id, PasswordForChangeDto passwordForChangeDto)
        {
            var userFromRepo = await _userService.GetUserForPassChange(id, passwordForChangeDto.OldPassword);

            if (userFromRepo == null)
                return BadRequest(new ReturnMessage()
                {
                    Message = $"پسورد قبلی اشتباه میباشد",
                    Status = false,
                    Title = "خطا"
                });

            if (await _userService.UpdateUserPassword(userFromRepo, passwordForChangeDto.NewPassword))
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ReturnMessage()
                {
                    Message = $"ویرایش پسورد کاربر انجام نشد",
                    Status = false,
                    Title = "خطا"
                });
            }
        }

        //[Route("GetProfileUser/{id}")]
        //[HttpGet]
        //public async Task<IActionResult> GetProfileUser(string id)
        //{
        //    if (User.FindFirst(ClaimTypes.NameIdentifier).Value == id)
        //    {
        //        var user = await _db.UserRepository.GetManyAsync(p => p.Id == id, null, "Photos");
        //        UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user.SingleOrDefault());
        //        return Ok(userToReturn);
        //    }
        //    else
        //    {
        //        return Unauthorized("شما به این اطلاعات دسترسی ندارید");
        //    }
        //}
    }
}
