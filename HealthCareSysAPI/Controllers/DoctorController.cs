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
using Microsoft.AspNetCore.Cors;

namespace HealthCareSysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]

    public class DoctorController : ControllerBase
    {
        RandomID random = new RandomID();
        
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
                    specializationSpecID = model.specializationSpecID,
                    price = model.price
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
        [HttpGet("ShowUser")]
        public async Task<IActionResult> ShowUser(string userID)
        {
            var user = _dbContext.Users.FirstOrDefault(x=>x.Id== userID);
            return Ok (user);
        }
        [HttpGet("ShowAllSpecs")]
        public async Task<IActionResult> AllSpecialization()
        {
            var specialization = _dbContext.Specializations;
            return Ok(specialization);
        }
        [HttpPost("AddSchedule")]
        public async Task <IActionResult> AddSchedule([FromBody] ScheduleAddition scheduleTiming) 
        {
            if (ModelState.IsValid)
            {
                var addedscheduletiming = new ScheduleTiming()
                {
                    DoctorID = scheduleTiming.DoctorID,
                    TimeFrom = scheduleTiming.TimeFrom,
                    TimeTo = scheduleTiming.TimeTo,
                    day = (ScheduleTiming.Day)scheduleTiming.day
                };
                if (addedscheduletiming!=null)
                {
                    _dbContext.ScheduleTimings.Add(addedscheduletiming);
                    _dbContext.SaveChanges();
                    return Ok("Schedule Timing Added");
                }
                else
                {
                    return BadRequest("Insufficient Data");
                }
            }
            return BadRequest("Insufficient Data");

        }
        [HttpGet("ShowSchedule")]
        public async Task<IActionResult> ShowDoctorSchedule(string doctorID)
        {
            var schedule = _dbContext.ScheduleTimings.Where(x=>x.DoctorID == doctorID);
            if(schedule!= null)
            {
                return Ok(schedule);
            }
            else
            {
                return NotFound("Doctor Not Found");
            }
        }
        [HttpDelete("DeleteSchedule")]
        public async Task<IActionResult> DeleteSchedule(int ScheduleID)
        {
            var schedule = _dbContext.ScheduleTimings.FirstOrDefault(x=>x.Id== ScheduleID);
            if (schedule != null)
            {
                _dbContext.ScheduleTimings.Remove(schedule);
                await _dbContext.SaveChangesAsync();
                return Ok("Schedule Deleted Successfully");
            }
            else
            {
                return NotFound("Schedule Not founded");
            }
        }
        [HttpPut("AcceptAppointment")]
        public IActionResult AcceptAppointment (int appointmentID)
        {
            var appointment = _dbContext.Appointments.Where(x=>x.AppointmentID== appointmentID).FirstOrDefault(); 
            if (appointment!= null)
            {
                appointment.IsAccepted= true;
                _dbContext.Appointments.Update(appointment);
                _dbContext.SaveChanges();
                return Ok("Appointment Accepted");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut("AppointmentCompleted")]
        public IActionResult AppointmentCompleted(int appointmentID)
        {
            var appointment = _dbContext.Appointments.Where(x => x.AppointmentID == appointmentID).FirstOrDefault();
            if (appointment != null)
            {
                appointment.IsDone = true;
                _dbContext.Appointments.Update(appointment);
                _dbContext.SaveChanges();
                return Ok("Appointment Completed");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("DeleteComment")]
        public IActionResult DeleteComment(int CommentID)
        {
            var comment = _dbContext.Comments.FirstOrDefault(x=>x.CommentID== CommentID);
            if (comment != null)
            {
                _dbContext.Comments.Remove(comment);
                _dbContext.SaveChanges();
                return Ok("Comment Deleted Successfully");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("ShowDoctorAppointment")]
        public IActionResult ShowDoctorAppointment (string doctorID)
        {
            var appointments = _dbContext.Appointments.Where(x=>x.DoctorID== doctorID && x.IsAccepted == false).ToList();
            if(appointments != null )
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

                return Ok(appointments.Where(x=>x.IsDone==false));
            }
            else { return BadRequest(); }
        }
        [HttpGet("ViewSpecPosts")]
        public IActionResult ShowPosts(int specID)
        {
            var Posts = _dbContext.Forums.Where(x=>x.specializationSpecID== specID).ToList();
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
    }
}
