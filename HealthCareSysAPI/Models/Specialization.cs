using System.ComponentModel.DataAnnotations;

namespace HealthCareSys.Models
{
    public class Specialization
    {
        [Key]
        public int SpecID { get; set; }
        public string SpecializationName { get; set; }
        public string SpecializationDescription { get; set;}
        public int NumberOfDoctors { get; set; } = 0;
    }
}
