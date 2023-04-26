using System.ComponentModel.DataAnnotations;

namespace HealthCareSysAPI.Tokens
{
    public class LoginSchema
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
