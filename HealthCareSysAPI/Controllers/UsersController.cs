﻿using HealthCareSysAPI.DBContext;
using HealthCareSysAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HealthCareSysDBContext _dbContext;
        private readonly UserManager<HealthCareSysUser> _userManager;
        private readonly SignInManager<HealthCareSysUser> _signInManager;

        public UsersController(HealthCareSysDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return Ok(users);
        }
        [HttpPost("Register")]
      
        public async Task<IActionResult> Register([FromBody] HealthCareSysUser model)
        {
            if (ModelState.IsValid)
            {
                var user = new HealthCareSysUser
                {
                    Id= model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Password = model.Password,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    type = model.type
                };

                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    return BadRequest(ModelState);
                }



                if (user != null)
                {

                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    return Ok(new { message = "Registration successful" });
                }


            }

            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<ActionResult> Login(HealthCareSysUser model)
        {
            var user =  _dbContext.Users.FirstOrDefault(x=>x.EmailAddress == model.EmailAddress);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                if(user.Password!= model.Password)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok();
                }
            }
            
        }

    }
}