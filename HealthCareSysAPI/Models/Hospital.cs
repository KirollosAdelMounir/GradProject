using System.ComponentModel.DataAnnotations;

namespace HealthCareSys.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalID { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool OutPatientClinic { get; set; }
    }
}
