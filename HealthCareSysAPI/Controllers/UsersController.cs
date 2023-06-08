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
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Cors;
using HealthCareSysAPI.CosmosModel;
using Microsoft.Azure.Cosmos;
using System.Reflection.Metadata;
using System.Net;

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class UsersController : ControllerBase
    {
        RandomID random = new RandomID();
        private readonly HealthCareSysDBContext _dbContext;
        private readonly UserManager<HealthCareSysUser> _userManager;
        private readonly SignInManager<HealthCareSysUser> _signInManager;
        private readonly messageservice _emailService;
        private readonly Dictionary<string, HealthCareSysUser> _users;
        private readonly CosmosContext _cosmosContext;

        public UsersController(HealthCareSysDBContext dbContext, messageservice emailService, CosmosContext cosmosContext)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _users = new Dictionary<string, HealthCareSysUser>();
            _cosmosContext = cosmosContext;
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
                    _emailService.Send(user.EmailAddress, confirmationLink);

                    return Ok(new { message = "Registration successful" });
                }
                

            }

            return BadRequest(ModelState);
        }
        private string GenerateConfirmationLink(string userId)
        {
            return $"https://healthcaresys.azurewebsites.net/api/Users/confirmEmail?userId={userId}";
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
                            _emailService.Send(confirmUser.EmailAddress, confirmationLink);
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
                    string uniqueFileName = user.Id + ".jpg";

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
            var favorites = _dbContext.Favorites.Where(x => x.UserID == userId);
            foreach (var fav in favorites)
            {
                var favDoc = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == fav.DoctorID);
                if (favDoc != null)
                {
                    var favDocUser = _dbContext.Users.FirstOrDefault(x => x.Id == favDoc.UserID);
                    fav.doctor = favDoc;
                    if(favDocUser!= null)
                    {
                        fav.doctor.User= favDocUser;
                    }
                }
            }
            return Ok(new { favorites });

        }
        [HttpPut("WriteReview")]
        public async Task<IActionResult> AddReview(int AppointmentID, int AppointmentRating, string AppointmentReview)
        {
            var appointment = _dbContext.Appointments.FirstOrDefault(x => x.AppointmentID == AppointmentID);
            if (appointment != null && appointment.IsDone == true)
            {
                var doctor = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == appointment.DoctorID);

                appointment.AppointmentRating = AppointmentRating;
                appointment.AppointmentReview = AppointmentReview;
                if (doctor != null)
                {
                    if (doctor.AverageRating == 0)
                    {
                        doctor.AverageRating = AppointmentRating;
                    }
                    else
                    {
                        doctor.AverageRating = (doctor.AverageRating + AppointmentRating) / 2;
                    }
                    _dbContext.Doctors.Update(doctor);
                }
                _dbContext.Appointments.Update(appointment);
                _dbContext.SaveChanges();
                return Ok("Review Added Successfully");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut("CommentRating")]
        public async Task<IActionResult> AddCommentReview(int CommentID, int CommentRating)
        {
            var comment = _dbContext.Comments.FirstOrDefault(x => x.CommentID == CommentID);
            if (comment != null)
            {
                comment.CommentRating = CommentRating;
                _dbContext.Comments.Update(comment);
                _dbContext.SaveChanges();
                return Ok("Comment Rating Added");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("DeletePost")]
        public IActionResult DeletePost(int Post) {

            var post = _dbContext.Forums.FirstOrDefault(x => x.PostID == Post);
            if (post != null)
            {
                _dbContext.Forums.Remove(post);
                _dbContext.SaveChanges();
                return Ok("Post Deleted Successfully");
            }
            else { return NotFound(); }
        }
        [HttpDelete("DeleteFavorite")]
        public IActionResult DeleteFavorite (string DoctorID)
        {
            var favDoc = _dbContext.Favorites.FirstOrDefault(x=>x.DoctorID== DoctorID);
            if(favDoc != null)
            {
                _dbContext.Favorites.Remove(favDoc);
                _dbContext.SaveChanges();
                return Ok("Favorite Delete Successfully");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("ViewPostComments")]
        public async Task<IActionResult> ViewPostComments(int postID)
        {
            var comments = _dbContext.Comments.Where(x=>x.ForumID== postID).ToList();
            if (comments != null)
            {
                foreach(var comment in comments) 
                {
                    var post = _dbContext.Forums.FirstOrDefault(x => x.PostID == comment.ForumID);
                    var doctor = _dbContext.Doctors.FirstOrDefault(x=>x.DoctorID== comment.DoctorID);

                    if (doctor != null && post != null )
                    {
                        var user = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
                        comment.forum = post;
                        comment.doctor = doctor;
                        if (user != null)
                        {
                            doctor.User = user;
                        }
                    }
                }
                return Ok(comments);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("ShowUserAppointment")]
        public IActionResult ShowUserAppointment(string userID)
        {
            var appointments = _dbContext.Appointments.Where(x => x.UserId == userID && x.IsAccepted == false).ToList();
            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    var user = _dbContext.Users.FirstOrDefault(x => x.Id == appointment.UserId);
                    if (user != null) { appointment.User = user; }
                    var doctor = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == appointment.DoctorID);
                    if (doctor != null)
                    {
                        var doctoruser = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
                        if (doctoruser != null)
                        {
                            appointment.doctor = doctor;
                            doctor.User = doctoruser;
                        }
                    }
                    if (appointment.AppointmentDate < DateTime.Now)
                    {
                        appointment.IsDone = true;
                        _dbContext.Appointments.Update(appointment);
                        _dbContext.SaveChanges();
                    }
                }

                return Ok(appointments.Where(x => x.IsDone == false));
            }
            else { return BadRequest(); }
        }
        [HttpGet("ViewUserPost")]
        public IActionResult ViewUserPost(string userID)
        {
            var Posts = _dbContext.Forums.Where(x=>x.UserID== userID).ToList();
            foreach (var post in Posts)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == post.UserID);
                var specialization = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == post.specializationSpecID);
                if (specialization != null && user != null)
                {
                    post.Specialization = specialization;
                    post.User = user;
                }
            }
            return Ok(Posts);
        }
        [HttpGet("ViewAcceptedAppointments")]
        public IActionResult ShowAcceptedDoctorAppointment(string userId)
        {
            var appointments = _dbContext.Appointments.Where(x => x.UserId == userId && x.IsAccepted == true).ToList();
            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    var user = _dbContext.Users.FirstOrDefault(x => x.Id == appointment.UserId);
                    if (user != null) { appointment.User = user; }
                    var doctor = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == appointment.DoctorID);
                    if (doctor != null)
                    {
                        var doctoruser = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
                        if (doctoruser != null)
                        {
                            appointment.doctor = doctor;
                            doctor.User = doctoruser;
                        }
                    }
                    if (appointment.AppointmentDate < DateTime.Now)
                    {
                        appointment.IsDone = true;
                        _dbContext.Appointments.Update(appointment);
                        _dbContext.SaveChanges();
                    }
                }

                return Ok(appointments.Where(x => x.IsDone == false));
            }
            else { return BadRequest(); }
        }

        [HttpPost("Predict")]
        public async Task<IActionResult> Predict([FromForm] HeartAttackData heartAttack)
        {
         
            using (var client = new HttpClient())
            {
                string apiUrl = "https://heartattack.azurewebsites.net/predict";
                var ht = new HeartAttackData()
                {
                    age = heartAttack.age,
                    sex = heartAttack.sex,
                    cp = heartAttack.cp,
                    trtbps = heartAttack.trtbps,
                    chol = heartAttack.chol,
                    fbs = heartAttack.fbs,
                    restecg = heartAttack.restecg,
                    thalachh = heartAttack.thalachh,
                    exng = heartAttack.exng,
                    oldpeak = heartAttack.oldpeak,
                    slp = heartAttack.slp,
                    caa = heartAttack.caa,
                    thall = heartAttack.thall
                };
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("age", ht.age.ToString()),
                    new KeyValuePair<string, string>("sex", ht.sex.ToString()),
                    new KeyValuePair<string, string>("cp", ht.cp.ToString()),
                    new KeyValuePair<string, string>("trtbps", ht.trtbps.ToString()),
                    new KeyValuePair<string, string>("chol", ht.chol.ToString()),
                    new KeyValuePair<string, string>("fbs", ht.fbs.ToString()),
                    new KeyValuePair<string, string>("restecg", ht.restecg.ToString()),
                    new KeyValuePair<string, string>("thalachh", ht.thalachh.ToString()),
                    new KeyValuePair<string, string>("exng", ht.exng.ToString()),
                    new KeyValuePair<string, string>("oldpeak", ht.oldpeak.ToString()),
                    new KeyValuePair<string, string>("slp", ht.slp.ToString()),
                    new KeyValuePair<string, string>("caa", ht.caa.ToString()),
                    new KeyValuePair<string, string>("thall", ht.thall.ToString())
                });


                HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return Ok(responseContent);
                }
                else
                {
                    return BadRequest (response.StatusCode);
                }
            }
        }
        [HttpGet("GetProfilePic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProfilePic(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user != null && !string.IsNullOrEmpty(user.Image))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "profiles");
                string filePath = Path.Combine(uploadsFolder, user.Image);

                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "image/jpeg"); // Return the image file
                }
            }

            return NotFound(); // Profile picture not found
        }
        [HttpPost("SetDefaultPic")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetDefaultProfilePicture([FromForm] string id, IFormFile file)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                if (file != null && file.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "profiles");
                    string uniqueFileName = "123.jpg";

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
        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage(Chat message)
        {
           var response = await _cosmosContext.SendMessage(message);
            if (response != null)
            {
                _cosmosContext.NotifyNewMessage(message);

                return Ok("Message Sent Successfully");

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("receive/{receiver}")]
        public async Task<IActionResult> ReceiveMessages(string receiver)
        {
            var messages = await _cosmosContext.GetChatMessagesByReceiverAsync(receiver);

            if (messages.Count > 0)
            {
                return Ok(messages);
            }

            var tcs = new TaskCompletionSource<IActionResult>();
            Action<Chat> callback = (message) =>
            {
                if (message.receiver == receiver)
                {
                    tcs.SetResult(Ok(new List<Chat> { message }));
                }
            };

               _cosmosContext.RegisterMessageCallback(callback);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            if (await Task.WhenAny(tcs.Task, timeoutTask) == timeoutTask)
            {
                _cosmosContext.UnRegisterMessageCallback(callback);
                return NoContent();
            }

            return await tcs.Task;
        }
        [HttpGet("FullConversation")]
        public async Task<IActionResult> GetFullConversation(string receiver, string sender)
        {
            var messages = await _cosmosContext.GetFullConversation(receiver,sender);

            if (messages.Count > 0)
            {
                return Ok(messages);
            }

            var tcs = new TaskCompletionSource<IActionResult>();
            Action<Chat> callback = (message) =>
            {
                if (message.receiver == receiver)
                {
                    tcs.SetResult(Ok(new List<Chat> { message }));
                }
            };

            _cosmosContext.RegisterMessageCallback(callback);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            if (await Task.WhenAny(tcs.Task, timeoutTask) == timeoutTask)
            {
                _cosmosContext.UnRegisterMessageCallback(callback);
                return NoContent();
            }

            return await tcs.Task;
        }
    }

    }


