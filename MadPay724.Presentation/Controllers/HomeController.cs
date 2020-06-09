using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Infrastructure;
using MadPay724.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadPay724.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork<MalpayDbContext> _db;
        public HomeController(IUnitOfWork<MalpayDbContext> dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            //var user = new User()
            //{
            //    Address = "",
            //    BirthDate = "",
            //    City = "",
            //    DateCreated = DateTime.Now,
            //    Gender = "",
            //    DateModified = DateTime.Now,
            //    IsActive = true,
            //    Name = "",
            //    PasswordHash = new byte[] {0x20 , 0x20, 0x20, 0x20, 0x20, } ,
            //    PasswordSalt = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, },
            //    PnoneNumber = "",
            //    Status = true,
            //    UserName = ""
            //};
            //await _db.UserRepository.InsertAsync(user);
            //await _db.SaveAsync();

            //var model = await _db.UserRepository.GetAllAsync();

            //return Ok(model);

            return Ok("value");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<string> Post([FromBody] string value)
        {
            return null;
        }

        [HttpPut("{id}")]
        public async Task<string> Put(int id,[FromBody] string value)
        {
            return null;
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete(int id)
        {
            return null;
        }
    }
}
