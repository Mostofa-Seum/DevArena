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
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string title { get; set; } = null!;
        [Required]
        public string message { get; set; } = null!;

        public int host_id { get; set; }
        public bool Is_active { get; set; } = true;

        [ForeignKey("host_id")]
        public virtual Host Host { get; set; } = null!;
    }
}