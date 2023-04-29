
using HealthCareSysAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthCareSys.Models
{
    public class Doctor
    {
        [Key]
        public string DoctorID { get; set; }
        public HealthCareSysUser User { get; set; }
        [ForeignKey("Id")]
        [Required]
        public string UserID { get; set; }
        public float AverageRating { get; set; }
        public Specialization specialization { get; set; }
        [ForeignKey("SpecID")]
        public int specializationSpecID { get; set; }

    }
}
