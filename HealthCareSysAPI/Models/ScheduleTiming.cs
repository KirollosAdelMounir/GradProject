using HealthCareSys.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareSysAPI.Models
{
    public class ScheduleTiming
    {
        [Key]
        public int Id { get; set; }
        public Doctor doctor { get; set; }
        [ForeignKey("DoctorID")]
        public string DoctorID { get; set; }
        [DataType(DataType.Time)]
        public DateTime TimeFrom { get; set; }
        [DataType(DataType.Time)]
        public DateTime TimeTo { get; set; }
        public Day day { get; set; }
        public enum Day
        {
            Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday
        }

    }
}
