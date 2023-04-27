using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HealthCareSys.Models;
using HealthCareSysAPI.Models;

namespace HealthCareSysAPI.Custom_Classes
{
    public  class JwtTokenHelperUser
    {

        public  string GenerateJwtToken(HealthCareSysUser user, string secretKey, int expiryMinutes)
        {
            int Age = DateTime.Now.Year - user.DateOfBirth.Year;
            string gender;
            if(user.Gender == true)
            {
                gender = "Male";
            }
            else
            {
                gender = "Female";
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.EmailAddress),
                    new Claim(ClaimTypes.Role, user.type.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("BloodType", user.Blood.ToString()),
                    new Claim ("Age", Age.ToString()),
                    new Claim("PhoneNumber",user.PhoneNumber),
                    new Claim("Gender" , gender),
                    new Claim ("Image",user.Image)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}