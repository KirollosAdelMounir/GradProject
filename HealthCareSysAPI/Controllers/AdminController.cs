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
        [HttpGet("AllDoctors")]
        public IActionResult GetUsers()
        {
            var doctors = _dbContext.Doctors.ToList();
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
            var doctor = _dbContext.Doctors.FirstOrDefault(x=>x.DoctorID== id);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == doctor.UserID);
            if (user != null&&doctor!= null) {
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
            var forum = _dbContext.Forums.FirstOrDefault(x=>x.PostID==id);
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
            var comment = _dbContext.Comments.FirstOrDefault(x=>x.CommentID== id);
            if (comment != null)
            {
                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
                return Ok("Comment Deleted Successfully");
            }
            return BadRequest();
        }



    }
}
