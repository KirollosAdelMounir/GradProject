using HealthCareSysAPI.Models;
using MessagePack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace HealthCareSys.Models
{
    public class MedicalHistory
    {
        [Key]
        public int MedicalId { get; set; }
        public HealthCareSysUser User { get; set; }
        [ForeignKey("Id")]
        [NotNull]
        public string UserID { get; set; }
        public string Disease { get; set; }
        public string RelationShip { get; set; }
        public DateTime DateOfInfection { get; set; }

    }
}
