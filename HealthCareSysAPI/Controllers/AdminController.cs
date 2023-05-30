using HealthCareSys.Models;
using HealthCareSysAPI.DBContext;
using HealthCareSysAPI.Models;
using HealthCareSysAPI.TokenRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly HealthCareSysDBContext _dbContext;

        public AdminController(HealthCareSysDBContext dbContext)
        {
            _dbContext = dbContext;

        }
        [HttpGet("AllUsers")]
        public IActionResult GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return Ok(users);
        }
        [HttpGet("AllDoctors")]
        public IActionResult GetDoctors()
        {
            var doctors = _dbContext.Doctors.ToList();
            foreach (var doctor in doctors)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
                if(user!=null)
                {
                    doctor.User = user;
                }
                var spec = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == doctor.specializationSpecID);
                if(spec!=null)
                {
                    doctor.specialization= spec;
                }
            }
            return Ok(doctors);
        }
        [HttpPost("AddSpecialization")]
        public async Task<IActionResult> AddSpecialization([FromBody] Specialization Spec)
        {
            if (ModelState.IsValid)
            {
                var newSpec = new Specialization()
                {
                    SpecializationName = Spec.SpecializationName,
                    SpecializationDescription = Spec.SpecializationDescription
                };
                if (newSpec != null)
                {
                    _dbContext.Specializations.Add(newSpec);
                    _dbContext.SaveChanges();
                    return Ok("New Specialization is added");
                }
            }
            return BadRequest();
        }
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return Ok("User Deleted Successfully");
            }
            return BadRequest();
        }
        [HttpDelete("DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = _dbContext.Doctors.FirstOrDefault(x => x.DoctorID == id);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
            if (user != null && doctor != null)
            {
                _dbContext.Doctors.Remove(doctor);
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return Ok("Doctor Deleted Successfully");
            }
            return BadRequest();
        }
        [HttpDelete("DeleteForum")]
        public async Task<IActionResult> DeleteForum(int id)
        {
            var forum = _dbContext.Forums.FirstOrDefault(x => x.PostID == id);
            if (forum != null)
            {
                _dbContext.Forums.Remove(forum);
                await _dbContext.SaveChangesAsync();
                return Ok("Post Deleted Successfully");
            }
            return BadRequest();
        }
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _dbContext.Comments.FirstOrDefault(x => x.CommentID == id);
            if (comment != null)
            {
                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
                return Ok("Comment Deleted Successfully");
            }
            return BadRequest();
        }
        [HttpPost("ChangeSpecPicture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeSpecPic([FromForm] int id, IFormFile file)
        {
            var spec = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == id);
            if (spec != null)
            {
                if (file != null && file.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "spec");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder); // Create the directory if it doesn't exist

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    spec.SpecImage = uniqueFileName;
                }



                _dbContext.Specializations.Update(spec);
                _dbContext.SaveChanges();
                return Ok(new { message = "Specialization Picture Changed Successfully" });
            }
            else
            {
                return BadRequest();
            }


        }
        [HttpGet("AllPosts")]
        public IActionResult GetPosts()
        {
            var Posts = _dbContext.Forums.ToList();
            foreach (var post in Posts)
            {
                var specialization = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == post.specializationSpecID);
                if (specialization != null)
                {
                    post.Specialization = specialization;
                }
            }
            return Ok(Posts);
        }
        [HttpGet("AllComments")]
        public IActionResult GetComments()
        {
            var comments = _dbContext.Comments.ToList();
            return Ok(comments);
        }
        [HttpDelete("DeleteSpec")]
        public async Task<IActionResult> DeleteSpec(int id)
        {
            var spec = _dbContext.Specializations.FirstOrDefaultAsync(x=>x.SpecID== id);
            if(spec != null)
            {
                _dbContext.Specializations.Remove(await spec);
                _dbContext.SaveChanges();
                return Ok("Specialization Deleted Successfully");
            }
            else { return BadRequest(); }
        }
        [HttpGet("ShowAllAppointments")]
        public IActionResult ShowAllAppointments()
        {
            var allAppointments = _dbContext.Appointments.Where(x=>x.IsDone == true).ToList();
            foreach(var appointment in allAppointments)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == appointment.UserId);
                if (user != null) { appointment.User = user; }
                var doctor = _dbContext.Doctors.FirstOrDefault(x=>x.DoctorID== appointment.DoctorID);
                if (doctor != null)
                {
                    var doctoruser = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
                    if (doctoruser != null)
                    {
                        appointment.doctor = doctor;
                        doctor.User = doctoruser;
                    }
                  }
                }
            return Ok(allAppointments);
        }

    }
}
