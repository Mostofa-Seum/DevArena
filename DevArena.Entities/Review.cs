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
        public int id { get; set; }

        [Required]
        public int submission_id { get; set; }

        [Required]
        public int judge_id { get; set; }

        [Required]
        public string verdict { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string feedback { get; set; } = null!;
        [Required]
        public DateTime reviewed_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("submission_id")]
        public virtual Submission Submission { get; set; } = null!;

        [ForeignKey("judge_id")]
        public virtual Judge Judge { get; set; } = null!;
    }
}