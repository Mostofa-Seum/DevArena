using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("judges")]
    public class Judge
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public int ContestId { get; set; }

        [Required]
        public int PromotedByHostId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ParticipantId")]
        public virtual Participants Participants { get; set; } = null!;

        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; } = null!;

        [ForeignKey("PromotedByHostId")]
        public virtual Host Host { get; set; } = null!;
    }
}