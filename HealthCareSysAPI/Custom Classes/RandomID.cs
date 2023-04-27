using System;
using System.Linq;

namespace HealthCareSysAPI.Custom_Classes
{
    public class RandomID
    {
        private static Random random = new Random();

        public  string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string randomId = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomId;
        }
    }
}
