
using HealthCareSys.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HealthCareSys.Models
{
    public class Forum
    {
        [Key]
        public int PostID { get; set; }
        public HealthCareSysUser User { get; set; }
        [ForeignKey("Id")]
        [NotNull]
        public string UserID { get; set; }

        public string PostText { get; set; }
        public DateTime DateOfPost { get; set; }
    }
}
