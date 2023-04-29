namespace HealthCareSysAPI.TokenRequest
{
    public class DiseaseAddition
    {
        public string UserID { get; set; }
        public string Disease { get; set; }
        public string RelationShip { get; set; }
        public DateTime DateOfInfection { get; set; }
    }
}
