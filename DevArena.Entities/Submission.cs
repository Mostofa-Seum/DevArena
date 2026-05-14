using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("submissions")]
    public class Submission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int participant_id { get; set; }

        [Required]
        public int contest_id { get; set; }

        [Required]
        public int problem_id { get; set; }

        [Required]
        public string code_text { get; set; } = null!;
         
        [Required]
        [StringLength(50)]
        public string status { get; set; } = null!;
        [Required]
        public DateTime submitted_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("participant_id")]
        public virtual Participants Participant { get; set; } = null!;

        [ForeignKey("contest_id")]
        public virtual Contests Contest { get; set; } = null!;

        [ForeignKey("problem_id")]
        public virtual Problems Problem { get; set; } = null!;
    }
}