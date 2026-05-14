using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("announcements")]
    public class Announcement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public int HostId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("HostId")]
        public virtual Host Host { get; set; } = null!;
    }
}