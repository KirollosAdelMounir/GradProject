using HealthCareSysAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareSys.Models
{
    public class LabTest
    {
        [Key]
        public int Id { get; set; }

        public HealthCareSysUser User { get; set; }
        [ForeignKey("Id")]
        public String UserID { get; set; }
        public string BodyComponent { get; set; }
        public int Value { get; set; }
        public string Grade { get; set; }
    }
}
