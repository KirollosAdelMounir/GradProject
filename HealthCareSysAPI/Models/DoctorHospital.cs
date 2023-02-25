using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareSys.Models
{
    public class DoctorHospital
    {
        [Key]
        public int Id { get; set; }
        public Doctor doctor { get; set; }
        [ForeignKey("DoctorID")]
        public string DoctorID { get; set; }
        public Hospital hospital { get; set; }
        [ForeignKey("HospitalID")]
        public int HospitalID { get; set; }
    }
}
