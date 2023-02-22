using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HealthCareSys.Areas.Identity.Data;

// Add profile data for application users by adding properties to the HealthCareSysUser class
public class HealthCareSysUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool Gender { get; set; }
    public string PhoneNumber { get; set; }
    public bool DoctorOrPatient { get; set; }
}

