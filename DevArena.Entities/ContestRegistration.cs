using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("contest_registration")]
    public class ContestRegistration
    {
        [Key]
        public int Id { get; set; }

        public int ContestId { get; set; }

        public int ParticipantId { get; set; }

        public DateTime RegTime { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;


        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual Participants Participant { get; set; }
    }
}
