using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Repository.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [Route("site/admin/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Site")]
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

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.UserRepository.GetManyAsync(null, null, "Photos,BankCards");
            IEnumerable<UserForListDto> usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _db.UserRepository.GetManyAsync(p => p.Id == id, null, "Photos");
            UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user.SingleOrDefault());
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id , UserForUpdateDto userForUpdateDto)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != id)
            {
                _logger.LogError($"{id} : your not allowed to edit this user");
                return Unauthorized("شما اجازه ویرایش این کاربر را ندارید");
            }

            var userFromRepo = await _db.UserRepository.GetByIdAsync(id);
            _mapper.Map(userForUpdateDto, userFromRepo);
            _db.UserRepository.Update(userFromRepo);
            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ReturnMessage()
                {
                    Message = $"ویرایش برای کاربر {userForUpdateDto.Name} انجام نشد.",
                    Status = false,
                    Title = "خطا"
                });
            }
        }

        [Route("ChangeUserPassword/{id}")]
        [HttpPut]
        public async Task<IActionResult> ChangeUserPassword(string id ,PasswordForChangeDto passwordForChangeDto)
        {
            var userFromRepo =await _userService.GetUserForPassChange(id, passwordForChangeDto.OldPassword);

            if (userFromRepo == null)
                return BadRequest(new ReturnMessage()
                {
                    Message = $"پسورد قبلی اشتباه میباشد",
                    Status = false,
                    Title = "خطا"
                });

            if (await _userService.UpdateUserPassword(userFromRepo , passwordForChangeDto.NewPassword))
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
