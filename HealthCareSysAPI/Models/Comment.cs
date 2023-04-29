using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareSys.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public Doctor doctor { get; set; }
        [ForeignKey("DoctorID")]
        public string DoctorID { get; set; }
        public Forum forum { get; set; }
        [ForeignKey("PostID")]
        public int ForumID { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
        public int CommentRating { get; set; } = 0;

    }
}
