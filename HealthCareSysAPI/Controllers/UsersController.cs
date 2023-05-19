﻿using HealthCareSysAPI.Custom_Classes;
using HealthCareSysAPI.DBContext;
using HealthCareSysAPI.Models;
using HealthCareSysAPI.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using HealthCareSys.Models;
using HealthCareSysAPI.TokenRequest;
using System.IO;
using Microsoft.AspNetCore.SignalR;

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        RandomID random = new RandomID();
        private readonly HealthCareSysDBContext _dbContext;
        private readonly UserManager<HealthCareSysUser> _userManager;
        private readonly SignInManager<HealthCareSysUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly Dictionary<string, HealthCareSysUser> _users;

        public UsersController(HealthCareSysDBContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _users = new Dictionary<string, HealthCareSysUser>();

        }

 
        [HttpPost("Register")]

        public async Task<IActionResult> Register([FromBody] HealthCareSysUser model)
        {

            if (ModelState.IsValid)
            {
                var user = new HealthCareSysUser
                {

                    Id = random.GenerateRandomId(10),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    type = model.type,
                    Blood = model.Blood,
                    Image = "123.jpg",
                    Address = model.Address
                };
                var existedUser = _dbContext.Users.FirstOrDefault(x => x.EmailAddress == user.EmailAddress);
                if (existedUser != null) { return BadRequest("Email existed please enter new email"); }

                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    return BadRequest(ModelState);
                }



                if (user != null)
                {

                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    var confirmationLink = GenerateConfirmationLink(user.Id); // Generate the confirmation link based on the user ID
                    _emailService.SendConfirmationEmail(user.EmailAddress, confirmationLink);

                    return Ok(new { message = "Registration successful" });
                }


            }

            return BadRequest(ModelState);
        }
        private string GenerateConfirmationLink(string userId)
        {
            return $"https://localhost:7036/api/Users/confirmEmail?userId={userId}";
        }
        [HttpGet("confirmEmail")]
        public IActionResult ConfirmEmail([FromQuery] string userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.ConfirmEmail = true;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                return Ok("User Confirmed Successfully");
            }
            else
            {
                return BadRequest("User Not found");
            }
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
                    if (!BCrypt.Net.BCrypt.Verify(user.Password, confirmUser.Password))
                    {
                        return Unauthorized(new { message = "Email or Password Incorrect" });
                    }
                    else
                    {
                        var confirmedUser = _dbContext.Users.FirstOrDefault(x => x.Id == confirmUser.Id);
                        if (confirmedUser != null && confirmedUser.ConfirmEmail == true)
                        {
                            JwtTokenHelperUser jwtToken = new JwtTokenHelperUser();
                            string token = jwtToken.GenerateJwtToken(confirmUser, confirmUser.Password, 60);
                            return Ok(token);
                        }
                        else
                        {
                            var confirmationLink = GenerateConfirmationLink(confirmUser.Id); // Generate the confirmation link based on the user ID
                            _emailService.SendConfirmationEmail(confirmUser.EmailAddress, confirmationLink);
                            return Unauthorized("Confirmation Email Sent");
                        }
                    }
                }
            }
            return Unauthorized();

        }
        [HttpPost("AddMedicalHistory")]
        public async Task<ActionResult> AddDisease([FromBody] DiseaseAddition model)
        {
            if (ModelState.IsValid)
            {
                var Disease = new MedicalHistory
                {
                    UserID = model.UserID,
                    Disease = model.Disease,
                    RelationShip = model.RelationShip,
                    DateOfInfection = model.DateOfInfection
                };
                if (Disease != null)
                {
                    _dbContext.MedicalHistories.Add(Disease);
                    _dbContext.SaveChanges();
                    return Ok(new { message = "History Recorded" });

                }
                return BadRequest(new { message = "Complete All the required data" });
            }
            return BadRequest();
        }
        [HttpPost("ChangeProfilePic")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeProfilePicAsync([FromForm] string id, IFormFile file)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                if (file != null && file.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "profiles");
                    string uniqueFileName = user.Id+ ".jpg";

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder); // Create the directory if it doesn't exist

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    user.Image = uniqueFileName;
                }



                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                return Ok(new { message = "Profile Picture Changed Successfully" });
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentSchema newAppointment)
        {
            if (ModelState.IsValid)
            {
                var addAppointment = new Appointment()
                {
                    UserId = newAppointment.UserId,
                    DoctorID = newAppointment.DoctorID,
                    AppointmentDate = newAppointment.AppointmentDate
                };
                if (addAppointment.AppointmentDate < DateTime.Now)
                {
                    return BadRequest("Invalid Date Please enter a valid date");
                }
                if (addAppointment != null)
                {
                    _dbContext.Appointments.Add(addAppointment);
                    _dbContext.SaveChanges();
                    return Ok("Appointment added Successfully");
                }
            }
            return BadRequest();


        }
        [HttpPost("NewPost")]
        public async Task<IActionResult> AddPost([FromBody] Post newPost)
        {
            if (ModelState.IsValid)
            {
                var post = new Forum()
                {
                    UserID = newPost.UserID,
                    PostText = newPost.PostText,
                    specializationSpecID = newPost.SpecID
                };
                if (post != null)
                {
                    _dbContext.Forums.Add(post);
                    _dbContext.SaveChanges();
                    return Ok("Post added Successfully");

                }
            }
            return BadRequest();
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userID, string currentpassword, string newPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);
            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(currentpassword, user.Password))
                {
                    return Unauthorized(new { message = "Incorrect Password" });
                }
                else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    _dbContext.Update(user);
                    _dbContext.SaveChanges();
                    return Ok("Password Changed Successfully");
                }
            }
            return BadRequest("User Not Found");
        }
        [HttpGet("ShowDoctor")]
        public async Task<IActionResult> ShowDoctor(string doctorId)
        {
            var doctor = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == doctorId);
            var doctoruser = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
            var spec = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == doctor.specializationSpecID);
            return Ok(new { doctor, doctoruser, spec });
        }
        [HttpPost("AddFavorite")]
        public async Task<IActionResult> AddFavorite(string userId, string doctorId)
        {
            var fav = new Favorite
            {
                UserID = userId,
                DoctorID = doctorId
            };
            _dbContext.Favorites.Add(fav);
            _dbContext.SaveChanges();
            return Ok("Favorite Added");
        }
        [HttpGet("ShowFavorites")]
        public async Task<IActionResult> ShowFavorites(string userId)
        {
            var favorites = _dbContext.Favorites.Where(x=>x.UserID== userId);
            return Ok(new { favorites });
        }
      
    }
}

