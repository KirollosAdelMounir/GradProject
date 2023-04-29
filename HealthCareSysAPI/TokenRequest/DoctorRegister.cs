using System.ComponentModel.DataAnnotations;

namespace HealthCareSysAPI.TokenRequest
{
    public class DoctorRegister
    {
        [Required]
        public string UserID { get; set; }
        [Required]
        public int specializationSpecID { get; set; }


    }
}
