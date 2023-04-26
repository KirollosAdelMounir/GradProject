using HealthCareSys.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthCareSysAPI.Models
{
    public class HealthCareSysUser
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        public TypeOfUser type { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        [Required]
        public BloodType Blood { get; set; }
        public bool ConfirmEmail { get; set; } = false;

        public enum TypeOfUser
        {
            Doctor,Patient,Admin
        }
        public enum BloodType
        {
            A1,A2,B1,B2,AB1,AB2,O1,O2
        }
    }
}
