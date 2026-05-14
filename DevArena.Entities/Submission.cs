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
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public int ContestId { get; set; }

        [Required]
        public int ProblemId { get; set; }

        [Required]
        public string CodeText { get; set; } = null!;
         
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!;
        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ParticipantId")]
        public virtual Participants Participant { get; set; } = null!;

        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; } = null!;

        [ForeignKey("ProblemId")]
        public virtual Problems Problem { get; set; } = null!;
    }
}