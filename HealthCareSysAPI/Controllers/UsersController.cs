using HealthCareSysAPI.Custom_Classes;
using HealthCareSysAPI.DBContext;
using HealthCareSysAPI.Models;
using HealthCareSysAPI.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


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
                RandomID random = new RandomID();
                var user = new HealthCareSysUser
                {
                    
                    Id = random.GenerateRandomId(10) ,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Password = BCrypt.Net.BCrypt.HashPassword( model.Password),
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    type = HealthCareSysUser.TypeOfUser.Patient ,
                    Blood = model.Blood,
                    Image = "123.jpg"
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
        public async Task<ActionResult> Login([FromBody] LoginSchema model)
        {
            if (ModelState.IsValid)
            {
                var user = new HealthCareSysUser
                {
                    EmailAddress = model.EmailAddress,
                    Password = model.Password
                };
                var confirmUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == user.EmailAddress);
                if (confirmUser == null)
                {
                    return Unauthorized(new { message = "Email or Password Incorrect" });
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(user.Password,confirmUser.Password))
                    {
                        return Unauthorized(new {message = "Email or Password Incorrect"});
                    }
                    else
                    {
                        JwtTokenHelperUser jwtToken = new JwtTokenHelperUser();
                        string token = jwtToken.GenerateJwtToken(confirmUser, confirmUser.Password, 60);
                        return Ok(token);
                    }
                }
            }
            return Unauthorized();
            
        }

    }
}
