using System.ComponentModel.DataAnnotations;

namespace HealthCareSysAPI.TokenRequest
{
    public class ScheduleAddition
    {
        public string DoctorID { get; set; }
        public string TimeFrom { get; set; }

        public string TimeTo { get; set; }
        public Day day { get; set; }
        public enum Day
        {
            Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
        }
    }
}
