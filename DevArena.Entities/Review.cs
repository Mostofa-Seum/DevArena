using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("reviews")]
    public class Review
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int JudgeId { get; set; }

        [Required]
        public string Verdict { get; set; }

        [Required]
        [StringLength(50)]
        public string Feedback { get; set; }

        [Required]
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SubmissionId")]
        public virtual Submission Submission { get; set; }

        [ForeignKey("JudgeId")]
        public virtual Judge Judge { get; set; }
    }
}