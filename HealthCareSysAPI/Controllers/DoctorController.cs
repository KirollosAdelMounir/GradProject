using HealthCareSysAPI.Custom_Classes;
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

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        RandomID random = new RandomID();
        string DrID;
        private readonly HealthCareSysDBContext _dbContext;
        public DoctorController(HealthCareSysDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("ContinueRegister")]
        public async Task<IActionResult> ContinueRegister([FromBody] DoctorRegister model)
        {
            if(ModelState.IsValid)
            {
                var doctor = new Doctor
                {
                    DoctorID = random.GenerateRandomId(15),
                    UserID = model.UserID,
                    AverageRating = 0,
                    specializationSpecID = model.specializationSpecID
                };
                var spec = _dbContext.Specializations.FirstOrDefault(x => x.SpecID == doctor.specializationSpecID);
                spec.NumberOfDoctors = spec.NumberOfDoctors + 1;
                if(doctor!= null && spec !=null)
                {
                    _dbContext.Doctors.Add(doctor);
                    _dbContext.Specializations.Update(spec);
                    _dbContext.SaveChanges();
                    return Ok(new { message = "Doctor Completed Registration successful" });
                }
            }
            return BadRequest(ModelState);

        }
        [HttpGet("ShowPatientHistory")]
        public IActionResult GetPatientDetails(string UserID)
        {
            var user =_dbContext.Users.FirstOrDefault(x=>x.Id== UserID);
            var diseases = _dbContext.MedicalHistories.Where(x=>x.UserID == UserID);
            if (user != null )
            {
                return Ok(new {diseases , Age= DateTime.Now.Year - user.DateOfBirth.Year });
            }
            return BadRequest(ModelState);

        }
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] Reply model)
        {
            if (ModelState.IsValid) {
                var comment = new Comment()
                {
                    DoctorID = model.DoctorID,
                    CommentText= model.CommentText,
                    ForumID = model.ForumID
                };
                if(comment!=null)
                {
                    _dbContext.Comments.Add(comment);
                    _dbContext.SaveChanges();
                    return Ok("Comment Added Successfully");
                }
            }
            return BadRequest();
        }


    }
}
