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
        [Key]
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int ContestId { get; set; }
        public int PromotedByHostId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ParticipantId")]
        public virtual Participants Participants { get; set; }

        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; }

        [ForeignKey("PromotedByHostId")]
        public virtual Host Host { get; set; }
    }
}
