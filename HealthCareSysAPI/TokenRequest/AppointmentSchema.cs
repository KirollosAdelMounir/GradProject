namespace HealthCareSysAPI.TokenRequest
{
    public class AppointmentSchema
    {
        public string UserId { get; set; }
        public string DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }

    }
}
