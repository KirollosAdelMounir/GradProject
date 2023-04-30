using HealthCareSys.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthCareSysAPI.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }
        public HealthCareSysUser User { get; set; }
        [ForeignKey("UserID")]
        public string UserID { get; set; }
        public Doctor doctor { get; set; }
        [ForeignKey("DoctorID")]
        public string DoctorID { get; set; }
    }
}
