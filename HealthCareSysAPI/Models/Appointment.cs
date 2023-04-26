using HealthCareSysAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthCareSys.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }
        public HealthCareSysUser User { get; set; }
        [ForeignKey("Id")]
        [NotNull]
        public string PatientID { get; set; }
        public Doctor doctor { get; set; }
        [ForeignKey("DoctorID")]
        public string DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int AppointmentRating { get; set; }
        public bool IsDone { get; set; }

    }
}
